using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using e_Invoice.Mvc.Models;
using eInvoicePdf;
using System.Configuration;

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
            if (file != null)
            {
                string fname;
                fname = file.FileName;

                // Get the complete folder path and store the file inside it.  
                var workingdirectory = ConfigurationManager.AppSettings.Get("WorkingDirectory");
                fname = Path.Combine(workingdirectory+"\\In", fname);
                file.SaveAs(fname);
            }
           

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
        public ActionResult AddParastatiko()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(PdfCreateViewModel pdfreateViewModel)
        {
            invoiceService.CreatePdf(MapToInvoiceViewModel(pdfreateViewModel));
            IList<InvoiceViewModel> invoices = new List<InvoiceViewModel>();
            invoices = invoiceService.LoadFromIn();
            //invoiceService.WriteToFile();
            return View("CreateParastastiko", invoices);
        }

        public InvoiceViewModel MapToInvoiceViewModel(PdfCreateViewModel pdfreateViewModel)
        {
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
            invoiceViewModel.Supplier = new PartyViewModel();

            invoiceViewModel.ID = pdfreateViewModel.AA;
            invoiceViewModel.InvoiceType = pdfreateViewModel.Kind;
            invoiceViewModel.IssueDate = pdfreateViewModel.Date ?? DateTime.MinValue;
            invoiceViewModel.Reason = pdfreateViewModel.Aitiologia;
            invoiceViewModel.Supplier.VAT = pdfreateViewModel.Afm;
            invoiceViewModel.Supplier.CityName = pdfreateViewModel.City;
            invoiceViewModel.Supplier.TaxationAuthority = pdfreateViewModel.Doy;
            invoiceViewModel.Supplier.StreetName = pdfreateViewModel.Address;
            invoiceViewModel.Supplier.PostalZone = pdfreateViewModel.Tk;
            invoiceViewModel.Supplier.Name = pdfreateViewModel.Name;
            invoiceViewModel.Supplier.BuildingNumber = pdfreateViewModel.BuildingNumber;
            invoiceViewModel.Supplier.IndustryClassificationCode = pdfreateViewModel.Industrycode;
            invoiceViewModel.Supplier.IndustryClassificationName = pdfreateViewModel.Job;
            invoiceViewModel.Lines = pdfreateViewModel.Lines.ToArray();

            return invoiceViewModel;
        }
    }
}