using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class ChatAttachmentDto
    {
         
            public long MessageId { get; set; }

            public string FileName { get; set; }

            public string FileUrl { get; set; }

            public string MessageType { get; set; }

            public DateTime CreatedDate { get; set; }
       
    }
}
