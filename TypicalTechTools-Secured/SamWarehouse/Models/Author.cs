using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BookStoreApp.Models
{
    public partial class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }

        [Display(Name = "Surname")]
        [MinLength(2, ErrorMessage ="Needs to be at least 2 characters long")]
        [MaxLength(150)]
        [Required]
        public string LastName { get; set; }

        [MinLength(2, ErrorMessage = "Needs to be at least 2 characters long")]
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [NotMapped]
        public string FullName 
        {
            get
            {
                return$"{FirstName} {LastName}";
            } 
        }

        public virtual ICollection<Book> Books { get; set; }
    }
}
