using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using Ultils.Calculators;
using Ultils.Extensions;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ItemStylePopupModel
    {
        public Customer Customer { get; set; }
        public Storage Storage { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Style { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public string EntryBy { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Remark { get; set; }

        public List<ItemStyleReceipt> ItemStyles { get; set; }
            = new List<ItemStyleReceipt>();

        public List<OrderDetailReceipt> OrderDetailReceipt { get; set; }
            = new List<OrderDetailReceipt>();
    }

    [DomainComponent]
    public class ItemStyleReceipt
    {
        [VisibleInListView(false)]
        public string Number { get; set; }
        public string CustomerStyle { get; set; }
        public string LSSStyle { get; set; }
        public string ProductDescription { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
    }

    [DomainComponent]
    public class OrderDetailReceipt
    {
        public string CustomerStyle { get; set; }
        public string LSSStyle { get; set; }
        public string ProductDescription { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string Location { get; set; }
        public int NumberOfCarton { get; set; }
        public string Remark { get; set; }
        public string UnitID { get; set; }
        public int OrderQuantity { get; set; }
        private int? quantity;
        public string ReceiptQuantity
        {
            get => quantity?.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var expression = value.CalculateExpression();
                    quantity = (int)InfixEvaluator.EvaluateInfix(value.ToString());
                }
                else
                {
                    quantity = null;
                }
            }
        }
    }
}
