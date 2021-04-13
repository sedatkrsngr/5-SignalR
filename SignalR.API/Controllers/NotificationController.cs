using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.API.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Controllers
{
    [Route("api/[controller]")]//APı projesi tamamen Web.UI tarafıyla uğraşmamak için yoksa API değilde normal MVC web de olur
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly IHubContext<MyHub> _hubContext;//Controller bazında Hub ulaşmak için kullanılır

        public NotificationController(Microsoft.AspNetCore.SignalR.IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }


        [HttpGet("{TakimSayisi}")]

        public  async Task<IActionResult> TakimSayisiDuyur(int TakimSayisi)
        {
            await _hubContext.Clients.All.SendAsync("Duyuru",$"Arkadaşlar takım {TakimSayisi} kişi olacaktır. Bilginize.");
            return Ok();
        }
    }
}
