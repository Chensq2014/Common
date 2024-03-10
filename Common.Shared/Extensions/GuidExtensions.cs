using Microsoft.Extensions.Options;
using System;
using Volo.Abp.Guids;

namespace Common.Extensions
{
    /// <summary>
    /// guid扩展
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// 根据Guid获取唯一数字序列(二进制数组就是唯一的转为long即可)
        /// </summary>
        /// <returns></returns>
        public static long ToInt64()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(),0);
        }

        /// <summary>
        /// NewIdString  
        /// </summary>
        /// <returns></returns>
        public static string NewIdString()
        {
            return Guid.NewGuid().ToString("N");
        }


        public static Guid GetSequentialGuid()
        {
            var option = new AbpSequentialGuidGeneratorOptions
            {
                //DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd//sqlserver=>SequentialAtEnd
                //DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary
                DefaultSequentialGuidType = SequentialGuidType.SequentialAsString//mysql/pgsql=>SequentialAsString
            };
            var optionWarpper = new OptionsWrapper<AbpSequentialGuidGeneratorOptions>(option);
            ////由timeStamp二进制转换的一定时间顺序的guid 够用约5900年，满足大部分项目
            var sequentialGuidGenerator = new SequentialGuidGenerator(optionWarpper);
            return sequentialGuidGenerator.Create();
        }
    }
}
