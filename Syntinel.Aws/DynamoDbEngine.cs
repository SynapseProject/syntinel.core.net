using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
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
        private string signalsTable = "syntinel-signals";
        private string reportersTable = "syntinel-reporters";
        private string channelsTable = "syntinel-channels";
        private string routerTable = "syntinel-router";
        private string templateTable = "syntinel-templates";

        private AmazonDynamoDBClient client;

        public DynamoDbEngine()
        {
            client = new AmazonDynamoDBClient();
        }

        public DynamoDbEngine(string signals, string reporters, string channels, string router, string templates)
        {
            client = new AmazonDynamoDBClient();
            this.signalsTable = signals;
            this.reportersTable = reporters;
            this.channelsTable = channels;
            this.routerTable = router;
            this.templateTable = templates;
        }

        private string GetTableName(System.Type t)
        {
            string tableName = "";
            if (t == typeof(SignalDbRecord))
                tableName = signalsTable;
            else if (t == typeof(ReporterDbRecord))
                tableName = reportersTable;
            else if (t == typeof(ChannelDbRecord))
                tableName = channelsTable;
            else if (t == typeof(RouterDbRecord))
                tableName = routerTable;
            else if (t == typeof(TemplateDbRecord))
                tableName = templateTable;

            return tableName;
        }

        private Document ConvertToDocument(object obj)
        {
            Document doc = Document.FromJson(JsonTools.Serialize(obj));
            return doc;
        }

        public T Get<T>(string id)
        {
            string[] ids = new string[1];
            ids[0] = id;
            return Get<T>(ids);
        }

        public T Get<T>(string[] ids)
        {
            T dbRecord = default(T);

            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);
            Task<Document> task;
            if (ids.Length <= 1)
                task = table.GetItemAsync(ids[0]);
            else
                task = table.GetItemAsync(ids[0], ids[1]);

            task.Wait(defaultTimeout);
            Document doc = task.Result;

            if (doc != null)
                dbRecord = JsonTools.Deserialize<T>(doc.ToJson());

            return dbRecord;
        }

        public List<T> Export<T>()
        {
            List<T> records = new List<T>();

            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);

            ScanFilter filter = new ScanFilter();
            Search search = table.Scan(filter);
            List<Document> documents = new List<Document>();
            Task<List<Document>> task;
            do
            {
                task = search.GetNextSetAsync();
                task.Wait(defaultTimeout);
                documents = task.Result;
                foreach (Document document in documents)
                {
                    records.Add(JsonTools.Deserialize<T>(document.ToJson()));
                }

            } while (!search.IsDone);


            return records;
        }

        public void Import<T>(List<T> records)
        {
            foreach (T record in records)
                Create<T>(record);
        }

        public T Create<T>(T record, bool failIfExists = false)
        {
            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);

            PutItemOperationConfig config = new PutItemOperationConfig();

            if (failIfExists)
            {
                config.ConditionalExpression = new Expression();
                foreach (string hash in table.HashKeys)
                {
                    config.ConditionalExpression.ExpressionAttributeNames.Add("#" + hash, hash);
                    string statement = "(attribute_not_exists(#" + hash + "))";

                    if (String.IsNullOrWhiteSpace(config.ConditionalExpression.ExpressionStatement))
                        config.ConditionalExpression.ExpressionStatement = statement;
                    else
                        config.ConditionalExpression.ExpressionStatement = " and " + statement;
                }
            }

            Document doc = ConvertToDocument(record);
            Task<Document> task = table.PutItemAsync(doc, config);
            task.Wait(defaultTimeout);

            return record;
        }

        public T Update<T>(T record, bool failIfMissing = false)
        {
            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);

            PutItemOperationConfig config = new PutItemOperationConfig();

            if (failIfMissing)
            {
                config.ConditionalExpression = new Expression();
                foreach (string hash in table.HashKeys)
                {
                    config.ConditionalExpression.ExpressionAttributeNames.Add("#" + hash, hash);
                    string statement = "(attribute_exists(#" + hash + "))";

                    if (String.IsNullOrWhiteSpace(config.ConditionalExpression.ExpressionStatement))
                        config.ConditionalExpression.ExpressionStatement = statement;
                    else
                        config.ConditionalExpression.ExpressionStatement = " and " + statement;
                }
            }

            Document doc = ConvertToDocument(record);
            Task<Document> task = table.PutItemAsync(doc, config);
            task.Wait(defaultTimeout);

            return record;
        }

        public void Delete<T>(string id, bool failIfMissing = false)
        {
            string[] ids = new string[1];
            ids[0] = id;
            Delete<T>(ids, failIfMissing);
        }

        public void Delete<T>(string[] ids, bool failIfMissing = false)
        {
            string tableName = GetTableName(typeof(T));
            Table table = Table.LoadTable(client, tableName);

            DeleteItemOperationConfig config = new DeleteItemOperationConfig();
            if (failIfMissing)
            {
                config.ConditionalExpression = new Expression();
                foreach (string hash in table.HashKeys)
                {
                    config.ConditionalExpression.ExpressionAttributeNames.Add("#" + hash, hash);
                    string statement = "(attribute_exists(#" + hash + "))";

                    if (String.IsNullOrWhiteSpace(config.ConditionalExpression.ExpressionStatement))
                        config.ConditionalExpression.ExpressionStatement = statement;
                    else
                        config.ConditionalExpression.ExpressionStatement = " and " + statement;
                }
            }

            Task<Document> task;
            if (ids.Length <= 1)
                task = table.DeleteItemAsync(ids[0], config);
            else
                task = table.DeleteItemAsync(ids[0], ids[1], config);

            task.Wait(defaultTimeout);

        }
    }
}
