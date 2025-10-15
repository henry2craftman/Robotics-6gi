using System;
using System.Collections.Concurrent; // ConcurrentQueue를 위해 추가
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 서버로부터 수신한 메시지를 파싱하기 위한 래퍼 클래스.
/// </summary>
[Serializable]
public class NetworkMessage
{
    public string type;
    public UserData data;
}

/// <summary>
/// 서버와 클라이언트 간에 주고받을 데이터 구조체.
/// JsonUtility를 통해 직렬화/역직렬화 됩니다.
/// </summary>
[Serializable]
public class UserData
{
    public string addr; // 유저의 고유 주소 (IP:Port)
    public Vector3 pos; // 유저의 위치
    public Vector3 rot; // 유저의 회전
}

/// <summary>
/// 서버와 비동기 TCP 통신을 수행하는 클라이언트 클래스.
/// 자신의 위치/회전 정보를 서버로 보내고, 다른 클라이언트의 정보를 받아와 동기화합니다.
/// </summary>
public class ClientAsync : MonoBehaviour
{
    // --- Public Fields (Unity Inspector에서 설정) ---
    [Header("Network Settings")]
    public string serverIP = "127.0.0.1";
    public int port = 7777;
    [Tooltip("서버로 데이터를 전송하는 주기 (초)")]
    public float sendIntervalSeconds = 0.1f;

    [Header("Game Settings")]
    public GameObject playerPrefab;
    
    [Header("UI (Optional)")]
    public TMP_InputField messageInput;

    // --- Private Fields ---
    private TcpClient _client;
    private NetworkStream _stream;
    private byte[] _receiveBuffer;
    private StringBuilder _stringBuilder;
    
    private Dictionary<string, GameObject> _userObjects = new Dictionary<string, GameObject>();
    private GameObject _myPlayerObject;

    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    // 메인 스레드와 네트워크 송신 스레드 간 데이터 공유를 위한 변수
    private readonly object _playerDataLock = new object();
    private string _latestPlayerDataJson = null;

    #region Unity Lifecycle Methods

    private void Update()
    {
        // 1. 메인 스레드에서 실행할 작업 처리 (플레이어 생성, 파괴 등)
        while (_mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }

        // 2. 네트워크로 보낼 내 플레이어 데이터를 미리 준비 (메인 스레드에서만 접근)
        PrepareDataForSending();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    #endregion

    #region Network Connection

    public async void ConnectToServer()
    {
        if (_client != null && _client.Connected) return;

        try
        {
            _client = new TcpClient();
            _receiveBuffer = new byte[4096];
            _stringBuilder = new StringBuilder();

            await _client.ConnectAsync(serverIP, port);
            _stream = _client.GetStream();

            Debug.Log("서버에 연결되었습니다.");

            _mainThreadActions.Enqueue(SpawnMyPlayer);

            _ = Task.Run(SendMessageLoop);
            _ = Task.Run(ReceiveDataLoop);
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e.Message}");
            Cleanup();
        }
    }

    private void Cleanup()
    {
        if (_client == null) return;

        try { _stream?.Close(); } catch { /* ignore */ }
        try { _client?.Close(); } catch { /* ignore */ }

        _stream = null;
        _client = null;
        _mainThreadActions.Enqueue(CleanupGameObjects);
        Debug.Log("클라이언트 연결을 종료하고 리소스를 정리했습니다.");
    }

    #endregion

    #region Network Communication (Background Threads)

    private async Task SendMessageLoop()
    {
        try
        {
            while (_client != null && _client.Connected)
            {
                string jsonToSend = null;
                lock (_playerDataLock)
                {
                    jsonToSend = _latestPlayerDataJson;
                }

                if (!string.IsNullOrEmpty(jsonToSend))
                {
                    byte[] data = Encoding.UTF8.GetBytes(jsonToSend + '\n');
                    await _stream.WriteAsync(data, 0, data.Length);
                }

                await Task.Delay((int)(sendIntervalSeconds * 1000));
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"메시지 전송 중 오류 발생: {e.Message}");
        }
    }

    private async Task ReceiveDataLoop()
    {
        try
        {
            while (_client != null && _client.Connected)
            {
                int bytesRead = await _stream.ReadAsync(_receiveBuffer, 0, _receiveBuffer.Length);
                if (bytesRead == 0) break;

                string receivedChunk = Encoding.UTF8.GetString(_receiveBuffer, 0, bytesRead);
                _stringBuilder.Append(receivedChunk);

                ProcessReceivedData();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"데이터 수신 중 오류 발생: {e.Message}");
        }
        finally
        {
            // 수신 루프가 끝나면 연결이 끊긴 것이므로 모든 리소스를 정리합니다.
            Cleanup();
        }
    }

    private void ProcessReceivedData()
    {
        string allData = _stringBuilder.ToString();
        int separatorIndex;

        while ((separatorIndex = allData.IndexOf('\n')) != -1)
        {
            string message = allData.Substring(0, separatorIndex);
            allData = allData.Substring(separatorIndex + 1);

            if (!string.IsNullOrWhiteSpace(message))
            {
                HandleMessage(message);
            }
        }

        _stringBuilder.Clear();
        _stringBuilder.Append(allData);
    }

    private void HandleMessage(string message)
    {
        try
        {
            NetworkMessage netMessage = JsonUtility.FromJson<NetworkMessage>(message);
            if (netMessage?.data == null) return;

            switch (netMessage.type)
            {
                case "update":
                    // 자신의 위치 정보 업데이트는 무시합니다.
                    if (netMessage.data.addr != null && netMessage.data.addr != _client.Client.LocalEndPoint.ToString())
                    {
                        _mainThreadActions.Enqueue(() => UpdateUserObject(netMessage.data));
                    }
                    break;
                case "disconnect":
                    _mainThreadActions.Enqueue(() => RemoveUserObject(netMessage.data.addr));
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"메시지 처리 실패 (잘못된 형식): {message}, 오류: {e.Message}");
        }
    }

    #endregion

    #region Game Logic (Main Thread)

    /// <summary>
    /// 네트워크로 전송할 데이터를 메인 스레드에서 준비합니다.
    /// </summary>
    private void PrepareDataForSending()
    {
        if (_myPlayerObject != null && _client != null && _client.Connected)
        {
            var user = new UserData
            {
                addr = _client.Client.LocalEndPoint.ToString(),
                pos = _myPlayerObject.transform.position,
                rot = _myPlayerObject.transform.eulerAngles
            };
            
            string msg = JsonUtility.ToJson(user);
            lock (_playerDataLock)
            {
                _latestPlayerDataJson = msg;
            }
        }
    }

    private void SpawnMyPlayer()
    {
        if (_myPlayerObject != null) Destroy(_myPlayerObject);
        
        _myPlayerObject = Instantiate(playerPrefab, transform.position, transform.rotation);

        PlayerController controller = _myPlayerObject.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.isLocalPlayer = true;
        }
        else
        {
            Debug.LogWarning("주의: playerPrefab에 PlayerController.cs 스크립트가 없습니다!");
        }

        Debug.Log("내 플레이어가 생성되었습니다.");
    }

    private void UpdateUserObject(UserData user)
    {
        if (_userObjects.ContainsKey(user.addr))
        {
            GameObject existingUser = _userObjects[user.addr];
            existingUser.transform.position = user.pos;
            existingUser.transform.rotation = Quaternion.Euler(user.rot);
        }
        else
        {
            Debug.Log($"{user.addr}가 새로 접속했습니다.");
            GameObject newObj = Instantiate(playerPrefab);
            newObj.transform.position = user.pos;
            newObj.transform.rotation = Quaternion.Euler(user.rot);
            _userObjects.Add(user.addr, newObj);

            // --- 카메라 문제 해결을 위해 추가된 코드 ---
            // 다른 플레이어의 캐릭터에 있는 카메라와 오디오 리스너는 비활성화합니다.
            // 씬에는 오직 하나의 활성화된 카메라와 오디오 리스너만 있어야 하기 때문입니다.
            Camera remoteCamera = newObj.GetComponentInChildren<Camera>();
            if (remoteCamera != null)
            {
                remoteCamera.gameObject.SetActive(false);
            }

            AudioListener remoteListener = newObj.GetComponentInChildren<AudioListener>();
            if (remoteListener != null)
            {
                remoteListener.enabled = false;
            }
        }
    }

    private void RemoveUserObject(string addr)
    {
        if (addr == null) return;
        if (_userObjects.TryGetValue(addr, out GameObject userObject))
        {
            Destroy(userObject);
            _userObjects.Remove(addr);
            Debug.Log($"{addr}의 접속이 종료되어 오브젝트를 삭제했습니다.");
        }
    }

    private void CleanupGameObjects()
    {
        if (_myPlayerObject != null)
        {
            Destroy(_myPlayerObject);
            _myPlayerObject = null;
        }

        foreach (var userObject in _userObjects.Values)
        {
            Destroy(userObject);
        }
        _userObjects.Clear();
        
        Debug.Log("모든 플레이어 오브젝트를 정리했습니다.");
    }

    #endregion
}
