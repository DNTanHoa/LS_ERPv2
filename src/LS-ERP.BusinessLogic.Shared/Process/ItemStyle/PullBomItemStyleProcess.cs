using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class PullBomItemStyleProcess
    {
        /// <summary>
        /// pull bom default
        /// </summary>
        /// <param name="itemStyles"></param>
        /// <param name="partRevisions"></param>
        /// <param name="newProductionBOMs"></param>
        /// <param name="updateProductionBoms"></param>
        /// <param name="jobHeads"></param>
        /// <param name="reservationEntries"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PullDefault(List<ItemStyle> itemStyles, List<PartRevision> partRevisions, string username,
            List<WastageSetting> wastageSettings,
            out List<ProductionBOM> newProductionBOMs, out List<ProductionBOM> updateProductionBoms,
            out List<JobHead> jobHeads, out List<ReservationEntry> reservationEntries, out string errorMessage)
        {
            errorMessage = string.Empty;
            reservationEntries = new List<ReservationEntry>();
            jobHeads = new List<JobHead>();
            updateProductionBoms = new List<ProductionBOM>();
            newProductionBOMs = new List<ProductionBOM>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<PartMaterial, ProductionBOM>();
            });

            var mapper = config.CreateMapper();

            foreach (var itemStyle in itemStyles)
            {
                var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.CustomerStyle.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.EffectDate != null &&
                                                                     x.Season.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.Season.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.IsConfirmed == true);
                if (partRevision != null)
                {
                    ///Check order detail and create job with reservation entry
                    foreach (var orderDetail in itemStyle.OrderDetails)
                    {
                        if (orderDetail.Quantity > (orderDetail.ReservedQuantity ?? 0 + orderDetail.ConsumedQuantity ?? 0))
                        {
                            var jobHead = new JobHead()
                            {
                                Number = itemStyle.LSStyle + "-" + orderDetail.Size + Nanoid.Nanoid.Generate("ABCDEF123456789", size: 8),
                                LSStyle = itemStyle.LSStyle,
                                CustomerStyle = itemStyle.CustomerStyle,
                                GarmentSize = orderDetail.Size,
                                PartRevisionID = partRevision.ID,
                                DueDate = itemStyle.ShipDate,
                                RequestDueDate = itemStyle.DeliveryDate != null ? itemStyle.DeliveryDate.GetValueOrDefault().AddDays(-1) : null,
                                Confirmed = true,
                                ProductionQuantity = orderDetail.Quantity -
                                                     (orderDetail.ReservedQuantity ?? 0) -
                                                     (orderDetail.ConsumedQuantity ?? 0),
                            };

                            jobHead.SetCreateAudit(username);

                            jobHeads.Add(jobHead);

                            var reservation = new ReservationEntry()
                            {
                                ReservedQuantity = orderDetail.Quantity -
                                                   (orderDetail.ReservedQuantity ?? 0) -
                                                   (orderDetail.ConsumedQuantity ?? 0),
                                JobHeadNumber = jobHead.Number,
                                JobHead = jobHead,
                                OrderDetailID = orderDetail.ID
                            };

                            reservation.SetCreateAudit(username);

                            reservationEntries.Add(reservation);
                            orderDetail.ReservationEntries.Add(reservation);

                            orderDetail.CalculateQuantity();
                        }

                        foreach (var reservation in orderDetail.ReservationEntries)
                        {
                            if (reservation.JobHead != null)
                            {
                                ///Get partmaterial has status approved
                                var partMaterials = partRevision.PartMaterials.Where(x => x.PartMaterialStatusCode == "3");

                                var proBoms = PullBomForJob(reservation.JobHead, itemStyle, username, partMaterials);
                                var calculateResult = CalculateRequiredQuantity(proBoms, false, username, null, out errorMessage);

                                if (!calculateResult)
                                {
                                    return false;
                                }

                                newProductionBOMs.AddRange(proBoms.Where(x => x.ID == 0));
                                updateProductionBoms.AddRange(proBoms.Where(x => x.ID > 0));
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Could not find master bom for style " + itemStyle.CustomerStyle + " season " + itemStyle.Season;
                    return false;
                }
            }

            updateProductionBoms?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            newProductionBOMs?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            return true;
        }

        /// <summary>
        /// pull bom default
        /// </summary>
        /// <param name="itemStyles"></param>
        /// <param name="partRevisions"></param>
        /// <param name="newProductionBOMs"></param>
        /// <param name="updateProductionBoms"></param>
        /// <param name="jobHeads"></param>
        /// <param name="reservationEntries"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PullForCustomerIFG(List<ItemStyle> itemStyles, List<PartRevision> partRevisions, string username,
            List<WastageSetting> wastageSettings,
            out List<ProductionBOM> newProductionBOMs, out List<ProductionBOM> updateProductionBoms,
            out List<JobHead> jobHeads, out List<ReservationEntry> reservationEntries, out string errorMessage)
        {
            errorMessage = string.Empty;
            reservationEntries = new List<ReservationEntry>();
            jobHeads = new List<JobHead>();
            updateProductionBoms = new List<ProductionBOM>();
            newProductionBOMs = new List<ProductionBOM>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<PartMaterial, ProductionBOM>();
            });

            var mapper = config.CreateMapper();

            foreach (var itemStyle in itemStyles)
            {
                var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.ContractNo.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.EffectDate != null &&
                                                                     x.Season.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.Season.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.IsConfirmed == true);
                if (partRevision != null)
                {
                    ///Check order detail and create job with reservation entry
                    foreach (var orderDetail in itemStyle.OrderDetails)
                    {
                        if (orderDetail.Quantity > (orderDetail.ReservedQuantity ?? 0 + orderDetail.ConsumedQuantity ?? 0))
                        {
                            var jobHead = new JobHead()
                            {
                                Number = itemStyle.LSStyle + "-" + orderDetail.Size + Nanoid.Nanoid.Generate("ABCDEF123456789", size: 8),
                                LSStyle = itemStyle.LSStyle,
                                CustomerStyle = itemStyle.CustomerStyle,
                                GarmentSize = orderDetail.Size,
                                PartRevisionID = partRevision.ID,
                                DueDate = itemStyle.ShipDate,
                                RequestDueDate = itemStyle.DeliveryDate != null ? itemStyle.DeliveryDate.GetValueOrDefault().AddDays(-1) : null,
                                Confirmed = true,
                                ProductionQuantity = orderDetail.Quantity -
                                                     (orderDetail.ReservedQuantity ?? 0) -
                                                     (orderDetail.ConsumedQuantity ?? 0),
                            };

                            jobHead.SetCreateAudit(username);

                            jobHeads.Add(jobHead);

                            var reservation = new ReservationEntry()
                            {
                                ReservedQuantity = orderDetail.Quantity -
                                                   (orderDetail.ReservedQuantity ?? 0) -
                                                   (orderDetail.ConsumedQuantity ?? 0),
                                JobHeadNumber = jobHead.Number,
                                JobHead = jobHead,
                                OrderDetailID = orderDetail.ID
                            };

                            reservation.SetCreateAudit(username);

                            reservationEntries.Add(reservation);
                            orderDetail.ReservationEntries.Add(reservation);

                            orderDetail.CalculateQuantity();
                        }

                        foreach (var reservation in orderDetail.ReservationEntries)
                        {
                            if (reservation.JobHead != null)
                            {
                                ///Get partmaterial has status approved
                                var partMaterials = partRevision.PartMaterials.Where(x => x.ItemStyleNumber.Replace(" ", "").Trim().ToUpper()
                                                    == itemStyle.CustomerStyle.Replace(" ", "").Trim().ToUpper() && x.PartMaterialStatusCode == "3");

                                var proBoms = PullBomForJob(reservation.JobHead, itemStyle, username, partMaterials);
                                var calculateResult = CalculateRequiredQuantity(proBoms, false, username, null, out errorMessage);

                                if (!calculateResult)
                                {
                                    return false;
                                }

                                newProductionBOMs.AddRange(proBoms.Where(x => x.ID == 0));
                                updateProductionBoms.AddRange(proBoms.Where(x => x.ID > 0));
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Could not find master bom for style " + itemStyle.CustomerStyle + " season " + itemStyle.Season;
                    return false;
                }
            }

            updateProductionBoms?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            newProductionBOMs?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            return true;
        }

        /// <summary>
        /// Pull bom for customer DE
        /// </summary>
        /// <param name="itemStyles"></param>
        /// <param name="partRevisions"></param>
        /// <param name="newProductionBOMs"></param>
        /// <param name="updateProductionBoms"></param>
        /// <param name="jobHeads"></param>
        /// <param name="reservationEntries"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PullForCustomerDE(List<ItemStyle> itemStyles, List<PartRevision> partRevisions, string username,
            out List<ProductionBOM> newProductionBOMs, out List<ProductionBOM> updateProductionBoms,
            out List<JobHead> jobHeads, out List<ReservationEntry> reservationEntries, out string errorMessage)
        {
            errorMessage = string.Empty;
            reservationEntries = new List<ReservationEntry>();
            jobHeads = new List<JobHead>();
            updateProductionBoms = new List<ProductionBOM>();
            newProductionBOMs = new List<ProductionBOM>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<PartMaterial, ProductionBOM>();
            });

            var mapper = config.CreateMapper();

            foreach (var itemStyle in itemStyles)
            {
                var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.CustomerStyle.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.EffectDate != null &&
                                                                     x.Season.Replace(" ", "").Trim().ToUpper()
                                                                     == itemStyle.Season.Replace(" ", "").Trim().ToUpper());
                if (partRevision != null)
                {
                    ///Check order detail and create job with reservation entry
                    foreach (var orderDetail in itemStyle.OrderDetails)
                    {
                        if (Math.Round(orderDetail.Quantity ?? 0) > Math.Round(((orderDetail.ReservedQuantity ?? 0) + (orderDetail.ConsumedQuantity ?? 0))))
                        {
                            var jobHead = new JobHead()
                            {
                                Number = itemStyle.LSStyle + "-" + orderDetail.Size + Nanoid.Nanoid.Generate("ABCDEF123456789", size: 8),
                                LSStyle = itemStyle.LSStyle,
                                CustomerStyle = itemStyle.CustomerStyle,
                                GarmentSize = orderDetail.Size,
                                PartRevisionID = partRevision.ID,
                                DueDate = itemStyle.ShipDate,
                                RequestDueDate = itemStyle.DeliveryDate.GetValueOrDefault().AddDays(-1),
                                Confirmed = true,
                                ProductionQuantity = orderDetail.Quantity -
                                                     (orderDetail.ReservedQuantity ?? 0) -
                                                     (orderDetail.ConsumedQuantity ?? 0),
                            };

                            jobHeads.Add(jobHead);

                            var reservation = new ReservationEntry()
                            {
                                ReservedQuantity = orderDetail.Quantity -
                                                   (orderDetail.ReservedQuantity ?? 0) -
                                                   (orderDetail.ConsumedQuantity ?? 0),
                                JobHeadNumber = jobHead.Number,
                                JobHead = jobHead,
                                OrderDetailID = orderDetail.ID
                            };

                            reservationEntries.Add(reservation);
                            orderDetail.ReservationEntries.Add(reservation);

                            orderDetail.CalculateQuantity();
                        }

                        foreach (var reservation in orderDetail.ReservationEntries)
                        {
                            if (reservation.JobHead != null)
                            {
                                var proBoms = PullBomForJob(reservation.JobHead, itemStyle, username, partRevision.PartMaterials);
                                var calculateResult = CalculateRequiredQuantity(proBoms, false, username, null, out errorMessage);

                                if (!calculateResult)
                                {
                                    return false;
                                }

                                newProductionBOMs.AddRange(proBoms.Where(x => x.ID == 0));
                                updateProductionBoms.AddRange(proBoms.Where(x => x.ID > 0));
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Could not find master bom for style " + itemStyle.CustomerStyle + " season " + itemStyle.Season;
                    return false;
                }
            }

            updateProductionBoms?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            newProductionBOMs?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            return true;
        }

        /// <summary>
        /// Pull bom for customer HA
        /// </summary>
        /// <param name="itemStyles"></param>
        /// <param name="partRevisions"></param>
        /// <param name="newProductionBOMs"></param>
        /// <param name="updateProductionBoms"></param>
        /// <param name="jobHeads"></param>
        /// <param name="reservationEntries"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PullForCustomerHA(List<ItemStyle> itemStyles, List<PartRevision> partRevisions, string username,
            List<WastageSetting> wastageSettings,
            out List<ProductionBOM> newProductionBOMs, out List<ProductionBOM> updateProductionBoms,
            out List<JobHead> jobHeads, out List<ReservationEntry> reservationEntries, out string errorMessage)
        {
            errorMessage = string.Empty;
            reservationEntries = new List<ReservationEntry>();
            jobHeads = new List<JobHead>();
            updateProductionBoms = new List<ProductionBOM>();
            newProductionBOMs = new List<ProductionBOM>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<PartMaterial, ProductionBOM>();
            });

            var mapper = config.CreateMapper();

            foreach (var itemStyle in itemStyles)
            {
                var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber.Replace(" ", "").Trim().ToUpper()
                                                                        == itemStyle.ContractNo.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.EffectDate != null &&
                                                                     x.Season.Replace(" ", "").Trim().ToUpper()
                                                                        == itemStyle.Season.Replace(" ", "").Trim().ToUpper());
                if (partRevision != null)
                {
                    ///Check order detail and create job with reservation entry
                    foreach (var orderDetail in itemStyle.OrderDetails)
                    {
                        if (orderDetail.Quantity > (orderDetail.ReservedQuantity ?? 0 + orderDetail.ConsumedQuantity ?? 0))
                        {
                            var jobHead = new JobHead()
                            {
                                Number = itemStyle.LSStyle + "-" + orderDetail.Size + Nanoid.Nanoid.Generate("ABCDEF123456789", size: 8),
                                LSStyle = itemStyle.LSStyle,
                                CustomerStyle = itemStyle.CustomerStyle,
                                GarmentSize = orderDetail.Size,
                                PartRevisionID = partRevision.ID,
                                DueDate = itemStyle.ShipDate,
                                RequestDueDate = itemStyle.DeliveryDate.GetValueOrDefault().AddDays(-1),
                                Confirmed = true,
                                ProductionQuantity = orderDetail.Quantity -
                                                     (orderDetail.ReservedQuantity ?? 0) -
                                                     (orderDetail.ConsumedQuantity ?? 0),
                            };
                            jobHead.SetCreateAudit(username);
                            jobHeads.Add(jobHead);

                            var reservation = new ReservationEntry()
                            {
                                ReservedQuantity = orderDetail.Quantity -
                                                   (orderDetail.ReservedQuantity ?? 0) -
                                                   (orderDetail.ConsumedQuantity ?? 0),
                                JobHeadNumber = jobHead.Number,
                                JobHead = jobHead,
                                OrderDetailID = orderDetail.ID
                            };
                            reservation.SetCreateAudit(username);

                            reservationEntries.Add(reservation);
                            orderDetail.ReservationEntries.Add(reservation);

                            orderDetail.CalculateQuantity();
                        }

                        foreach (var reservation in orderDetail.ReservationEntries)
                        {
                            if (reservation.JobHead != null)
                            {
                                ///Get partmaterial has status approved and color group = customer style
                                var partMaterials = partRevision.PartMaterials
                                    .Where(x => x.ItemStyleNumber.Replace(" ", "").Trim().ToUpper()
                                                    == itemStyle.CustomerStyle.Replace(" ", "").Trim().ToUpper() &&
                                                x.PartMaterialStatusCode == "3");

                                var proBoms = PullBomForJob(reservation.JobHead, itemStyle,
                                    username, partMaterials);
                                var calculateResult = CalculateRequiredQuantity(proBoms, true,
                                    username, wastageSettings, out errorMessage);

                                if (!calculateResult)
                                {
                                    return false;
                                }

                                newProductionBOMs.AddRange(proBoms.Where(x => x.ID == 0));
                                updateProductionBoms.AddRange(proBoms.Where(x => x.ID > 0));
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Could not find master bom for style " + itemStyle.CustomerStyle +
                        " season " + itemStyle.Season;
                    return false;
                }
            }

            updateProductionBoms?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            newProductionBOMs?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            return true;
        }

        /// <summary>
        /// Pull bom for customer PU
        /// </summary>
        /// <param name="itemStyles"></param>
        /// <param name="partRevisions"></param>
        /// <param name="newProductionBOMs"></param>
        /// <param name="updateProductionBoms"></param>
        /// <param name="jobHeads"></param>
        /// <param name="reservationEntries"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PullForCustomerPU(List<ItemStyle> itemStyles, List<PartRevision> partRevisions, string username,
            List<WastageSetting> wastageSettings,
            List<PartMaterial> partMaterials,
            out List<ProductionBOM> newProductionBOMs, out List<ProductionBOM> updateProductionBoms,
            out List<JobHead> jobHeads, out List<ReservationEntry> reservationEntries, out string errorMessage)
        {
            errorMessage = string.Empty;
            reservationEntries = new List<ReservationEntry>();
            jobHeads = new List<JobHead>();
            updateProductionBoms = new List<ProductionBOM>();
            newProductionBOMs = new List<ProductionBOM>();

            var config = new MapperConfiguration(config =>
            {
                config.CreateMap<PartMaterial, ProductionBOM>();
            });

            var mapper = config.CreateMapper();

            Dictionary<long, List<PartMaterial>> dicPartMaterials = new Dictionary<long, List<PartMaterial>>();
            foreach (var itemPartMaterial in partMaterials)
            {
                List<PartMaterial> partMaterials1 = new List<PartMaterial>();

                if (dicPartMaterials.TryGetValue((long)itemPartMaterial.PartRevisionID, out List<PartMaterial> rsPartMaterials))
                {
                    partMaterials1 = rsPartMaterials;
                }

                partMaterials1.Add(itemPartMaterial);
                dicPartMaterials[(long)itemPartMaterial.PartRevisionID] = partMaterials1;
            }

            foreach (var itemStyle in itemStyles)
            {
                var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber.Replace(" ", "").Trim().ToUpper()
                                                                        == itemStyle.ContractNo.Replace(" ", "").Trim().ToUpper() &&
                                                                     x.EffectDate != null &&
                                                                     x.Season.Replace(" ", "").Trim().ToUpper()
                                                                        == itemStyle.Season.Replace(" ", "").Trim().ToUpper());
                if (partRevision != null)
                {
                    ///Check order detail and create job with reservation entry
                    foreach (var orderDetail in itemStyle.OrderDetails)
                    {
                        if (orderDetail.Quantity > (orderDetail.ReservedQuantity ?? 0 + orderDetail.ConsumedQuantity ?? 0))
                        {
                            var jobHead = new JobHead()
                            {
                                Number = itemStyle.LSStyle + "-" + orderDetail.Size + Nanoid.Nanoid.Generate("ABCDEF123456789", size: 8),
                                LSStyle = itemStyle.LSStyle,
                                CustomerStyle = itemStyle.CustomerStyle,
                                GarmentSize = orderDetail.Size,
                                PartRevisionID = partRevision.ID,
                                DueDate = itemStyle.ShipDate,
                                RequestDueDate = itemStyle.DeliveryDate.GetValueOrDefault().AddDays(-1),
                                Confirmed = true,
                                ProductionQuantity = orderDetail.Quantity -
                                                     (orderDetail.ReservedQuantity ?? 0) -
                                                     (orderDetail.ConsumedQuantity ?? 0),
                            };

                            jobHeads.Add(jobHead);

                            var reservation = new ReservationEntry()
                            {
                                ReservedQuantity = orderDetail.Quantity -
                                                   (orderDetail.ReservedQuantity ?? 0) -
                                                   (orderDetail.ConsumedQuantity ?? 0),
                                JobHeadNumber = jobHead.Number,
                                JobHead = jobHead,
                                OrderDetailID = orderDetail.ID
                            };

                            reservationEntries.Add(reservation);
                            orderDetail.ReservationEntries.Add(reservation);

                            orderDetail.CalculateQuantity();
                        }

                        foreach (var reservation in orderDetail.ReservationEntries)
                        {
                            if (reservation.JobHead != null)
                            {
                                if (dicPartMaterials.TryGetValue(partRevision.ID, out List<PartMaterial> rsPartMaterials))
                                {
                                    ///Get partmaterial has status approved and color group = customer style
                                    var partMaterialList = rsPartMaterials
                                        .Where(x => x.ItemStyleNumber.Replace(" ", "").Trim().ToUpper()
                                                        == itemStyle.CustomerStyle.Replace(" ", "").Trim().ToUpper() &&
                                                    x.PartMaterialStatusCode == "3");

                                    var proBoms = PullBomForJob(reservation.JobHead, itemStyle,
                                        username, partMaterialList);
                                    var calculateResult = CalculateRequiredQuantity(proBoms, true,
                                        username, wastageSettings, out errorMessage);



                                    if (!calculateResult)
                                    {
                                        return false;
                                    }

                                    newProductionBOMs.AddRange(proBoms.Where(x => x.ID == 0));
                                    updateProductionBoms.AddRange(proBoms.Where(x => x.ID > 0));
                                }
                            }
                        }
                    }
                }
                //else
                //{
                //    errorMessage = "Could not find master bom for contract no " + itemStyle.ContractNo +
                //        " season " + itemStyle.Season;
                //    return false;
                //}
            }

            updateProductionBoms?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            newProductionBOMs?.ForEach(x =>
            {
                x.PriceUnit = null;
                x.PerUnit = null;
            });

            return true;
        }
        #region support function

        /// <summary>
        /// Pull bom for job
        /// </summary>
        /// <param name="jobHead"></param>
        /// <param name="itemStyle"></param>
        /// <param name="partMaterials"></param>
        /// <returns></returns>
        public static List<ProductionBOM> PullBomForJob(JobHead jobHead, ItemStyle itemStyle,
            string username,
            IEnumerable<PartMaterial> partMaterials)
        {
            var result = new List<ProductionBOM>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PartMaterial, ProductionBOM>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ExternalCode, y => y.MapFrom(s => s.ExternalCode))
                .ForMember(x => x.PartMaterialID, y => y.MapFrom(s => s.ID))
                .ForMember(x => x.ItemStyleNumber, y => y.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore());
            });

            var mapper = config.CreateMapper();

            foreach (var partMaterial in partMaterials)
            {
                ProductionBOM productionBOM = null;

                if (!string.IsNullOrEmpty(partMaterial.GarmentSize) &&
                    partMaterial.GarmentSize.Replace(" ", "").Trim().ToUpper() != jobHead.GarmentSize.Replace(" ", "").Trim().ToUpper())
                {
                    continue;
                }

                if (partMaterial.PartRevision?.CustomerID != "PU")
                {
                    if (partMaterial.GarmentColorCode.Replace(" ", "").Trim().ToUpper() != itemStyle.ColorCode.Replace(" ", "").Trim().ToUpper())
                    {
                        continue;
                    }
                }


                if (!string.IsNullOrEmpty(partMaterial.GarmentSize))
                {
                    if (partMaterial.PartRevision?.CustomerID != "PU")
                    {
                        productionBOM = jobHead.ProductionBOMs?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            itemStyle.ColorCode.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentColorCode.Replace(" ", "").Trim().ToUpper() &&
                            jobHead.GarmentSize.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentSize.Replace(" ", "").Trim().ToUpper());
                    }
                    else
                    {
                        productionBOM = jobHead.ProductionBOMs?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            jobHead.GarmentSize.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentSize.Replace(" ", "").Trim().ToUpper());
                    }

                }
                else
                {
                    if (partMaterial.PartRevision?.CustomerID != "PU")
                    {
                        productionBOM = jobHead.ProductionBOMs?.FirstOrDefault(x =>
                           x.ExternalCode == partMaterial.ExternalCode &&
                           itemStyle.ColorCode.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentColorCode.Replace(" ", "").Trim().ToUpper());
                    }
                    else
                    {
                        productionBOM = jobHead.ProductionBOMs?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode);
                    }

                }

                if (productionBOM != null &&
                   //(productionBOM.ReservedQuantity > 0) &&
                  !(IsNeedToReCalculate(productionBOM)))
                {
                    continue;
                }

                if (productionBOM == null)
                {
                    productionBOM = new ProductionBOM();
                    productionBOM.SetCreateAudit(username);
                }
                else
                {
                    productionBOM.SetUpdateAudit(username);
                }

                mapper.Map(partMaterial, productionBOM);

                if (string.IsNullOrEmpty(productionBOM.ItemStyleNumber))
                {
                    productionBOM.ItemStyleNumber = itemStyle.Number;
                    productionBOM.ItemStyle = itemStyle;
                }

                if (string.IsNullOrEmpty(productionBOM.JobHeadNumber))
                {
                    productionBOM.JobHead = jobHead;
                    productionBOM.JobHeadNumber = jobHead.Number;
                }

                productionBOM.ProductionQuantity = jobHead.ProductionQuantity ?? 0;
                result.Add(productionBOM);

            }

            return result;
        }

        /// <summary>
        /// Calculate required quantity base on job quantity
        /// </summary>
        /// <param name="productionBOMs"></param>
        public static bool CalculateRequiredQuantity(List<ProductionBOM> productionBOMs, bool IsApplyWastageSetting, string username,
            List<WastageSetting> wastageSettings, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var productionBOM in productionBOMs)
            {
                var rounding = productionBOM.PriceUnit != null ? productionBOM.PriceUnit?.Rouding : 4;
                decimal factorUnit = 1;

                if (productionBOM.PerUnitID != productionBOM.PriceUnitID)
                {
                    factorUnit = (decimal)productionBOM.PriceUnit.Factor > 0 ? (decimal)productionBOM.PriceUnit.Factor : 1;
                }

                if (IsApplyWastageSetting)
                {
                    var wastageSetting = wastageSettings?.FirstOrDefault(x => (x.MaterialTypeClass == productionBOM.MaterialTypeClass || string.IsNullOrEmpty(productionBOM.MaterialTypeClass)) &&
                                                                              x.MaterialTypeCode == productionBOM.MaterialTypeCode &&
                                                                              (x.MaxQuantity >= productionBOM.ItemStyle?.TotalQuantity || x.MaxQuantity == null) &&
                                                                              x.MinQuantity <= productionBOM.ItemStyle?.TotalQuantity);

                    if (wastageSetting != null)
                    {
                        if (productionBOM.WastagePercent > wastageSetting.WastageStandard)
                        {
                            errorMessage = "Invalid wastage for item " + productionBOM.ItemID + "-"
                                + productionBOM.ItemName;
                            return false;
                        }

                        rounding = wastageSetting.Rounding;
                    }
                }

                if (productionBOM.FabricWeight > 0 &&
                   productionBOM.FabricWidth > 0 &&
                   productionBOM.MaterialTypeCode == "FB" &&
                   productionBOM.PerUnitID != productionBOM.PriceUnitID)
                {
                    productionBOM.ConsumptionQuantity = ((36 * productionBOM.FabricWidth * productionBOM.FabricWeight) / 1550000)
                        * productionBOM.QuantityPerUnit * productionBOM.JobHead?.ProductionQuantity;
                    productionBOM.LessQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.LessPercent / 100;
                    productionBOM.FreeQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.FreePercent / 100;
                    productionBOM.RequiredQuantity = ((36 * productionBOM.FabricWidth * productionBOM.FabricWeight) / 1550000)
                        * productionBOM.QuantityPerUnit * productionBOM.JobHead?.ProductionQuantity
                        * (1 + productionBOM.WastagePercent / 100);

                    productionBOM.WareHouseQuantity = productionBOM.RequiredQuantity;
                }
                else if (productionBOM.FabricWeight > 0 &&
                         productionBOM.FabricWidth > 0)
                {
                    productionBOM.ConsumptionQuantity = productionBOM.QuantityPerUnit * productionBOM.JobHead?.ProductionQuantity;
                    productionBOM.LessQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.LessPercent / 100;
                    productionBOM.FreeQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.FreePercent / 100;
                    productionBOM.RequiredQuantity =
                        productionBOM.QuantityPerUnit * productionBOM.JobHead?.ProductionQuantity
                        * (1 + productionBOM.WastagePercent / 100);

                    productionBOM.WareHouseQuantity = productionBOM.RequiredQuantity;
                }
                else
                {
                    productionBOM.ConsumptionQuantity = productionBOM.QuantityPerUnit
                        * productionBOM.JobHead?.ProductionQuantity;
                    productionBOM.LessQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.LessPercent / 100;
                    productionBOM.FreeQuantity = productionBOM.JobHead?.ProductionQuantity
                        * productionBOM.FreePercent / 100;
                    productionBOM.RequiredQuantity =
                        productionBOM.QuantityPerUnit * productionBOM.JobHead?.ProductionQuantity
                        * (1 + productionBOM.WastagePercent / 100);

                    productionBOM.WareHouseQuantity = productionBOM.RequiredQuantity /
                        factorUnit;
                }

                if (productionBOM.FabricWeight > 0 &&
                   productionBOM.FabricWidth > 0 &&
                    productionBOM.MaterialTypeCode == "FB")
                {
                    productionBOM.RequiredQuantity = Math.Round((decimal)productionBOM.RequiredQuantity, (int)rounding);
                    productionBOM.ConsumptionQuantity = Math.Round((decimal)productionBOM.ConsumptionQuantity, (int)rounding);

                    if (productionBOM.LessQuantity != null)
                        productionBOM.LessQuantity = Math.Round((decimal)productionBOM.LessQuantity, (int)rounding);

                    if (productionBOM.FreeQuantity != null)
                        productionBOM.FreeQuantity = Math.Round((decimal)productionBOM.FreeQuantity, (int)rounding);

                    productionBOM.WareHouseQuantity = Math.Round((decimal)productionBOM.WareHouseQuantity, (int)rounding);
                    productionBOM.WastageQuantity = productionBOM.RequiredQuantity - productionBOM.ConsumptionQuantity;
                    productionBOM.RemainQuantity = productionBOM.RequiredQuantity - (productionBOM.ReservedQuantity ?? 0);
                }
                else
                {
                    productionBOM.RequiredQuantity = Math.Round((decimal)productionBOM.RequiredQuantity /
                      factorUnit, (int)rounding);
                    productionBOM.ConsumptionQuantity = Math.Round((decimal)productionBOM.ConsumptionQuantity /
                     factorUnit, (int)rounding);

                    if (productionBOM.LessQuantity != null)
                        productionBOM.LessQuantity = Math.Round((decimal)productionBOM.LessQuantity, (int)rounding);

                    if (productionBOM.FreeQuantity != null)
                        productionBOM.FreeQuantity = Math.Round((decimal)productionBOM.FreeQuantity, (int)rounding);

                    productionBOM.WareHouseQuantity = Math.Round((decimal)productionBOM.WareHouseQuantity, (int)rounding);
                    productionBOM.WastageQuantity = productionBOM.RequiredQuantity - productionBOM.ConsumptionQuantity;
                    productionBOM.RemainQuantity = productionBOM.RequiredQuantity - (productionBOM.ReservedQuantity ?? 0);
                }

                productionBOM.SetUpdateAudit(username);
            }

            return true;
        }

        public static bool IsNeedToReCalculate(ProductionBOM productionBOM)
        {
            //if (productionBOM.ProductionQuantity != productionBOM.JobHead?.ProductionQuantity)
            //{
            //    return true;
            //}
            return true;
        }
        #endregion
    }
}
