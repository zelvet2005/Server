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
            const int PORT = 8080;

            Console.OutputEncoding = Encoding.UTF8;

            ConnectTCP(IPAddress.Parse(IP), PORT);

            Console.ReadLine();
        }
        public static void ConnectTCP(IPAddress IP, int PORT)
        {
            var endPoint = new IPEndPoint(IP, PORT);

            Console.WriteLine("З'єднання успішне, введіть 'help' щоб переглянути команди"); 

            while (true)
            {
                var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                tcpSocket.Connect(endPoint);

                Console.Write("Введіть повідомлення: ");
                var message = Console.ReadLine();

                var data = Encoding.UTF8.GetBytes(message);

                tcpSocket.Send(data);

                if (message.ToLower().Equals("end"))
                {
                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();
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
                var answer = new StringBuilder();

                do
                {
                    size = tcpSocket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (tcpSocket.Available > 0);

                Console.WriteLine(answer);

                tcpSocket.Shutdown(SocketShutdown.Both);
                tcpSocket.Close();
            }
        }
    }
}