using System;
using System.Net.Sockets;
using System.Threading;

namespace ClientsImitation
{
    class Program
    {
        static void Main(string[] args)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Connect("Command1");
            }).Start();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Connect("Command2");
            }).Start();

            Console.ReadKey(true);
        }

        static void Connect(string message)
        {
            try
            {
                int port = 9899;
                TcpClient client = new TcpClient("localhost", port);
                NetworkStream stream = client.GetStream();
                int count = 0;
                while (count++ < 2)
                {                    
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Отправка сообщения
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent: {0}", message);

                    data = new byte[256];
                    string response = string.Empty;
                    
                    // Чтение ответа от сервера
                    int bytes = stream.Read(data, 0, data.Length);
                    response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    Console.WriteLine("Received: {0}", response);
                    Thread.Sleep(200);
                }
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }            
        }
    }
}
