using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using e_Invoice.Mvc.Models;
namespace e_Invoice.Mvc.Controllers
{
    public class HomeController : Controller
    {
        InvoiceService invoiceService = new InvoiceService();

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
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateParastastiko()
        {
            return View();
        }

        public ActionResult UploadPdf()
        {
            return View();
        }

        public ActionResult InvoicesList()
        {
            List<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;


                        fname = file.FileName;

                        

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/In/"), fname);
                        file.SaveAs(fname);

                    }

                    string[] filess = Directory.GetFiles(Server.MapPath("~/In/"));
                    foreach (var item in filess)
                    {
                        FileInfo fileIn = new FileInfo(item);
                        invoices.Add(invoiceService.LoadFromFile(fileIn));
                       
                    }
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }

            return View(invoices);
        }


        public ActionResult ViewModal()
        {
            return PartialView("_ViewModal");
        }
        public ActionResult _ViewModal()
        {
            return View();
        }
    }
}