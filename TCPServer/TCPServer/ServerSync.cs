// 목표: 서버와 클라이언트가 동기화 되어서 통신한다.(블로킹 방식)
using System.Net;
using System.Net.Sockets;
using System.Text;

class ServerSync
{
    static void Main2()
    {
        IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        int port = 5000;

        TcpListener server = new TcpListener(localAddress, port);

        // 1. 서버 시작
        server.Start();
        Console.WriteLine($"서버가 로컬주소 {port}포트 에서 시작되었습니다. " +
            $"클라이언트 접속 대기중...");

        // 2. 클라이언트 연결 대기(프로그램 멈춤)
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("클라이언트가 접속했습니다!");

        // 3. 클라이언트와 데이터를 주고받기위한 스트림 받기
        NetworkStream stream = client.GetStream();

        // 4. 스트림의 정보를 저장하기 위한 버퍼 만들기
        byte[] buffer = new byte[1024];
        int bytesRead = 0;

        // 5. 클라이언트로 부터 메시지 수신
        while((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            // 6. 저장된 버퍼의 데이터를 문자열로 바꾸기
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"수신: {data}");

            // 7. 에코: 받은 메시지에 따른 답을 클라이언트에게 보내기
            byte[] answer = Encoding.UTF8.GetBytes($"{data} from Server");
            stream.Write(answer, 0, answer.Length);
            Console.WriteLine($"송신 {data}");
        }

        // 8. 클라이언트 연결 종료
        client.Close();

        // 9. 서버 연결 종료
        server.Stop();
        Console.WriteLine("서버를 종료합니다.");
    }
}