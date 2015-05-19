using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using PagedList;
namespace WebApplication2.Controllers
{
   
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;
        public HomeController()
        {
            _db = new ApplicationDbContext();
        }
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Контактная информация:";

            return View();
        }

        [HttpGet]
        public ActionResult Download(string link) 
        {
            var fl = _db.Files.Single(s => s.DownloadLink == link);
            if (fl == null)
            {
                return HttpNotFound();
            }
            
            return View(fl);
        }

        public FileResult Save(string path)
        {
            var fl = _db.Files.Single(s => s.DownloadLink == path);

            byte[] fileBytes = System.IO.File.ReadAllBytes(fl.Path);
            
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet,fl.Name);
        }

        public ActionResult Search(string searchTerm, int? page)
        {
            ViewBag.Term = searchTerm;
            
            var model = _db.Files
                .Where(r => searchTerm == null || r.Name.Contains(searchTerm))
                .OrderBy(r=>r.Name)
                .Select(r => new FileViewModel
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Description = r.Description,
                                DownloadLink = r.DownloadLink
                            });
            ViewBag.Count = model.Count();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_FilesList", model.ToPagedList(pageNumber, pageSize));
            }
            return View(model.ToPagedList(pageNumber, pageSize));
            
        }

      /// <summary>
      /// Edit action
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _db.Files.Single(r => r.Id == id);
            return View(model);
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            var model = _db.Files.Single(r => r.Id == id);
            _db.Files.Remove(model);
            _db.SaveChanges();
            return RedirectToAction("UserFiles","Account",null);
        }


        [HttpPost]
        public ActionResult Edit(FileEntity fl)
        {
            if (ModelState.IsValid)
            {
               
                _db.Entry(fl).State=EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("UserFiles","Account",null);
            }
            return View(fl);
        }
       
        public ActionResult SaveUploadedFile()
        {
            FileEntity someFile = new FileEntity();
            bool isSavedSuccessfully = true;
            string fName = "";
            string exp = "";
            string we = "";
            try
            {

                foreach (string fileName in Request.Files)
                {
                   
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);
                       
                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        var pt = "~/Uploads/imagepath/" + file.FileName;
                        someFile.MimeType = MimeMapping.GetMimeMapping(file.FileName);
                        someFile.UrlPath = pt;
                        someFile.DateTime = DateTime.Now;
                        file.SaveAs(path);
                        
                        someFile.Name = fName;
                        someFile.Path = path;
                        if (User.Identity.IsAuthenticated)
                        {
                            someFile.User = User.Identity.GetUserId();
                        }
                        Linker lk = new Linker();
                        someFile.DownloadLink = lk.CreateDownloadLink();
                        _db.Files.Add(someFile);
                        _db.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                exp=ex.Message;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = someFile.DownloadLink });
            }
            else
            {
                return Json(new { Message = "Error in saving file! "+exp });
            }

        }
       
        
        
       public JsonResult Giveme(string path){
           FileEntity tt = _db.Files.Single(s => s.DownloadLink == path);
           return Json(new {Name=tt.Name,Description=tt.Description,Privacy=tt.Private,ExpDate=tt.ExpirationDate} );
       }
      
       [HttpPost]
        public JsonResult UpdateRecords(AdditionalData tr)
        {
           
            FileEntity sr = _db.Files.Single(s => s.DownloadLink == tr.link);
           
           sr.Description=tr.text;
           sr.ExpirationDate=tr.expDate;
           sr.Name=tr.name;
           sr.Private=tr.publc;
           _db.Entry(sr).State=EntityState.Modified;
           _db.SaveChanges();
           return Json(new { Message = "success"});
        }

      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}