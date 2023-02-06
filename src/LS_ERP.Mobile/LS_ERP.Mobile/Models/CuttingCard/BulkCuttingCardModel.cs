using System;
using System.Collections.Generic;
using System.Text;

namespace LS_ERP.Mobile.Models
{
    public class BulkCuttingCardModel
    {
        public List<string> Ids { get; set; }
        public StorageBinEntryModel StorageBinEntry { get; set; }
        public string UserName { get; set; }
        public string CurrentOperation { get; set; }
        public bool IsCompling { get; set; }
        public DeliveryNoteModel DeliveryNote { get; set; }
    }
}
