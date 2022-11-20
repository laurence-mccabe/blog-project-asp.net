using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj_12_10_22.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0}", MinimumLength = 2)]
        public string? Text { get; set; }
        public virtual Post? Post { get; set; }
        public virtual BlogUser? BlogUser { get; set; }

    }
}
