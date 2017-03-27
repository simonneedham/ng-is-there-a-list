using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsThereAList.Models
{
    [Table("ListType")]
    public class ListType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [InverseProperty("ListType")]
        public virtual ICollection<List> Lists { get; set; }
    }
}
