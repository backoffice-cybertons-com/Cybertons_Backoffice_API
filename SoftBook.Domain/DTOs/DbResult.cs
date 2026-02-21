using System;

namespace SoftBook.Domain.DTOs
{
    public class DbResult
    {
        public bool Success { get; set; }
        public dynamic? Data { get; set; }
        public string? Error { get; set; }
        public string? Detail { get; set; }
        public string? Code { get; set; }
        public string? Constraint { get; set; }

        public static DbResult Ok(dynamic? data = null)
        {
            return new DbResult { Success = true, Data = data };
        }

        public static DbResult Failure(string error, string? code = null, string? detail = null, string? constraint = null)
        {
            return new DbResult 
            { 
                Success = false, 
                Error = error, 
                Code = code, 
                Detail = detail, 
                Constraint = constraint 
            };
        }
    }
}
