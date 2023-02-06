using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseOrderLineExportQuantity : INotifyPropertyChanged
    {
        private Template _Template = Template.Template1;
        [ImmediatePostData]
        public Template Template
        {
            get { return _Template; }
            set
            {
                if (value != _Template)
                {
                    _Template = value;
                    NotifyPropertyChanged(nameof(Template));
                }
            }
        }

        private GroupQtyEnum _GroupQty = GroupQtyEnum.Color;
        [XafDisplayName("Group")]
        public GroupQtyEnum GroupQty
        {
            get { return _GroupQty; }
            set
            {
                if (value != _GroupQty)
                {
                    _GroupQty = value;
                    NotifyPropertyChanged(nameof(GroupQty));
                }
            }
        }

        private int _Rounding = 0;
        public int Rounding
        {
            get { return _Rounding; }
            set
            {
                if (value != _Rounding)
                {
                    _Rounding = value;
                    NotifyPropertyChanged(nameof(Rounding));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Template
    {
        [XafDisplayName("Template 1 (Model or Item; Qty) - ADVN")]
        Template1,
        [XafDisplayName("Template 2 (Code DSM, Model or Item; Qty; Supplier Cnuf; Shipment; Incoterm; FG CC; Comments; Item feature)")]
        Template2
    }

    public enum GroupQtyEnum
    {
        Color,
        Item
    };
}
