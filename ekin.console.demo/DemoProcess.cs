using System;
using System.Threading;

namespace ekin.console.demo
{
    public class DemoProcess : IDemoProcess
    {
        public void Execute(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine("Process is running...");
                
                Thread.Sleep(5000);
            }
        }
    }
}