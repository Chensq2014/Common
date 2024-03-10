using Microsoft.AspNetCore.Http;
using Common.Dtos;
using System;
using Common.Dtos;

namespace Common.Options
{
    /// <summary>
    /// 浏览器检查接口
    /// </summary>
    public interface IBrowserCheck
    {
        Tuple<bool, string> CheckBrowser(HttpContext httpContext,BrowserFilterOptions options);
    }
}
