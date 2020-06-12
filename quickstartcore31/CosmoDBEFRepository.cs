using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore;
using quickstartcore31.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace quickstartcore31
{
    public class CosmoDBEFRepository<T> : IDocumentDBRepository<T> where T : class
    {

        private readonly TodoDbContext context;

        public CosmoDBEFRepository(TodoDbContext context)
        {
            this.context = context;
        }

        public async Task<T> CreateItemAsync(T item)
        {
            var entity = context.Add(item);
            await context.SaveChangesAsync();
            return entity.Entity;

        }

        public async Task DeleteItemAsync(string id, T item)
        {
            //var item = await context.Items.FindAsync(id);
            if (item != null)
            {
                context.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task<T> GetItemAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(nameof(id));
            }
            T item = await context.FindAsync<T>(id);

            return item;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var list = await context.Set<T>().Where(predicate).ToListAsync<T>();
            return list;
        }

        public async Task<T> UpdateItemAsync(string id, T item)
        {
            //https://github.com/dotnet/efcore/issues/15289
            //https://stackoverflow.com/questions/60689345/error-alternate-key-property-id-is-null-with-update-call-to-cosmos-db-and
            var itemEntry = context.Entry(item);            
            itemEntry.Property<string>("id").CurrentValue = "Item|" + id;
            itemEntry.State = EntityState.Modified;            
            await context.SaveChangesAsync();
            return itemEntry.Entity;
            //return (T)(dynamic)itemobj;
        }
    }
}
