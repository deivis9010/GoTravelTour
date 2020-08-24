using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class TokenQB
    {
        public int TokenQBId { get; set; }

        public string RefreshToken { get; set; }
        public string RealmId { get; set; }
    }
}
