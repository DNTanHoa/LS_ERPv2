using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class PullBOMForecastOverallProcess
    {
        public static bool PullBOM(List<ForecastOverall> overalls,
            string userName,
            List<PartRevision> partRevisions,
            List<WastageSetting> wastageSettings,
            out List<ForecastMaterial> newForecastMaterials,
            out List<ForecastMaterial> updateForcastMaterial,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            updateForcastMaterial = new List<ForecastMaterial>();
            newForecastMaterials = new List<ForecastMaterial>();

            foreach (var overall in overalls)
            {
                if (overall.ForecastDetails.Any())
                {
                    var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber == overall.CustomerStyle &&
                                                                     x.EffectDate != null &&
                                                                     x.Season == overall.Season);


                    foreach (var forecastDetail in overall.ForecastDetails)
                    {
                        PullBOMForForecastDetail(forecastDetail,
                            overall, userName, partRevision.PartMaterials.ToList(),
                            wastageSettings,
                            ref newForecastMaterials,
                            ref updateForcastMaterial,
                            out errorMessage);
                    }
                }
            }

            return true;
        }


        public static bool PullBOMGaran(List<ForecastOverall> overalls,
            string userName,
            List<PartRevision> partRevisions,
            List<WastageSetting> wastageSettings,
            out List<ForecastMaterial> newForecastMaterials,
            out List<ForecastMaterial> updateForcastMaterial,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            updateForcastMaterial = new List<ForecastMaterial>();
            newForecastMaterials = new List<ForecastMaterial>();

            foreach (var overall in overalls)
            {
                if (overall.ForecastDetails.Any())
                {
                    var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber == overall.CustomerStyle &&
                                                                     x.EffectDate != null &&
                                                                     x.IsConfirmed == true);
                    if (partRevision == null)
                    {
                        errorMessage = "Re-check BOM " + overall.CustomerStyle + " because not confirm, please";
                    }

                    foreach (var forecastDetail in overall.ForecastDetails)
                    {
                        PullBOMForForecastDetailGaran(forecastDetail,
                            overall, userName, partRevision.PartMaterials.ToList(),
                            wastageSettings,
                            ref newForecastMaterials,
                            ref updateForcastMaterial,
                            out errorMessage);
                    }
                }
            }

            return true;
        }

        public static bool CalculateRequiredQuantity(List<ForecastOverall> overalls,
           string userName,
           List<PartRevision> partRevisions,
           List<WastageSetting> wastageSettings,
           out List<ForecastMaterial> updateForcastMaterial,
           out string errorMessage)
        {
            errorMessage = string.Empty;
            updateForcastMaterial = new List<ForecastMaterial>();

            foreach (var overall in overalls)
            {
                if (overall.ForecastDetails.Any())
                {
                    var partRevision = partRevisions.OrderByDescending(x => x.EffectDate)
                                                .FirstOrDefault(x => x.PartNumber == overall.CustomerStyle &&
                                                                     x.EffectDate != null);

                    foreach (var forecastDetail in overall.ForecastDetails)
                    {
                        foreach (var itemMtl in forecastDetail.ForecastMaterials)
                        {
                            updateForcastMaterial.Add(itemMtl);
                        }
                    }
                }
            }

            CalculateRequiredQuantity(updateForcastMaterial, true, userName, wastageSettings, out errorMessage);

            return true;
        }

        #region support function

        /// <summary>
        /// Pull bom for forecast overall for Garan
        /// </summary>
        /// <param name="forecastDetail"></param>
        /// <param name="forecastOverall"></param>
        /// <param name="username"></param>
        /// <param name="partMaterials"></param>
        /// <returns></returns>
        public static void PullBOMForForecastDetailGaran(ForecastDetail forecastDetail,
            ForecastOverall forecastOverall,
            string username,
            List<PartMaterial> partMaterials,
            List<WastageSetting> wastageSettings,
            ref List<ForecastMaterial> newForecastMaterials,
            ref List<ForecastMaterial> updateForecastMaterials,
            out string errorMessage)
        {
            if (newForecastMaterials == null)
                newForecastMaterials = new List<ForecastMaterial>();

            if (updateForecastMaterials == null)
                updateForecastMaterials = new List<ForecastMaterial>();
            errorMessage = string.Empty;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PartMaterial, ForecastMaterial>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ExternalCode, y => y.MapFrom(s => s.ExternalCode))
                .ForMember(x => x.PartMaterialID, y => y.MapFrom(s => s.ID))
                .ForMember(x => x.ForecastOverallID, y => y.Ignore())
                .ForMember(x => x.ForecastOverall, y => y.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore());
            });

            var mapper = config.CreateMapper();

            foreach (var partMaterial in partMaterials)
            {

                if (partMaterial.PartMaterialStatusCode != "3")
                {
                    continue;
                }

                ForecastMaterial forecastMaterial = null;

                if (!string.IsNullOrEmpty(partMaterial.GarmentSize) &&
                    partMaterial.GarmentSize.Replace(" ","").Trim().ToUpper() != forecastDetail.GarmentSize.Replace(" ", "").Trim().ToUpper())
                {
                    continue;
                }

                if (partMaterial.GarmentColorCode != forecastOverall.GarmentColorCode)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(partMaterial.GarmentSize))
                {
                    forecastMaterial = forecastDetail.ForecastMaterials?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            forecastOverall.GarmentColorCode == partMaterial.GarmentColorCode &&
                            forecastDetail.GarmentSize.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentSize.Replace(" ", "").Trim().ToUpper());
                }
                else
                {
                    forecastMaterial = forecastDetail.ForecastMaterials?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            forecastOverall.GarmentColorCode == partMaterial.GarmentColorCode);
                }

                if (forecastMaterial != null &&
                   (forecastMaterial.ReservedQuantity > 0 || forecastMaterial.ReservedQuantity != null))
                {
                    continue;
                }

                if (forecastMaterial == null)
                {
                    forecastMaterial = new ForecastMaterial();
                    forecastMaterial.SetCreateAudit(username);
                }
                else
                {
                    forecastMaterial.SetUpdateAudit(username);
                }

                mapper.Map(partMaterial, forecastMaterial);

                if (string.IsNullOrEmpty(forecastMaterial.ForecastOverallID))
                {
                    forecastMaterial.ForecastOverallID = forecastOverall.ID;
                    forecastMaterial.ForecastOverall = forecastOverall;
                }

                if (forecastMaterial.ForecastDetailID == null ||
                    forecastMaterial.ForecastDetailID == 0)
                {
                    forecastMaterial.ForecastDetailID = (long)forecastDetail.ID;
                    forecastMaterial.ForecastDetail = forecastDetail;
                }

                if (string.IsNullOrEmpty(forecastMaterial.ID.ToString()) || forecastMaterial.ID == 0)
                {
                    newForecastMaterials.Add(forecastMaterial);
                }
                else
                {
                    updateForecastMaterials.Add(forecastMaterial);
                }

                //result.Add(forecastMaterial);
                //reservationForecastEntries.Add(reservationForecastEntry);
            }

            CalculateRequiredQuantity(newForecastMaterials, true, username, wastageSettings, out errorMessage);
            CalculateRequiredQuantity(updateForecastMaterials, true, username, wastageSettings, out errorMessage);

        }

        /// <summary>
        /// Pull bom for forecast overall
        /// </summary>
        /// <param name="forecastDetail"></param>
        /// <param name="forecastOverall"></param>
        /// <param name="username"></param>
        /// <param name="partMaterials"></param>
        /// <returns></returns>
        public static void PullBOMForForecastDetail(ForecastDetail forecastDetail,
            ForecastOverall forecastOverall,
            string username,
            List<PartMaterial> partMaterials,
            List<WastageSetting> wastageSettings,
            ref List<ForecastMaterial> newForecastMaterials,
            ref List<ForecastMaterial> updateForecastMaterials,
            out string errorMessage)
        {
            if (newForecastMaterials == null)
                newForecastMaterials = new List<ForecastMaterial>();

            if (updateForecastMaterials == null)
                updateForecastMaterials = new List<ForecastMaterial>();

            errorMessage = string.Empty;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PartMaterial, ForecastMaterial>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ExternalCode, y => y.MapFrom(s => s.ExternalCode))
                .ForMember(x => x.PartMaterialID, y => y.MapFrom(s => s.ID))
                .ForMember(x => x.ForecastOverallID, y => y.Ignore())
                .ForMember(x => x.ForecastOverall, y => y.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore());
            });

            var mapper = config.CreateMapper();

            foreach (var partMaterial in partMaterials)
            {
                ForecastMaterial forecastMaterial = null;

                if (!string.IsNullOrEmpty(partMaterial.GarmentSize) &&
                    partMaterial.GarmentSize.Replace(" ","").Trim().ToUpper() != 
                        forecastDetail.GarmentSize.Replace(" ", "").Trim().ToUpper())
                {
                    continue;
                }

                if (partMaterial.GarmentColorCode != forecastOverall.GarmentColorCode)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(partMaterial.GarmentSize))
                {
                    forecastMaterial = forecastDetail.ForecastMaterials?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            forecastOverall.GarmentColorCode == partMaterial.GarmentColorCode &&
                            forecastDetail.GarmentSize.Replace(" ", "").Trim().ToUpper() == partMaterial.GarmentSize.Replace(" ", "").Trim().ToUpper());
                }
                else
                {
                    forecastMaterial = forecastDetail.ForecastMaterials?.FirstOrDefault(x =>
                            x.ExternalCode == partMaterial.ExternalCode &&
                            forecastOverall.GarmentColorCode == partMaterial.GarmentColorCode);
                }

                if (forecastMaterial != null &&
                   (forecastMaterial.ReservedQuantity > 0 || forecastMaterial.ReservedQuantity != null))
                {
                    continue;
                }

                if (forecastMaterial == null)
                {
                    forecastMaterial = new ForecastMaterial();
                    forecastMaterial.SetCreateAudit(username);
                }
                else
                {
                    forecastMaterial.SetUpdateAudit(username);
                }

                mapper.Map(partMaterial, forecastMaterial);

                if (string.IsNullOrEmpty(forecastMaterial.ForecastOverallID))
                {
                    forecastMaterial.ForecastOverallID = forecastOverall.ID;
                    forecastMaterial.ForecastOverall = forecastOverall;
                }

                if (forecastMaterial.ForecastDetailID == null ||
                    forecastMaterial.ForecastDetailID == 0)
                {
                    forecastMaterial.ForecastDetailID = (long)forecastDetail.ID;
                    forecastMaterial.ForecastDetail = forecastDetail;
                }

                forecastMaterial.RemainQuantity = forecastMaterial.ReservedQuantity;

                if (string.IsNullOrEmpty(forecastMaterial.ID.ToString()) || forecastMaterial.ID == 0)
                {
                    newForecastMaterials.Add(forecastMaterial);
                }
                else
                {
                    updateForecastMaterials.Add(forecastMaterial);
                }
                //reservationForecastEntries.Add(reservationForecastEntry);
            }

            CalculateRequiredQuantity(newForecastMaterials, true, username, wastageSettings, out errorMessage);
            CalculateRequiredQuantity(updateForecastMaterials, true, username, wastageSettings, out errorMessage);
        }

        /// <summary>
        /// Calculate required quantity base on job quantity
        /// </summary>
        /// <param name="productionBOMs"></param>
        public static bool CalculateRequiredQuantity(List<ForecastMaterial> forecastMaterials, bool IsApplyWastageSetting, string username,
            List<WastageSetting> wastageSettings, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var forecastMaterial in forecastMaterials)
            {
                var rounding = forecastMaterial.PerUnit != null ? forecastMaterial.PerUnit?.Rouding : 4;
                decimal factorUnit = 1;

                if (forecastMaterial.PerUnitID != forecastMaterial.PriceUnitID)
                {
                    factorUnit = (decimal)forecastMaterial.PriceUnit?.Factor > 0 ?
                        (decimal)forecastMaterial.PriceUnit?.Factor : 1;
                }

                if (IsApplyWastageSetting)
                {
                    var wastageSetting = wastageSettings?
                        .FirstOrDefault(x => (x.MaterialTypeClass == forecastMaterial.MaterialClassType || string.IsNullOrEmpty(forecastMaterial.MaterialClassType)) &&
                                              x.MaterialTypeCode == forecastMaterial.MaterialTypeCode &&
                                              (x.MaxQuantity >= forecastMaterial.ForecastDetail?.PlannedQuantity || x.MaxQuantity == null) &&
                                              x.MinQuantity <= forecastMaterial.ForecastDetail?.PlannedQuantity);

                    if (wastageSetting != null)
                    {
                        if (forecastMaterial.WastagePercent > wastageSetting.WastageStandard)
                        {
                            errorMessage = "Invalid wastage for item " + forecastMaterial.ItemID + "-"
                                + forecastMaterial.ItemName;
                            return false;
                        }

                        rounding = wastageSetting.Rounding;
                    }
                }

                if (forecastMaterial.FabricWeight > 0 &&
                   forecastMaterial.FabricWidth > 0 &&
                   forecastMaterial.MaterialTypeCode == "FB")
                {
                    forecastMaterial.ConsumptionQuantity = ((36 * forecastMaterial.FabricWidth * forecastMaterial.FabricWeight) / 1550000)
                        * forecastMaterial.QuantityPerUnit * forecastMaterial.ForecastDetail?.PlannedQuantity;
                    forecastMaterial.LessQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.LessPercent / 100;
                    forecastMaterial.FreeQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.FreePercent / 100;
                    forecastMaterial.RequiredQuantity = ((36 * forecastMaterial.FabricWidth * forecastMaterial.FabricWeight) / 1550000)
                        * forecastMaterial.QuantityPerUnit * forecastMaterial.ForecastDetail?.PlannedQuantity
                        * (1 + forecastMaterial.WastagePercent / 100);

                    forecastMaterial.WareHouseQuantity = forecastMaterial.RequiredQuantity;
                }
                else if (forecastMaterial.FabricWeight > 0 &&
                         forecastMaterial.FabricWidth > 0)
                {
                    forecastMaterial.ConsumptionQuantity = forecastMaterial.QuantityPerUnit * forecastMaterial.ForecastDetail?.PlannedQuantity;
                    forecastMaterial.LessQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.LessPercent / 100;
                    forecastMaterial.FreeQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.FreePercent / 100;
                    forecastMaterial.RequiredQuantity =
                        forecastMaterial.QuantityPerUnit * forecastMaterial.ForecastDetail?.PlannedQuantity
                        * (1 + forecastMaterial.WastagePercent / 100);

                    forecastMaterial.WareHouseQuantity = forecastMaterial.RequiredQuantity;
                }
                else
                {
                    forecastMaterial.ConsumptionQuantity = forecastMaterial.QuantityPerUnit
                        * forecastMaterial.ForecastDetail?.PlannedQuantity;
                    forecastMaterial.LessQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.LessPercent / 100;
                    forecastMaterial.FreeQuantity = forecastMaterial.ForecastDetail?.PlannedQuantity
                        * forecastMaterial.FreePercent / 100;
                    forecastMaterial.RequiredQuantity =
                        forecastMaterial.QuantityPerUnit * forecastMaterial.ForecastDetail?.PlannedQuantity
                        * (1 + forecastMaterial.WastagePercent / 100);

                    forecastMaterial.WareHouseQuantity = forecastMaterial.RequiredQuantity /
                        factorUnit;
                }

                forecastMaterial.RequiredQuantity = Math.Round((decimal)forecastMaterial.RequiredQuantity /
                      factorUnit, (int)rounding);
                forecastMaterial.ConsumptionQuantity = Math.Round((decimal)forecastMaterial.ConsumptionQuantity /
                      factorUnit, (int)rounding);
                if (forecastMaterial.LessQuantity != null)
                    forecastMaterial.LessQuantity = Math.Round((decimal)forecastMaterial.LessQuantity, (int)rounding);

                if (forecastMaterial.FreeQuantity != null)
                    forecastMaterial.FreeQuantity = Math.Round((decimal)forecastMaterial.FreeQuantity, (int)rounding);

                forecastMaterial.WareHouseQuantity = Math.Round((decimal)forecastMaterial.WareHouseQuantity, (int)rounding);
                forecastMaterial.WastageQuantity = forecastMaterial.RequiredQuantity - forecastMaterial.ConsumptionQuantity;
                forecastMaterial.RemainQuantity = forecastMaterial.RequiredQuantity;
                forecastMaterial.SetUpdateAudit(username);
            }

            return true;
        }
        #endregion
    }
}
