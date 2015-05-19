using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DownloadLink { get; set; }
        public string Description { get; set; }
        [DisplayFormat(NullDisplayText = "anonymous")]
        public string User { get; set; }
        public DateTime DateAdded { get; set; }
    }
}