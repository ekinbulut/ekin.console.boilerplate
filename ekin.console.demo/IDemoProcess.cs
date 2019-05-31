using System.Threading;

namespace ekin.console.demo
{
    public interface IDemoProcess
    {
        void Execute(CancellationToken token);
    }
}