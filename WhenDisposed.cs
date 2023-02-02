using System;

namespace Ideka.BHUDCommon
{
    public readonly struct WhenDisposed : IDisposable
    {
        private readonly Action _act;

        public WhenDisposed(Action act)
        {
            _act = act;
        }

        public void Dispose()
        {
            _act();
        }
    }
}
