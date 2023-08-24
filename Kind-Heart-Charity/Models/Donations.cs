using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kind_Heart_Charity.Models
{
    public class Donations
    {
        public Donations() { }

        public Donations(string name, string amount, string email, int v1, int v2)
        {
            Name = name;
            Amount = amount;
            Email = email;
            IsActive = Convert.ToByte(v1);
            Role = v2;
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Email { get; set; }
        public byte IsActive { get; set; }
        public int Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}