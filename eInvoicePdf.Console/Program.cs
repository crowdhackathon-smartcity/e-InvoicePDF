using System.IO;

namespace eInvoicePdf.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new InvoiceService();
            var items = service.LoadFromIn();

            //FileInfo fi = new FileInfo(@"C:\eInvoicePdf\In\Invoice.pdf");
            FileInfo fi = new FileInfo(Path.Combine(service.InDir.FullName, items[0].Filename));
            InvoiceViewModel dto = service.LoadFromFile(fi);
            service.CreatePdf(dto);

            //var filename = dto.Filename;            
            //service.WriteToFile(dto, new FileInfo(@"C:\eInvoicePdf\Out\out.pdf"));

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
