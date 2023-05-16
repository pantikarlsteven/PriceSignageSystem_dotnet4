using System.Collections.Generic;

namespace PriceSignageSystem.Models.Interface
{
    public interface ISizeRepository
    {
        IEnumerable<Size> GetAllSizes();
    }
}