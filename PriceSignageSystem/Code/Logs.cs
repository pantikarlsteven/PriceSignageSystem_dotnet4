using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Code
{
    public class Logs
    {
        public static void WriteToFile(string Message)
        {
            Console.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt") + " " + Message);

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy") + " " + Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy") + " " + Message);
                }
            }
        }
    }
}