using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalRCovidChart.API.Models;
using SignalRCovidChart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCovidChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _covidService;
        public CovidsController(CovidService covidService)//startupta tanımladığımız addscoped ile yeni bir nesne örneği oluşturur
        {
            _covidService = covidService;
        }


        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _covidService.SaveCovid(covid);

          //  IQueryable<Covid> covidList = _covidService.GetList();

            return Ok(_covidService.GetCovidChartList());
        }

        [HttpGet]
        public  IActionResult InitializeCovid()//Amacı sanki dışardan veri girişi oluyor gibi düşünelim ve veritabanına veri girişi yapalım
        {

            Random random = new Random();

            Enumerable.Range(1, 10).ToList().ForEach(x=> {

                foreach (ECity item in Enum.GetValues(typeof(ECity)))//şehirlerin Id si ile şehirleri döneriz
                {
                    var newCovid = new Covid
                    {
                        City = item,
                        Count = random.Next(100, 1000),
                        CovidDate = DateTime.Now.AddDays(x)//10 güne kadar değerler gelecek
                    };

                    _covidService.SaveCovid(newCovid).Wait();//başına Task olduğu için await ekleyecektik fakat foreach içerisinde kabul etmediğiiçin sonuna aynı işlemi yapan Wait() eklendi
                    System.Threading.Thread.Sleep(1000);//Chart değişimini izleyelim diye konuldu
                }
            
            });


            return Ok("Datalar veritabanına kaydedildi");
        }

    }
}
