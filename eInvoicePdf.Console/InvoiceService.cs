using iTextSharp.text.pdf;
using iTextSharp.xmp.impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eInvoicePdf.Console
{

    class InvoiceService
    {
        IList<IndustryClassificationCodeViewModel> getIndustryClassificationCodes()
        {
            return new List<IndustryClassificationCodeViewModel>();

        }

        IList<InvoiceTypeViewModel> getInvoiceTypes()
        {
            return new List<InvoiceTypeViewModel>();
        }

        public IList<InvoiceViewModel> LoadFromIn()
        {
            return LoadFromDirectory(new DirectoryInfo(@"C:\eInvoicePdf\In"));
        }

        public IList<InvoiceViewModel> LoadFromOut()
        {
            return LoadFromDirectory(new DirectoryInfo(@"C:\eInvoicePdf\Out"));
        }


        public IList<InvoiceViewModel> LoadFromDirectory(DirectoryInfo dir)
        {

            return new List<InvoiceViewModel>();
        }

        public InvoiceViewModel LoadFromFile(FileInfo file)
        {
            PdfReader reader = new PdfReader(file.FullName);
            byte[] metadata = reader.Metadata;

            //XmpCore.XmpMetaFactory.
            XmpCore.IXmpMeta meta = XmpCore.XmpMetaFactory.ParseFromBuffer(reader.Metadata);
            reader.Close();
            string ns = "http://www.eInvoicePdf.gr/ns/XMP/";
            string ublKey = "e:file";

            string content = meta.GetPropertyString(ns, ublKey);

            XmlSerializer ser = new XmlSerializer(typeof(InvoiceType));
            StringReader rdr = new System.IO.StringReader(content);
            InvoiceType invoice = (InvoiceType)ser.Deserialize(rdr);

            var dto = Convert2Dto(invoice);
            dto.Filename = file.Name;
            return dto;
        }

        public InvoiceViewModel Convert2Dto(InvoiceType src)
        {
            var dest = new InvoiceViewModel();
            dest.ID = src.ID.Value;
            dest.IssueDate = src.IssueDate.Value;
            if (src.Note.Length > 0)
                dest.Reason = src.Note[0].Value;

            dest.InvoiceType = src.InvoiceTypeCode.Value;

            dest.Supplier = new PartyViewModel();
            dest.Supplier.AccountID = src.AccountingSupplierParty.CustomerAssignedAccountID.Value;
            dest.Supplier.Name = src.AccountingSupplierParty.Party.PartyName[0].Name.Value;
            dest.Supplier.VAT = src.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value;
            dest.Supplier.Address = src.AccountingSupplierParty.Party.PostalAddress.StreetName.Value;
            dest.Supplier.Address += " " + src.AccountingSupplierParty.Party.PostalAddress.BuildingNumber.Value;
            dest.Supplier.CityName = src.AccountingSupplierParty.Party.PostalAddress.CityName.Value;
            dest.Supplier.PostalZone = src.AccountingSupplierParty.Party.PostalAddress.PostalZone.Value;
            if (src.AccountingSupplierParty.Party.IndustryClassificationCode != null)
            {
                dest.Supplier.IndustryClassificationCode = src.AccountingSupplierParty.Party.IndustryClassificationCode.Value;
                dest.Supplier.IndustryClassificationName = src.AccountingSupplierParty.Party.IndustryClassificationCode.name;
            }

             
            var lines = new List<InvoiceLineViewModel>();
            foreach (var item in src.InvoiceLine)
            {
                var lineDto = new InvoiceLineViewModel();
                lineDto.ID = item.ID.Value;
                if (item.UUID != null)
                    lineDto.UUID = item.UUID.Value;

                lineDto.ItemName = item.Item.Name.Value;
                lineDto.UnitCode = item.InvoicedQuantity.unitCode;
                lineDto.InvoicedQuantity = item.InvoicedQuantity.Value;
                lineDto.VatPercentage = item.TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value;
                lineDto.TaxAmount = item.TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                lines.Add(lineDto);
            }

            dest.Lines = lines.ToArray();
            return dest;
        }

        public void WriteToFile(InvoiceViewModel dto, FileInfo fi)
        {
            var tmpFilename = fi.FullName + "_tmp";
            File.Copy(fi.FullName, tmpFilename, true);
            //FileStream fs = fi.Open(FileMode.Open);
            //FileStream fs = new FileStream(tmpFilename, FileMode.Open);
            PdfReader reader = new PdfReader(tmpFilename);
            
            
            byte[] metadata = reader.Metadata;
            //reader.Close();
            //@"C:\eInvoicePdf\out.pdf"

            string ns = "http://www.eInvoicePdf.gr/ns/XMP/";
            string ublKey = "e:file";


            XmpCore.IXmpMeta meta = XmpCore.XmpMetaFactory.ParseFromBuffer(metadata);
            string content = meta.GetPropertyString(ns, ublKey);

            //XmpCore.XmpMetaFactory.SchemaRegistry.RegisterNamespace(targetNs, "e");
            //meta.DeleteProperty(ns, ublKey);
            
            
            meta.SetProperty(ns, ublKey, content);

            PdfStamper stamper = new PdfStamper(reader, new FileStream(fi.FullName , FileMode.Create));
            XmpCore.Options.SerializeOptions opts = new XmpCore.Options.SerializeOptions();
            metadata = XmpCore.XmpMetaFactory.SerializeToBuffer(meta, opts);
            stamper.XmpMetadata = metadata;
            stamper.Close();
            reader.Close();
            File.Delete(tmpFilename);

        }

        public InvoiceType Convert2Invoice(InvoiceViewModel src)
        {
            var dest = new InvoiceType();
            dest.ID = new IDType() { Value = src.ID };
            dest.IssueDate = new IssueDateType() { Value = src.IssueDate };
            dest.Note = new NoteType[]  { new NoteType() { Value = src.Reason }};
            dest.InvoiceTypeCode = new InvoiceTypeCodeType { Value = src.InvoiceType };
            

            return dest;
        }
    }
}
