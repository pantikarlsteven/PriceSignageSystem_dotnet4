using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WinSCP;

namespace PriceSignageSystem.Services
{
    public class FTPService
    {
        [Obsolete]
        public static SessionOptions GetSessionOptions(string server, int protocol)
        {
            SessionOptions sessionOptions = new SessionOptions();
            try
            {
                sessionOptions = new SessionOptions
                {
                    Protocol = (Protocol)Enum.ToObject(typeof(Protocol), protocol),
                    HostName = server,
                    //UserName = ConfigurationManager.AppSettings["Username"],
                    //Password = ConfigurationManager.AppSettings["Password"],
                    UserName = "snrit",
                    Password = "snrit@2021",
                    Timeout = TimeSpan.FromMinutes(30),
                    GiveUpSecurityAndAcceptAnySshHostKey = protocol == 0 ? true : false
                };
            }
            catch (Exception ex)
            {
                //WriteToFile.Log(ex.Message);
            }

            return sessionOptions;
        }
    }
}