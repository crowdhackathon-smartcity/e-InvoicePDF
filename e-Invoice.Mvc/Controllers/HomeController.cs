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
            IList<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
            invoices = invoiceService.LoadFromOut();
            return View(invoices);
        }
        public ActionResult UploadPdf()
        {
            IList<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
            invoices = invoiceService.LoadFromIn();
            return View(invoices);
        }
        [HttpPost]
        public ActionResult UploadPdf(HttpPostedFileBase file)
        {
            IList<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
            string fname;
            fname = file.FileName;



            // Get the complete folder path and store the file inside it.  
            fname = Path.Combine(@"C:\\eInvoicePdf\\In", fname);
            file.SaveAs(fname);

            invoices = invoiceService.LoadFromIn();
            return View(invoices);
        }

        public ActionResult InvoicesList()
        {
            IList<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
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
                    invoices = invoiceService.LoadFromIn();
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

        public ActionResult CreatPdf(PdfreateViewModel pdfreateViewModel)
        {

           //invoiceService.WriteToFile();
            return RedirectToAction("CreateParastastiko");
        }
    }
}