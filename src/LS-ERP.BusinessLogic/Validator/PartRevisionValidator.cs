using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class PartRevisionValidator
    {
        public PartRevisionValidator()
        {

        }

        public bool IsValid(PartRevision partRevision, out string errorMessage)
        {
            errorMessage = string.Empty;

            return true;
        }
    }
}
