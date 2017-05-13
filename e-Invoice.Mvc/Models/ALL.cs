using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Invoice.Mvc.Models
{
    public class ALL
    {
    }



    public class InvoiceTypeViewModel
    {
        public string Code { get; set; }
        public string Desc { get; set; }

    }

    public class InvoiceViewModel
    {
        public string ID { get; set; }
        public DateTime IssueDate { get; set; }
        public PartyViewModel Supplier { get; set; }
        public PartyViewModel Customer { get; set; }
        public string Reason { get; set; }
        public string InvoiceType { get; set; }
        public InvoiceLineViewModel[] Lines { get; set; }
        public string Filename { get; set; }

    }

    public class PartyViewModel
    {
        public string AccountID { get; set; }
        public string Name { get; set; }
        public string VAT { get; set; }
        public string TaxationAuthority { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }

        //public string Address { get; set; }
        public string CityName { get; set; }
        public string PostalZone { get; set; }
        public string IndustryClassificationCode { get; set; }
        public string IndustryClassificationName { get; set; }

    }

    class IndustryClassificationCodeViewModel
    {
        public string Code { get; set; }
        public string Desc { get; set; }
    }

    public class InvoiceLineViewModel
    {

        public string ID { get; set; }
        public string UUID { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal PriceAmount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal TaxAmount { get; set; }

    }
}