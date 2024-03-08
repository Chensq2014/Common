using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Common.TcpMudule.Interfaces
{
    public interface IForwardHandler : IHandlerType
    {
        Task<bool> Handler(string wrapper);
    }
}
