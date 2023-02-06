using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CommonPagingResponseModel<T> : CommonResponseModel<T>
    {
        public CommonPagingResponseModel<T> Paging(int pageIndex, int totalCount, int pageSize)
        {
            PageInformation = new PagingModel<T>()
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return this;
        }

        public CommonPagingResponseModel<T> Paging(T data, int pageIndex, int totalCount, int pageSize)
        {
            PageInformation = new PagingModel<T>()
            {
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
            Data = data;
            Success = true;
            return this;
        }


        public PagingModel<T>? PageInformation { get; set; }
    }

    public class PagingModel<T>
    {
        public int PageIndex { get; set; }
        public int TotalCount { get; set; } = 0;
        public int PageSize { get; set; }
    }
}
