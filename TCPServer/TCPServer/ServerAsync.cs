using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

// 서버에 접속한 사람을 알리고, 다른 클라이언트에게 메시지 동기화
namespace TCPServer
{
    internal class ServerAsync
    {
        // 접속된 클라이언트 목록
        static List<TcpClient> clients = new List<TcpClient>();

        static async Task Main2()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            // 1. 서버 작동
            TcpListener server = new TcpListener(ipAddress, port);
            server.Start();
            Console.WriteLine($"비동기 서버가 {port}에서 시작되었습니다. " +
                $"클라이언트 접속 대기중...");

            while (true)    
            {
                // 2. 클라이언트 접속을 비동기적으로 대기
                TcpClient client = await server.AcceptTcpClientAsync();

                // 클라이언트를 리스트에 추가
                clients.Add(client);

                string clientId = client.Client.RemoteEndPoint.ToString();

                Console.WriteLine($"{clientId} 클라이언트 접속!");

                await BroadcastMessageAsync($"{clientId} 클라이언트 접속!\n", client);

                // 3. 각 클라이언트의 스트림을 태스크로 처리
                // * 계산이 오래 걸리는 메서는 스레드로 처리
                HandleClientAsync(client);
            }
        }

        static async void HandleClientAsync(TcpClient client)
        {
            // 4. 데이터를 읽고 쓰기위한 클라이언트의 스트림 가져오기
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while (true)
                {
                    // 5. 데이터의 수신을 비동기적으로 처리
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    // 6. 바이트 배열을 문자열로 인코딩
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"수신: {data}");


                    // 7. 받은 데이터에 따라 결과를 전달해줌
                    byte[] answer = Encoding.UTF8.GetBytes($"{data} from Server");
                    await stream.WriteAsync(answer, 0, answer.Length);
                    Console.WriteLine($"송신: {data}");

                    // 받은 메시지를 모든 클라이언트에게 전달해줌.
                    string newData = $"{client.Client.RemoteEndPoint}: {data}\n";
                    await BroadcastMessageAsync(newData, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 {ex}");
            }
            finally
            {
                // 클라이언트 방에서 나감
                clients.Remove(client);
                await BroadcastMessageAsync($"{client.Client.RemoteEndPoint}가 나갔습니다.\n", client);

                // 8. 스트림, 클라이언트 연결해제
                stream.Close();
                client.Close();
                Console.WriteLine("클라이언트 연결 종료");
            }
        }

        // 접속한 모든 클라이언트에게 메시지를 보내는 메서드
        static async Task BroadcastMessageAsync(string msg, TcpClient sender)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);

            // 클라이언트 목록을 순회하며 메시지 보내기
            foreach (var client in clients)
            {
                if (client == sender)
                    continue;

                // 목록의 클라이언트가 연결된 상태라면
                if(client.Connected)
                {
                    await client.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
                }
            }
        }
    }
}
