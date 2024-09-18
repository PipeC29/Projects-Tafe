using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace TypicalTechTools.Models
{
    public class Product
    {
        // This attribute indicates that the property is required and must have a value.
        [Required]
        // This attribute designates the property as the primary key of the entity
        [Key]
        [DisplayName("Product Code")]
        public int ProductCode { get; set; }
        [Required]
        [DisplayName("Product Name")]
        //Makes the text need to start with a capital letter then followed by lower case letters
        [RegularExpression("^[A-Z][A-Za-z0-9 ]*$")]
        public string? ProductName { get; set; }
        [Required]
        [DisplayName("Product Price in $")]
        [Range(0, double.MaxValue, ErrorMessage = "Product price must be a non-negative value.")]
        public decimal? ProductPrice { get; set; }
        [Required]
        [DisplayName("Product Description")]
        [StringLength(maximumLength:100, MinimumLength = 10)]
        public string? ProductDescription { get; set; }

        [Required]
        [DisplayName("Date Last Updated")]
       // [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy hh:mm tt}", ApplyFormatInEditMode = true)]

        [DateInThePast(ErrorMessage = "UpdatedDate cannot be in the future.")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;



        // Used to set up foreign key relationship to the Comment entity
        public virtual ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    }
    // Custom validation attribute to check if a date is in the past
    public class DateInThePastAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Now;
            }
            return false;
        }
    }
}
