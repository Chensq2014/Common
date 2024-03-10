using System;

namespace Common.Locks
{
    /// <summary>
    /// 检查用户锁
    /// </summary>
    public interface IUserLock : IDisposable
    {
        /// <summary>
        /// 检查用户锁
        /// </summary>
        /// <returns></returns>
        IUserLock CheckLock();
    }
}
