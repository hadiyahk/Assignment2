using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Lab5.Models
{
    public class Fan
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Last name required")]
        [StringLength(50)]
        [Display(Name =  "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "First name required")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        // Calculated property
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }

        public ICollection<Subscription> Subscriptions { get; set; }
    }

}
