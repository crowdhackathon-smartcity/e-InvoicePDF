using System;

namespace eInvoicePdf.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Chilkat.Rest rest = new Chilkat.Rest();
            Chilkat.Xmp xmp = new Chilkat.Xmp();
            bool success = xmp.UnlockComponent("Anything for 30-day trial.");

            if (success != true)
            {
                Console.WriteLine(xmp.LastErrorText);
                return;
            }

            success = xmp.LoadAppFile(@"C:\eInvoicePdf\Invoice.pdf");

            Console.WriteLine("Num embedded XMP docs: " + Convert.ToString(xmp.NumEmbedded));

            //  Assuming there is at least one, get the 1st.
            //  (There is typically never more than one, but theoretically it's possible.)
            Chilkat.Xml xml = null;
            xml = xmp.GetEmbedded(0);
            if (!(xml == null))
            {

                string propVal = xmp.GetSimpleStr(xml, "Iptc4xmpCore:Location");
                if (xmp.LastMethodSuccess != true)
                {
                    Console.WriteLine("Not found.");
                }
                else
                {
                    Console.WriteLine(propVal);
                }

            }
            else
            {
                Console.WriteLine(xmp.LastErrorText);
            }


            /*
            PdfReader reader = new PdfReader(@"C:\eInvoicePdf\Invoice.pdf");
            byte[] metadata = reader.Metadata;
            XmpCore.XmpMetaFactory.
            XmpCore.IXmpMeta meta = XmpCore.XmpMetaFactory.ParseFromBuffer(reader.Metadata);
            string content = meta.GetPropertyString("http://www.CraneSoftwrights.com/ns/XMP/", "c:file");
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(InvoiceType));
            System.IO.StringReader rdr = new System.IO.StringReader(content);
            InvoiceType invoice = (InvoiceType)ser.Deserialize(rdr);
            */
        }
    }
}