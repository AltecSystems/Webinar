using System;
using Ascon.Pilot.Server.Api;

namespace WebinarTelegram.PilotWrapper
{
    internal class ConnectionLostListener : IConnectionLostListener
    {
        private readonly Action<Exception> _action;

        public ConnectionLostListener(Action<Exception> action)
        {
            _action = action;
        }

        public void ConnectionLost(Exception ex)
        {
            _action?.Invoke(ex);
        }
    }
}