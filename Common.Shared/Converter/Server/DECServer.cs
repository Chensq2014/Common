using System.Threading.Tasks;

namespace Common.Converter.Server
{
    /// <summary>
    /// 十进制转换器
    /// </summary>
    public class DECServer : BaseServer
    {
        public override char[] CharArray => CommonConsts.DECCharArray.ToCharArray();
        public override int BitType => CommonConsts.DECType;

        public override async Task<string> DEC2Self(string originalValue)
        {
            if (IsValid(originalValue, CharArray) == false)
            {
                return "值无效";
            }
            return await Task.FromResult(originalValue);
        }

        public override async Task<string> Self2DEC(string originalValue)
        {
            if (IsValid(originalValue, CharArray)==false)
            {
                return "值无效";
            }
            return await Task.FromResult(originalValue);
        }
    }
}
