using System;
using System.Collections.Generic;
using System.Text;
using Common.Entities;

namespace Common.Dtos
{
    public class TicketWrapper
    {
        public Ticket Ticket { get; set; }

        public List<LicenseResource> LicenseResources { get; set; }

        /// <summary>
        /// 缓存预计过期时间
        /// </summary>
        public DateTime ExpiredAt { get; set; }

        /// <summary>
        /// 调用次数
        /// </summary>
        public long RequestCount { get; set; }
    }
}
