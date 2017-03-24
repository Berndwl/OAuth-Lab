using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using StackExchange.Redis;
using TFiN.Domain.Entities.Dossiers;
using TFiN.Domain.ProjectOpvoeren.Model;
using TFiN.OAuthLab.Areas.OAuth.Models;

namespace TFiN.OAuthLab.Helpers
{
    public static class DocumentDbRepository<T>
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DocumentDB.OAuth.Db"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["DocumentDB.OAuth.Collection"];
        private static readonly string endpoint = ConfigurationManager.AppSettings["DocumentDB.OAuth.URI"];
        private static readonly string authKey = ConfigurationManager.AppSettings["DocumentDB.OAuth.Key"];
        private static Database _database;
        private static DocumentCollection _collection;
        private static DocumentClient _client;


        private static Database Database
        {
            get
            {
                return _database ?? (_database = Client.CreateDatabaseQuery()
                           .Where(d => d.Id == DatabaseId)
                           .AsEnumerable()
                           .FirstOrDefault());
            }
        }


        private static DocumentCollection Collection
        {
            get
            {
                return _collection ?? (_collection = Client.CreateDocumentCollectionQuery(Database.SelfLink)
                           .Where(c => c.Id == CollectionId)
                           .AsEnumerable()
                           .FirstOrDefault());
            }
        }


        private static DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    var endpointUri = new Uri(endpoint);

                    var timeout = TimeSpan.FromSeconds(5);

                    var cp = new ConnectionPolicy
                    {
                        RequestTimeout = timeout
                    };
                    _client = new DocumentClient(endpointUri, authKey, cp);
                }

                return _client;
            }
        }

        public static async Task<Document> CreateDocument(object obj)
        {
            var result = await Client.CreateDocumentAsync(Collection.SelfLink, obj);

            return result;
        }

        public static async Task<T> GetCodeByString(string codeString)
        {
            var objectTypeName = typeof(T).Name;

            var res =
                Client.CreateDocumentQuery<AutorisatieCode>(Collection.DocumentsLink)
                    .Where(code => code.CodeString == codeString && code.Type == objectTypeName)
                    .AsDocumentQuery();

            var batch = await res.ExecuteNextAsync<T>();
            return batch.FirstOrDefault();
        }

        public static async Task<T> GetTokenByString(string tokenString)
        {
            var objectTypeName = typeof(T).Name;

            var res =
                Client.CreateDocumentQuery<Token>(Collection.DocumentsLink)
                    .Where(token => token.TokenString == tokenString && token.Type == objectTypeName)
                    .AsDocumentQuery();

            var batch = await res.ExecuteNextAsync<T>();
            return batch.FirstOrDefault();
        }

        public static async Task<T> GetAccessTokenByRefreshTokenString(string tokenString)
        {
            var objectTypeName = typeof(T).Name;

            var res =
                Client.CreateDocumentQuery<BearerToken>(Collection.DocumentsLink)
                    .Where(token => token.RefreshTokenString == tokenString && token.Type == objectTypeName)
                    .AsDocumentQuery();

            var batch = await res.ExecuteNextAsync<T>();
            return batch.FirstOrDefault();
        }

        public static async Task<T> GetCodeOrTokenByIds(Guid clientId, Guid userId)
        {
            var objectTypeName = typeof(T).Name;

            var res =
                Client.CreateDocumentQuery<AutorisatieCode>(Collection.DocumentsLink)
                    .Where(code => code.ClientId == clientId && code.UserId == userId && code.Type == objectTypeName)
                    .AsDocumentQuery();

            var batch = await res.ExecuteNextAsync<T>();
            return batch.FirstOrDefault();
        }

        public static Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public static async Task<Document> DeleteDocument(string id)
        {
            var doc = GetDocument(id);
            return await Client.DeleteDocumentAsync(doc.SelfLink);
        }


        // Only call to create a new DocumentCollection on Azure
        private static async Task CreateDocumentCollectionIfNotExists()
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    var collectionInfo = new DocumentCollection();
                    collectionInfo.Id = CollectionId;

                    // Enable TTL and update existing collection
                    collectionInfo.DefaultTimeToLive = -1;

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) {Precision = -1});

                    // Create a collection with 400 RU/s.
                    await Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        collectionInfo,
                        new RequestOptions {OfferThroughput = 400});
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task EnableTTL()
        {
            DocumentCollection collection =
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));

            // Enable TTL and update existing collection
            collection.DefaultTimeToLive = -1;
            await Client.ReplaceDocumentCollectionAsync(collection);
        }
    }
}