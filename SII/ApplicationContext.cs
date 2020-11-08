using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SII
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Lection> Lections { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMark> UserMarks { get; set; }
        public DbSet<UserLection> UserLections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=application.db");

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
    }
}
