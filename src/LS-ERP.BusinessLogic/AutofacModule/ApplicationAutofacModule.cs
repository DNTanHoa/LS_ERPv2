using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Mapping;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.BusinessLogic.Queries.Customer;
using LS_ERP.BusinessLogic.Queries.SalesOrder;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Shared.Repositories;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using MediatR.Extensions.Autofac.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.AutofacModule
{
    public class ApplicationAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterMediatR(typeof(CreateCustomerCommand).GetTypeInfo().Assembly);
            builder.RegisterAutoMapper(typeof(CustomerMappingProfile).Assembly);

            ///Repos and queries
            builder.RegisterType<BrandQueries>()
               .As<IBrandQueries>()
               .InstancePerLifetimeScope();
            builder.RegisterType<BrandRepository>()
                .As<IBrandRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomerQueries>()
                .As<ICustomerQueries>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CustomerRepository>()
                .As<ICustomerRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SalesOrderQueries>()
                .As<ISalesOrderQueries>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SalesOrderRepository>()
                .As<ISalesOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DivisionQueries>()
                .As<IDivisionQueries>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DivisionRepository>()
                .As<IDivisionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CurrencyQueries>()
                .As<ICurrencyQueries>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CurrencyRepository>()
                .As<ICurrencyRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PaymentTermQueries>()
               .As<IPaymentTermQueries>()
               .InstancePerLifetimeScope();
            builder.RegisterType<PaymentTermRepository>()
                .As<IPaymentTermRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PriceTermQueries>()
               .As<IPriceTermQueries>()
               .InstancePerLifetimeScope();
            builder.RegisterType<PriceTermRepository>()
                .As<IPriceTermRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitQueries>()
               .As<IUnitQueries>()
               .InstancePerLifetimeScope();
            builder.RegisterType<UnitRepository>()
                .As<IUnitRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<VendorQueries>()
              .As<IVendorQueries>()
              .InstancePerLifetimeScope();
            builder.RegisterType<VendorRepository>()
                .As<IVendorRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseOrderQueries>()
                .As<IPurchaseOrderQueries>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderRepository>()
                .As<IPurchaseOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FabricPurchaseOrderRepository>()
                .As<IFabricPurchaseOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JobHeadQueries>()
              .As<IJobHeadQueries>()
              .InstancePerLifetimeScope();
            builder.RegisterType<JobHeadRepository>()
                .As<IJobHeadRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PartRevisionQueries>()
              .As<IPartRevisionQueries>()
              .InstancePerLifetimeScope();
            builder.RegisterType<PartRevisionRepository>()
                .As<IPartRevisionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SalesContractRepository>()
                .As<ISalesContractRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SalesContractDetailRepository>()
                .As<ISalesContractDetailRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ReceiptRepository>()
                .As<IReceiptRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ItemStyleRepository>()
                .As<IItemStyleRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ItemPriceRepository>()
                .As<IItemPriceRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EntitySequenceNumberRepository>()
                .As<IEntitySequenceNumberRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductionBOMRepository>()
                .As<IProductionBOMRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WastageSettingRepository>()
                .As<IWastageSettingRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ItemPriceRepository>()
               .As<IItemPriceRepository>()
               .InstancePerLifetimeScope();

            builder.RegisterType<PartRepository>()
              .As<IPartRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<ItemModelRepository>()
              .As<IItemModelRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<StorageDetailRepository>()
              .As<IStorageDetailRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<ShipmentDetailRepository>()
              .As<IShipmentDetailRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<StorageDetailQueries>()
             .As<IStorageDetailQueries>()
             .InstancePerLifetimeScope();

            builder.RegisterType<StorageBinEntryQueries>()
            .As<IStorageBinEntryQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseOrderGroupLineRepository>()
              .As<IPurchaseOrderGroupLineRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<BoxDimensionRepository>()
              .As<IBoxDimensionRepository>()
              .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseRequestRepository>()
             .As<IPurchaseRequestRepository>()
             .InstancePerLifetimeScope();

            builder.RegisterType<SizeRepository>()
             .As<ISizeRepository>()
             .InstancePerLifetimeScope();

            builder.RegisterType<MaterialTypeRepository>()
             .As<IMaterialTypeRepository>()
             .InstancePerLifetimeScope();

            builder.RegisterType<MaterialTransactionQueries>()
            .As<IMaterialTransactionQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseOrderTypeRepository>()
            .As<IPurchaseOrderTypeRepository>()
            .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseOrderLineRepository>()
            .As<IPurchaseOrderLineRepository>()
            .InstancePerLifetimeScope();

            builder.RegisterType<ItemRepository>()
            .As<IItemRepository>()
            .InstancePerLifetimeScope();

            builder.RegisterType<OrderDetailRepository>()
            .As<IOrderDetailRepository>()
            .InstancePerLifetimeScope();

            builder.RegisterType<PadRepository>()
            .As<IPadRepository>()
            .InstancePerLifetimeScope();

            builder.RegisterType<ForecastOverallRepository>()
           .As<IForecastOverallRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<PartMaterialRepository>()
           .As<IPartMaterialRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<PackingListRepository>()
           .As<IPackingListRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<LabelPortRepository>()
           .As<ILabelPortRepository>()
           .InstancePerLifetimeScope();

            /// SHIPPING
            builder.RegisterType<InvoiceRepository>()
           .As<IInvoiceRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<InvoiceDetailRepository>()
           .As<IInvoiceDetailRepository>()
           .InstancePerLifetimeScope();

            /// Fabric request
            builder.RegisterType<FabricRequestRepository>()
           .As<IFabricRequestRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<FabricRequestLogRepository>()
           .As<IFabricRequestLogRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<FabricRequestDetailRepository>()
           .As<IFabricRequestDetailRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<FabricRequestDetailLogRepository>()
           .As<IFabricRequestDetailLogRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<StatusRepository>()
           .As<IStatusRepository>()
           .InstancePerLifetimeScope();

            builder.RegisterType<ShipmentRepository>()
          .As<IShipmentRepository>()
          .InstancePerLifetimeScope();

            ///Size
            builder.RegisterType<SizeQueries>()
             .As<ISizeQueries>()
             .InstancePerLifetimeScope();

            /// Job Queries
            builder.RegisterType<JobOutputQueries>()
             .As<IJobOutputQueries>()
             .InstancePerLifetimeScope();
            builder.RegisterType<JobOperationQueries>()
             .As<IJobOperationQueries>()
             .InstancePerLifetimeScope();

            builder.RegisterType<LoadingPlanQueries>()
             .As<ILoadingPlanQueries>()
             .InstancePerLifetimeScope();

            builder.RegisterType<ItemStyleQueries>()
             .As<IItemStyleQueries>()
             .InstancePerLifetimeScope();

            builder.RegisterType<WorkingTimeQueries>()
            .As<IWorkingTimeQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<OperationQueries>()
            .As<IOperationQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<OperationDetailQueries>()
            .As<IOperationDetailQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<StorageDetailQueries>()
            .As<IStorageDetailQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<WorkCenterQueries>()
            .As<IWorkCenterQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<DailyTargetQueries>()
            .As<IDailyTargetQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<DailyTargetDetailQueries>()
            .As<IDailyTargetDetailQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<ProblemQueries>()
            .As<IProblemQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<CuttingOutputQueries>()
            .As<ICuttingOutputQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<FabricContrastQueries>()
            .As<IFabricContrastQueries>()
            .InstancePerLifetimeScope();

            /// Cutting card
            builder.RegisterType<CuttingCardQueries>()
            .As<ICuttingCardQueries>()
            .InstancePerLifetimeScope();

            builder.RegisterType<AllocDailyOutputQueries>()
            .As<IAllocDailyOutputQueries>()
            .InstancePerLifetimeScope();

            /// Part Price
            builder.RegisterType<PartPriceQueries>()
            .As<IPartPriceQueries>()
            .InstancePerLifetimeScope();

            /// Job Price
            builder.RegisterType<JobPriceQueries>()
            .As<IJobPriceQueries>()
            .InstancePerLifetimeScope();

            /// Report queries
            builder.RegisterType<ReportQueries>()
              .As<IReportQueries>()
              .InstancePerLifetimeScope();
            builder.RegisterType<ReceiptQueries>()
              .As<IReceiptQueries>()
              .InstancePerLifetimeScope();

            // ProductonBOM queries
            builder.RegisterType<ProductionBOMQueries>()
              .As<IProductionBOMQueries>()
              .InstancePerLifetimeScope();

            /// Box info queries
            builder.RegisterType<BoxInfoQueries>()
              .As<IBoxInfoQueries>()
              .InstancePerLifetimeScope();

            /// Scan Result
            builder.RegisterType<ScanResultQueries>()
              .As<IScanResultQueries>()
              .InstancePerLifetimeScope();

            // delivery
            builder.RegisterType<DeliveryNoteQueries>()
                .As<IDeliveryNoteQueries>()
                .InstancePerLifetimeScope();

            /// Validators
            builder.RegisterType<CustomerValidator>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<SalesOrderValidator>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<BrandValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            builder.RegisterType<PartMaterialValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            builder.RegisterType<PartRevisionValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            builder.RegisterType<PurchaseOrderValidator>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<SalesContractValidator>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<ItemPriceValidator>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<PurchaseRequestValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            builder.RegisterType<ForecastEntryValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            builder.RegisterType<PackingListValidator>()
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();

            
        }
    }
}
