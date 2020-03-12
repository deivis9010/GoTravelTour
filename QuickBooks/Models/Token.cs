using System;
using System.Collections.Generic;

namespace QuickBooks.Models
{
    public partial class Token
    {
        public string RealmId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
