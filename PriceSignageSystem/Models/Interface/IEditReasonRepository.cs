using System.Collections.Generic;

namespace PriceSignageSystem.Models.Interface
{
    public interface IEditReasonRepository
    {
        IEnumerable<EditReason> GetAllReasons();
    }
}