using Microsoft.AspNetCore.Http;

namespace SoftBook.API.Models
{
    public class SendAttachmentRequest
    {
        public long CompanyId { get; set; }

        public long SenderUserId { get; set; }

        public long ReceiverUserId { get; set; }

        public IFormFile File { get; set; }
    }
}
