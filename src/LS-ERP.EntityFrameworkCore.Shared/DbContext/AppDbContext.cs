using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(120));
        }

        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrencyExchange> CurrencyExchange { get; set; }
        public virtual DbSet<CurrencyExchangeType> CurrencyExchangeType { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Division> Division { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemMeta> ItemMeta { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Part> Part { get; set; }
        public virtual DbSet<PartRevision> PartRevision { get; set; }
        public virtual DbSet<PaymentTerm> PaymentTerm { get; set; }
        public virtual DbSet<PriceTerm> PriceTerm { get; set; }
        public virtual DbSet<Tax> Tax { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }

        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<SalesOrderType> SalesOrderType { get; set; }
        public virtual DbSet<SalesOrderStatus> SalesOrderStatus { get; set; }

        public virtual DbSet<ItemStyle> ItemStyle { get; set; }
        public virtual DbSet<ItemStyleBarCode> ItemStyleBarCode { get; set; }
        public virtual DbSet<ItemStyleStatus> ItemStyleStatus { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }

        public virtual DbSet<ShippingMethod> ShippingMethod { get; set; }
        public virtual DbSet<ShippingTerm> ShippingTerm { get; set; }
        public virtual DbSet<SupplierCNUF> SupplierCNUF { get; set; }

        public virtual DbSet<ProductionBOM> ProductionBOM { get; set; }
        public virtual DbSet<ForecastMaterial> ForecastMaterial { get; set; }

        public virtual DbSet<ReservationEntry> ReservationEntry { get; set; }
        public virtual DbSet<JobHead> JobHead { get; set; }

        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<PurchaseOrderGroupLine> PurchaseOrderGroupLine { get; set; }
        public virtual DbSet<PurchaseOrderLine> PurchaseOrderLine { get; set; }
        public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatus { get; set; }
        public virtual DbSet<PurchaseOrderType> PurchaseOrderType { get; set; }
        public virtual DbSet<IncoTerm> IncoTerm { get; set; }
        public virtual DbSet<WastageSetting> WastageSetting { get; set; }

        public virtual DbSet<PartMaterial> PartMaterial { get; set; }
        public virtual DbSet<PartMaterialStatus> PartMaterialStatus { get; set; }
        public virtual DbSet<ItemPrice> ItemPrice { get; set; }
        public virtual DbSet<MaterialType> MaterialType { get; set; }

        public virtual DbSet<Size> Size { get; set; }
        public virtual DbSet<ItemModel> ItemModel { get; set; }


        public virtual DbSet<Storage> Storage { get; set; }
        public virtual DbSet<StorageDetail> StorageDetail { get; set; }
        public virtual DbSet<Receipt> Receipt { get; set; }
        public virtual DbSet<ReceiptGroupLine> ReceiptGroupLine { get; set; }
        public virtual DbSet<ReceiptLine> ReceiptLine { get; set; }

        public virtual DbSet<MaterialTransaction> MaterialTransaction { get; set; }
        public virtual DbSet<Issued> Issued { get; set; }
        public virtual DbSet<IssuedGroupLine> IssuedGroupLine { get; set; }
        public virtual DbSet<IssuedLine> IssuedLine { get; set; }

        public virtual DbSet<EntitySequenceNumber> EntitySequenceNumber { get; set; }
        public virtual DbSet<PartRevisionLog> PartRevisionLog { get; set; }
        public virtual DbSet<PartRevisionLogDetail> PartRevisionLogDetail { get; set; }

        public virtual DbSet<MessageTemplate> MessageTemplate { get; set; }

        public virtual DbSet<ForecastGroup> ForecastGroup { get; set; }
        public virtual DbSet<ForecastOverall> ForecastOverall { get; set; }
        public virtual DbSet<ForecastDetail> ForecastDetail { get; set; }
        public virtual DbSet<ForecastEntry> ForecastEntry { get; set; }

        public virtual DbSet<Week> Week { get; set; }

        public virtual DbSet<PurchaseRequest> PurchaseRequest { get; set; }
        public virtual DbSet<PurchaseRequestStatus> PurchaseRequestStatus { get; set; }
        public virtual DbSet<PurchaseRequestLog> PurchaseRequestLog { get; set; }
        public virtual DbSet<PurchaseRequestGroupLine> PurchaseRequestGroupLine { get; set; }
        public virtual DbSet<PurchaseRequestLine> PurchaseRequestLine { get; set; }
        public virtual DbSet<FabricPurchaseOrder> FabricPurchaseOrder { get; set; }

        public virtual DbSet<SalesContract> SalesContract { get; set; }
        public virtual DbSet<SalesContractDetail> SalesContractDetail { get; set; }
        public virtual DbSet<ShippingMark> ShippingMark { get; set; }
        public virtual DbSet<BoxDimension> BoxDimension { get; set; }
        public virtual DbSet<StyleNetWeight> StyleNetWeight { get; set; }
        public virtual DbSet<PackingList> PackingList { get; set; }
        public virtual DbSet<PackingLine> PackingLine { get; set; }
        public virtual DbSet<PackingOverQuantity> PackingOverQuantity { get; set; }
        public virtual DbSet<PackingUnit> PackingUnit { get; set; }

        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<SalesQuote> SalesQuote { get; set; }
        public virtual DbSet<SalesQuoteStatus> SalesQuoteStatus { get; set; }
        public virtual DbSet<SalesQuoteDetail> SalesQuoteDetail { get; set; }
        public virtual DbSet<SalesQuoteLog> SalesQuoteLog { get; set; }
        public virtual DbSet<PackingListImageThumbnail> PackingListImageThumbnail { get; set; }
        public virtual DbSet<Pad> Pad { get; set; }
        public virtual DbSet<ReservationForecastEntry> ReservationForecastEntry { get; set; }
        public virtual DbSet<PullBomType> PullBomType { get; set; }

        public virtual DbSet<ViewCustomerStyle> ViewCustomerStyle { get; set; }

        public virtual DbSet<ViewPONumber> ViewPONumber { get; set; }

        /// <summary>
        /// Đồng bộ nguyên phụ liệu
        /// </summary>
        public virtual DbSet<ItemStyleSyncMaster> ItemStyleSyncMasters { get; set; }
        public virtual DbSet<ItemStyleSyncAction> ItemStyleSyncActions { get; set; }
        public virtual DbSet<OrderDetailSync> OrderDetailSyncs { get; set; }
        public virtual DbSet<MaterialSync> MaterialSyncs { get; set; }
        public virtual DbSet<MaterialSyncDetail> MaterialSyncDetails { get; set; }

        /// <summary>
        /// Sản lượng sản xuất
        /// </summary>
        /// <param name="builder"></param>
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<OperationDetail> OperationDetail { get; set; }
        public virtual DbSet<WorkCenter> WorkCenter { get; set; }
        public virtual DbSet<WorkCenterType> WorkCenterType { get; set; }
        public virtual DbSet<JobOperation> JobOperation { get; set; }
        public virtual DbSet<JobOutput> JobOuput { get; set; }
        public virtual DbSet<WorkingTime> WorkingTime { get; set; }
        public virtual DbSet<DailyTarget> DailyTarget { get; set; }
        public virtual DbSet<DailyTargetDetail> DailyTargetDetail { get; set; }
        public virtual DbSet<Problem> Problem { get; set; }
        public virtual DbSet<AllocDailyOutput> AllocDailyOutput { get; set; }

        public virtual DbSet<AllocTransaction> AllocTransaction { get; set; }
        public virtual DbSet<AllocPriority> AllocPriority { get; set; }

        public virtual DbSet<FabricContrast> FabricContrast { get; set; }
        public virtual DbSet<CuttingOutput> CuttingOutput { get; set; }
        public virtual DbSet<CuttingLot> CuttingLot { get; set; }
        public virtual DbSet<CuttingCard> CuttingCard { get; set; }
        public virtual DbSet<PartPrice> PartPrice { get; set; }
        public virtual DbSet<JobPrice> JobPrice { get; set; }


        /// <summary>
        /// Import tồn kho
        /// </summary>
        public virtual DbSet<StorageImport> StorageImport { get; set; }
        public virtual DbSet<StorageImportDetail> StorageImportDetail { get; set; }
        public virtual DbSet<StorageBinEntry> StorageBinEntry { get; set; }
        public virtual DbSet<IssuedType> IssuedType { get; set; }
        public virtual DbSet<ReceiptType> ReceiptType { get; set; }
        public virtual DbSet<StorageStatus> StorageStatus { get; set; }
        public virtual DbSet<Shipment> Shipment { get; set; }
        public virtual DbSet<ShipmentDetail> ShipmentDetail { get; set; }

        /// <summary>
        /// Shipping
        /// </summary>
        public virtual DbSet<InvoiceType> InvoiceType { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<Consignee> Consignee { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetail { get; set; }
        public virtual DbSet<Port> Port { get; set; }
        public virtual DbSet<InvoiceDocumentType> InvoiceDocumentType { get; set; }
        public virtual DbSet<InvoiceDocument> InvoiceDocument { get; set; }
        public virtual DbSet<ShippingPlan> ShippingPlans { get; set; }
        public virtual DbSet<ShippingPlanDetail> ShippingPlanDetails { get; set; }
        public virtual DbSet<PackingOutput> PackingOutputs { get; set; }
        public virtual DbSet<ProductionDept> ProductionDept { get; set; }
        /// <summary>
        /// Đắp đơn
        /// </summary>
        public virtual DbSet<SalesOrderOffset> SalesOrderOffset { get; set; }

        /// <summary>
        /// Yêu cầu nguyên phụ liệu
        /// </summary>
        public virtual DbSet<MaterialRequest> MaterialRequests { get; set; }
        public virtual DbSet<MaterialRequestDetail> MaterialRequestDetails { get; set; }
        public virtual DbSet<MaterialRequestOrderDetail> MaterialRequestOrderDetails { get; set; }

        /// <summary>
        /// Scan barcode GA
        /// </summary>
        public virtual DbSet<BoxGroup> BoxGroups { get; set; }
        public virtual DbSet<BoxDetail> BoxDetails { get; set; }
        public virtual DbSet<BoxModel> BoxModels { get; set; }

        /// <summary>
        /// Item master from part material
        /// </summary>
        public virtual DbSet<ItemMaster> ItemMaster { get; set; }
        public virtual DbSet<StitchingThread> StitchingThread { get; set; }


        public virtual DbSet<LabelPort> LabelPort { get; set; }

        /// <summary>
        ///  QA
        /// </summary>
        public virtual DbSet<QualityAssurance> QualityAssurance { get; set; }
        public virtual DbSet<QualityStatus> QualityStatus { get; set; }
        public virtual DbSet<QualityAudit> QualityAudit { get; set; }

        /// <summary>
        /// Finish Goods DE
        /// </summary>
        public virtual DbSet<PartMaster> PartMaster { get; set; }

        /// <summary>
        /// Request Issued Fabric
        /// </summary>
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<FabricRequest> FabricRequest { get; set; }
        public virtual DbSet<FabricRequestLog> FabricRequestLog { get; set; }
        public virtual DbSet<GroupMail> GroupMail { get; set; }
        public virtual DbSet<FabricRequestDetail> FabricRequestDetail { get; set; }
        public virtual DbSet<FabricRequestDetailLog> FabricRequestDetailLog { get; set; }

        /// <summary>
        /// Barcode for DE - ShuLoangingPlan
        /// </summary>
        public virtual DbSet<LoadingPlan> LoadingPlan { get; set; }

        /// Scan for DE
        public virtual DbSet<BoxInfo> BoxInfo { get; set; }
        public virtual DbSet<ScanResult> ScanResult { get; set; }
        public virtual DbSet<ScanResultDetail> ScanResultDetail { get; set; }
        public virtual DbSet<ScanResultAudit> ScanResultAudit { get; set; }
        public virtual DbSet<Shu> Shus { get; set; }

        /// <summary>
        /// Finish Good Inventory
        /// </summary>
        public virtual DbSet<InventoryFG> InventoryFG { get; set; }
        public virtual DbSet<InventoryDetailFG> InventoryDetailFG { get; set; }
        public virtual DbSet<InventoryPeriod> InventoryPeriod { get; set; }
        public virtual DbSet<InventoryPeriodEntry> InventoryPeriodEntry { get; set; }
        public virtual DbSet<FinishGoodTransaction> FinishGoodTransaction { get; set; }

        /// Export Packing List GA 
        public virtual DbSet<PackingSheetName> PackingSheetName { get; set; }

        /// Invoice GA
        public virtual DbSet<SeparationPackingList> SeparationPackingList { get; set; }
        public virtual DbSet<SeparationPackingLine> SeparationPackingLine { get; set; }

        /// <summary>
        /// Production order for MTS
        /// </summary>
        public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }
        public virtual DbSet<ProductionOrderLine> ProductionOrderLines { get; set; }
        /// <summary>
        /// Delivery 
        /// </summary>
        public virtual DbSet<DeliveryNote> DeliveryNote { get; set; }
        public virtual DbSet<DeliveryNoteDetail> DeliveryNoteDetail { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new BoxDimensionConfiguration());
            builder.ApplyConfiguration(new BrandConfiguration());
            builder.ApplyConfiguration(new CompanyConfiguration());
            builder.ApplyConfiguration(new CurrencyConfiguration());
            builder.ApplyConfiguration(new CurrencyExchangeConfiguration());
            builder.ApplyConfiguration(new CurrencyExchangeTypeConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new DivisionConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new ItemMetaConfiguration());
            builder.ApplyConfiguration(new LogConfiguration());
            builder.ApplyConfiguration(new PartConfiguration());
            builder.ApplyConfiguration(new PartRevisionConfiguration());
            builder.ApplyConfiguration(new PartMaterialConfiguration());
            builder.ApplyConfiguration(new PartMetaConfiguration());
            builder.ApplyConfiguration(new PriceTermConfiguration());
            builder.ApplyConfiguration(new PaymentTermConfiguration());
            builder.ApplyConfiguration(new TaxConfiguration());
            builder.ApplyConfiguration(new UnitConfiguration());
            builder.ApplyConfiguration(new VendorConfiguration());
            builder.ApplyConfiguration(new ProductionBOMConfiguration());

            builder.ApplyConfiguration(new SalesOrderConfiguration());
            builder.ApplyConfiguration(new SalesOrderStatusConfiguration());
            builder.ApplyConfiguration(new SalesOrderTypeConfiguration());
            builder.ApplyConfiguration(new SalesContractConfiguration());
            builder.ApplyConfiguration(new SalesContractDetailConfiguration());
            builder.ApplyConfiguration(new ShippingMarkConfiguration());

            builder.ApplyConfiguration(new ItemStyleConfiguration());
            builder.ApplyConfiguration(new ItemStyleBarCodeConfiguration());
            builder.ApplyConfiguration(new ItemStyleStatusConfiguration());
            builder.ApplyConfiguration(new OrderDetailConfiguration());

            builder.ApplyConfiguration(new ShippingMethodConfiguration());
            builder.ApplyConfiguration(new ShippingTermConfiguration());
            builder.ApplyConfiguration(new SupplierCNUFConfiguration());

            builder.ApplyConfiguration(new ReservationEntryConfiguration());
            builder.ApplyConfiguration(new JobHeadConfiguration());

            builder.ApplyConfiguration(new PurchaseOrderConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderGroupLineConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderLineConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderStatusConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderTypeConfiguration());
            builder.ApplyConfiguration(new IncoTermConfiguration());

            builder.ApplyConfiguration(new PartMaterialConfiguration());
            builder.ApplyConfiguration(new PartMaterialStatusConfiguration());
            builder.ApplyConfiguration(new ItemPriceConfiguration());
            builder.ApplyConfiguration(new MaterialTypeConfiguration());

            builder.ApplyConfiguration(new SizeConfiguration());
            builder.ApplyConfiguration(new ItemModelConfiguration());

            builder.ApplyConfiguration(new PlantConfiguration());

            builder.ApplyConfiguration(new StorageConfiguration());
            builder.ApplyConfiguration(new StorageDetailConfiguration());
            builder.ApplyConfiguration(new ReceiptConfiguration());
            builder.ApplyConfiguration(new ReceiptGroupLineConfiguration());
            builder.ApplyConfiguration(new ReceiptLineConfiguration());

            builder.ApplyConfiguration(new MaterialTransactionConfiguration());
            builder.ApplyConfiguration(new IssuedConfiguration());
            builder.ApplyConfiguration(new IssuedGroupLineConfiguration());
            builder.ApplyConfiguration(new IssuedLineConfiguration());

            builder.ApplyConfiguration(new PartRevisionLogDetailConfiguration());
            builder.ApplyConfiguration(new PartRevisionLogConfiguration());

            builder.ApplyConfiguration(new EntitySequenceNumberConfiguration());

            builder.ApplyConfiguration(new MessageTemplateConfiguration());

            builder.ApplyConfiguration(new ForecastOverallConfiguration());
            builder.ApplyConfiguration(new ForecastGroupConfiguration());
            builder.ApplyConfiguration(new ForecastDetailConfiguration());
            builder.ApplyConfiguration(new ForecastEntryConfiguration());

            builder.ApplyConfiguration(new PurchaseRequestConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestStatusConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestLogConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestGroupLineConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestLineConfiguration());
            builder.ApplyConfiguration(new FabricPurchaseOrderConfiguration());

            builder.ApplyConfiguration(new WeekConfiguration());
            builder.ApplyConfiguration(new StyleNetWeightConfiguration());

            builder.ApplyConfiguration(new SalesQuoteConfiguration());
            builder.ApplyConfiguration(new SalesQuoteStatusConfiguration());
            builder.ApplyConfiguration(new SalesQuoteLogConfiguration());
            builder.ApplyConfiguration(new SalesQuoteDetailConfiguration());
            builder.ApplyConfiguration(new GenderConfiguration());

            builder.ApplyConfiguration(new PackingListConfiguration());
            builder.ApplyConfiguration(new PackingLineConfiguration());
            builder.ApplyConfiguration(new PackingOverQuantityConfiguration());
            builder.ApplyConfiguration(new PackingListImageThumbnailConfiguration());
            builder.ApplyConfiguration(new PadConfiguration());
            builder.ApplyConfiguration(new PullBomTypeConfiguration());

            /// Đồng bộ nguyên phụ liệu
            builder.ApplyConfiguration(new ItemStyleSyncMasterConfiguration());
            builder.ApplyConfiguration(new ItemStyleSyncActionConfiguration());
            builder.ApplyConfiguration(new MaterialSyncConfiguration());
            builder.ApplyConfiguration(new MaterialSyncDetailConfiguration());
            builder.ApplyConfiguration(new OrderDetailSyncConfiguration());

            /// Sản lượng sản xuất
            builder.ApplyConfiguration(new WorkCenterConfiguration());
            builder.ApplyConfiguration(new JobOperationConfiguration());
            builder.ApplyConfiguration(new WorkCenterTypeConfiguration());
            builder.ApplyConfiguration(new OperationConfiguration());
            builder.ApplyConfiguration(new OperationDetailConfiguration());
            builder.ApplyConfiguration(new JobOutPutConfiguration());
            builder.ApplyConfiguration(new WorkingTimeConfiguration());
            builder.ApplyConfiguration(new DailyTargetConfiguration());
            builder.ApplyConfiguration(new DailyTargetDetailConfiguration());
            builder.ApplyConfiguration(new ProblemConfiguration());
            builder.ApplyConfiguration(new AllocDailyOutputConfiguration());
            builder.ApplyConfiguration(new AllocTransactionConfiguration());
            builder.ApplyConfiguration(new AllocPriorityConfiguration());
            builder.ApplyConfiguration(new FabricContrastConfiguration());
            builder.ApplyConfiguration(new CuttingOutputConfiguration());
            builder.ApplyConfiguration(new CuttingLotConfiguration());
            builder.ApplyConfiguration(new CuttingCardConfiguration());
            builder.ApplyConfiguration(new PartPriceConfiguration());
            builder.ApplyConfiguration(new JobPriceConfiguration());

            /// Import dữ liệu tồn kho
            builder.ApplyConfiguration(new StorageImportConfiguration());
            builder.ApplyConfiguration(new StorageImportDetailConfiguration());
            builder.ApplyConfiguration(new StorageBinEntryConfiguration());
            builder.ApplyConfiguration(new IssuedTypeConfiguration());
            builder.ApplyConfiguration(new ReceiptTypeConfiguration());
            builder.ApplyConfiguration(new StorageStatusConfiguration());
            builder.ApplyConfiguration(new ShipmentConfiguration());
            builder.ApplyConfiguration(new ShipmentDetailConfiguration());

            /// Shipping
            builder.ApplyConfiguration(new InvoiceTypeConfiguration());
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new BankAccountConfiguration());
            builder.ApplyConfiguration(new ConsigneeConfiguration());
            builder.ApplyConfiguration(new InvoiceConfiguration());
            builder.ApplyConfiguration(new InvoiceDetailConfiguration());
            builder.ApplyConfiguration(new PortConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentTypeConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentConfiguration());
            builder.ApplyConfiguration(new ShippingPlanConfiguration());
            builder.ApplyConfiguration(new ShippingPlanDetailConfiguration());
            builder.ApplyConfiguration(new PackingOutputConfiguration());
            builder.ApplyConfiguration(new ProductionDeptConfiguration());

            ///Đắp đơn
            builder.ApplyConfiguration(new SalesOrderOffsetConfiguration());
            builder.ApplyConfiguration(new MaterialRequestConfiguration());
            builder.ApplyConfiguration(new MaterialRequestDetailConfiguration());
            builder.ApplyConfiguration(new MaterialRequestOrderDetailConfiguration());

            /// Scan barcode GA
            builder.ApplyConfiguration(new BoxGroupConfiguration());
            builder.ApplyConfiguration(new BoxDetailConfiguration());
            builder.ApplyConfiguration(new BoxModelConfiguration());

            /// Item master from part material
            builder.ApplyConfiguration(new ItemMasterConfiguration());
            builder.ApplyConfiguration(new StitchingThreadConfiguration());

            builder.ApplyConfiguration(new LabelPortConfiguration());

            ///QA
            builder.ApplyConfiguration(new QualityAssuranceConfiguration());
            builder.ApplyConfiguration(new QualityStatusConfiguration());
            builder.ApplyConfiguration(new QualityAuditConfiguration());

            /// Request Fabric
            builder.ApplyConfiguration(new StatusConfiguration());
            builder.ApplyConfiguration(new GroupMailConfiguration());
            builder.ApplyConfiguration(new FabricRequestConfiguration());
            builder.ApplyConfiguration(new FabricRequestLogConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailLogConfiguration());

            builder.ApplyConfiguration(new PartMasterConfiguration());

            /// Barcode for DE - ShuLoangingPlan
            builder.ApplyConfiguration(new LoadingPlanConfiguration());
            builder.ApplyConfiguration(new ShuConfiguration());

            /// Scan for DE
            builder.ApplyConfiguration(new BoxInfoConfiguration());
            builder.ApplyConfiguration(new ScanResultConfiguration());
            builder.ApplyConfiguration(new ScanResultDetailConfiguration());
            builder.ApplyConfiguration(new ScanResultAuditConfiguration());

            /// Finish Good Inventory
            builder.ApplyConfiguration(new InventoryFGConfiguration());
            builder.ApplyConfiguration(new InventoryDetailFGConfiguration());
            builder.ApplyConfiguration(new InventoryPeriodConfiguration());
            builder.ApplyConfiguration(new InventoryPeriodEntryConfiguration());
            builder.ApplyConfiguration(new FinishGoodTransactionConfiguration());

            /// Export Packing List GA 
            builder.ApplyConfiguration(new PackingSheetNameConfiguration());

            /// Invoice GA
            builder.ApplyConfiguration(new SeparationPackingListConfiguration());
            builder.ApplyConfiguration(new SeparationPackingLineConfiguration());

            /// Production order
            builder.ApplyConfiguration(new ProductionOrderLineConfiguration());
            builder.ApplyConfiguration(new ProductionOrderConfiguration());

            /// Hanger
            builder.ApplyConfiguration(new HangerConfiguration());

            /// Delivery
            builder.ApplyConfiguration(new DeliveryNoteConfiguration());
            builder.ApplyConfiguration(new DeliveryNoteDetailConfiguration());
        }
    }
}
