using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class FileEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string DownloadLink { get; set; }
        public string UrlPath { get; set; }
        public DateTime DateTime { get; set; }
        public string MimeType { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public int ExpirationDate { get; set; }
        public bool Private { get; set; }
    }
}