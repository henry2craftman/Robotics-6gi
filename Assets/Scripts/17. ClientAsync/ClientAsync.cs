using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// 목표: 서버와 비동기 통신을 하는 클라이언트(non-blocking, 네크워크용 스레드 사용)
public class ClientAsync : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int port = 7777;

    // 서버로 메시지 전송을 위한 input
    public TMP_InputField messageInput;

    TcpClient client;
    NetworkStream stream;

    // 1. 서버 연결을 위한 비동기 메서드
    public async void ConnectToServer()
    {
        client = new TcpClient();

        // 비동기로 서버와 연결(비동기로 다른 스레드에서 작동, Unity를 멈추지 않음)
        await client.ConnectAsync(serverIP, port);

        stream = client.GetStream();

        _ = ReceiveDataFromServer();

        Debug.Log("서버에 연결되었습니다.");
    }


    public class UserData
    {
        public string addr;
        public Vector3 pos;
        public Vector3 rot;
    }
    UserData user = new UserData();
    List<UserData> users = new List<UserData>();

    // 2. 서버로 메시지를 보내는 비동기 메서드
    public async void SendMessage()
    {
        while (client.Connected)
        {
            // string msg = messageInput.text;
            user.addr = client.Client.LocalEndPoint.ToString();
            user.pos = transform.position;
            user.rot = transform.eulerAngles;

            string msg = JsonUtility.ToJson(user);

            // 바이트 배열로 변환
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");

            // 데이터를 스트림에 써주기
            await stream.WriteAsync(data, 0, data.Length);

            Debug.Log($"송신: {msg}");
            // 데이터 형식: Vector3, Quternion

            await Task.Delay(1000);
        }
    }

    private Dictionary<string, GameObject> userObjects = new Dictionary<string, GameObject>();
    // 3. 서버로 부터 계속 데이터를 받는 비동기 메서드
    public async Task ReceiveDataFromServer()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.LogWarning("서버와 연결이 끊겼습니다.");
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if(message.Contains("사용자"))
                {
                    continue;
                }

                if (message.Contains(client.Client.LocalEndPoint.ToString()))
                {
                    continue;
                }
                else
                {
                    UserData user = JsonUtility.FromJson<UserData>(message);

                    // 192.168.10.95:6666
                    string[] userAddress = user.addr.Split(':');

                    // 유저 정보가 이미 있으면, 위치 정보만 업데이트
                    if (userObjects.ContainsKey(user.addr))
                    {
                        GameObject existingUser = userObjects[user.addr];
                        existingUser.transform.position = user.pos;
                        existingUser.transform.rotation = Quaternion.Euler(user.rot);
                    }
                    // 없으면 새로운 유저 게임오브젝트 생성
                    else
                    {
                        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        newObj.transform.position = user.pos;
                        newObj.transform.rotation = Quaternion.Euler(user.rot);

                        userObjects.Add(user.addr, newObj);

                        Debug.Log($"{user.addr}가 접속했습니다.");
                    }
                }
                
                Debug.Log($"수신: {message}");
            }
        }
        catch(Exception ex)
        {
            Debug.LogWarning($"서버가 종료되었습니다. {ex.Message}");
        }

        stream.Close();
        client.Close();
        Debug.Log("클라이언트를 종료합니다.");
    }
}
