using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;

namespace FreeSqlDemo.Infrastructure.DI
{
    /// <summary>
    /// FreeSql配置类
    /// </summary>
    public class FreeSqlConfig : IOptions<FreeSqlConfig>
    {
        /// <summary>
        /// 主库
        /// </summary>
        public string MasterConnetion { get; set; }
        /// <summary>
        /// 从库链接
        /// </summary>
        public List<SlaveConnection> SlaveConnections { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataType DataType { get; set; }
        public FreeSqlConfig Value => this;
    }
    /// <summary>
    /// 从库链接
    /// </summary>
    public class SlaveConnection
    {
        public string ConnectionString { get; set; }
    }

}
