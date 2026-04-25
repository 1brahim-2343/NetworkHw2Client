using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NetworkHw2Client
{
    internal class Program
    {
        static List<AppClient>? onlineAppClients = [];
        static void Main(string[] args)
        {
            var client = new TcpClient();
            var ipAddress = IPAddress.Parse("192.168.1.71");
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
                Console.Write("ERROR: ", Console.ForegroundColor = ConsoleColor.DarkYellow);
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
                if (Helper.IsJson(result))
                {
                    onlineAppClients = JsonSerializer.Deserialize<List<AppClient>>(result);
                    string title = " ONLINE USERS ";
                    title.PrintHeader();
                    for (int i = 0; i < onlineAppClients!.Count; i++)
                    {
                        int portClient = ((IPEndPoint)client.Client.LocalEndPoint!).Port;
                        int portAppClient = int.Parse((onlineAppClients[i].ServerSideRemoteEndPoint!.Split(":"))[1]);
                        if (portClient == portAppClient)
                        {
                            string onlineText = $"{i + 1}. You are online".PadLeft(30 - title.Length / 2);
                            Console.WriteLine(onlineText, Console.ForegroundColor = ConsoleColor.Green);
                        }
                        else
                        {
                            string onlineText = $"{i + 1}. {onlineAppClients[i].Name} is online".PadLeft(30 - title.Length / 2);
                            Console.WriteLine(onlineText, Console.ForegroundColor = ConsoleColor.Green);
                        }
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



        private static void SendMessage(TcpClient client, string msg)
        {
            var bw = new BinaryWriter(client.GetStream());
            if (!msg.StartsWith("_"))
            {
                try
                {
                    bw.Write(msg);
                }
                catch (Exception)
                {
                }
            }
            switch (msg)
            {
                case "_chat":
                    {


                        bw.Write("_who");
                        Thread.Sleep(15);
                        if (onlineAppClients!.Count > 1)
                        {
                            int.TryParse(Console.ReadLine()!, out int clientChoice);
                            string remoteEP = onlineAppClients[clientChoice - 1].ServerSideRemoteEndPoint!;
                            var name = onlineAppClients[clientChoice - 1].Name;
                            while (true)
                            {
                                Console.Write($"Enter message to send to {name} OR type _exit: ");
                                string message = Console.ReadLine()!;
                                if (message == "_exit")
                                    break;
                                string stringToSend = new string(message + "\n" + remoteEP);
                                var jsonMessage = JsonSerializer.Serialize(stringToSend);

                                bw.Write(jsonMessage);

                            }

                        }
                        else
                        {
                            Console.WriteLine("No one to chat with", Console.ForegroundColor = ConsoleColor.DarkYellow);
                            break;
                        }

                        break;
                    }
                default:
                    break;
            }
        }

        private static void SendMessageToUser(TcpClient client)
        {
            var stream = client.GetStream();
            var br = new BinaryReader(stream);
        }
    }
}
