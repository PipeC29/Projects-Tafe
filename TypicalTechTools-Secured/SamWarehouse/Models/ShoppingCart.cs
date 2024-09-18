using BookStoreApp.Data;
using System;
using System.Collections.Generic;

namespace BookStoreApp.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public double? Total { get; set; }

        //Tracks whether the user has completed the checkout on the cart
        public bool IsFinalised { get; set; } = false;

        //Linking refrences to the other tables that this table has a relationship with.
        public virtual User CartUser { get; set; }
        public virtual List<ShoppingCartItem> CartItems { get; set; }

    }
}
