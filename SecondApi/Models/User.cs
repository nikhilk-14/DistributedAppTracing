using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecondApi.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string Occupation { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
    }
}
