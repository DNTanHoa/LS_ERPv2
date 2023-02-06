using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Model
{
    public class CommonPagingResponseModel<T>
    {
        public T Data { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
    }
}
