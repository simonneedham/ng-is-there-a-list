using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsThereAList.Models
{
    [Table("User")]
    public class User
    {
        private string _emailAddress = String.Empty;

        public User()
        {
            this.SendEmails = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName ?? String.Empty, LastName ?? String.Empty);
            }
        }

        [Required, EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                _emailAddress = value ?? String.Empty;
                //this.UserName = _emailAddress;
            }
        }

        [Required]
        [Display(Name = "Send email notifications")]
        public bool SendEmails { get; set; }

        [Range(1, 31)]
        [Required]
        [Display(Name = "Day of Birth")]
        public int DobDay { get; set; }

        [Range(1, 12)]
        [Required]
        [Display(Name = "Month of birth")]
        public int DobMonth { get; set; }

        //[InverseProperty("UserPurchased")]
        //public virtual ICollection<ListItem> PurchasedListItems { get; set; }

        //[InverseProperty("UserUpdated")]
        //public virtual ICollection<ListItem> UpdatedListItems { get; set; }

        //[InverseProperty("Owner")]
        //public virtual ICollection<List> Lists { get; set; }
    }
}
