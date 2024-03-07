namespace Common.Converter.Server
{
    public class OCTServer : BaseServer
    {
        public override char[] CharArray => CommonConsts.OCTCharArray.ToCharArray();
        public override int BitType => CommonConsts.OCTType;
    }
}
