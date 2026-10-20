using SoftBook.Domain.DTOs.GstIrn;
using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Classes;

public class GstIrnPayloadBuilder
{
    public GenerateIrnReqModel Build(
        SalePur header,
        List<SalePurTran> lines,
        CompanyProfile company,
        Party party,
        PartyAddress? billing,
        PartyAddress? shipping,
        IReadOnlyDictionary<int, string> unitUqcById,
        IReadOnlyDictionary<int, string> productNameById)
    {
        var sellerStateCode = GetStateCode(company.Gstin, company.State);
        var buyerStateCode = GetStateCode(party.GSTNO, party.PlaceOfSupply);
        var isInterState = !string.IsNullOrWhiteSpace(sellerStateCode)
            && !string.IsNullOrWhiteSpace(buyerStateCode)
            && sellerStateCode != buyerStateCode;

        var invNo = (header.InvNo ?? string.Empty).TrimStart('0');
        if (string.IsNullOrWhiteSpace(invNo))
        {
            invNo = header.InvNo ?? string.Empty;
        }

        var buyerGstin = party.GSTNO ?? string.Empty;
        var supplyType = string.IsNullOrWhiteSpace(buyerGstin) ? "B2C" : "B2B";

        var seller = new SellerDtls
        {
            Gstin = company.Gstin ?? string.Empty,
            LglNm = company.CompanyName ?? string.Empty,
            Addr1 = Truncate(company.CompanyAddress, 100),
            Loc = company.State ?? string.Empty,
            Pin = ParsePin(null),
            Stcd = sellerStateCode,
            Em = company.Email
        };

        var buyer = new BuyerDtls
        {
            Gstin = buyerGstin,
            LglNm = party.DisplayName,
            Pos = buyerStateCode,
            Addr1 = Truncate(billing?.Street1 ?? header.BillingAddress1 ?? header.BillingAddress, 100),
            Addr2 = Truncate(billing?.Street2 ?? header.BillingAddress2, 100),
            Loc = billing?.City ?? string.Empty,
            Pin = ParsePin(billing?.PinCode),
            Stcd = buyerStateCode,
            Ph = party.PhoneMobile ?? party.PhoneWork,
            Em = party.Email
        };

        var ship = new ShipDtls
        {
            Gstin = buyerGstin,
            LglNm = party.DisplayName,
            Addr1 = Truncate(shipping?.Street1 ?? header.ShippingAddress1 ?? header.ShippingAddress ?? buyer.Addr1, 100),
            Addr2 = Truncate(shipping?.Street2 ?? header.ShippingAddress2, 100),
            Loc = shipping?.City ?? buyer.Loc,
            Pin = ParsePin(shipping?.PinCode) > 0 ? ParsePin(shipping?.PinCode) : buyer.Pin,
            Stcd = buyerStateCode
        };

        var disp = new DispDtls
        {
            Nm = company.CompanyName ?? string.Empty,
            Addr1 = seller.Addr1,
            Loc = seller.Loc,
            Pin = seller.Pin,
            Stcd = sellerStateCode,
            Addr2 = null
        };

        var taxable = header.TaxableAmount ?? lines.Sum(l => l.Amount);
        var igst = header.IgstAmount ?? lines.Sum(l => l.IgstAmount ?? 0);
        var cgst = header.CgstAmount ?? lines.Sum(l => l.CgstAmount ?? 0);
        var sgst = header.SgstAmount ?? lines.Sum(l => l.SgstAmount ?? 0);
        var totInvVal = header.FinalInvoiceAmount ?? header.GrandTotal ?? (taxable + igst + cgst + sgst + (header.RoundOff ?? 0));

        var valDtls = new ValDtls
        {
            AssVal = Round2(taxable),
            IgstVal = Round2(igst),
            CgstVal = Round2(cgst),
            SgstVal = Round2(sgst),
            Discount = Round2(header.TotDiscount),
            RndOffAmt = Round2(header.RoundOff ?? 0),
            TotInvVal = Round2(totInvVal)
        };

        var itemList = new List<ItemList>();
        var slNo = 1;
        foreach (var line in lines)
        {
            var lineDiscount = line.Amount * line.DiscountPer / 100m;
            var gstRate = (double)((line.IgstPercent ?? 0) > 0
                ? line.IgstPercent ?? 0
                : (line.CgstPercent ?? 0) + (line.SgstPercent ?? 0));

            var productName = line.ProductDetails;
            if (string.IsNullOrWhiteSpace(productName) && line.ProductId.HasValue)
            {
                productNameById.TryGetValue(line.ProductId.Value, out productName);
            }

            var unitCode = "OTH";
            if (line.Unit.HasValue)
            {
                unitUqcById.TryGetValue(line.Unit.Value, out unitCode);
            }
            unitCode ??= "OTH";

            itemList.Add(new ItemList
            {
                SlNo = slNo.ToString(),
                PrdDesc = Truncate(productName ?? "Product", 300),
                IsServc = "N",
                HsnCd = line.HsnCode ?? string.Empty,
                Qty = (double)line.Quantity,
                Unit = unitCode,
                UnitPrice = (double)line.Rate,
                TotAmt = Round2(line.Amount),
                Discount = Round2(lineDiscount),
                AssAmt = Round2(line.Amount),
                GstRt = gstRate,
                IgstAmt = Round2(line.IgstAmount ?? 0),
                CgstAmt = Round2(line.CgstAmount ?? 0),
                SgstAmt = Round2(line.SgstAmount ?? 0),
                TotItemVal = Round2(line.TotalAmount)
            });
            slNo++;
        }

        return new GenerateIrnReqModel
        {
            Version = "1.1",
            TranDtls = new TranDtls
            {
                TaxSch = "GST",
                SupTyp = supplyType,
                IgstOnIntra = isInterState ? "N" : "N"
            },
            DocDtls = new DocDtls
            {
                Typ = "INV",
                No = invNo,
                Dt = header.InvDate.ToString("dd/MM/yyyy")
            },
            SellerDtls = seller,
            BuyerDtls = buyer,
            DispDtls = disp,
            ShipDtls = ship,
            ValDtls = valDtls,
            ExpDtls = new ExpDtls(),
            ItemList = itemList
        };
    }

    private static string GetStateCode(string? gstin, string? fallback)
    {
        if (!string.IsNullOrWhiteSpace(gstin) && gstin.Length >= 2 && char.IsDigit(gstin[0]) && char.IsDigit(gstin[1]))
        {
            return gstin[..2];
        }

        if (!string.IsNullOrWhiteSpace(fallback) && fallback.Length == 2 && int.TryParse(fallback, out _))
        {
            return fallback;
        }

        return "24";
    }

    private static int ParsePin(string? pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
        {
            return 0;
        }

        var digits = new string(pin.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var value) ? value : 0;
    }

    private static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Length <= max ? value : value[..max];
    }
}
