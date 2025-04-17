using MJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace WindNight.Core.SQL.Abstractions
{
    public interface IQueryPageBase
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }

        /// <summary>  </summary>
        [MJsonIgnore, NJsonIgnore]
        int IndexFrom { get; set; }

    }

    public interface IQueryPageInfo : IQueryPageBase
    {
        /// <summary>
        ///     表名(多表连接表名实例："test1 as a left join test2 as b on a.cid=b.cid")
        /// </summary>
        string TableName { get; set; }

        /// <summary>  字段名(全部字段为*) </summary>
        string Fields { get; set; }

        /// <summary>  条件语句(不用加where) </summary>
        string SqlWhere { get; set; }

        /// <summary>  排序字段(必须需要!支持多字段，不用加order by) </summary>
        string OrderField { get; set; }

    }
}
