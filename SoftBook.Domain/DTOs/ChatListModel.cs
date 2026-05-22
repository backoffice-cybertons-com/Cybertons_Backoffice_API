using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class ChatListModel
    {

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageDate { get; set; }

        public int UnreadCount { get; set; }
    }
}
