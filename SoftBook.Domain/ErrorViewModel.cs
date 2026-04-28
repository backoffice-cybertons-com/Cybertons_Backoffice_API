using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.Entities
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
