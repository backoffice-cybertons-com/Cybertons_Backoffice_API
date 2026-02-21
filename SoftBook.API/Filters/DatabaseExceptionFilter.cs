using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;
using System.Net;

namespace SoftBook.API.Filters
{
    public class DatabaseExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // Check if the exception is from PostgreSQL
            if (exception is PostgresException pgEx || 
               (exception.InnerException is PostgresException innerPgEx && (pgEx = innerPgEx) != null))
            {
                string message = "A database error occurred.";
                var statusCode = HttpStatusCode.InternalServerError;

                // PostgreSQL Error Codes:
                // 23505: unique_violation (Duplicate Key)
                // 23503: foreign_key_violation (Cannot delete/update due to references)
                
                if (pgEx.SqlState == "23505")
                {
                    message = "Duplicate record found. This entry already exists in the system.";
                    statusCode = HttpStatusCode.Conflict; // 409
                }
                else if (pgEx.SqlState == "23503")
                {
                    statusCode = HttpStatusCode.BadRequest; // 400
                    
                    // Distinguish between Insert (linking to non-existent parent) and Delete (parent in use)
                    if (pgEx.Detail != null && pgEx.Detail.Contains("not present in table"))
                    {
                        message = "The selected parent record (or ID) does not exist. Please check your IDs.";
                    }
                    else
                    {
                        message = "This record cannot be deleted because it is being used by another process or record.";
                    }
                }

                var result = new ObjectResult(new
                {
                    error = message,
                    detail = pgEx.MessageText, // Cleaner detail from Postgres
                    constraint = pgEx.ConstraintName,
                    code = pgEx.SqlState
                })
                {
                    StatusCode = (int)statusCode
                };

                context.Result = result;
                context.ExceptionHandled = true;
            }
        }
    }
}
