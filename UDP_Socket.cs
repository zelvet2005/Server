using System.Net.Sockets;
using System.Net;

namespace ServerTCP_UDP
{
    struct UDP_Socket
    {
        public IPAddress IP { get; set; } 
        public int PORT { get; set; }
        public IPEndPoint udpEndPoint { get; }
        public Socket udpSocket { get; }
        public UDP_Socket(IPAddress ipAddress, int port)
        {
            IP = ipAddress;
            PORT = port;
            udpEndPoint = new IPEndPoint(IP, PORT);
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
    }
}