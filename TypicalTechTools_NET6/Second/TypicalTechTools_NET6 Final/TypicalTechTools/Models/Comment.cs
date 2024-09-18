using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class Comment
    {

        public int CommentId { get; set; }

        [Display(Name = "Comment")]
        [StringLength(50)]
        [Required(ErrorMessage ="In words 50 or less,please type in your comment")]
        public string? CommentText { get; set; }

        [Display(Name = "Product Code")]
        public int ProductCode { get; set; }

        public string? SessionId { get; set; }

        //// Default value set to current date and time
        [Display(Name = "Comment added on:")]

        public DateTime CreatedDate { get; set; } = DateTime.Now;


        // User to set up foreign key relationship to the Product entity
        public ICollection<Product> Products { get; set; }  
       

        /// <summary>
        /// Return a CSV formatted string of the a comment object
        /// </summary>
        /// <returns></returns>
        public string ToCSVString()
        {
            return $"{CommentId},{CommentText},{ProductCode}";
        }

    }
}
