using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CommonCommandResult
    {
        public CommonCommandResult SetResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
            return this;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
