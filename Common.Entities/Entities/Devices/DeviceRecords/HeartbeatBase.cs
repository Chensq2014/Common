﻿using System;
using System.ComponentModel;

namespace Common.Entities
{
    /// <summary>
    /// 心跳
    /// </summary>
    [Description("心跳")]
    public class HeartbeatBase : DeviceRecordBase
    {
        public HeartbeatBase()
        {
        }

        public HeartbeatBase(Guid id) : base(id)
        {
        }
    }
}
