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
    public class InMemoryTests
    {
        private readonly ITestOutputHelper _output;

        public InMemoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void AddItemToDatabase_ItemCountOne_ReturnOneItem()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase($"TodoList{Guid.NewGuid()}")
                .UseLoggerFactory(new LoggerFactory(new[] { new LogToActionLoggerProvider((log) =>
                {
                    _output.WriteLine(log);
                })}))
                .Options;

            //Arrange
            using (var context = new TodoDbContext(options))
            {

                context.Database.OpenConnection();
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
            var options = new DbContextOptionsBuilder<TodoDbContext>()
               .UseInMemoryDatabase($"TodoList{Guid.NewGuid()}")
               .Options;

            //Arrange
            using (var context = new TodoDbContext(options)) //hente fra andre prosjektet
            {
                //act
                var inMemoryRepository = new CosmoDBEFRepository<Models.Item>(context);
                //var item = await 

                _ = Assert.ThrowsAsync<ArgumentException>(async () => await inMemoryRepository.GetItemAsync(""));
            }
        }
    }
}
