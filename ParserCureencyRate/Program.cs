using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using System.Configuration;
using Dapper.Contrib.Extensions;
using System.Threading;

namespace ParserCureencyRate
{
    class Program
    {
        protected static readonly string InternalContext = ConfigurationManager.ConnectionStrings["NKMT"].ConnectionString;

        static void Main(string[] args)
        {
            for (int y = 2019; y <= 2021; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    Console.WriteLine(string.Format("執行{0}年{1}月匯率", y,m));


                    //線上匯率API
                    string url = @"https://portal.sw.nat.gov.tw/APGQO/GC331!query";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
                    //postParams.Add("formBean.crrnCd", _CurrencyCode);
                    postParams.Add("formBean.year", y.ToString());
                    postParams.Add("formBean.mon", m.ToString().PadLeft(2, '0'));
                    //postParams.Add("formBean.tenDay", d.ToString());


                    byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                    }

                    string responseStr = string.Empty;
                    //發出Request
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                        }
                    }

                    var data = JsonConvert.DeserializeObject<CurrencyRateModel>(responseStr);


                    List<CO_CurrencyRate> currencyRateList = new List<CO_CurrencyRate>();

                    var tempDate = new DateTime(y, m, 1);

                    for (int d = 1; d <= tempDate.AddMonths(1).AddDays(-1).Day; d++)
                    {
                        int tenDay = 0;

                        if(d<11)
                        {
                            tenDay = 1;
                        }
                        else if(d >= 11 && d < 21)
                        {
                            tenDay = 2;
                        }
                        else if (d >= 21)
                        {
                            tenDay = 2;
                        }


                        currencyRateList.Add(new CO_CurrencyRate()
                        {
                            CurrencyCode = "RMB",
                            CurrencyRate = data.data.FirstOrDefault(x => x.CRRN_CD == "CNY" && x.TEN_DAY == tenDay).IN_RATE,
                            CurrencyDate = new DateTime(y, m, d)
                        });

                        currencyRateList.Add(new CO_CurrencyRate()
                        {
                            CurrencyCode = "USD",
                            CurrencyRate = data.data.FirstOrDefault(x => x.CRRN_CD == "USD" && x.TEN_DAY == tenDay).IN_RATE,
                            CurrencyDate = new DateTime(y, m, d)
                        });

                        currencyRateList.Add(new CO_CurrencyRate()
                        {
                            CurrencyCode = "EUR",
                            CurrencyRate = data.data.FirstOrDefault(x => x.CRRN_CD == "EUR" && x.TEN_DAY == tenDay).IN_RATE,
                            CurrencyDate = new DateTime(y, m, d)
                        });
                    }


                    using (var cn = new SqlConnection(InternalContext))
                    {
                        cn.Insert(currencyRateList); ;
                    }


                    Console.WriteLine("寫入完成");
                    Console.WriteLine("等待五秒...");

                    Thread.Sleep(5000);
                }
            }




        }
    }
}
