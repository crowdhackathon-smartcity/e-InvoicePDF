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
        IList<IndustryClassificationCode> getIndustryClassificationCodes()
        {
            return new List<IndustryClassificationCode>();

        }

        IList<InvoiceTypeDto> getInvoiceTypes()
        {
            return new List<InvoiceTypeDto>();
        }

        public IList<InvoiceDto> LoadFromIn()
        {
            return LoadFromDirectory(new DirectoryInfo(@"C:\eInvoicePdf\In"));
        }

        public IList<InvoiceDto> LoadFromOut()
        {
            return LoadFromDirectory(new DirectoryInfo(@"C:\eInvoicePdf\Out"));
        }


        public IList<InvoiceDto> LoadFromDirectory(DirectoryInfo dir)
        {

            return new List<InvoiceDto>();
        }

        public InvoiceDto LoadFromFile(FileInfo file)
        {
            PdfReader reader = new PdfReader(file.FullName);
            byte[] metadata = reader.Metadata;

            //XmpCore.XmpMetaFactory.
            XmpCore.IXmpMeta meta = XmpCore.XmpMetaFactory.ParseFromBuffer(reader.Metadata);

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

        public InvoiceDto Convert2Dto(InvoiceType src)
        {
            var dest = new InvoiceDto();
            dest.ID = src.ID.Value;
            dest.IssueDate = src.IssueDate.Value;
            if (src.Note.Length > 0)
                dest.Reason = src.Note[0].Value;

            dest.InvoiceType = src.InvoiceTypeCode.Value;

            dest.Supplier = new Party();
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


            var lines = new List<InvoiceLineDto>();
            foreach (var item in src.InvoiceLine)
            {
                var lineDto = new InvoiceLineDto();
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

        public void WriteToFile(InvoiceDto dto, FileInfo fi)
        {
            var tmpFilename = fi.FullName + "_tmp";
            File.Copy(fi.FullName, tmpFilename);
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

        public InvoiceType Convert2Invoice(InvoiceDto dto)
        {
            var invoice = new InvoiceType();



            return invoice;
        }
    }
}
