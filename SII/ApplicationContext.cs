using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SII
{
    public class ApplicationContext : DbContext
    {
        DbSet<Lection> Lections { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<UserMark> UserMarks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=application.db");

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
    }
}
