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
        void UpdateStatus(IEnumerable<ReportDto> data);
        void QueueMultipleItems(decimal[] skus);
        List<ItemQueueDto> GetHistory(string username);
        int RequeueItem(int id, string username);
    }
}
