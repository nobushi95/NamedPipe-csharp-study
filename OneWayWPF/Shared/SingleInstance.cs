using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    internal class SingleInstance : IDisposable
    {
        private string _mutexName { get; init; }

        private Mutex? _mutex = default;

        public SingleInstance(string mutexName) => _mutexName = mutexName;

        ~SingleInstance() => Dispose();

        public bool IsRunning()
        {
            if (_mutex is not null)
                return true;

            try
            {
                _mutex = Mutex.OpenExisting(_mutexName);
                return true;
            }
            catch
            {
                _mutex = new Mutex(true, _mutexName);
                return false;
            }
        }

        public async Task<bool> IsRunningAsync(TimeSpan timeout = default)
        {
            var cur = TimeSpan.Zero;
            while (cur.TotalMilliseconds <= timeout.TotalMilliseconds)
            {
                if (IsRunning())
                    return false;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                cur += TimeSpan.FromMilliseconds(100);
            }
            return false;
        }

        public void Dispose()
        {
            _mutex?.Dispose();
            _mutex = null;
        }

    }

}

