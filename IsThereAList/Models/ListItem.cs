using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IsThereAList.Models
{
    public class ListItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListItemId { get; set; }

        public int ListId { get; set; }

        [Required, MaxLength(255)]
        [Display(Name = "Suggestion")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Web Link")]
        public string Url { get; set; }

        [Display(Name = "Picture")]
        public string PictureUrl { get; set; }

        public int? UserIdPurchased { get; set; }

        public bool Deleted { get; set; }

        public int UserIdUpdated { get; set; }

        public DateTime Updated { get; set; }

        [NotMapped]
        public bool HasBeenPurchased
        {
            get { return this.UserIdPurchased.HasValue; }
        }

        [ForeignKey("ListId")]
        public virtual List List { get; set; }

        [Display(Name = "Purchased by")]
        [ForeignKey("UserIdPurchased")]
        public virtual User UserPurchased { get; set; }

        [ForeignKey("UserIdUpdated")]
        public virtual User UserUpdated { get; set; }
    }
}
