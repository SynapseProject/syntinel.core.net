using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syntinel.Core;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;


namespace Syntinel.Aws
{
    public class DynamoDbEngine : IDatabaseEngine
    {
        private const int defaultTimeout = 30000;

        private AmazonDynamoDBClient client;

        public DynamoDbEngine()
        {
            client = new AmazonDynamoDBClient();
        }

        public DynamoDbEngine(RegionEndpoint region)
        {
            client = new AmazonDynamoDBClient(region);
        }

        private string GetTableName(System.Type t)
        {
            string tableName = "";
            if (t == typeof(SignalDbRecord))
                tableName = "syntinel-signals";
            else if (t == typeof(ReporterDbRecord))
                tableName = "syntinel-reporters";

            return tableName;
        }

        public T Get<T>(string id)
        {
            T dbRecord = default(T);

            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);
            Task<Document> task = table.GetItemAsync(id);
            task.Wait(defaultTimeout);
            Document doc = task.Result;

            if (doc != null)
                dbRecord = JsonTools.Deserialize<T>(doc.ToJson());

            return dbRecord;
        }
    }
}
