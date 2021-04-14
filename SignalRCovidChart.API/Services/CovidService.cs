using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRCovidChart.API.Hubs;
using SignalRCovidChart.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SignalRCovidChart.API.Services
{
    public class CovidService
    {
        private readonly CovidDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(CovidDbContext context, Microsoft.AspNetCore.SignalR.IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        
        public IQueryable<Covid> GetList()
        {
           return  _context.Covids.AsQueryable();//AsQueryable ile to list yapmadığımız sürece veritabanına gitmez en son istediğimiz veriyi tolist() ile alırız
           //Inumerable veriyi çeker sonra memory ile kısıtlama yapar IQueryable ise kısıtlamayla birlikte sorguyu çeker ve daha hızlıdır
        }

        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("CovidListesiniAl",GetCovidChartList());//CovidHub ile aynı isim dikkat et. 
        }

        public List<CovidChart>  GetCovidChartList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            using (var command= _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT tarih,[1],[2],[3],[4],[5] FROM (SELECT[City],[Count], CAST([CovidDate] AS date) AS tarih FROM[Covids]) AS CovidT  Pivot(Sum(COUNT) FOR City In([1], [2], [3], [4], [5])) AS PivotT order by tarih asc";

                command.CommandType = CommandType.Text;

                _context.Database.OpenConnection();

                using (var reader= command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covidData = new CovidChart();

                        covidData.CovidDate
                            = reader.GetDateTime(0).ToShortDateString();//0.cı sıradaki covidtable değeri tarih

                        Enumerable.Range(1, 5).ToList().ForEach(x=> {//veri tabanında 1,2,3,4,5 şehir var sorguda biz de il il gidicez

                            if (DBNull.Value.Equals(reader[x]))//ilgili tarihte ilgili ilde vaka yoksa(null sa)
                            {
                                covidData.Counts.Add(0);//ilgili gün için ilgili ilin count=0 olsun
                            }
                            else
                            {
                                covidData.Counts.Add(reader.GetInt32(x));
                            }                        
                        });

                        covidCharts.Add(covidData);
                    }
                }

            }
            _context.Database.CloseConnection();

            return covidCharts;
        }

    }
}



