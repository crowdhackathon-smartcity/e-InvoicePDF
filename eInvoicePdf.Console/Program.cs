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
            InvoiceViewModel dto = service.LoadFromFile(fi);

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
    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">380</cbc:InvoiceTypeCode>
    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">393</cbc:InvoiceTypeCode>
    //<cbc:InvoiceTypeCode listID = "UN/ECE 1001 Subset" listAgencyID="6">384</cbc:InvoiceTypeCode>
}
