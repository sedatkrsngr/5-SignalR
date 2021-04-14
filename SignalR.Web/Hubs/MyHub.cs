using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Web.Hubs
{
    public class MyHub : Hub 
    {
        public async Task SendMessage(string message)//API Yerine Server tarafında da kullanabiliriz.  Index tarafında bu sefer farklı çağırıcaz
        {
            await Clients.All.SendAsync("MesajGonder", message);
           
        }

    }
}
