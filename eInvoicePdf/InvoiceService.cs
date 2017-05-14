using eInvoicePdf.Model;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.xmp.impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace eInvoicePdf
{

    public class InvoiceService
    {
        public IList<IndustryClassificationCodeViewModel> getIndustryClassificationCodes()
        {
            var retList = new List<IndustryClassificationCodeViewModel>();
            retList.Add(new IndustryClassificationCodeViewModel() { Code = "62.01.11.03", Desc = "Υπηρεσίες ανάπτυξης λογισμικού πολυμέσων" });
            retList.Add(new IndustryClassificationCodeViewModel() { Code = "71.12.19.04", Desc = "Υπηρεσίες μελετών λιμενικών έργων" });
            retList.Add(new IndustryClassificationCodeViewModel() { Code = "46.47.1", Desc = "Χονδρικό εμπόριο επίπλων" });
            return retList;
        }

        public IList<InvoiceTypeViewModel> getInvoiceTypes()
        {
            var retList = new List<InvoiceTypeViewModel>();
            retList.Add(new InvoiceTypeViewModel { Desc = "ΤΙΜΟΛΟΓΙΟ", Code = "01" });
            retList.Add(new InvoiceTypeViewModel { Desc = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "02" });
            retList.Add(new InvoiceTypeViewModel { Desc = "ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ", Code = "03" });
            return retList;
        }

        public IList<CpvViewModel> getCPVs()
        {
            var retList = new List<CpvViewModel>();
            retList.Add(new CpvViewModel { Code = "30194220-3", Desc = "Μεμβράνες σχεδίασης" });
            retList.Add(new CpvViewModel { Code = "30234500-3", Desc = "Μέσα αποθήκευσης μνήμης" });
            retList.Add(new CpvViewModel { Code = "45262522-6", Desc = "Εργασίες τοιχοποιίας" });
            retList.Add(new CpvViewModel { Code = "72600000-6", Desc = "Υπηρεσίες παροχής συμβουλών και υποστήριξης Η/Υ" });
            retList.Add(new CpvViewModel { Code = "80540000-1", Desc = "Υπηρεσίες περιβαλλοντικής εκπαίδευσης" });
            return retList;
        }

        public IList<InvoiceViewModel> LoadFromIn()
        {
            return LoadFromDirectory(InDir);
        }

        public IList<InvoiceViewModel> LoadFromOut()
        {
            return LoadFromDirectory(OutDir);
        }

        public IList<InvoiceViewModel> LoadFromDirectory(DirectoryInfo dir)
        {
            var files = dir.EnumerateFiles("*.pdf").ToList();
            var retList = new List<InvoiceViewModel>();
            foreach (var item in files)
            {
                retList.Add(LoadFromFile(item));
            }
            return retList;
        }

        public DirectoryInfo OutDir
        {
            get
            {
                var dirPath = Path.Combine(Profile.WorkingDirectory, "Out");
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                return new DirectoryInfo(dirPath);
            }
        }

        public DirectoryInfo InDir
        {
            get
            {
                var dirPath = Path.Combine(Profile.WorkingDirectory, "In");// @"C:\eInvoicePdf\In";
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                return new DirectoryInfo(dirPath);
            }
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
            dest.Supplier.StreetName = src.AccountingSupplierParty.Party.PostalAddress.StreetName.Value;
            dest.Supplier.BuildingNumber = src.AccountingSupplierParty.Party.PostalAddress.BuildingNumber.Value;
            dest.Supplier.CityName = src.AccountingSupplierParty.Party.PostalAddress.CityName.Value;
            dest.Supplier.PostalZone = src.AccountingSupplierParty.Party.PostalAddress.PostalZone.Value;
            if (src.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxLevelCode != null)
                dest.Supplier.TaxationAuthority = src.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxLevelCode.Value;

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
                {
                    lineDto.UUID = item.UUID.Value;
                }


                lineDto.ItemName = item.Item.Name.Value;
                lineDto.UnitCode = item.InvoicedQuantity.unitCode;
                lineDto.InvoicedQuantity = item.InvoicedQuantity.Value;
                lineDto.VatPercentage = item.TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value;
                lineDto.TaxAmount = item.TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                if (item.Price != null)
                    lineDto.PriceAmount = item.Price.PriceAmount.Value;
                lines.Add(lineDto);
            }

            dest.Lines = lines.ToArray();
            return dest;
        }

        public void Merge(InvoiceViewModel dto, FileInfo inPdf)
        {
            var tmpFilename = inPdf.FullName + "_tmp";
            File.Copy(inPdf.FullName, tmpFilename, true);
            //FileStream fs = fi.Open(FileMode.Open);
            //FileStream fs = new FileStream(tmpFilename, FileMode.Open);
            PdfReader reader = new PdfReader(tmpFilename);


            byte[] metadata = reader.Metadata;
            //reader.Close();
            //@"C:\eInvoicePdf\out.pdf"

            string ns = "http://www.eInvoicePdf.gr/ns/XMP/";
            string ublKey = "e:file";


            XmpCore.IXmpMeta meta = null;
            if (meta != null)
                meta = XmpCore.XmpMetaFactory.ParseFromBuffer(metadata);
            else
                meta = XmpCore.XmpMetaFactory.Create();
            //string content = meta.GetPropertyString(ns, ublKey);

            //if (! XmpCore.XmpMetaFactory.SchemaRegistry.Namespaces.an)
            XmpCore.XmpMetaFactory.SchemaRegistry.RegisterNamespace(ns, "e");
            var invoice = Convert2Invoice(dto);
            var content = string.Empty;

            XmlSerializer ser = new XmlSerializer(typeof(InvoiceType));
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    ser.Serialize(writer, invoice);
                    content = sww.ToString(); // Your XML
                }
            }

            meta.SetProperty(ns, ublKey, content);

            PdfStamper stamper = new PdfStamper(reader, new FileStream(inPdf.FullName, FileMode.Create));
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
            dest.Note = new NoteType[] { new NoteType() { Value = src.Reason } };
            dest.InvoiceTypeCode = new InvoiceTypeCodeType { Value = src.InvoiceType };

            dest.AccountingSupplierParty = new SupplierPartyType
            {
                CustomerAssignedAccountID = new CustomerAssignedAccountIDType() { Value = src.Supplier.AccountID },
                Party = new PartyType()
                {
                    PartyName = new PartyNameType[] { new PartyNameType() { Name = new NameType1 { Value = src.Supplier.Name } } },
                    PartyTaxScheme = new PartyTaxSchemeType[] { new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { ID = new IDType { Value = src.Supplier.VAT } } } },
                    PostalAddress = new AddressType
                    {
                        StreetName = new StreetNameType { Value = src.Supplier.StreetName },
                        BuildingNumber = new BuildingNumberType { Value = src.Supplier.BuildingNumber },
                        CityName = new CityNameType { Value = src.Supplier.CityName },
                        PostalZone = new PostalZoneType { Value = src.Supplier.PostalZone }
                    }
                }
            };

            if (!string.IsNullOrEmpty(src.Supplier.IndustryClassificationCode))
            {
                dest.AccountingSupplierParty.Party.IndustryClassificationCode = new IndustryClassificationCodeType { Value = src.Supplier.IndustryClassificationCode };
            }

            if (!string.IsNullOrEmpty(src.Supplier.TaxationAuthority))
                dest.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxLevelCode = new TaxLevelCodeType { Value = src.Supplier.TaxationAuthority }; 
            //dest.Supplier.TaxationAuthority = ;

            var lines = new List<InvoiceLineType>();
            foreach (var item in src.Lines)
            {
                var line = new InvoiceLineType();
                line.ID = new IDType { Value = item.ID };
                if (!string.IsNullOrEmpty(item.UUID))
                    line.UUID = new UUIDType { Value = item.UUID };

                line.InvoicedQuantity = new InvoicedQuantityType { unitCode = item.UnitCode, Value = item.InvoicedQuantity };
                line.Item = new ItemType { Name = new NameType1 { Value = item.ItemName } };
                line.TaxTotal = new TaxTotalType[]
                {
                     new TaxTotalType { TaxSubtotal = new TaxSubtotalType[]
                                        {
                                            new TaxSubtotalType
                                            {
                                                TaxCategory = new TaxCategoryType { Percent = new PercentType1 { Value = item.VatPercentage } },
                                                 TaxAmount = new TaxAmountType { Value = item.TaxAmount}
                                            }
                                        }
                                      }
                };
                line.Price = new PriceType { PriceAmount = new PriceAmountType { Value = item.PriceAmount } };
                lines.Add(line);
            }
            dest.InvoiceLine = lines.ToArray();
            return dest;
        }

        public void CreatePdf(InvoiceViewModel invoice)
        {
            Document document = new Document();
            try
            {

                string invoiceHtml = LoadInvoiceHtml(invoice);
                //string invoiceLineHtml = LoadInvoiceLineHtml();

                StringBuilder lines = new StringBuilder();
                decimal grandTotal = 0;
                foreach (var item in invoice.Lines)
                {
                    lines.AppendLine(LoadInvoiceLineHtml(item));
                    grandTotal += (item.PriceAmount * item.InvoicedQuantity);
                }

                invoiceHtml = invoiceHtml.Replace("@InvoiceLine", lines.ToString()).Replace("@GrandTotal", grandTotal.ToString()).Replace("@PartyColor", Profile.Color);
                var htmlFile = Path.Combine(OutDir.FullName, "_tmp.html"); //@"C:\eInvoicePdf\xtra\_tmp.html";

                File.WriteAllText(htmlFile, invoiceHtml);
                //if (string.IsNullOrEmpty(invoice.Filename))
                invoice.Filename = Guid.NewGuid().ToString() + ".pdf";

                var pdfFile = Path.Combine(OutDir.FullName, invoice.Filename);
                var pdfGeneratorTool = ConfigurationManager.AppSettings.Get("PdfGenerator");  //"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe";
                //Process.Start("wkhtmltopdf.exe", "\"C:\\eInvoicePdf\\xtra\\_tmp.html\"  \"C:\\eInvoicePdf\\out\\generated.pdf\"");
                Process.Start(pdfGeneratorTool, "\"" + htmlFile + "\"  " + "\"" + pdfFile + "\"");

                System.Threading.Thread.Sleep(500);

                Merge(invoice, new FileInfo(pdfFile));

                //PdfWriter.GetInstance(document, new FileStream(@"C:\eInvoicePdf\Out\generated.pdf", FileMode.Create));
                //document.Open();
                //var worker = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                //worker.Parse(new StringReader(invoiceHtml));
                //document.Close();

                //PdfSharp.Pdf.PdfDocument pdf = PdfGenerator.GeneratePdf(invoiceHtml, PdfSharp.PageSize.A4);
                //pdf.Save(@"C:\eInvoicePdf\Out\generated.pdf");
            }
            catch
            {
                throw;
            }
        }

        public string LoadInvoiceHtml(InvoiceViewModel item)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Invoice.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var pattern = reader.ReadToEnd();
                var result = pattern.Replace("@InvoiceType", item.InvoiceType)
                    .Replace("@Supplier", item.Supplier.Name)
                    .Replace("@InvoiceId", item.ID)
                    .Replace("@Reason", item.Reason)
                    .Replace("@IssueDate", item.IssueDate.ToShortDateString())
                    .Replace("@VAT", item.Supplier.VAT)
                    .Replace("@Address", item.Supplier.StreetName + " " + item.Supplier.BuildingNumber.ToString())
                    .Replace("@TaxationAuthority", item.Supplier.TaxationAuthority)
                    .Replace("@CityName", item.Supplier.CityName)
                    .Replace("@IndustryClassification", item.Supplier.IndustryClassificationName)
                    .Replace("@PostalZone", item.Supplier.PostalZone);
                return result;
            }
        }

        public string LoadInvoiceLineHtml(InvoiceLineViewModel item)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".InvoiceLine.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var pattern = reader.ReadToEnd();
                var result = pattern.Replace("@CPV", item.UUID)
                    .Replace("@ItemName", item.ItemName.ToString())
                    .Replace("@UnitCode", item.UnitCode)
                    .Replace("@PriceAmount", item.PriceAmount.ToString())
                    .Replace("@InvoicedQuantity", item.InvoicedQuantity.ToString())
                    .Replace("@Tax", item.TaxAmount.ToString())
                    .Replace("@LineTotal", (item.PriceAmount * item.InvoicedQuantity).ToString());
                return result;
            }

        }

        public static PartyProfile Profile
        {
            get
            {
                var settings = ConfigurationManager.AppSettings;

                return new PartyProfile
                {
                    WorkingDirectory = settings.Get("WorkingDirectory"),
                    Color = settings.Get("Color"),
                    Name = settings.Get("PartyName"),
                    VAT = settings.Get("VAT"),
                    StreetName = settings.Get("StreetName"),
                    BuildingNumber = settings.Get("BuildingNumber"),
                    CityName = settings.Get("CityName"),
                    TaxationAuthority = settings.Get("TaxationAuthority"),
                    IndustryClassification = settings.Get("IndustryClassification"),
                    PostalZone = settings.Get("PostalZone")
                };
            }
        }
    }

    public class PartyProfile
    {
        public string Name { get; set; }
        public string VAT { get; set; }
        public string Color { get; set; }
        public string WorkingDirectory { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string CityName { get; set; }
        public string TaxationAuthority { get; set; }
        public string IndustryClassification { get; set; }
        public string PostalZone { get; set; }

    }
}
