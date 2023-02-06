using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetClaimByKey(this HttpContext context, string key)
        {
            var claims = context.User.Claims.ToList();
            return claims?.FirstOrDefault(x => x.Type == key)?.Value ?? string.Empty;
        }
    }
}
