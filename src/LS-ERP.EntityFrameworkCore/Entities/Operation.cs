using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    /// <summary>
    /// Danh mục công đoạn sản xuất
    /// Ex: SPREADING - Trải vải
    /// Ex: CUTTING - Cắt
    /// </summary>
    public class Operation : Audit
    {
        public Operation()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 5);
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        /// <summary>
        /// Thứ tự công đoạn
        /// </summary>
        public int Index { get; set; }
    }
}
