using System;

namespace SoftBook.Domain.DTOs
{
    public class AnnouncementModel
    {
        public long AnnouncementId { get; set; }
        public string? Title { get; set; }
        public string? ShortMessage { get; set; }
        public string? FullMessage { get; set; }
        public string? ImageUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonLink { get; set; }
        public string? PopupType { get; set; }
        public int Priority { get; set; } = 1;
        public DateTime? StartsAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public bool DeleteFlag { get; set; }
    }

    public class CreateAnnouncementRequest
    {
        public string? Title { get; set; }
        public string? ShortMessage { get; set; }
        public string? FullMessage { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonLink { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateAnnouncementRequest : CreateAnnouncementRequest
    {
    }

    public class MarkAnnouncementSeenRequest
    {
        public long AnnouncementId { get; set; }
    }

    public class SetAnnouncementActiveRequest
    {
        public bool IsActive { get; set; }
    }
}
