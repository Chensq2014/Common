using System;
using System.ComponentModel;
using Common.Entities.Devices;

namespace Common.Entities
{
    /// <summary>
    /// 设备记录父类
    /// </summary>
    [Description("设备记录父类")]
    public class DeviceRecordBase : DeviceRecord
    {
        public DeviceRecordBase()
        {
        }

        public DeviceRecordBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual Device Device { get; set; }
    }
}
