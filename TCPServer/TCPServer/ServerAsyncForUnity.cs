using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    internal class ServerAsyncForUnity
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static async Task Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("192.168.10.95");
            TcpListener server = new TcpListener(ip, 7777);
            server.Start();
            Console.WriteLine("비동기 서버가 시작되었습니다. 클라이언트 접속 대기 중...");

            while (true)
            {
                // 클라이언트 접속을 비동기적으로 대기
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("클라이언트 접속!");

                clients.Add(client);

                await BroadcastMessageAsync($"사용자 {clients.Count - 1}({client.Client.RemoteEndPoint}) 입장!", client);

                // 각 클라이언트를 별도의 태스크로 처리
                _ = HandleClientAsync(client);
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    // 데이터 수신을 비동기적으로 대기
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // 클라이언트가 연결을 끊음

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"수신: {data}");

                    // 에코 메시지를 비동기적으로 전송
                    //byte[] msg = Encoding.UTF8.GetBytes(data);
                    //await stream.WriteAsync(msg, 0, msg.Length);
                    //Console.WriteLine($"송신: {data}");

                    await BroadcastMessageAsync(data, client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"클라이언트 처리 중 오류: {e.Message}");
            }
            finally
            {
                // 클라이언트가 방에서 나감
                await BroadcastMessageAsync($"사용자{clients.IndexOf(client)}가 방에서 나갔습니다.", client);
                clients.Remove(client);

                stream.Close();
                client.Close();
                Console.WriteLine("클라이언트 연결 종료.");
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
                if (client.Connected)
                {
                    await client.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
                }
            }
        }
    }
}
