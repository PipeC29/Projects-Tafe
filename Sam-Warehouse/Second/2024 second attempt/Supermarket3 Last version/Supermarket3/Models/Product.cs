using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Supermarket3.Models
{

    public partial class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
       

        //Linking refrences to the other tables that this table has a relationship with.
      
        public virtual List<ShoppingCartItem> CartItems { get; set; }
    }
}
