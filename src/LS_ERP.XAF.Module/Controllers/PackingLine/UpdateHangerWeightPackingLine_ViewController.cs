using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateHangerWeightPackingLine_ViewController : ObjectViewController<ListView, PackingLine>
    {
        public UpdateHangerWeightPackingLine_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateHangerWeightPackingLineAction = new PopupWindowShowAction(this, "UpdateHangerWeightPackingLineAction", PredefinedCategory.Unspecified);
            updateHangerWeightPackingLineAction.ImageName = "WeightedPies_32x32";
            updateHangerWeightPackingLineAction.Caption = "Update Hanger (Ctrl + H)";
            updateHangerWeightPackingLineAction.TargetObjectType = typeof(PackingLine);
            updateHangerWeightPackingLineAction.TargetViewType = ViewType.ListView;
            updateHangerWeightPackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateHangerWeightPackingLineAction.Shortcut = "CtrlH";

            updateHangerWeightPackingLineAction.CustomizePopupWindowParams += UpdateHangerWeightPackingLineAction_CustomizePopupWindowParams;
            updateHangerWeightPackingLineAction.Execute += UpdateHangerWeightPackingLineAction_Execute;
        }

        private void UpdateHangerWeightPackingLineAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var packingLines = (View as ListView).CollectionSource.List
                    .Cast<PackingLine>().ToList().OrderBy(x => x.SequenceNo);
            var param = e.PopupWindowViewCurrentObject as ViewPackingLineHangerParam;
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            var errorMessage = "";
            decimal hangerGW = 0;

            try
            {
                packingLines?.ToList().ForEach(l =>
                {
                    l.GrossWeight = l.NetWeight + (l?.BoxDimension?.Weight ?? 0) * l.TotalCarton;
                });

                var hangers = param?.Hangers?.Where(x => !string.IsNullOrEmpty(x?.Hanger?.Code)).ToList();
                if (hangers.Any())
                {

                    var isAssortedSize = hangers?.FirstOrDefault()?.
                            PrePack == "Assorted Size - Solid Color" ? true : false;
                    if (hangers.Count == 1)
                    {
                        var hangerWeight = hangers?.FirstOrDefault()?.Hanger?.Weight ?? 0;
                        var hangerUnit = hangers?.FirstOrDefault()?.Unit?.ID?.ToUpper() ?? "PCS";
                        var unitFactor = hangers?.FirstOrDefault()?.Unit?.Factor ?? 0;
                        if (isAssortedSize)
                        {
                            hangerGW = (decimal)((hangerUnit == "SET" || hangerUnit == "SETS")
                                ? (packingList?.TotalQuantity * hangerWeight / (unitFactor == 0 ? 2 : unitFactor))
                                : packingList?.TotalQuantity * hangerWeight);

                            packingLines?.ToList().ForEach(x =>
                            {
                                x.HangerCode = hangers?.FirstOrDefault()?.Hanger?.Code;
                                x.UnitID = hangerUnit;
                                x.GrossWeight += hangerGW;
                            });
                        }
                        else
                        {
                            param.Hangers.ToList().ForEach(h =>
                            {
                                var updateLines = packingLines.Where(x => x.Size == h.Size).ToList();

                                hangerGW = (decimal)((hangerUnit == "SET" || hangerUnit == "SETS")
                                    ? (updateLines?.Sum(l => l.TotalQuantity) * hangerWeight / (unitFactor == 0 ? 2 : unitFactor))
                                    : updateLines?.Sum(l => l.TotalQuantity) * hangerWeight);

                                updateLines.ForEach(x =>
                                {
                                    x.HangerCode = hangers?.FirstOrDefault()?.Hanger?.Code;
                                    x.UnitID = hangerUnit;
                                    x.GrossWeight += hangerGW;
                                });
                            });
                        }
                    }
                    else
                    {
                        if (isAssortedSize)
                        {
                            hangerGW = 0;
                            param.Hangers.ToList().ForEach(h =>
                            {
                                var updateLines = packingLines.Where(x => x.Size == h.Size).ToList();
                                var unitFactor = h?.Unit?.Factor ?? 0;

                                hangerGW += (decimal)((h?.Unit?.ID == "SET" || h?.Unit?.ID == "SETS")
                                    ? (updateLines?.Sum(l => l.QuantitySize * l.TotalCarton) * h?.Hanger?.Weight / (unitFactor == 0 ? 2 : unitFactor))
                                    : updateLines?.Sum(l => l.QuantitySize * l.TotalCarton) * h?.Hanger?.Weight);

                                updateLines.ForEach(x =>
                                {
                                    x.HangerCode = h?.Hanger?.Code;
                                    x.UnitID = h?.Unit?.ID ?? "PCS";
                                });
                            });

                            packingLines.ToList().ForEach(x =>
                            {
                                x.GrossWeight += hangerGW;
                            });
                        }
                        else
                        {
                            param.Hangers.ToList().ForEach(h =>
                            {
                                var updateLines = packingLines.Where(x => x.Size == h.Size).ToList();
                                var unitFactor = h?.Unit?.Factor ?? 0;

                                hangerGW = (decimal)((h?.Unit?.ID == "SET" || h?.Unit?.ID == "SETS")
                                    ? (updateLines?.Sum(l => l.TotalQuantity) * h?.Hanger?.Weight / (unitFactor == 0 ? 2 : unitFactor))
                                    : updateLines?.Sum(l => l.TotalQuantity) * h?.Hanger?.Weight);

                                updateLines.ForEach(x =>
                                {
                                    x.HangerCode = h?.Hanger?.Code;
                                    x.UnitID = h?.Unit?.ID ?? "PCS";
                                    x.GrossWeight += hangerGW;
                                });
                            });
                        }
                    }

                    objectSpace.CommitChanges();
                    var message = Message.GetMessageOptions("Update hanger weight successful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
                else
                {
                    packingLines?.ToList().ForEach(l =>
                    {
                        l.Hanger = null;
                        l.Unit = null;
                    });

                    objectSpace.CommitChanges();
                    var message = Message.GetMessageOptions("Update hanger weight successful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
            }
            catch (Exception ex)
            {
                var error = Message.GetMessageOptions(ex.Message, "Error", InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
            }

            

            View.Refresh();
            ((DetailView)View.ObjectSpace.Owner).Refresh();
        }
        private void UpdateHangerWeightPackingLineAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var packingLines = ((ListView)View).CollectionSource.List
                    .Cast<PackingLine>().OrderBy(x => x.SequenceNo).ToList();

            var hangerUnits = ObjectSpace.GetObjects<Unit>(CriteriaOperator.Parse("[ID] IN ('PCS','SETS')"));

            var model = new ViewPackingLineHangerParam();
            if (packingLines.Any())
            {
                var hangerLines = new List<HangerForPacking>();
                var firstLine = packingLines.FirstOrDefault();
                var sortedPackingLines = packingLines.Where(x => x.LSStyle == firstLine.LSStyle)
                                                .OrderBy(x => x.SequenceNo).ToList();
                if (firstLine.PrePack.Trim() == "Assorted Size - Solid Color")
                {
                    foreach (var data in sortedPackingLines)
                    {
                        var hanger = new HangerForPacking();
                        hanger.FromNo = firstLine?.FromNo ?? 0;
                        hanger.ToNo = firstLine?.ToNo ?? 0;
                        hanger.TotalQuantity = (decimal)(packingLines?
                            .Where(x => x.Size == data.Size)
                            .Sum(x => x.QuantitySize * x.TotalCarton));
                        hanger.TotalCarton = firstLine?.TotalCarton ?? 0;
                        hanger.Size = data.Size;
                        hanger.PrePack = firstLine?.PrePack;
                        hanger.Hanger = data?.Hanger;
                        hanger.Unit = data?.Unit ?? hangerUnits.OrderBy(x => x.ID).FirstOrDefault();

                        hangerLines.Add(hanger);
                    }
                }
                else
                {
                    foreach (var data in sortedPackingLines.OrderBy(x => x.SequenceNo))
                    {
                        var hanger = new HangerForPacking();
                        hanger.FromNo = data?.FromNo ?? 0;
                        hanger.ToNo = data?.ToNo ?? 0;
                        hanger.TotalQuantity = (decimal)(packingLines?
                            .Where(x => x.Size == data.Size)
                            .Sum(x => x.QuantitySize * x.TotalCarton));
                        hanger.TotalCarton = data?.TotalCarton ?? 0;
                        hanger.Size = data.Size;
                        hanger.PrePack = firstLine?.PrePack;
                        hanger.Hanger = data?.Hanger;
                        hanger.Unit = data?.Unit ?? hangerUnits.OrderBy(x => x.ID).FirstOrDefault();

                        hangerLines.Add(hanger);
                    }
                }
                model.Hangers = hangerLines;
            }

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
