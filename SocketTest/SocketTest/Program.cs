using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var cancel = new CancellationTokenSource();
            var listener = new TcpListener(IPAddress.Any, 17000);
            listener.Start();

            Task.Factory.StartNew(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        client.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server - {ex.Message}");
                    }
                }
            });

            Task.Factory.StartNew(async () =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        var client = new TcpClient();
                        await client.ConnectAsync("localhost", 17000);
                        client.Dispose();
                        await Task.Delay(100, cancel.Token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Client - {ex.Message}");
                    }
                }
            });

            Console.ReadLine();
            cancel.Cancel();
            listener.Stop();
        }

    }
}