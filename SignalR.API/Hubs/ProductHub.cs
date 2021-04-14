using Microsoft.AspNetCore.SignalR;
using SignalR.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{
    public class ProductHub : Hub<IProductHub>
    {
        public async Task SendProduct(Product product)
        {
            await Clients.All.ProductGonder(product);//İnterface ile yapacağımız metodları belirlersek  //await Clients.All.SendAsync("ProductGonder", product); gibi string yerine metodumuzu direkt yaabiliriz ve daha hatasız bir şekilde ilerleyebiliriz. Tüm Clieent isteklerinde metod olarak belirlenir böylecek. örn. Clilents.Group.ProductGonder veya Caller.ProductGonder ...


        }
    }
}
