using LS_ERP.EntityFrameworkCore.Configuration;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using LS_ERP.EntityFrameworkCore.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Context
{
    public class SqlServerAppDbContext : AppDbContext
    {
        public SqlServerAppDbContext()
        {

        }

        public SqlServerAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new BrandSqlServerConfiguration());
            builder.ApplyConfiguration(new CurrencyExchangeSqlSeverConfiguration());
            builder.ApplyConfiguration(new CurrencySqlServerConfiguration());
            builder.ApplyConfiguration(new CustomerSqlServerConfiguration());
            builder.ApplyConfiguration(new DivisionSqlServerConfiguration());
            builder.ApplyConfiguration(new TaxSqlServerConfiguration());
            builder.ApplyConfiguration(new UnitSqlServerConfiguration());
            builder.ApplyConfiguration(new VendorSqlServerConfiguration());

            builder.ApplyConfiguration(new SalesOrderSqlSeverConfiguration());
            builder.ApplyConfiguration(new ItemStyleSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemStyleBarCodeSqlServerConfiguration());
            builder.ApplyConfiguration(new OrderDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new ReservationEntrySqlServerConfiguration());

            builder.ApplyConfiguration(new PurchaseOrderLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderGroupLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderStatusSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderTypeSqlServerConfiguration());

            builder.ApplyConfiguration(new PartMaterialSqlServerConfiguration());
            builder.ApplyConfiguration(new PartRevisionSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemPriceSqlServerConfiguration());
            builder.ApplyConfiguration(new WastageSettingSqlServerConfiguration());

            builder.ApplyConfiguration(new SizeSqlServerConfiguration());
            builder.ApplyConfiguration(new ItemModelSqlServerConfiguration());

            builder.ApplyConfiguration(new StorageDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new ReceiptGroupLineSqlServerConfiguration());
            builder.ApplyConfiguration(new ReceiptLineSqlServerConfiguration());

            builder.ApplyConfiguration(new IssuedGroupLineSqlServerConfiguration());
            builder.ApplyConfiguration(new IssuedLineSqlServerConfiguration());

            builder.ApplyConfiguration(new PartRevisionLogDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new ForecastDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new ForecastMaterialSqlServerConfiguration());

            builder.ApplyConfiguration(new PurchaseRequestGroupLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseRequestLineSqlServerConfiguration());
            builder.ApplyConfiguration(new SalesContractDetailSqlServerConfiguration());

            builder.ApplyConfiguration(new ForecastEntrySqlServerConfiguration());

            builder.ApplyConfiguration(new MaterialTransactionSqlServerConfiguration());
            builder.ApplyConfiguration(new JobHeadSqlServerConfiguration());
            builder.ApplyConfiguration(new ForecastOverallSqlServerConfiguration());
            builder.ApplyConfiguration(new PurchaseOrderSqlServerConfiguration());
            builder.ApplyConfiguration(new ProductionBOMSqlServerConfiguration());
            builder.ApplyConfiguration(new BoxDimensionSqlServerConfiguration());

            builder.ApplyConfiguration(new StyleNetWeightSqlServerConfiguration());

            builder.ApplyConfiguration(new SalesQuoteSqlServerConfiguration());
            builder.ApplyConfiguration(new SalesQuoteDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new PackingListSqlServerConfiguration());
            builder.ApplyConfiguration(new PackingLineSqlServerConfiguration());
            builder.ApplyConfiguration(new PackingOverQuantitySqlServerConfiguration());


            builder.ApplyConfiguration(new PackingListImageThumbnailSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricPurchaseOrderSqlServerConfiguration());
            builder.ApplyConfiguration(new ShipmentSqlServerConfiguration());
            builder.ApplyConfiguration(new ShipmentDetailSqlServerConfiguration());

            /// Shipping
            builder.ApplyConfiguration(new InvoiceTypeSqlServerConfiguration());
            builder.ApplyConfiguration(new CountrySqlServerConfiguration());
            builder.ApplyConfiguration(new BankAccountSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new ConsigneeSqlServerConfiguration());
            builder.ApplyConfiguration(new PortSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceDocumentTypeSqlServerConfiguration());
            builder.ApplyConfiguration(new InvoiceSqlServerConfiguration());

            /// Sản xuất
            builder.ApplyConfiguration(new WorkingTimeSqlServerConfiguration());
            builder.ApplyConfiguration(new DailyTargetSqlServerConfiguration());
            builder.ApplyConfiguration(new DailyTargetDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new ProblemSqlServerConfiguration());
            builder.ApplyConfiguration(new CuttingLotSqlServerConfiguration());
            builder.ApplyConfiguration(new CuttingCardSqlServerConfiguration());
            builder.ApplyConfiguration(new OperationDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new PartPriceSqlServerConfiguration());
            builder.ApplyConfiguration(new JobPriceSqlServerConfiguration());
            builder.ApplyConfiguration(new AllocPrioritySqlServerConfiguration());

            builder.ApplyConfiguration(new LabelPortSqlServerConfiguration());

            builder.ApplyConfiguration(new StitchingThreadSqlServerConfiguration());

            /// QA
            builder.ApplyConfiguration(new QualityAssuranceSqlServerConfiguration());
            builder.ApplyConfiguration(new QualityAuditSqlServerConfiguration());

            /// Fabric Request
            builder.ApplyConfiguration(new FabricRequestSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestLogSqlServerConfiguration());
            builder.ApplyConfiguration(new GroupMailSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new FabricRequestDetailLogSqlServerConfiguration());

            /// Barcode for DE - ShuLoangingPlan
            builder.ApplyConfiguration(new LoadingPlanSqlServerConfiguration());

            /// Scan for DE
            builder.ApplyConfiguration(new BoxInfoSqlServerConfiguration());
            builder.ApplyConfiguration(new ScanResultSqlServerConfiguration());
            builder.ApplyConfiguration(new ScanResultDetailSqlServerConfiguration());
            builder.ApplyConfiguration(new ScanResultAuditSqlServerConfiguration());

            /// Finish Good Inventory
            builder.ApplyConfiguration(new InventoryFGSqlServerConfiguration());
            builder.ApplyConfiguration(new InventoryDetailFGSqlServerConfiguration());
            builder.ApplyConfiguration(new InventoryPeriodSqlServerConfiguration());
            builder.ApplyConfiguration(new InventoryPeriodEntrySqlServerConfiguration());
            builder.ApplyConfiguration(new FinishGoodTransactionSqlServerConfiguration());

            ///  Export Packing List GA 
            builder.ApplyConfiguration(new PackingSheetNameSqlServerConfiguration());

            /// Inovice GA  
            builder.ApplyConfiguration(new SeparationPackingListSqlServerConfiguration());
            builder.ApplyConfiguration(new SeparationPackingLineSqlServerConfiguration());

            builder.ApplyConfiguration(new HangerSqlServerConfiguration());

            /// Delivery
            builder.ApplyConfiguration(new DeliveryNoteSqlServerConfiguration());
            builder.ApplyConfiguration(new DeliveryNoteDetailSqlServerConfiguration());
        }
    }

}