using System.Collections.Generic;

namespace Common.TcpMudule.Sockets
{
    public class AsyncUserTokenCollection
    {
        private readonly List<AsyncUserToken> _userTokens;
        private static object _lock = new object();

        public AsyncUserTokenCollection()
        {
            _userTokens = new List<AsyncUserToken>();
        }

        public void Add(AsyncUserToken userToken)
        {
            lock (_lock)
            {
                _userTokens.Add(userToken);
            }
        }

        public void Remove(AsyncUserToken userToken)
        {
            lock (_lock)
            {
                _userTokens.Remove(userToken);
            }
        }

        public void CopyList(ref AsyncUserToken[] array)
        {
            lock (_lock)
            {
                array = new AsyncUserToken[_userTokens.Count];
                _userTokens.CopyTo(array);
            }
        }
    }
}
