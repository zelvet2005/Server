using System.Net;
using System.Net.Sockets;

namespace ServerTCP_UDP
{
    struct TCP_Socket
    {
        public IPAddress IP { get; set; }
        public int PORT { get; set; }
        public IPEndPoint tcpEndPoint { get; }
        public Socket tcpSocket { get; }
        public TCP_Socket(IPAddress ipAddress, int port)
        {
            IP = ipAddress;
            PORT = port;
            tcpEndPoint = new IPEndPoint(IP, PORT);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }
}