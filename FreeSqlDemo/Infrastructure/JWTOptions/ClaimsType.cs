using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Infrastructure.JWTOptions
{
    public static class ClaimsType
    {
        public const string RealName = "realname";
        public const string Account = "account";
        public const string Subject = "sub";
        public const string UserId = "userid";
        public const string TerantId = "terantid";
        public const string TerantName = "terantname";
        public const string TerantCreateDate = "terantcreatedate";
        public const string TerantEffective = "teranteffective";
        public const string PhoneNumber = "phonenumber";
        public const string Roles = "roles";
    }
}
