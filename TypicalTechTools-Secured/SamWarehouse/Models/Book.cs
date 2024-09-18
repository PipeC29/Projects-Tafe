using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookStoreApp.Models
{
    public partial class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }

        //Linking refrences to the other tables that this table has a relationship with.
        public virtual Author Author { get; set; }
        public virtual List<ShoppingCartItem> CartItems { get; set; }
    }
}
