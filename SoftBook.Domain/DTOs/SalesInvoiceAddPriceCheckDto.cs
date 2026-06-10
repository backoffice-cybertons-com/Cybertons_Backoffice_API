using System;

namespace SoftBook.Domain.DTOs
{
    public class SalesInvoiceAddPriceCheckDto
    {
        public int SpUniqueCode { get; set; }
        public string InvNo { get; set; } = string.Empty;
        public DateTime InvDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int PartyId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PartyType { get; set; } = string.Empty;
        public decimal ActualAddPrice { get; set; }
    }
}
