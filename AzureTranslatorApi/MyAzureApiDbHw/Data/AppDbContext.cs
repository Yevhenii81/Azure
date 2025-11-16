using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyAzureApiDbHw.Models;


namespace MyAzureApiDbHW.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}
