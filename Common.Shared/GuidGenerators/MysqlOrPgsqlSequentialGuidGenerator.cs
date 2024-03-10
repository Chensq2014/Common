using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Common.GuidGenerators
{
    /// <summary>
    /// mysql/pgsql 有序guid生成器  通过IGuidGenerator依赖注入  继承下来可以调试测试
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class MysqlOrPgsqlSequentialGuidGenerator : SequentialGuidGenerator//,IGuidGenerator
    {
        public MysqlOrPgsqlSequentialGuidGenerator([NotNull] IOptions<AbpSequentialGuidGeneratorOptions> options) : base(options)
        {
        }
    }
}
