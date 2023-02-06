using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.BusinessObjects;

namespace LS_ERP.XAF.Module
{
    public sealed partial class XAFModule : ModuleBase
    {
        public XAFModule()
        {
            InitializeComponent();
            AdditionalExportedTypes.AddRange(
            new Type[]
            {
                typeof(Brand), typeof(Customer), typeof(SalesOrder), typeof(PurchaseOrder), typeof(PartRevision),
                typeof(Unit), typeof(Vendor), typeof(PartMaterial), typeof(Audit), typeof(ItemStyle),
                typeof(PurchaseOrder), typeof(PurchaseOrderType),typeof(PurchaseOrderStatus),typeof(OrderDetail), typeof(ItemStyleBarCode),
                typeof(ItemStyleStatus), typeof(ItemPrice), typeof(Company), typeof(JobHead), typeof(ItemModel), typeof(Receipt), typeof(Storage),
                typeof(Issued), typeof(MaterialTransaction), typeof(EntitySequenceNumber), typeof(PartRevisionLog), typeof(PartRevisionLogDetail),
                typeof(MessageTemplate), typeof(ForecastGroup), typeof(ForecastDetail), typeof(ForecastOverall), typeof(ForecastMaterial),
                typeof(SalesContract), typeof(PurchaseRequest), typeof(PartRevisionLogDetail), typeof(Part), typeof(WastageSetting),
                typeof(SalesQuote), typeof(PackingList), typeof(Item), typeof(ItemStyleSyncMaster), typeof(WorkCenter), typeof(JobOutput),
                typeof(Operation), typeof(StorageImport), typeof(StorageImportDetail), typeof(PullBomType), typeof(Invoice), typeof(InvoiceDetail),
                typeof(Consignee), typeof(InvoiceType), typeof(Country), typeof(BankAccount), typeof(SalesOrderOffset),
                typeof(StorageBinEntry), typeof(Problem), typeof(Port), typeof(InvoiceDocument), typeof(InvoiceDocumentType),
                typeof(ShippingPlan), typeof(FabricPurchaseOrder), typeof(IssuedType), typeof(ReceiptType), typeof(LabelPort),
                typeof(StorageStatus), typeof(Status), typeof(FabricRequest), typeof(FabricRequestDetail),  typeof(FabricRequestLog), typeof(FabricRequestDetailLog),
                typeof(ScanResult), typeof(ScanResultDetail), typeof(Shu),
                typeof(InventoryFG), typeof(GroupMail), typeof(Shipment), typeof(ShipmentDetail), typeof(ProductionOrder), typeof(ProductionOrderLine)
            });
            DevExpress.ExpressApp.Security.SecurityModule.UsedExportedTypes = DevExpress.Persistent.Base.UsedExportedTypes.Custom;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            CompositeObjectSpace compositeObjectSpace = e.ObjectSpace as CompositeObjectSpace;
            if (compositeObjectSpace != null)
            {
                if (!(compositeObjectSpace.Owner is CompositeObjectSpace))
                {
                    compositeObjectSpace.PopulateAdditionalObjectSpaces((XafApplication)sender);
                }
            }
        }
    }
}
