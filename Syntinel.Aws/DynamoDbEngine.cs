﻿using System;
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

        private Document ConvertToDocument(object obj)
        {
            Document doc = Document.FromJson(JsonTools.Serialize(obj));
            return doc;
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
    }
}
