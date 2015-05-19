using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Linker
    {
        private ApplicationDbContext _db;
        public Linker()
        {
            _db = new ApplicationDbContext();
        }
        private string GetDownloadToken(int length)
        {
            int intZero = '0';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strDownloadToken = "";
            Random objRandom = new Random(System.DateTime.Now.Millisecond);
            while (intCount < length)
            {
                intRandomNumber = objRandom.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) &&
                  (intRandomNumber <= intNine) ||
                  (intRandomNumber >= intA) && (intRandomNumber <= intZ)))
                {
                    strDownloadToken = strDownloadToken + (char)intRandomNumber;
                    intCount++;
                }
            }
            try{
                FileEntity dd=_db.Files.Single(s => s.DownloadLink == strDownloadToken);
               
                return GetDownloadToken(12);
            } catch{ return strDownloadToken;}
          
            }
           
            
        
        public string CreateDownloadLink()
        {
            return GetDownloadToken(12);
        }
       
    }
}