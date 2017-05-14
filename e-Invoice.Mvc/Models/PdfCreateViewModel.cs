using eInvoicePdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Invoice.Mvc.Models
{
    public class PdfCreateViewModel
    {
        public PdfCreateViewModel() {
            Lines = new List<InvoiceLineViewModel>();
        }

        public string AA { get; set; }
        public string Aitiologia { get; set; }
        public string Industrycode { get; set; }
        public string Name { get; set; }
        public string Afm { get; set; }
        public string Doy { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Kind { get; set; }
        public string Tk { get; set; }
        public string Job { get; set; }
        public DateTime? Date { get; set; }
        public List<InvoiceLineViewModel> Lines { get; set; }
    }
}
