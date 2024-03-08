using Microsoft.AspNetCore.Authorization;

namespace Parakeet.NetCore.ROServer.Handlers
{
    /// <summary>
    /// 授权 Policy（grpc)-->GrpcRequirement
    /// </summary>
    public class GrpcRequirement : IAuthorizationRequirement
    {
    }
} 