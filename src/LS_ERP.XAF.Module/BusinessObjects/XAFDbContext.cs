using System;
using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using LS_ERP.EntityFrameworkCore.SqlServer.Configuration;
using LS_ERP.EntityFrameworkCore;
using LS_ERP.EntityFrameworkCore.Shared;

namespace LS_ERP.XAF.Module.BusinessObjects
{
    // For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
    public class XAFContextInitializer : DbContextTypesInfoInitializerBase
    {
        protected override DbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<XAFEFCoreDbContext>()
                .UseSqlServer(@";");
            var context = new XAFEFCoreDbContext(optionsBuilder.Options);

            return context;
        }
    }
    //This factory creates DbContext for design-time services. For example, it is required for database migration.
    public class XAFDesignTimeDbContextFactory : IDesignTimeDbContextFactory<XAFEFCoreDbContext>
    {
        public XAFEFCoreDbContext CreateDbContext(string[] args)
        {
            throw new InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
            //var optionsBuilder = new DbContextOptionsBuilder<XAFEFCoreDbContext>();
            //optionsBuilder.UseSqlServer(@"Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=LS_ERP.XAF");
            //return new XAFEFCoreDbContext(optionsBuilder.Options);
        }
    }
    [TypesInfoInitializer(typeof(XAFContextInitializer))]
    public class XAFEFCoreDbContext : DbContext
    {
        public XAFEFCoreDbContext(DbContextOptions<XAFEFCoreDbContext> options) : base(options)
        {
        }
        public DbSet<ModuleInfo> ModulesInfo { get; set; }
        public DbSet<ModelDifference> ModelDifferences { get; set; }
        public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
        public DbSet<PermissionPolicyRole> Roles { get; set; }
        public DbSet<LS_ERP.XAF.Module.BusinessObjects.ApplicationUser> Users { get; set; }
        public DbSet<LS_ERP.XAF.Module.BusinessObjects.ApplicationUserLoginInfo> UserLoginInfos { get; set; }


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
        public virtual DbSet<PurchaseOrderLine> PurchaseOrderLine { get; set; }
        public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatus { get; set; }
        public virtual DbSet<PurchaseOrderType> PurchaseOrderType { get; set; }
        public virtual DbSet<FabricPurchaseOrder> FabricPurchaseOrder { get; set; }
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

        public virtual DbSet<ReceiptType> ReceiptType { get; set; }
        public virtual DbSet<IssuedType> IssuedType { get; set; }

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

        public virtual DbSet<Week> Week { get; set; }
        public virtual DbSet<SalesContract> SalesContract { get; set; }
        public virtual DbSet<SalesContractDetail> SalesContractDetail { get; set; }
        public virtual DbSet<ShippingMark> ShippingMark { get; set; }
        public virtual DbSet<BoxDimension> BoxDimension { get; set; }
        public virtual DbSet<StyleNetWeight> StyleNetWeight { get; set; }

        /// <summary>
        /// Packing output
        /// </summary>
        public virtual DbSet<PackingList> PackingList { get; set; }
        public virtual DbSet<PackingLine> PackingLine { get; set; }
        public virtual DbSet<PackingOverQuantity> PackingOverQuantity { get; set; }
        public virtual DbSet<PackingOutput> PackingOutputs { get; set; }


        public virtual DbSet<PurchaseRequest> PurchaseRequest { get; set; }
        public virtual DbSet<PurchaseRequestStatus> PurchaseRequestStatus { get; set; }
        public virtual DbSet<PurchaseRequestLog> PurchaseRequestLog { get; set; }
        public virtual DbSet<PurchaseRequestGroupLine> PurchaseRequestGroupLine { get; set; }
        public virtual DbSet<PurchaseRequestLine> PurchaseRequestLine { get; set; }

        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<SalesQuote> SalesQuote { get; set; }
        public virtual DbSet<SalesQuoteStatus> SalesQuoteStatus { get; set; }
        public virtual DbSet<SalesQuoteDetail> SalesQuoteDetail { get; set; }
        public virtual DbSet<SalesQuoteLog> SalesQuoteLog { get; set; }
        public virtual DbSet<Pad> Pad { get; set; }
        public virtual DbSet<PullBomType> PullBomType { get; set; }

        /// <summary>
        /// Đồng bộ nguyên phụ liệu
        /// </summary>
        public virtual DbSet<ItemStyleSyncMaster> ItemStyleSyncMasters { get; set; }
        public virtual DbSet<ItemStyleSyncAction> ItemStyleSyncActions { get; set; }
        public virtual DbSet<MaterialSync> MaterialSyncs { get; set; }
        public virtual DbSet<MaterialSyncDetail> MaterialSyncDetails { get; set; }
        public virtual DbSet<OrderDetailSync> OrderDetailSyncs { get; set; }

        /// <summary>
        /// Sản lượng sản xuất
        /// </summary>
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<WorkCenter> WorkCenter { get; set; }
        public virtual DbSet<WorkCenterType> WorkCenterType { get; set; }
        public virtual DbSet<JobOperation> JobOperation { get; set; }
        public virtual DbSet<JobOutput> JobOuput { get; set; }
        public virtual DbSet<DailyTarget> DailyTarget { get; set; }
        public virtual DbSet<DailyTargetDetail> DailyTargetDetail { get; set; }
        public virtual DbSet<Problem> Problem { get; set; }

        /// <summary>
        /// Scan result
        /// </summary>
        public virtual DbSet<ScanResult> ScanResult { get; set; }
        public virtual DbSet<ScanResultDetail> ScanResultDetail { get; set; }

        /// <summary>
        /// Shipping
        /// </summary>
        public virtual DbSet<InvoiceType> InvoiceType { get; set; }
        public virtual DbSet<EntityFrameworkCore.Entities.Country> Country { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<Consignee> Consignee { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetail { get; set; }
        public virtual DbSet<Port> Port { get; set; }
        public virtual DbSet<InvoiceDocument> InvoiceDocument { get; set; }
        public virtual DbSet<InvoiceDocumentType> InvoiceDocumentType { get; set; }
        public virtual DbSet<ShippingPlan> ShippingPlans { get; set; }
        public virtual DbSet<ShippingPlanDetail> ShippingPlanDetails { get; set; }
        public virtual DbSet<Shipment> Shipment { get; set; }
        public virtual DbSet<ShipmentDetail> ShipmentDetail { get; set; }
        public virtual DbSet<ProductionDept> ProductionDept { get; set; }

        /// <summary>
        /// Đắp đơn
        /// </summary>
        public virtual DbSet<SalesOrderOffset> SalesOrderOffset { get; set; }


        /// <summary>
        /// Yêu cầu xuất kho
        /// </summary>
        public virtual DbSet<StorageBinEntry> StorageBinEntry { get; set; }
        public virtual DbSet<StorageStatus> StorageStatus { get; set; }
        public virtual DbSet<MaterialRequest> MaterialRequests { get; set; }
        public virtual DbSet<MaterialRequestDetail> MaterialRequestDetails { get; set; }
        public virtual DbSet<MaterialRequestOrderDetail> MaterialRequestOrderDetails { get; set; }

        /// Scan barcode GA
        public virtual DbSet<BoxGroup> BoxGroups { get; set; }
        public virtual DbSet<BoxDetail> BoxDetails { get; set; }
        public virtual DbSet<BoxModel> BoxModels { get; set; }

        public virtual DbSet<LabelPort> LabelPort { get; set; }

        /// QA
        public virtual DbSet<QualityAssurance> QualityAssurance { get; set; }
        public virtual DbSet<QualityStatus> QualityStatus { get; set; }
        public virtual DbSet<QualityAudit> QualityAudit { get; set; }

        /// <summary>
        /// Request Issued Fabric
        /// </summary>
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<GroupMail> GroupMail { get; set; }
        public virtual DbSet<FabricRequest> FabricRequest { get; set; }
        public virtual DbSet<FabricRequestLog> FabricRequestLog { get; set; }
        public virtual DbSet<FabricRequestDetail> FabricRequestDetail { get; set; }
        public virtual DbSet<FabricRequestDetailLog> FabricRequestDetailLog { get; set; }

        /// Barcode for DE - ShuLoangingPlan
        public virtual DbSet<LoadingPlan> LoadingPlan { get; set; }
        public virtual DbSet<PartMaster> PartMaster { get; set; }

        /// Scan for DE
        public virtual DbSet<BoxInfo> BoxInfo { get; set; }

        /// Inventory FG for DE
        public virtual DbSet<InventoryFG> InventoryFG { get; set; }
        public virtual DbSet<InventoryDetailFG> InventoryDetailFG { get; set; }
        public virtual DbSet<FinishGoodTransaction> FinishGoodTransaction { get; set; }
        public virtual DbSet<InventoryPeriod> InventoryPeriod { get; set; }

        /// Invoice
        public virtual DbSet<PartPrice> PartPrice { get; set; }

        /// Invoice GA
        public virtual DbSet<SeparationPackingList> SeparationPackingList { get; set; }
        public virtual DbSet<SeparationPackingLine> SeparationPackingLine { get; set; }

        /// <summary>
        /// Production order for MTS
        /// </summary>
        public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }
        public virtual DbSet<ProductionOrderLine> ProductionOrderLines { get; set; }
        public virtual DbSet<Hanger> Hanger { get; set; }

        /// <summary>
        /// Cutting Lot
        /// </summary>
        public virtual DbSet<CuttingLot> CuttingLot { get; set; }
        public virtual DbSet<AllocDailyOutput> AllocDailyOutput { get; set; }
        public virtual DbSet<Shu> Shus { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new BrandSqlServerConfiguration());
            builder.ApplyConfiguration(new CompanyConfiguration());
            builder.ApplyConfiguration(new CurrencySqlServerConfiguration());
            builder.ApplyConfiguration(new CurrencyExchangeConfiguration());
            builder.ApplyConfiguration(new CurrencyExchangeTypeConfiguration());
            builder.ApplyConfiguration(new CustomerSqlServerConfiguration());
            builder.ApplyConfiguration(new DivisionSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new ItemMetaConfiguration());
            builder.ApplyConfiguration(new LogConfiguration());
            builder.ApplyConfiguration(new PartConfiguration());
            builder.ApplyConfiguration(new PartRevisionSqlServerConfiguration());
            builder.ApplyConfiguration(new PartMaterialSqlServerConfiguration());
            builder.ApplyConfiguration(new PartMetaConfiguration());
            builder.ApplyConfiguration(new PriceTermConfiguration());
            builder.ApplyConfiguration(new PaymentTermConfiguration());
            builder.ApplyConfiguration(new TaxSqlServerConfiguration());
            builder.ApplyConfiguration(new UnitSqlServerConfiguration());
            builder.ApplyConfiguration(new VendorSqlServerConfiguration());

            builder.ApplyConfiguration(new SalesOrderConfiguration());
            builder.ApplyConfiguration(new SalesOrderStatusConfiguration());
            builder.ApplyConfiguration(new SalesOrderTypeConfiguration());

            builder.ApplyConfiguration(new ItemStyleSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemStyleBarCodeSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemStyleStatusConfiguration());
            builder.ApplyConfiguration(new OrderDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new ShippingMethodConfiguration());
            builder.ApplyConfiguration(new ShippingTermConfiguration());
            builder.ApplyConfiguration(new SupplierCNUFConfiguration());

            builder.ApplyConfiguration(new ReservationEntrySqlServerConfiguration());
            builder.ApplyConfiguration(new JobHeadSqlServerConfiguration());

            builder.ApplyConfiguration(new PurchaseOrderSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderGroupLineConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderStatusSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderTypeSqlServerConfiguration());
            builder.ApplyConfiguration(new IncoTermConfiguration());

            builder.ApplyConfiguration(new PartMaterialSqlServerConfiguration());
            builder.ApplyConfiguration(new PartMaterialStatusConfiguration());
            builder.ApplyConfiguration(new ItemPriceSqlServerConfiguration());
            builder.ApplyConfiguration(new MaterialTypeSqlServerConfiguration());

            builder.ApplyConfiguration(new SizeSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemModelSqlServerConfiguration());

            builder.ApplyConfiguration(new PlantConfiguration());

            builder.ApplyConfiguration(new StorageConfiguration());
            builder.ApplyConfiguration(new StorageStatusConfiguration());
            builder.ApplyConfiguration(new StorageDetailConfiguration());
            builder.ApplyConfiguration(new ReceiptConfiguration());
            builder.ApplyConfiguration(new ReceiptGroupLineConfiguration());
            builder.ApplyConfiguration(new ReceiptLineConfiguration());


            builder.ApplyConfiguration(new MaterialTransactionConfiguration());
            builder.ApplyConfiguration(new IssuedConfiguration());
            builder.ApplyConfiguration(new IssuedGroupLineConfiguration());
            builder.ApplyConfiguration(new IssuedLineConfiguration());

            builder.ApplyConfiguration(new PartRevisionLogDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new PartRevisionLogConfiguration());

            builder.ApplyConfiguration(new EntitySequenceNumberConfiguration());

            builder.ApplyConfiguration(new MessageTemplateConfiguration());

            builder.ApplyConfiguration(new ForecastOverallSqlServerConfiguration());
            builder.ApplyConfiguration(new ForecastGroupConfiguration());
            builder.ApplyConfiguration(new ForecastDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new PurchaseRequestConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestStatusConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestLogConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestGroupLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestLineSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricPurchaseOrderSqlServerConfiguration());

            builder.ApplyConfiguration(new WeekConfiguration());
            builder.ApplyConfiguration(new SalesContractConfiguration());
            builder.ApplyConfiguration(new SalesContractDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new SalesQuoteSqlServerConfiguration());
            builder.ApplyConfiguration(new SalesQuoteStatusConfiguration());
            builder.ApplyConfiguration(new SalesQuoteLogConfiguration());
            builder.ApplyConfiguration(new SalesQuoteDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new GenderConfiguration());

            /// Packing output
            builder.ApplyConfiguration(new PackingListSqlServerConfiguration());
            builder.ApplyConfiguration(new PackingLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PackingOverQuantitySqlServerConfiguration());
            builder.ApplyConfiguration(new StyleNetWeightSqlServerConfiguration());
            builder.ApplyConfiguration(new BoxDimensionSqlServerConfiguration());
            builder.ApplyConfiguration(new ShippingMarkConfiguration());
            builder.ApplyConfiguration(new PackingOutputConfiguration());

            builder.ApplyConfiguration(new ProductionBOMConfiguration());
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
            builder.ApplyConfiguration(new JobOutPutConfiguration());
            builder.ApplyConfiguration(new DailyTargetConfiguration());
            builder.ApplyConfiguration(new DailyTargetDetailConfiguration());
            builder.ApplyConfiguration(new ProblemConfiguration());

            /// Import dữ liệu tồn kho
            builder.ApplyConfiguration(new StorageImportConfiguration());
            builder.ApplyConfiguration(new StorageImportDetailConfiguration());
            builder.ApplyConfiguration(new StorageBinEntryConfiguration());
            builder.ApplyConfiguration(new IssuedTypeConfiguration());
            builder.ApplyConfiguration(new ReceiptTypeConfiguration());

            /// Shipping
            builder.ApplyConfiguration(new InvoiceTypeConfiguration());
            builder.ApplyConfiguration(new InvoiceSqlServerConfiguration());
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new BankAccountConfiguration());
            builder.ApplyConfiguration(new InvoiceDetailConfiguration());
            builder.ApplyConfiguration(new InvoiceConfiguration());
            builder.ApplyConfiguration(new ConsigneeConfiguration());
            builder.ApplyConfiguration(new PortSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentTypeSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentSqlServerConfiguration());
            builder.ApplyConfiguration(new ShippingPlanConfiguration());
            builder.ApplyConfiguration(new ShippingPlanDetailConfiguration());

            ///Đắp đơn
            builder.ApplyConfiguration(new SalesOrderOffsetConfiguration());

            builder.ApplyConfiguration(new MaterialRequestConfiguration());
            builder.ApplyConfiguration(new MaterialRequestDetailConfiguration());
            builder.ApplyConfiguration(new MaterialRequestOrderDetailConfiguration());

            /// Scan barcode GA
            builder.ApplyConfiguration(new BoxGroupConfiguration());
            builder.ApplyConfiguration(new BoxDetailConfiguration());
            builder.ApplyConfiguration(new BoxModelConfiguration());

            builder.ApplyConfiguration(new LabelPortSqlServerConfiguration());

            /// QA
            builder.ApplyConfiguration(new QualityAssuranceSqlServerConfiguration());
            builder.ApplyConfiguration(new QualityStatusConfiguration());
            builder.ApplyConfiguration(new QualityAuditConfiguration());

            /// Request Fabric
            builder.ApplyConfiguration(new StatusConfiguration());
            builder.ApplyConfiguration(new GroupMailSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestLogSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailLogSqlServerConfiguration());

            /// Barcode for DE - ShuLoangingPlan
            builder.ApplyConfiguration(new LoadingPlanSqlServerConfiguration());

            builder.ApplyConfiguration(new PartMasterConfiguration());

            /// Scan for DE
            builder.ApplyConfiguration(new BoxInfoSqlServerConfiguration());
            builder.ApplyConfiguration(new ScanResultConfiguration());
            builder.ApplyConfiguration(new ScanResultDetailConfiguration());

            /// Inventory FG for DE
            builder.ApplyConfiguration(new InventoryDetailFGSqlServerConfiguration());
            builder.ApplyConfiguration(new InventoryFGSqlServerConfiguration());
            builder.ApplyConfiguration(new FinishGoodTransactionSqlServerConfiguration());

            /// Invoice
            builder.ApplyConfiguration(new PartPriceSqlServerConfiguration());

            /// Invoice GA
            builder.ApplyConfiguration(new SeparationPackingListSqlServerConfiguration());
            builder.ApplyConfiguration(new SeparationPackingLineSqlServerConfiguration());

            /// Production order
            builder.ApplyConfiguration(new ProductionOrderLineConfiguration());
            builder.ApplyConfiguration(new ProductionOrderConfiguration());

            builder.ApplyConfiguration(new HangerSqlServerConfiguration());

            /// Cutting Lot
            builder.ApplyConfiguration(new CuttingLotSqlServerConfiguration());
            builder.ApplyConfiguration(new AllocDailyOutputSqlServerConfiguration());

            builder.ApplyConfiguration(new ShuConfiguration());
        }
    }
}
