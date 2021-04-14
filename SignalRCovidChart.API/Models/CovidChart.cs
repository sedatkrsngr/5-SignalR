using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCovidChart.API.Models
{
    public class CovidChart
    {
        public CovidChart()
        {
            Counts = new List<int>();
        }
        public string CovidDate { get; set; }
        public List<int> Counts { get; set; }//gelecek şehirlerin hastalık countlarını tanımladık ama ilerde artar diye Liste şeklinde tutalım yoksa 5 tane ayrı property de tanımlayabilirdik
        //Aşağıdaki sorguyu veritabanında chart oluşturmak için pivot yaptık 1,2,3,4,5 şehirleri temsil ediyor ve sütun adı gibi gelen değerler de şehirlere göre dağılacak
    }
}



//SELECT tarih,
//       [1],
//       [2],
//       [3],
//       [4],
//       [5]
//FROM  (SELECT[City],[Count],CAST([CovidDate] AS date) AS tarih FROM [Covids]) AS CovidT 
//  Pivot (Sum(COUNT) FOR City In([1], [2], [3], [4], [5])) AS PivotT order by tarih asc