using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class SendMessageDto
    {
        public long CompanyId { get; set; }

        public long SenderUserId { get; set; }

        public long ReceiverUserId { get; set; }

        public string? MessageText { get; set; }

        public string MessageType { get; set; } = "text";

        public long? ReplyMessageId { get; set; }
    }
}
