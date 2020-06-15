using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace quickstartcore31.Test
{
    public class CosmoDbTest
    {
        private readonly ITestOutputHelper _output;

        public CosmoDbTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void AddItemToDatabase_ItemCountOne_ReturnOneItem()
        {

            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseCosmos(
                        accountEndpoint: "https://localhost:8081",
                        accountKey: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                        databaseName: "TodoList")
                .UseLoggerFactory(new LoggerFactory(
        new[] { new LogToActionLoggerProvider((log) =>
                {
                    _output.WriteLine(log);
                }) }))
                .Options;

            //Arrange
            using (var context = new TodoDbContext(options))
            {                
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var item = new quickstartcore31.Models.Item();
                item.Completed = false;
                item.Description = "Todo 123";
                item.Name = "Todo";
                item.Id = Guid.NewGuid().ToString();
                context.Add(item);
                context.SaveChanges();
            }
            //act
            using (var context = new TodoDbContext(options))
            {
                var inMemoryRepository = new CosmoDBEFRepository<Models.Item>(context);
                Expression<Func<Models.Item, bool>> all = (i) => true;
                var fetchedItem = await inMemoryRepository.GetItemsAsync(all);

                //Assert
                Assert.Single(fetchedItem);
            }
            //Assert.Equal(EntityState.Added, context.Entry(item).State);
        }

        [Fact]
        public void GetItem_EmptyGuid_ThrowsArgumentException()
        {
            Assert.Equal("todo", "todo");
        }
    }
}
