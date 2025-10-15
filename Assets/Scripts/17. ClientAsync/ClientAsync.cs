using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[Serializable]
public class NetworkMessage
{
    public string type;
    public UserData data;
}

[Serializable]
public class UserData
{
    public string addr;
    public Vector3 pos;
    public Vector3 rot;
}

public class ClientAsync : MonoBehaviour
{
    [Header("Network Settings")]
    public string serverIP = "127.0.0.1";
    public int port = 7777;
    [Tooltip("위치 정보 전송 주기 (초)")]
    public float sendIntervalSeconds = 0.1f;

    [Header("Game Settings")]
    public GameObject playerPrefab;
    
    [Header("UI (Optional)")]
    public TMP_InputField messageInput;

    private TcpClient _client;
    private NetworkStream _stream;
    private byte[] _receiveBuffer;
    private StringBuilder _stringBuilder;
    
    private Dictionary<string, GameObject> _userObjects = new Dictionary<string, GameObject>();
    private GameObject _myPlayerObject;

    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    private readonly object _playerDataLock = new object();
    private string _latestUpdateMessageJson = null;

    #region Unity Lifecycle Methods

    private void Update()
    {
        while (_mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
        PrepareUpdateMessage();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    #endregion

    #region Public Methods for other scripts

    public void SendFireMessage(Vector3 pos, Vector3 rot)
    {
        try
        {
            string myAddr = _client.Client.LocalEndPoint.ToString();

            // 1. 로컬 예측: 내 총알을 즉시 생성합니다.
            if (BulletPoolManager.Instance != null)
            {
                Bullet bullet = BulletPoolManager.Instance.GetBullet(myAddr);
                if (bullet != null)
                {
                    bullet.transform.position = pos;
                    bullet.transform.rotation = Quaternion.Euler(rot);
                }
            }
            else
            {
                Debug.LogError("BulletPoolManager.Instance가 없습니다!");
            }

            // 2. 네트워크 전송: 다른 클라이언트에게 발사 사실을 알립니다.
            var fireData = new UserData { addr = myAddr, pos = pos, rot = rot };
            var netMessage = new NetworkMessage { type = "fire", data = fireData };
            string json = JsonUtility.ToJson(netMessage);
            
            Debug.Log($"--> [SENDER] Sending FIRE message: {json}");
            
            SendMessageAsync(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"SendFireMessage failed: {e.Message}");
        }
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

            _ = Task.Run(UpdateMessageLoop);
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

    #region Network Communication

    private async Task UpdateMessageLoop()
    {
        while (_client != null && _client.Connected)
        {
            string jsonToSend = null;
            lock (_playerDataLock)
            {
                jsonToSend = _latestUpdateMessageJson;
            }

            if (!string.IsNullOrEmpty(jsonToSend))
            {
                await SendMessageAsync(jsonToSend);
            }

            await Task.Delay((int)(sendIntervalSeconds * 1000));
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
        catch (Exception e) { Debug.LogWarning($"데이터 수신 중 오류 발생: {e.Message}"); }
        finally { Cleanup(); }
    }

    private void ProcessReceivedData()
    {
        string allData = _stringBuilder.ToString();
        int separatorIndex;
        while ((separatorIndex = allData.IndexOf('\n')) != -1)
        {
            string message = allData.Substring(0, separatorIndex);
            allData = allData.Substring(separatorIndex + 1);
            if (!string.IsNullOrWhiteSpace(message)) HandleMessage(message);
        }
        _stringBuilder.Clear();
        _stringBuilder.Append(allData);
    }

    private void HandleMessage(string message)
    {
        // [디버그 로그 추가]
        Debug.Log($"<-- [RECEIVER] Received message raw: {message}");
        try
        {
            NetworkMessage netMessage = JsonUtility.FromJson<NetworkMessage>(message);
            
            if (netMessage == null)
            {
                Debug.LogWarning("--> [RECEIVER] Message parsed to null.");
                return;
            }
            
            // [디버그 로그 추가]
            Debug.Log($"--> [RECEIVER] Parsed message type: {netMessage.type}");

            switch (netMessage.type)
            {
                case "update":
                    _mainThreadActions.Enqueue(() => UpdateUserObject(netMessage.data));
                    break;
                case "disconnect":
                    _mainThreadActions.Enqueue(() => RemoveUserObject(netMessage.data.addr));
                    break;
                case "fire":
                    _mainThreadActions.Enqueue(() => SpawnRemoteBullet(netMessage.data));
                    break;
            }
        }
        catch (Exception e) { Debug.LogWarning($"메시지 처리 실패 (잘못된 형식): {message}, 오류: {e.Message}"); }
    }

    private async Task SendMessageAsync(string jsonMessage)
    {
        if (_client == null || !_client.Connected) return;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonMessage + '\n');
            await _stream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception e) { Debug.LogWarning($"메시지 전송 중 오류 발생: {e.Message}"); }
    }

    #endregion

    #region Game Logic (Main Thread)

    private void PrepareUpdateMessage()
    {
        if (_myPlayerObject != null && _client != null && _client.Connected)
        {
            var userData = new UserData
            {
                addr = _client.Client.LocalEndPoint.ToString(),
                pos = _myPlayerObject.transform.position,
                rot = _myPlayerObject.transform.eulerAngles
            };
            var netMessage = new NetworkMessage { type = "update", data = userData };
            string json = JsonUtility.ToJson(netMessage);
            lock (_playerDataLock)
            {
                _latestUpdateMessageJson = json;
            }
        }
    }

    private void SpawnMyPlayer()
    {
        if (_myPlayerObject != null) Destroy(_myPlayerObject);
        _myPlayerObject = Instantiate(playerPrefab, transform.position, transform.rotation);
        PlayerController controller = _myPlayerObject.GetComponent<PlayerController>();
        if (controller != null) controller.isLocalPlayer = true;
        else Debug.LogWarning("주의: playerPrefab에 PlayerController.cs 스크립트가 없습니다!");
        
        // Gun 스크립트가 ClientAsync를 찾을 수 있도록 설정
        Gun gun = _myPlayerObject.GetComponentInChildren<Gun>();
        if (gun != null) gun.SetClient(this);

        Debug.Log("내 플레이어가 생성되었습니다.");
    }

    private void UpdateUserObject(UserData user)
    {
        if (user == null || user.addr == _client.Client.LocalEndPoint.ToString()) return;

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

            Camera remoteCamera = newObj.GetComponentInChildren<Camera>();
            if (remoteCamera != null) remoteCamera.gameObject.SetActive(false);

            AudioListener remoteListener = newObj.GetComponentInChildren<AudioListener>();
            if (remoteListener != null) remoteListener.enabled = false;
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

    private void SpawnRemoteBullet(UserData fireData)
    {
        // 내가 쏜 총알에 대한 메시지라면 무시합니다 (이미 로컬에서 생성했기 때문).
        if (fireData.addr == _client.Client.LocalEndPoint.ToString())
        {
            return;
        }

        if (BulletPoolManager.Instance == null)
        {
            Debug.LogError("BulletPoolManager.Instance가 없습니다!");
            return;
        }

        // 다른 플레이어의 주소(addr)를 ID로 사용하여 해당 클라이언트의 풀에서 총알을 가져옵니다.
        Bullet bullet = BulletPoolManager.Instance.GetBullet(fireData.addr);
        if (bullet != null)
        {
            bullet.transform.position = fireData.pos;
            bullet.transform.rotation = Quaternion.Euler(fireData.rot);
        }
    }

    private void CleanupGameObjects()
    {
        if (_myPlayerObject != null)
        {
            Destroy(_myPlayerObject);
            _myPlayerObject = null;
        }
        foreach (var userObject in _userObjects.Values) Destroy(userObject);
        _userObjects.Clear();
        Debug.Log("모든 플레이어 오브젝트를 정리했습니다.");
    }

    #endregion
}
