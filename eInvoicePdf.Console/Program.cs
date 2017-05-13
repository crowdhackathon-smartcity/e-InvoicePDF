using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eInvoicePdf.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string inFile = @"C:\eInvoicePdf\In\Invoice.pdf";
            FileInfo fi = new FileInfo(inFile);
            var service = new InvoiceService();
            InvoiceDto dto = service.LoadFromFile(fi);

            var filename = dto.Filename;

            
            var outFile = @"C:\eInvoicePdf\Out\out.pdf";
            service.WriteToFile(dto, new FileInfo(outFile));

            /*

            meta.SetProperty(ns, ublKey, content);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(@"C:\eInvoicePdf\out.pdf", FileMode.Create));
            XmpCore.Options.SerializeOptions opts = new XmpCore.Options.SerializeOptions();
            metadata = XmpCore.XmpMetaFactory.SerializeToBuffer(meta, opts);
            stamper.XmpMetadata = metadata;
            stamper.Close();

            */
        }
   



    }


    class InvoiceTypeDto
    {
        public string Code { get; set; }
        public string Desc { get; set; }

    }

    class InvoiceDto
    {
        public string ID { get; set; }
        public DateTime IssueDate { get; set; }
        public Party Supplier { get; set; }
        public Party Customer { get; set; }
        public string Reason { get; set; }
        public string InvoiceType  { get; set; }
        public InvoiceLineDto[] Lines { get; set; }
        public string Filename { get; set; }

    }

    class Party
    {
        public string AccountID { get; set; }
        public string Name { get; set; }
        public string VAT { get; set; }
        public string TaxationAuthority { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string PostalZone { get; set; }
        public string IndustryClassificationCode { get; set; }
        public string IndustryClassificationName { get; set; }

    }

    class IndustryClassificationCode
    {
        public string Code { get; set; }
        public string Desc { get; set; }
    }

    class InvoiceLineDto
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

    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">380</cbc:InvoiceTypeCode>
    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">393</cbc:InvoiceTypeCode>
    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">384</cbc:InvoiceTypeCode>
}
