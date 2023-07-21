using PriceSignageAutoPrintingService.Repository;
using PriceSignageSystem.Models.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PriceSignageAutoPrintingService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)
        private readonly IPrintSignageRepository _iPrintSignageRepository;
        public Service1()
        {
            InitializeComponent();
            _iPrintSignageRepository = new PrintSignageRepository();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile.Log("Service is started at " + DateTime.Now);
            PrintPCAWithInventory();
            //PrintSKUChangesWithInventory();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000; //number in miliseconds. 60000 = 1min
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            WriteToFile.Log("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            PrintPCAWithInventory();
            //PrintSKUChangesWithInventory();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000; //number in miliseconds
            timer.Enabled = true;
        }

        #region For SKU CHANGES
        //public void PrintSKUChangesWithInventory()
        //{
        //    List<decimal> skuList = new List<decimal>();
        //    // Set up the connection and command
        //    using (var connection = new SqlConnection("Data Source=199.84.0.201;Initial Catalog=PriceSignageDb;User ID=sa;Password=@dm1n@8800;"))
        //    using (var command = new SqlCommand("sp_AutoPrintSignage", connection))
        //    {
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@loc", SqlDbType.Decimal).Value = "201";
        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            var record = new STRPRCLogDto
        //            {
        //                O3SKU = (decimal)reader["O3SKU"],
        //                IsPrinted = (bool)reader["IsPrinted"] ? "Yes" : "No",
        //                Inv = reader["INV2"].ToString(),
        //            };

        //            if (record.IsPrinted == "No" && record.Inv == "Y")
        //            {
        //                skuList.Add(record.O3SKU);
        //            }
        //        }
        //        PrintSignage.Print(skuList);
        //        reader.Close();
        //        connection.Close();
        //    }
        //}
        #endregion

        public void PrintPCAWithInventory()
        {
            var status = _iPrintSignageRepository.PrintStatus();
            if (!status.IsPrinted && status.O3DATE != 999999)
            {
                WriteToFile.Log("Starting " + status.O3DATE);
                var skuList = _iPrintSignageRepository.GetSKUs("sp_AutoPrintSignagePCA", status.O3DATE);
                _iPrintSignageRepository.Print(skuList);
            }
        }
    }
}
