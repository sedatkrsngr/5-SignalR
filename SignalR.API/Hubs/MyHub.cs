using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{//API ve Web uygulaması farketmez aynı yöntem kullanılır
    public class MyHub : Hub//clientlar içindeki bir methoda istek yaptıklarında yeni bir nesne örneği oluşur. Önemli burası bu yüzden data tutmak istersek static tanımlamak gerekli
    {
        private readonly AppDbContext _dbContext;

        public MyHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static List<string> Names { get; set; } = new List<string>();
        private static int KullaniciSayisi { get; set; } = 0;
        public static int takimSayisi { get; set; } = 7;

        public async Task SendName(string name)//method isimlerini biz belirleriz
        {
            if (Names.Count > takimSayisi)//Eklenen isimler belirtilen takim sayisindan büyükse
            {
                await Clients.Caller.SendAsync("Hata", $"Takım en fazla {takimSayisi} kişi olabilir");//sadece aşımı yapan kişiye gider mesaj
            }
            else
            {
                await Clients.All.SendAsync("AdiAl", name);
                Names.Add(name);
                //All tüm clientlara bildiri gönderir method adı ve her türden veri gönderebiliriz ve birden fazla veri de gönderebiliriz. message,message1,message2.. 10 adete kadar. En son da cansellationtoken var istersek kullanabiliriz, İşlemleri iptal etmek için. Aynı zamanda Clientlar metodumuza üye olduysa mesajı alabilirler
            }


        }

        public async Task GetName()
        {
            await Clients.All.SendAsync("AdlariAl", Names);
            //All tüm clientlara bildiri gönderir method adı ve her türden veri gönderebiliriz ve birden fazla veri de gönderebiliriz. message,message1,message2.. 10 adete kadar. En son da cansellationtoken var istersek kullanabiliriz, İşlemleri iptal etmek için. Aynı zamanda Clientlar metodumuza üye olduysa mesajı alabilirler
        }

        public async override Task OnConnectedAsync()//override yazınca geliyor. Connect olunca bu method çalışır
        {
            KullaniciSayisi++;

            await Clients.All.SendAsync("KullaniciSayisiDön", KullaniciSayisi);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)//disconnect olunca bu method çalışır.
        {
            KullaniciSayisi--;
            await Clients.All.SendAsync("KullaniciSayisiDön", KullaniciSayisi);
            await base.OnDisconnectedAsync(exception);
        }




        //Takım işlemleri   Takım grup aynı şeyler şaşırma

        public async Task TakimaEkle(string teamName)//Belirttiğimiz gruba sanal olarak giriş yaparız o gruba işlem yapıldığında biz de görürüz
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task TakimdanCikar(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task TakimaİsimGönder(string userName, string teamName)
        {
            var team = _dbContext.Teams.Where(t => t.Name == teamName).FirstOrDefault();

            if (team != null)
            {
                team.Users.Add(new User { Name = userName });
            }
            else
            {
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = userName });

                _dbContext.Teams.Add(newTeam);
            }

            await _dbContext.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("GrubaisimBildir", userName, team.Id);//kullanıcıyı ve takım Id göndeririz
        }

        public async  Task TakimaİsimleriGönder()
        {
            var teams =  _dbContext.Teams.Include(x => x.Users).Select(x => new
            {
                teamId = x.Id,
                Users=x.Users.ToList()
            });


            await Clients.All.SendAsync("GrubaİsimleriBildir",teams);
        }



    }
}
