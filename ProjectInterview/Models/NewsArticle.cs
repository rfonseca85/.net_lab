using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectInterview.Models
{

//News articles should include the name of the newspaper they were published in, date, title, and excerpt.

    public class NewsArticle
    {
        [Key]
        public int NewsArticleID { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }     
        
        [StringLength(200, MinimumLength = 3)]
        [Display(Name = "Name of Newspaper")]
        public string NewspaperName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        [StringLength(2000, MinimumLength = 1)]
        public string Excerpt { get; set; }

    }
}