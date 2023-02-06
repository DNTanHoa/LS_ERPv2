using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Audit
    {
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Audit SetCreateAudit(string user)
        {
            CreatedBy = user;
            LastUpdatedBy = user;
            CreatedAt = DateTime.Now;
            LastUpdatedAt = DateTime.Now;
            return this;
        }

        public Audit SetUpdateAudit(string user)
        {
            LastUpdatedBy = user;
            LastUpdatedAt = DateTime.Now;
            return this;
        }
    }
}
