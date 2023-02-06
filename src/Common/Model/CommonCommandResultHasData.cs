using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CommonCommandResultHasData<T> : CommonCommandResult
    {
        public CommonCommandResultHasData<T> SetData(T data)
        {
            this.Data = data;
            return this;
        }
        public T Data { get; set; }
    }
}
