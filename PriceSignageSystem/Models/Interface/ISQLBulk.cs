using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Interface
{
    public interface ISQLBulk
    {
        void InsertBulk(DataTable data, string tableName);
        void RemoveBulk(string tableName);
    }
}