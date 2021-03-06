﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;

namespace quickstartcore31
{

    public class DocumentDBRepository<T> : IDocumentDBRepository<T> where T : class
    {



        private readonly string Endpoint = "";//"DOCUMENTDBURL";
        private readonly string Key = ""; 
        private readonly string DatabaseId = "";//"DOCUMENTDBDATABASEID";
        private readonly string CollectionId = "Items";//"DOCUMENTDBCOLLECTIONID";
        private DocumentClient client;

        public DocumentDBRepository(IConfiguration Configuration)
        {
            Endpoint = Configuration["CosmosDbInfo:endpoint"];
            Key = Configuration["CosmosDbInfo:authKey"];
            DatabaseId = Configuration["CosmosDbInfo:database"];

            this.client = new DocumentClient(new Uri(Endpoint), Key);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }
        public async Task<T> GetItemAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(nameof(id));
            }
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<T> CreateItemAsync(T item)
        {
            var document = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);

            return (T)(dynamic)document.Resource;


        }

        public async Task<T> UpdateItemAsync(string id, T item)
        {
            var document = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
            return (T)(dynamic)document.Resource;
        }

        public async Task DeleteItemAsync(string id, T item)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }


        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
