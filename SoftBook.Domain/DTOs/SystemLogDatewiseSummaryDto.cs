namespace SoftBook.Domain.DTOs
{
    public class SystemLogDatewiseSummaryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int LoginCount { get; set; }
        public int LogoutCount { get; set; }
        public int AddCount { get; set; }
        public int EditCount { get; set; }
        public int DeleteCount { get; set; }
    }
}
