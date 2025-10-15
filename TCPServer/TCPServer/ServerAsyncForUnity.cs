using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TCPServer
{
    // 클라이언트와 동일한 데이터 구조를 서버에도 정의합니다.
    public class UserData
    {
        public string addr { get; set; }
        public Vector3 pos { get; set; }
        public Vector3 rot { get; set; }
    }

    public class Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public override string ToString() => $"({x:F1}, {y:F1}, {z:F1})";
    }

    internal class ServerAsyncForUnity
    {
        private static readonly ConcurrentBag<ClientHandler> _clients = new ConcurrentBag<ClientHandler>();
        // --- 콘솔 출력 관리를 위한 정적 변수 ---
        private static int _nextConsoleLine = 1; // 0번 줄은 서버 시작 메시지를 위해 비워둠
        internal static readonly object _consoleLock = new object(); // 콘솔 동시 접근 방지용 락

        static async Task Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 7777);
            try
            {
                server.Start();
                Console.WriteLine("[서버] 비동기 서버가 시작되었습니다. 클라이언트 접속 대기 중...");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    
                    lock (_consoleLock)
                    {
                        // 새로운 클라이언트를 위한 핸들러 생성 및 시작 (콘솔 라인 번호 할당)
                        var clientHandler = new ClientHandler(client, _nextConsoleLine++);
                        _clients.Add(clientHandler);
                        _ = clientHandler.RunAsync();
                    }
                }
            }
            finally
            {
                server.Stop();
            }
        }

        public static async Task BroadcastMessageAsync(string message, ClientHandler sender)
        {
            foreach (var client in _clients)
            {
                if (client == sender) continue;
                await client.SendMessageAsync(message);
            }
        }
        
        public static void RemoveClient(ClientHandler clientHandler) { /* No-op for ConcurrentBag */ }
    }

    internal class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly byte[] _receiveBuffer = new byte[4096];
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly int _consoleLine;
        private readonly string _clientEndpoint;

        // NetworkMessage 클래스를 서버에도 추가
        public class NetworkMessage
        {
            public string type { get; set; }
            public UserData data { get; set; }
        }

        public ClientHandler(TcpClient client, int consoleLine)
        {
            _client = client;
            _stream = client.GetStream();
            _consoleLine = consoleLine;
            _clientEndpoint = client.Client.RemoteEndPoint.ToString();
        }

        public async Task RunAsync()
        {
            try
            {
                while (true)
                {
                    int bytesRead = await _stream.ReadAsync(_receiveBuffer, 0, _receiveBuffer.Length);
                    if (bytesRead == 0) break;

                    string receivedChunk = Encoding.UTF8.GetString(_receiveBuffer, 0, bytesRead);
                    _stringBuilder.Append(receivedChunk);

                    ProcessReceivedData();
                }
            }
            catch { /* ignore */ }
            finally
            {
                var disconnectMessage = new NetworkMessage
                {
                    type = "disconnect",
                    data = new UserData { addr = _clientEndpoint }
                };
                string disconnectJson = JsonSerializer.Serialize(disconnectMessage);
                await ServerAsyncForUnity.BroadcastMessageAsync(disconnectJson + '\n', this);
                
                ClearConsoleLine(_consoleLine, $"클라이언트 접속 종료: {_clientEndpoint}");

                ServerAsyncForUnity.RemoveClient(this);
                _client.Close();
            }
        }

        private void ProcessReceivedData()
        {
            string allData = _stringBuilder.ToString();
            int separatorIndex;

            while ((separatorIndex = allData.IndexOf('\n')) != -1)
            {
                string message = allData.Substring(0, separatorIndex + 1); // 개행문자 포함
                allData = allData.Substring(separatorIndex + 1);

                if (string.IsNullOrWhiteSpace(message)) continue;

                // 서버는 메시지 내용을 해석하지 않고 그대로 브로드캐스트합니다.
                Console.WriteLine($"Relaying message from {_clientEndpoint}");
                _ = ServerAsyncForUnity.BroadcastMessageAsync(message, this);
            }

            _stringBuilder.Clear();
            _stringBuilder.Append(allData);
        }

        private static void UpdateConsoleLine(int line, string text)
        {
            lock (ServerAsyncForUnity._consoleLock)
            {
                Console.SetCursorPosition(0, line);
                Console.Write(text + new string(' ', Console.WindowWidth - text.Length - 1));
            }
        }

        private static void ClearConsoleLine(int line, string message)
        {
            lock (ServerAsyncForUnity._consoleLock)
            {
                Console.SetCursorPosition(0, line);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, line);
                Console.WriteLine(message);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (!_client.Connected) return;
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
            catch { /* ignore */ }
        }
    }
}

