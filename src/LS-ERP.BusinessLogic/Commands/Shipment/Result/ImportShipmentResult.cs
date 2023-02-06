﻿using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class ImportShipmentResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public Shipment Result { get; set; }
    }
}
