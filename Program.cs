using ServerTCP_UDP;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main()
        {
            const string IP = "127.0.0.1";
            const int TCP_PORT = 8080;
            const int UDP_PORT = 8081;

            Console.OutputEncoding = Encoding.UTF8;

            var tcp = new TCP_Socket(IPAddress.Parse(IP), TCP_PORT);
            var udp = new UDP_Socket(IPAddress.Parse(IP), UDP_PORT);
            
            TCPAsync(tcp);
            UDPAsync(udp);
            
            Console.ReadLine();
        }
        static async void TCPAsync(TCP_Socket socket)
        {
            await Task.Run(() => TryTCP(socket));
        }
        static void TryTCP(TCP_Socket socket)
        {
            try
            {
                TCP(socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка! " + ex.Message);
            }
        }
        static void TCP(TCP_Socket socket)
        {
            socket.tcpSocket.Bind(socket.tcpEndPoint);
            socket.tcpSocket.Listen(5);

            while (true)
            {
                var listener = socket.tcpSocket.Accept();

                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (socket.tcpSocket.Available > 0);

                if (data.ToString().ToLower().Equals("end"))
                {
                    Console.WriteLine("TCP connection closed");
                    break;
                }
                else if (!CheckCommandTCP(data.ToString().ToLower(), false))
                {
                    Console.WriteLine($"TCP notification: {data}");
                    listener.Send(Encoding.UTF8.GetBytes("Повідомлення відправлено успішно"));
                }
                else
                {
                    CheckCommandTCP(data.ToString().ToLower());
                }

                WorkWithFileTCP(data, listener);
            }
        }
        static void WorkWithFileTCP(StringBuilder data, Socket listener)
        {
            if (data.ToString().ToLower().Equals("history"))
            {
                using (var sw = new StreamWriter("tcp.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine($"Історія за {DateTime.Now}");
                }
                using (var sr = new StreamReader("tcp.txt", Encoding.UTF8))
                {
                    listener.Send(Encoding.UTF8.GetBytes(sr.ReadToEnd()));
                }
            }
            else if (data.ToString().ToLower().Equals("clean"))
            {
                using (var stream = File.Open("tcp.txt", FileMode.Truncate))
                {
                    stream.SetLength(0);
                }
                listener.Send(Encoding.UTF8.GetBytes("Історія успішно видалена"));
            }
            else if (!data.ToString().ToLower().Equals("help"))
            {
                using (var sw = new StreamWriter("tcp.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(data);
                }
            }
        }
        static bool CheckCommandTCP(string command, bool execute = true)
        {
            string[] commands = { "help", "history", "clean" };
            if (!commands.Contains(command))
            {
                return false;
            } 

            if (execute)
            {
                if (command.Equals("help"))
                {
                    Console.WriteLine("TCP user used 'help' command");
                }
                else if (command.Equals("history"))
                {
                    Console.WriteLine("TCP user has requested history");
                }
                else if (command.Equals("clean"))
                {
                    Console.WriteLine("TCP user removed history");
                }
            }

            return true;
        }
        static async void UDPAsync(UDP_Socket socket)
        {
            await Task.Run(() => TryUDP(socket));
        }
        static void TryUDP(UDP_Socket socket)
        {
            try
            {
                UDP(socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка! " + ex.Message);
            }
        }
        static void UDP(UDP_Socket socket)
        {
            socket.udpSocket.Bind(socket.udpEndPoint);

            while (true)
            {
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                do
                {
                    size = socket.udpSocket.ReceiveFrom(buffer, ref sender);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (socket.udpSocket.Available > 0);
                
                if (data.ToString().ToLower().Equals("end"))
                {
                    Console.WriteLine("UDP connection closed");
                    break;
                }
                else if (!CheckCommandUDP(data.ToString().ToLower(), false))
                {
                    Console.WriteLine($"UDP Notification: {data}");
                    socket.udpSocket.SendTo(Encoding.UTF8.GetBytes("Повідомлення відправлено успішно"), sender);
                }
                else
                {
                    CheckCommandUDP(data.ToString().ToLower());
                }

                WorkWithFileUDP(data, socket, sender);
            }
            socket.udpSocket.Shutdown(SocketShutdown.Both);
            socket.udpSocket.Close();
        }
        static void WorkWithFileUDP(StringBuilder data, UDP_Socket socket, EndPoint sender)
        {
            if (data.ToString().ToLower().Equals("history"))
            {
                using (var sw = new StreamWriter("udp.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine($"Історія за {DateTime.Now}");
                }
                using (var sr = new StreamReader("udp.txt", Encoding.UTF8))
                {
                    socket.udpSocket.SendTo(Encoding.UTF8.GetBytes(sr.ReadToEnd()), sender);
                }
            }
            else if (data.ToString().ToLower().Equals("clean"))
            {
                using (var stream = File.Open("udp.txt", FileMode.Truncate))
                {
                    stream.SetLength(0);
                }
                socket.udpSocket.SendTo(Encoding.UTF8.GetBytes("Історія успішно видалена"), sender);
            }
            else if (!data.ToString().ToLower().Equals("help"))
            {
                using (var sw = new StreamWriter("udp.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(data);
                }
            }
        }
        static bool CheckCommandUDP(string command, bool execute = true)
        {
            string[] commands = { "help", "history", "clean" };
            if (!commands.Contains(command))
            {
                return false;
            }
            if (execute)
            {
                if (command.Equals("help"))
                {
                    Console.WriteLine("UDP user used 'help' command");
                }
                else if (command.Equals("history"))
                {
                    Console.WriteLine("UDP user has requested history");
                }
                else if (command.Equals("clean"))
                {
                    Console.WriteLine("UDP user removed history");
                }
            }
            return true;
        }
    }
}