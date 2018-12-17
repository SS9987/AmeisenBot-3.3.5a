using AmeisenBotUtilities;
using AmeisenBotUtilities.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBot.Clients
{
    public class AmeisenNavmeshClient
    {
        private string Ip { get; set; }
        private int Port { get; set; }
        TcpClient TcpClient { get; set; }

        public AmeisenNavmeshClient(string ip, int port)
        {
            Ip = ip;
            Port = port;

            TcpClient = new TcpClient();
            TcpClient.Connect(ip, port);
        }

        ~AmeisenNavmeshClient()
        {
            TcpClient.Close();
            TcpClient.Dispose();
        }

        public List<Vector3> RequestPath(PathRequest pathRequest)
        {
            List<Vector3> path = new List<Vector3>();

            if (!TcpClient.Connected)
            {
                TcpClient.Connect(Ip, Port);
                if (!TcpClient.Connected)
                {
                    return path;
                }
            }

            StreamReader sReader = new StreamReader(TcpClient.GetStream(), Encoding.ASCII);
            StreamWriter sWriter = new StreamWriter(TcpClient.GetStream(), Encoding.ASCII);

            bool isConnected = true;
            string pathJson = "";

            while (isConnected)
            {
                sWriter.WriteLine(JsonConvert.SerializeObject(pathRequest) + " &gt;");
                sWriter.Flush();
                
                pathJson = sReader.ReadLine().Replace("&gt;", "");
                path = JsonConvert.DeserializeObject<List<Vector3>>(pathJson);
                return path;
            }

            return path;
        }
    }
}
