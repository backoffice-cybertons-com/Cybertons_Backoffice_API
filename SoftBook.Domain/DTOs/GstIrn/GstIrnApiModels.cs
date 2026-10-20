using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs.GstIrn;

public class GstIrnAuthRequest
{
    public string Password { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool ForceRefreshAccessToken { get; set; }
}

public class GstIrnRequestPayload
{
    public string Data { get; set; } = string.Empty;
}

public class GstIrnAuthResponse
{
    public string? Status { get; set; }
    public List<GstIrnErrorDetail>? ErrorDetails { get; set; }
    public GstIrnAuthData? Data { get; set; }
}

public class GstIrnErrorDetail
{
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}

public class GstIrnAuthData
{
    public string? ClientId { get; set; }
    public string? UserName { get; set; }
    public string? AuthToken { get; set; }
    public string? Sek { get; set; }
    public string? TokenExpiry { get; set; }
    public long AckNo { get; set; }
    public DateTimeOffset AckDt { get; set; }
    public string? Irn { get; set; }
    public string? SignedInvoice { get; set; }
    public string? SignedQrCode { get; set; }
    public string? Status { get; set; }
    public string? EwbNo { get; set; }
    public string? EwbDt { get; set; }
    public string? EwbValidTill { get; set; }
    public long SPCode { get; set; }
    public string? InvNo { get; set; }
}

public class GenerateIrnReqModel
{
    public string Version { get; set; } = "1.1";
    public TranDtls? TranDtls { get; set; }
    public DocDtls? DocDtls { get; set; }
    public SellerDtls? SellerDtls { get; set; }
    public BuyerDtls? BuyerDtls { get; set; }
    public DispDtls? DispDtls { get; set; }
    public ShipDtls? ShipDtls { get; set; }
    public ValDtls? ValDtls { get; set; }
    public ExpDtls? ExpDtls { get; set; }
    public List<ItemList>? ItemList { get; set; }
}

public class TranDtls
{
    public string TaxSch { get; set; } = "GST";
    public string SupTyp { get; set; } = "B2B";
    public string IgstOnIntra { get; set; } = "N";
    public object? RegRev { get; set; }
    public object? EcmGstin { get; set; }
}

public class DocDtls
{
    public string Typ { get; set; } = "INV";
    public string No { get; set; } = string.Empty;
    public string Dt { get; set; } = string.Empty;
}

public class SellerDtls
{
    public string Gstin { get; set; } = string.Empty;
    public string LglNm { get; set; } = string.Empty;
    public object? TrdNm { get; set; }
    public string Addr1 { get; set; } = string.Empty;
    public object? Addr2 { get; set; }
    public string Loc { get; set; } = string.Empty;
    public int Pin { get; set; }
    public string Stcd { get; set; } = string.Empty;
    public object? Ph { get; set; }
    public object? Em { get; set; }
}

public class BuyerDtls
{
    public string Gstin { get; set; } = string.Empty;
    public string LglNm { get; set; } = string.Empty;
    public object? TrdNm { get; set; }
    public string Pos { get; set; } = string.Empty;
    public string Addr1 { get; set; } = string.Empty;
    public string? Addr2 { get; set; }
    public string Loc { get; set; } = string.Empty;
    public int Pin { get; set; }
    public string Stcd { get; set; } = string.Empty;
    public object? Ph { get; set; }
    public object? Em { get; set; }
}

public class DispDtls
{
    public string Nm { get; set; } = string.Empty;
    public string Addr1 { get; set; } = string.Empty;
    public string Loc { get; set; } = string.Empty;
    public int Pin { get; set; }
    public string Stcd { get; set; } = string.Empty;
    public string? Addr2 { get; set; }
}

public class ShipDtls
{
    public string Gstin { get; set; } = string.Empty;
    public string LglNm { get; set; } = string.Empty;
    public string Addr1 { get; set; } = string.Empty;
    public string Loc { get; set; } = string.Empty;
    public int Pin { get; set; }
    public string Stcd { get; set; } = string.Empty;
    public string? Addr2 { get; set; }
}

public class ValDtls
{
    public decimal AssVal { get; set; }
    public decimal IgstVal { get; set; }
    public decimal CgstVal { get; set; }
    public decimal SgstVal { get; set; }
    public decimal CesVal { get; set; }
    public decimal StCesVal { get; set; }
    public decimal Discount { get; set; }
    public decimal OthChrg { get; set; }
    public decimal RndOffAmt { get; set; }
    public decimal TotInvVal { get; set; }
}

public class ExpDtls
{
    public object? ShipBNo { get; set; }
    public object? ShipBDt { get; set; }
    public object? Port { get; set; }
    public object? RefClm { get; set; }
    public object? ForCur { get; set; }
    public object? CntCode { get; set; }
    public decimal ExpDuty { get; set; }
}

public class ItemList
{
    public string SlNo { get; set; } = string.Empty;
    public string PrdDesc { get; set; } = string.Empty;
    public string IsServc { get; set; } = "N";
    public string HsnCd { get; set; } = string.Empty;
    public double Qty { get; set; }
    public string Unit { get; set; } = "OTH";
    public double UnitPrice { get; set; }
    public decimal TotAmt { get; set; }
    public decimal Discount { get; set; }
    public decimal PreTaxVal { get; set; }
    public decimal AssAmt { get; set; }
    public double GstRt { get; set; }
    public decimal IgstAmt { get; set; }
    public decimal CgstAmt { get; set; }
    public decimal SgstAmt { get; set; }
    public decimal CesRt { get; set; }
    public decimal CesAmt { get; set; }
    public decimal CesNonAdvlAmt { get; set; }
    public decimal StateCesRt { get; set; }
    public decimal StateCesAmt { get; set; }
    public decimal StateCesNonAdvlAmt { get; set; }
    public decimal OthChrg { get; set; }
    public decimal TotItemVal { get; set; }
}
