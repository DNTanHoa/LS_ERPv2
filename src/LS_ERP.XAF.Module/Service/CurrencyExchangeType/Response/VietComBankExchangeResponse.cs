using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ExrateList
    {
        private string dateTimeField;

        private ExrateListExrate[] exrateField;

        private string sourceField;

        /// <remarks/>
        public string DateTime
        {
            get
            {
                return this.dateTimeField;
            }
            set
            {
                this.dateTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Exrate")]
        public ExrateListExrate[] Exrate
        {
            get
            {
                return this.exrateField;
            }
            set
            {
                this.exrateField = value;
            }
        }

        /// <remarks/>
        public string Source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ExrateListExrate
    {

        private string currencyCodeField;

        private string currencyNameField;

        private string buyField;

        private string transferField;

        private string sellField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CurrencyCode
        {
            get
            {
                return this.currencyCodeField;
            }
            set
            {
                this.currencyCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CurrencyName
        {
            get
            {
                return this.currencyNameField;
            }
            set
            {
                this.currencyNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Buy
        {
            get
            {
                return this.buyField;
            }
            set
            {
                this.buyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transfer
        {
            get
            {
                return this.transferField;
            }
            set
            {
                this.transferField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Sell
        {
            get
            {
                return this.sellField;
            }
            set
            {
                this.sellField = value;
            }
        }
    }
}
