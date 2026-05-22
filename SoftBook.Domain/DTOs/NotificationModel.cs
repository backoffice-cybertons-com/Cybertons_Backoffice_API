using System;

namespace SoftBook.Domain.DTOs
{
    public class NotificationModel
    {
        public long NotificationId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool ToastShown { get; set; }
        public int NotificationCount { get; set; } = 1;
        public long? SenderUserId { get; set; }
    }
}
