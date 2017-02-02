#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    var databaseName = "";
    var endpointUrl = "";
    var authorizationKey = "";
    var collectionName = "";

    using (var client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
    {
        var database = client.CreateDatabaseQuery().Where(db => db.Id == databaseName).AsEnumerable().FirstOrDefault();
        log.Info("1. Query for a database returned: {0}", database == null ? "no results" : database.Id);

        if (database == null)
        {

            database = await client.CreateDatabaseAsync(new Database { Id = databaseName });
            log.Info("\n2. Created Database: id - {0}", database.Id);

            var collection = await client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(databaseName),
                new DocumentCollection { Id = collectionName },
                new RequestOptions { OfferThroughput = 400 });

            log.Info("\n2.1. Created Collection \n{0}", simpleCollection);
        }
    }

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.first == null || data.last == null) {
        return req.CreateResponse(HttpStatusCode.OK, new
        {
            greeting = "Hello Nobody!"
        });
    }

    return req.CreateResponse(HttpStatusCode.OK, new {
        greeting = $"Hello {data.first} {data.last}!"
    });
}
