using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectInterview.Models
{
    public class Blog
    {
        [Key]
        public int BlogID { get; set; }
        
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [Url]
        public string Url { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        [StringLength(2000, MinimumLength = 1)]
        public string Excerpt { get; set; }

    }
}