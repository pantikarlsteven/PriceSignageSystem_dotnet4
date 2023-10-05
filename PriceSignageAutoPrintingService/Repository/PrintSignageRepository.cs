using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Win32;
using PdfiumViewer;
using PriceSignageSystem.Code;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageAutoPrintingService.Repository
{
    public interface IPrintSignageRepository
    {
        AutoPrintStatusDto PrintStatus();
        void Print(List<decimal> skuList);
        List<decimal> GetSKUs(string sp, decimal o3Date);
    }

    public class PrintSignageRepository : IPrintSignageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        private readonly int storeId;
        private readonly string defaultPDFViewerLocation;
        private readonly string printer;
        private readonly string crystalReportPath;

        public PrintSignageRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            defaultPDFViewerLocation = ConfigurationManager.AppSettings["DefaultPDFViewerLocation"];
            printer = ConfigurationManager.AppSettings["Printer"];
            crystalReportPath = ConfigurationManager.AppSettings["CrystalReportPath"];
            _db = new ApplicationDbContext();
        }

        public void AddMultipleInventoryPrintingLog(List<decimal> skuList, string user)
        {
            WriteToFile.Log("Start add inventory printing logs");
            foreach (var item in skuList)
            {
                _db.InventoryPrintingLogs.Add(new InventoryPrintingLog()
                {
                    O3SKU = item,
                    PrintedBy = user,
                    DateCreated = DateTime.Now
                });
            }
            _db.SaveChanges();
            WriteToFile.Log("end add inventory printing logs");
        }

        public List<ReportDto> GetReportDataList(List<decimal> skuList)
        {
            var data = new List<ReportDto>();
            try
            {
                data = (from a in _db.STRPRCs
                        join b in _db.Countries on a.O3TRB3 equals b.iatrb3 into ab
                        from c in ab.DefaultIfEmpty()
                        where skuList.Contains(a.O3SKU)
                        select new ReportDto
                        {
                            O3LOC = a.O3LOC,
                            O3CLAS = a.O3CLAS,
                            O3IDSC = a.O3IDSC,
                            O3SKU = a.O3SKU,
                            O3SCCD = a.O3SCCD,
                            O3UPC = a.O3UPC,
                            O3VNUM = a.O3VNUM,
                            O3TYPE = a.O3TYPE,
                            O3DEPT = a.O3DEPT,
                            O3SDPT = a.O3SDPT,
                            O3SCLS = a.O3SCLS,
                            O3POS = a.O3POS,
                            O3POSU = a.O3POSU,
                            O3REG = a.O3REG,
                            O3REGU = a.O3REGU,
                            O3ORIG = a.O3ORIG,
                            O3ORGU = a.O3ORGU,
                            O3EVT = a.O3EVT,
                            O3PMMX = a.O3PMMX,
                            O3PMTH = a.O3PMTH,
                            O3PDQT = a.O3PDQT,
                            O3PDPR = a.O3PDPR,
                            O3SDT = a.O3SDT,
                            O3EDT = a.O3EDT,
                            O3TRB3 = a.O3TRB3,
                            O3FGR = a.O3FGR,
                            O3FNAM = a.O3FNAM,
                            O3MODL = a.O3MODL,
                            O3LONG = a.O3LONG,
                            O3SLUM = a.O3SLUM,
                            O3DIV = a.O3DIV,
                            O3TUOM = a.O3TUOM,
                            O3DATE = a.O3DATE,
                            O3CURD = a.O3CURD,
                            O3USER = a.O3USER,
                            DateUpdated = a.DateUpdated,
                            TypeId = a.TypeId,
                            SizeId = a.SizeId,
                            CategoryId = a.CategoryId,
                            country_img = c.country_img
                        }).ToList();
            }
            catch (Exception ex)
            {
                WriteToFile.Log(ex.Message);
            }

            return data;
        }

        public List<decimal> GetSKUs(string sp, decimal o3Date)
        {
            List<decimal> skuList = new List<decimal>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@loc", SqlDbType.Int).Value = storeId;
                command.Parameters.Add("@sdt", SqlDbType.Int).Value = o3Date;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    skuList.Add((decimal)reader["O3SKU"]);
                }
                reader.Close();
                connection.Close();
            }
            return skuList;
        }

        public void Print(List<decimal> skuList)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string pdfPath = baseDirectory + @"Reports";

            #region for testing on local
            //pdfPath = @"C:\users\syste\source\repos\PriceSignageSystem_dotnet4\PriceSignageSystem_dotnet4\PriceSignageAutoPrintingService\Reports";
            //crystalReportPath = @"C:\users\syste\source\repos\PriceSignageSystem_dotnet4\PriceSignageSystem_dotnet4\PriceSignageSystem\Reports\CrystalReports\";
            #endregion

            //Create Reports folder if not existing
            if (!Directory.Exists(pdfPath))
                Directory.CreateDirectory(pdfPath);
            
            var user = "Admin";
            try
            {
                if (skuList.Count > 0)
                {
                    var data = GetReportDataList(skuList);
                    foreach (var item in data)
                    {
                        item.UserName = user;
                        var textToImage = new TextToImage();
                        textToImage.GetImageWidth(item.O3FNAM, item.O3IDSC, ReportConstants.Size.Whole);
                        item.IsSLBrand = textToImage.IsSLBrand;
                        item.IsSLDescription = textToImage.IsSLDescription;
                    }
                    var dataTable = ConversionHelper.ConvertListToDataTable(data);
                    var reportPath = string.Empty;
                    reportPath = crystalReportPath + "Dynamic_SkinnyReport.rpt";
                    WriteToFile.Log(reportPath);

                    ReportDocument report = new ReportDocument();
                    report.Load(reportPath);
                    report.SetDataSource(dataTable);
                    Guid guid = Guid.NewGuid();
                    var pdf = pdfPath + "\\" + guid + ".pdf";

                    //Export report to pdf
                    ExportOptions CrExportOptions;
                    DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                    PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                    CrDiskFileDestinationOptions.DiskFileName = pdf;
                    WriteToFile.Log(CrDiskFileDestinationOptions.DiskFileName);
                    CrExportOptions = report.ExportOptions;
                    {
                        CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                        CrExportOptions.FormatOptions = CrFormatTypeOptions;
                    }
                    report.Export();

                    const string pdfFileExtension = ".pdf";
                    // Check if the PDF viewer executable path exists
                    if (!System.IO.File.Exists(defaultPDFViewerLocation))
                    {
                        WriteToFile.Log("PDF viewer executable path does not exist.");
                        return;
                    }

                    // Set the PDF viewer application path as the default handler for .pdf files
                    using (RegistryKey pdfFileKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\.pdf", true))
                    {
                        if (pdfFileKey != null)
                        {
                            // Create a new subkey for the file type
                            using (RegistryKey fileTypeKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + pdfFileExtension.Substring(1)))
                            {
                                // Set the default value to the PDF viewer application name
                                fileTypeKey.SetValue("", "PDFFile", RegistryValueKind.String);
                            }
                            // Set the default value for the file type key to the PDF viewer executable path
                            pdfFileKey.SetValue("", "PDFFile", RegistryValueKind.String);
                        }
                    }

                    // Set the default command to open PDF files with the specified viewer
                    using (RegistryKey openCommandKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\PDFFile\\shell\\open\\command", true))
                    {
                        if (openCommandKey != null)
                        {
                            openCommandKey.SetValue("", "\"" + defaultPDFViewerLocation + "\" \"%1\"", RegistryValueKind.String);
                        }
                    }

                    WriteToFile.Log("Default PDF viewer set successfully.");

                    PrinterSettings printerSettings = new PrinterSettings()
                    {
                        PrinterName = printer,
                        Copies = 1
                    };

                    PageSettings pageSettings = new PageSettings(printerSettings)
                    {
                        Margins = new Margins(0, 0, 0, 0)
                    };

                    foreach (System.Drawing.Printing.PaperSize paperSize in printerSettings.PaperSizes)
                    {
                        if (paperSize.PaperName == "Letter")
                        {
                            pageSettings.PaperSize = paperSize;
                            break;
                        }
                    }

                    using (PdfDocument pdfDocument = PdfDocument.Load(pdf))
                    {
                        using (PrintDocument printDocument = pdfDocument.CreatePrintDocument())
                        {
                            printDocument.PrinterSettings = printerSettings;
                            printDocument.DefaultPageSettings = pageSettings;
                            printDocument.PrintController = (PrintController)new StandardPrintController();
                            printDocument.Print();
                            WriteToFile.Log("Start printing");
                        }
                    }

                    report.Close();
                    report.Dispose();
                    UpdatePrintStatus();
                    //UpdateMultipleStatus(skuList);
                    //AddMultipleInventoryPrintingLog(o3skus, user);

                }
                else
                    throw new Exception("No Selected Id");
            }
            catch (Exception ex)
            {
                WriteToFile.Log(ex.Message);
            }
        }

        public AutoPrintStatusDto PrintStatus()
        {
            var dto = new AutoPrintStatusDto();
            using (var connection = new SqlConnection(connectionString))
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM AutoPrintStatus", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dto.IsPrinted = reader.GetBoolean(0);
                            dto.O3DATE = reader.GetDecimal(1);
                        }
                        else
                        {
                            dto.IsPrinted = false;
                            dto.O3DATE = 999999;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return dto;
        }

        public void UpdatePrintStatus()
        {
            WriteToFile.Log("Start update autoprint IsPrinted status");
            using (var connection = new SqlConnection(connectionString))
            try
            {
                connection.Open();
                string sqlUpdateQuery = "UPDATE AutoPrintStatus SET IsPrinted = 1";

                using (SqlCommand command = new SqlCommand(sqlUpdateQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    WriteToFile.Log("IsPrinted status set to 1");
                }
            }
            catch (Exception ex)
            {
                WriteToFile.Log(ex.Message);
            }
            WriteToFile.Log("end");
        }

        public void UpdateMultipleStatus(List<decimal> skuList)
        {
            WriteToFile.Log("Start update strprc and strprclogs IsPrinted status");
            foreach (var item in skuList)
            {
                var data = _db.STRPRCs.Where(a => a.O3SKU == item).FirstOrDefault();
                data.IsPrinted = true;

                var logs = _db.STRPRCLogs.Where(a => a.O3SKU == item).ToList();

                foreach (var log in logs)
                {
                    log.IsPrinted = true;
                }
            }
            _db.SaveChanges();
            WriteToFile.Log("end");
        }
    }
}
