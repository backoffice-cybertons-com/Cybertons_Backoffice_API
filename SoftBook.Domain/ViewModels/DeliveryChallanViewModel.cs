using System.Threading.Tasks;
using System.Data;
using System;
using System.Collections.Generic;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.ViewModels
{
    public class DeliveryChallanViewModel
    {
        public DeliveryChallanViewModel()
        {
            Header = new DeliveryChallan();
            Items = new List<DeliveryChallanItem>();
        }

        public DeliveryChallan Header { get; set; }
        public List<DeliveryChallanItem> Items { get; set; }
    }
}
