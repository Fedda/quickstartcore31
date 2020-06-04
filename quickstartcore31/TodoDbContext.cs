using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using quickstartcore31.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quickstartcore31
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Items");
            //modelBuilder.Entity<Item>().HasAlternateKey(p => p.ItemId);
            //modelBuilder.Entity<Item>(entity =>
            //{
            //    entity.HasNoKey();

            //});
        }
        public DbSet<Models.Item> Items { get; set; }
    }
}
