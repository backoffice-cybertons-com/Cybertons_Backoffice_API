using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class Message
    {
        public long MessageId { get; set; }

        public long CompanyId { get; set; }

        public long SenderUserId { get; set; }

        public long ReceiverUserId { get; set; }

        public string? MessageText { get; set; }

        public string MessageType { get; set; } = "text";

        public string? FileLink { get; set; }

        public DateTime? ReadAt { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEdited { get; set; }

        public long? ReplyMessageId { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
