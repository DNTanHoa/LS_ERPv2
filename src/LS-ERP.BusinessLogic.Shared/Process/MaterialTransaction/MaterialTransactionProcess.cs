using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class MaterialTransactionProcess
    {
        public static List<MaterialTransaction> CreateTransactions(Receipt receipt, string username)
        {
            if (receipt != null)
            {
                if (receipt.ReceiptGroupLines != null &&
                   receipt.ReceiptGroupLines.Any())
                {
                    var transactions = new List<MaterialTransaction>();
                    var config = new MapperConfiguration(c =>
                    {
                        c.CreateMap<ReceiptGroupLine, MaterialTransaction>()
                        .ForMember(x => x.TransactionDate, y => y.MapFrom(s => DateTime.Now))
                        .ForMember(x => x.Quantity, y => y.MapFrom(s => s.ReceiptQuantity));
                    });

                    var mapper = config.CreateMapper();

                    foreach (var groupLine in receipt.ReceiptGroupLines)
                    {
                        var transaction = mapper.Map<MaterialTransaction>(groupLine);
                        transaction.ReceiptNumber = receipt.Number;
                        transaction.Receipt = receipt;
                        transaction.ReceiptGroupLine = groupLine;
                        transaction.InvoiceNumberNoTotal = receipt.InvoiceNumberNoTotal;
                        transaction.ProductionMethodCode = receipt.ProductionMethodCode;

                        transaction.StorageCode = receipt.StorageCode;
                        transaction.Supplier = receipt.VendorName;
                        transaction.SetCreateAudit(username);

                        groupLine.Transactions.Add(transaction);
                        transactions.Add(transaction);
                    }

                    return transactions;
                }
            }
            return null;
        }

        public static List<MaterialTransaction> CreateTransactions(Issued issued, string username)
        {
            if (issued != null)
            {
                if (issued.IssuedGroupLines != null &&
                   issued.IssuedGroupLines.Any())
                {
                    var transactions = new List<MaterialTransaction>();

                    foreach (var groupLine in issued.IssuedGroupLines)
                    {
                        if (groupLine.IssuedLines.Any())
                        {
                            foreach (var line in groupLine.IssuedLines)
                            {
                                var transaction = new MaterialTransaction()
                                {
                                    ItemID = line.ItemID,
                                    ItemName = line.ItemName,
                                    ItemColorCode = line.ItemColorCode,
                                    ItemColorName = line.ItemColorName,
                                    Position = line.Position,
                                    Specify = line.Specify,
                                    Season = line.Season,
                                    UnitID = line.UnitID,

                                    GarmentColorCode = line.GarmentColorCode,
                                    GarmentSize = line.GarmentSize,
                                    GarmentColorName = line.GarmentColorName,
                                    CustomerStyle = line.CustomerStyle,
                                    LSStyle = line.LSStyle,
                                    LotNumber = line.LotNumber,
                                    DyeLotNumber = line.DyeLotNumber,
                                    InvoiceNumber = issued.InvoiceNumber,
                                    InvoiceNumberNoTotal = issued.InvoiceNumberNoTotal,

                                    TransactionDate = DateTime.Now,
                                    Quantity = -line.IssuedQuantity,
                                    Roll = -line.Roll,
                                    IssuedNumber = issued.Number,
                                    IssuedLine = line,
                                    RequestQuantity = line.RequestQuantity,
                                    StorageCode = issued.StorageCode,

                                    Issued = issued,
                                };

                                transaction.SetCreateAudit(username);

                                transactions.Add(transaction);
                            }
                        }
                    }

                    return transactions;
                }
            }

            return null;
        }

        public static List<MaterialTransaction> CreateTransactionsFB(Issued issued, string username)
        {
            if (issued != null)
            {
                if (issued.IssuedGroupLines != null &&
                   issued.IssuedGroupLines.Any())
                {
                    var transactions = new List<MaterialTransaction>();

                    foreach (var groupLine in issued.IssuedGroupLines)
                    {
                        if (groupLine.IssuedLines.Any())
                        {
                            foreach (var line in groupLine.IssuedLines)
                            {
                                var transaction = new MaterialTransaction()
                                {
                                    ItemID = line.ItemID,
                                    ItemName = line.ItemName,
                                    ItemColorCode = line.ItemColorCode,
                                    ItemColorName = line.ItemColorName,
                                    Position = line.Position,
                                    Specify = line.Specify,
                                    Season = line.Season,
                                    UnitID = line.UnitID,

                                    GarmentColorCode = line.GarmentColorCode,
                                    GarmentSize = line.GarmentSize,
                                    GarmentColorName = line.GarmentColorName,
                                    CustomerStyle = line.CustomerStyle,
                                    LSStyle = line.LSStyle,
                                    LotNumber = line.LotNumber,
                                    DyeLotNumber = line.DyeLotNumber,
                                    InvoiceNumber = issued.InvoiceNumber,
                                    InvoiceNumberNoTotal = issued.InvoiceNumberNoTotal,

                                    TransactionDate = DateTime.Now,
                                    Quantity = -line.IssuedQuantity,
                                    Roll = -line.Roll,
                                    IssuedNumber = issued.Number,
                                    IssuedLine = line,
                                    FabricRequestDetailID = line.FabricRequestDetailID,
                                    RequestQuantity = line.RequestQuantity,
                                    StorageDetailID = line.StorageDetailID,
                                    StorageCode = issued.StorageCode,
                                    FabricPurchaseOrderNumber = line.FabricPurchaseOrderNumber,
                                    PurchaseOrderNumber = line.PurchaseOrderNumber,

                                    Issued = issued,
                                };

                                transaction.SetCreateAudit(username);

                                transactions.Add(transaction);
                            }
                        }
                    }

                    return transactions;
                }
            }

            return null;
        }
    }
}
