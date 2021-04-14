using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Models
{
    public class Team
    {
        public Team()//Team üzerinden User ekleyeceğimiz için constructorda yeni user listesi oluşturulması gerekiyor. Yoksa ekleyemeyiz
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
