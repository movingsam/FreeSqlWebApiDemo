using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FreeSqlDemo.Infrastructure.JWTOptions
{
    public class JwtOptions : IOptions<JwtOptions>
    {
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;//是否验证Audience
        public bool ValidateLifetime { get; set; } = true;//是否验证失效时间
        public bool ValidateIssuerSigningKey { get; set; } = true;//是否验证SecurityKey
        public string ValidAudience { get; set; } = "yourdomain.com"; //Audience
        public string ValidIssuer { get; set; } = "yourdomain.com"; //Issuer，这两项和前面签发jwt的设置一致
        public string IssuerSigningKey { get; set; } = "123456";
        public JwtOptions Value => this;

    }
}
