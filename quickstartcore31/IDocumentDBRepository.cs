using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace quickstartcore31
{
    public interface IDocumentDBRepository<T> where T : class
    {
        Task<T> CreateItemAsync(T item);
        Task DeleteItemAsync(string id, T item);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<T> UpdateItemAsync(string id, T item);
    }
}
