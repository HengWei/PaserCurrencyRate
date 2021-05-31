using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace ParserCureencyRate
{
    class Model
    {
    }

    public class CurrencyRateModel
    {
        public string msg { get; set; }

        public string status { get; set; }

        public string guest { get; set; }

        public IEnumerable<CurrencyData> data { get; set; }
    }


    public class CurrencyData
    {
        public DateTime? UP_DATE { get; set; }

        public string CRRN_CD { get; set; }

        public string UP_PERSON { get; set; }

        public decimal IN_RATE { get; set; }

        public int TEN_DAY { get; set; } = 0;

        public decimal EX_RATE { get; set; }

        public string MON { get; set; }

        public int? ORDER_FLAG { get; set; } = 0;

        public string YEAR { get; set; }
    }


    [Table("CO_CurrencyRate")]
    public class CO_CurrencyRate
    {
        public string CurrencyCode { get; set; }

        public DateTime CurrencyDate { get; set; }

        public decimal CurrencyRate { get; set; }
    }
}
