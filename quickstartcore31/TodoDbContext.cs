﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
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

        public DbSet<Models.Item> Items { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Items");
            //modelBuilder.Entity<Item>().HasAlternateKey(p => p.ItemId);
            //modelBuilder.Entity<Item>(entity =>
            //{
            //    entity.HasNoKey();

            //});
            //Test data
            //modelBuilder.Entity<Models.Item>().HasData(
            //    new Item()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        Completed = false,
            //        Description = "todo todo",
            //        Name = "todo todo todo"
            //    });

            //base.OnModelCreating(modelBuilder);
        }

        //public static readonly ILoggerFactory ConsoleLoggerFactory
        //    = LoggerFactory.Create(builder =>
        //    {
        //        && category == DbLoggerCategory.Database.Command.Name
        //        builder
        //            .AddFilter((category, level) => level == LogLevel.Trace)
        //            .AddConsole()
        //            .AddDebug();
        //    });

        public static readonly Microsoft.Extensions.Logging.LoggerFactory LoggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
        });

        //Doesn't work is visual studio output.... https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging?tabs=v3
    //    public static readonly ILoggerFactory MyLoggerFactory =
    //LoggerFactory.Create(builder => { builder.AddConsole().AddDebug(); });
    }
}
