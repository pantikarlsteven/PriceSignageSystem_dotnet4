using PriceSignageSystem.Models.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface IQueueRepository
    {
        ItemQueue AddItemQueue(STRPRCDto model);
        List<ReportDto> GetQueueListPerUser(string username);
    }
}
