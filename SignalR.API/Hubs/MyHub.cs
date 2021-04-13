using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{//API ve Web uygulaması farketmez aynı yöntem kullanılır
    public class MyHub : Hub//clientlar içindeki bir methoda istek yaptıklarında yeni bir nesne örneği oluşur. Önemli burası bu yüzden data tutmak istersek static tanımlamak gerekli
    {
        private static List<string> Names { get; set; } = new List<string>();
        private static int KullaniciSayisi { get; set; } = 0;

        public  async Task SendName(string name)
        {
           await Clients.All.SendAsync("AdiAl", name);
            Names.Add(name);
            //All tüm clientlara bildiri gönderir method adı ve her türden veri gönderebiliriz ve birden fazla veri de gönderebiliriz. message,message1,message2.. 10 adete kadar. En son da cansellationtoken var istersek kullanabiliriz, İşlemleri iptal etmek için. Aynı zamanda Clientlar metodumuza üye olduysa mesajı alabilirler
        }

        public async Task GetName()
        {
            await Clients.All.SendAsync("AdlariAl", Names);
            //All tüm clientlara bildiri gönderir method adı ve her türden veri gönderebiliriz ve birden fazla veri de gönderebiliriz. message,message1,message2.. 10 adete kadar. En son da cansellationtoken var istersek kullanabiliriz, İşlemleri iptal etmek için. Aynı zamanda Clientlar metodumuza üye olduysa mesajı alabilirler
        }

        public async override Task OnConnectedAsync()//override yazınca geliyor. Connect olunca bu method çalışır
        {
            KullaniciSayisi++;

            await Clients.All.SendAsync("KullaniciSayisiDön",KullaniciSayisi);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)//disconnect olunca bu method çalışır.
        {
            KullaniciSayisi--;
            await Clients.All.SendAsync("KullaniciSayisiDön",KullaniciSayisi);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
