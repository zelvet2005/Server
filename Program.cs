using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main()
        {
            const string IP = "127.0.0.1";
            const int PORT = 8082;
            const int SERVER_PORT = 8081;

            Console.OutputEncoding = Encoding.UTF8;

            ConnectUDP(IPAddress.Parse(IP), PORT, SERVER_PORT);

            Console.ReadLine();
        }
        static void ConnectUDP(IPAddress ip, int port, int serverPort)
        {
            var endPoint = new IPEndPoint(ip, port);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(endPoint);

            Console.WriteLine("З'єднання успішне, введіть 'help' щоб переглянути команди");

            while (true)
            {
                Console.Write("Введіть повідомлення: ");
                var message = Console.ReadLine();

                var serverEndPoint = new IPEndPoint(ip, serverPort);
                udpSocket.SendTo(Encoding.UTF8.GetBytes(message), serverEndPoint);

                if (message.ToLower().Equals("end"))
                {
                    udpSocket.Shutdown(SocketShutdown.Both);
                    udpSocket.Close();
                    Environment.Exit(0);
                    break;
                }
                else if (message.ToString().ToLower().Equals("help"))
                {
                    Console.WriteLine("clean - очистка історії запитів\nhistory - перегляд історія запитів\nend - закриття з'єднання");
                    continue;
                }

                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                EndPoint senderEndPoint = new IPEndPoint(ip, port);

                do
                {
                    size = udpSocket.ReceiveFrom(buffer, ref senderEndPoint);
                    data.Append(Encoding.UTF8.GetString(buffer));
                }
                while (udpSocket.Available > 0);

                Console.WriteLine(data);
            }
        }
    }
}