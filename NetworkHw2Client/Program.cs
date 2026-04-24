using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NetworkHw2Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new TcpClient();
            var ipAddress = IPAddress.Parse("192.168.0.239");
            var port = 44000;
            var ep = new IPEndPoint(ipAddress, port);
            Console.Write("Enter name: ");
            string name = Console.ReadLine()!;
            Task.Run(() => { ConnectServer(ep, client, name); });
            Task.Delay(10);
            while (true)
            {
                string msg = Console.ReadLine()!;
                SendMessage(client, msg);
            }
        }

        private static void ConnectServer(IPEndPoint ep, TcpClient client, string name)
        {
            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    Console.WriteLine("Connected successfully", Console.ForegroundColor = ConsoleColor.Green);
                    Console.ResetColor();
                    
                    var stream = client.GetStream();
                    var bw = new BinaryWriter(stream);
                    bw.Write(name ?? "Unknown");
                    ServerReader(client);
                }
            }
            catch (Exception ex)
            {
                Console.Write("Server disconnected: ",Console.ForegroundColor = ConsoleColor.DarkYellow);
                Console.WriteLine(ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                Console.ResetColor();
                Environment.Exit(0);
            }
        }
        private static void ServerReader(TcpClient client)
        {
            var stream = client.GetStream();
            var br = new BinaryReader(stream);
            while (true)
            {
                var result = br.ReadString();
                if (IsJson(result))
                {
                    var onlineUsers = JsonSerializer.Deserialize<List<AppClient>>(result);
                    foreach (var user in onlineUsers!)
                    {
                        Console.WriteLine($"{
                            ((user.RemoteEndPoint == client.Client.RemoteEndPoint!.ToString()) ?
                            user.Name + " is" : "You are")
                            } online",Console.ForegroundColor = ConsoleColor.Green);
                        Console.ResetColor();
                    }
                }
                else if (!result.StartsWith('_'))
                {
                    Console.WriteLine($"Message from server: {result}");
                }
                else
                    continue;
            }

        }

        private static bool IsJson(string result)
        {
            if (result.Trim().StartsWith('{') || result.Trim().StartsWith('[') &&
                result.Trim().EndsWith('}') || result.Trim().EndsWith(']'))
                return true;
            return false;
        }

        private static void SendMessage(TcpClient client, string msg)
        {
            try
            {
                var bw = new BinaryWriter(client.GetStream());
                bw.Write(msg);
            }
            catch (Exception)
            {
            }
        }
    }
}
