// 목표: 서버에 접속, 데이터를 보내고 받는다.
using System.Net.Sockets;
using System.Text;

class Program
{
    static async Task Main()
    {
        string serverIP = "192.168.10.95";
        int port = 5000;

        // 1. 서버에 접속(서버가 켜져 있다면)
        TcpClient client = new TcpClient(serverIP, port);
        Console.WriteLine($"서버({port}번 포트)에 접속했습니다.");

        // 2. 데이터 전송을 위한 스트림 정의
        NetworkStream stream = client.GetStream();

        Task task = ReceiveMessageAsync(stream);

        while (true)
        {
            Console.WriteLine("보낼 메시지를 입력해 주세요. (종료: q): ");
            string msg = Console.ReadLine();

            if(msg == "q")
                break;

            // 3. 바이트 배열로 문자열 인코딩
            byte[] data = Encoding.UTF8.GetBytes(msg);

            // 4. 서버에 메시지 전송
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"송신: {msg}");

            // 5. 서버로부터 에코 메시지 수신
            //byte[] buffer = new byte[1024];
            //int bytesRead = stream.Read(buffer, 0, buffer.Length);
            //string responseFromServer = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            //Console.WriteLine($"수신: {responseFromServer}");
        }

        stream.Close();
        client.Close();

        Console.WriteLine("클라이언트를 종료합니다...");
    }

    // 서버로 부터 메시지를 계속 수신하여 콘솔에 출력
    static async Task ReceiveMessageAsync(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteRead == 0)
                {
                    Console.WriteLine("서버와의 연결이 끊어졌습니다.");
                    break;
                }

                string responseData = Encoding.UTF8.GetString(buffer, 0, byteRead);
                Console.WriteLine(responseData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("서버가 종료되었습니다. " + ex.ToString());
        }
    }
}