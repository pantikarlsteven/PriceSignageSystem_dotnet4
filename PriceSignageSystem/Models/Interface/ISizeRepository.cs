﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface ISizeRepository
    {
        List<Size> GetAllSizes();
    }
}
