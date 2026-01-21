using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


using SoftBook.Domain.Entities;

namespace SoftBook.Domain.Data
{
  public class UserContext : DbContext
  {
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<ThemeUser> User { get; set; } = default!;
  }
}
