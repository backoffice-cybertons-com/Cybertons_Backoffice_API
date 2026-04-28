using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoftBook.Domain.ViewModels
{
  public class PartyViewModel
  {
    public PartyViewModel()
    {
      // Always initialize objects & collections
      Party = new Party();

      Addresses = new List<PartyAddress>
            {
                new PartyAddress { AddressType = "Billing" },
                new PartyAddress { AddressType = "Shipping" }
            };

      BankDetails = new List<PartyBankDetails>();
      ContactPersons = new List<PartyContactPerson>();
      ReportingTags = new List<PartyReportingTag>();
      PartyTypes = new List<PartyTypeMaster>();
    }

    public Party Party { get; set; }

    public List<PartyAddress> Addresses { get; set; }
    public List<PartyBankDetails> BankDetails { get; set; }
    public List<PartyContactPerson> ContactPersons { get; set; }
    public List<PartyReportingTag> ReportingTags { get; set; }
    public List<PartyTypeMaster> PartyTypes { get; set; }

    // For form context
    public string? Context { get; set; }          // "Purchase", "Sales", or null
    public string? DefaultPartyType { get; set; } // "Vendor", "Customer", or null
  }
}
