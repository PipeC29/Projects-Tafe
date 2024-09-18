using Supermarket3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket3.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        
        //Linking references to the other tables that this table has a relationship with.
        public virtual List<ShoppingCart> Carts { get; set; }
    }
}
