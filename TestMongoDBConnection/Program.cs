using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace TestMongoDBConnection
{
    internal class Program
    {
        private const string Host = "localhost";
        private const string DatabaseName = "MaheshDataBase";
        private const string CloudDatabaseName = "sample_mflix";
        private const string UserName = "mahesh";
        private const string Password = "12345";
        private const int Port = 27017;
        private const string CollectionName = "Employees";
        private const string CloudCollectionName = "comments";
        private static readonly TimeSpan ConnectTimeout = new TimeSpan(0, 0, 5, 0);
        private readonly Random _rnd = new Random();
        // Replace the placeholder with your Atlas connection string
        private const string ConnectionUri = "mongodb+srv://maheshydfdfadavddsds604dfd84:ls9xZrLkzaMdfdfdObdfd3NV@cluster0.htdm1qo.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
        private static readonly StringBuilder Log = new StringBuilder();

        private static void Main()
        {
            TestConnectionString();
            var readCollectionDocuments = ReadCollectionDocuments();
            TestCloudConnectionString();
            var readCloudCollectionDocuments = ReadCloudCollectionDocuments();

            DumpLog(Log);

            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }

        private static void TestConnectionString()
        {
            Log.AppendLine();
            Log.AppendLine($"====== Test Date: {DateTime.Now.ToString(CultureInfo.InvariantCulture)} ======");

            try
            {
                var client = GetMongoClient();
                var resultPing = PingMongoDb(client);
                if (resultPing)
                {
                    var mongoServer = client.GetServer();
                    mongoServer.Connect();
                    var buildInfos = mongoServer.Instances.Select(x => x.BuildInfo).Where(w => w != null);
                    var versionStrings = buildInfos.Select(x => x.VersionString);
                    Log.AppendLine();
                    Log.AppendLine($"MongoDB version: {string.Join(" | ", versionStrings)}.");
                    Log.AppendLine("Successfully connected to mongoDb Server.");
                    Log.AppendLine();
                }
                else
                {
                    Log.AppendLine();
                    Log.AppendLine("Unable to ping mongoDb server.");
                }
            }
            catch (Exception exception)
            {
                Log.AppendLine($"{exception.Message}{exception.InnerException}{exception.StackTrace}");
            }
        }
        
        private static void TestCloudConnectionString()
        {
            Log.AppendLine();
            Log.AppendLine($"====== Test Date: {DateTime.Now.ToString(CultureInfo.InvariantCulture)} ======");

            try
            {
                var client = GetMongoCloudClient();
                var resultPing = PingMongoDb(client);
                if (resultPing)
                {
                    var mongoServer = client.GetServer();
                    mongoServer.Connect();
                    var buildInfos = mongoServer.Instances.Select(x => x.BuildInfo).Where(w => w != null);
                    var versionStrings = buildInfos.Select(x => x.VersionString);
                    Log.AppendLine();
                    Log.AppendLine($"Cloud mongoDB version: {string.Join(" | ", versionStrings)}.");
                    Log.AppendLine("Successfully connected to cloud mongoDb Server.");
                    Log.AppendLine();
                }
                else
                {
                    Log.AppendLine();
                    Log.AppendLine("Unable to ping mongoDb server.");
                }
            }
            catch (Exception exception)
            {
                Log.AppendLine($"{exception.Message}{exception.InnerException}{exception.StackTrace}");
            }
        }


        /// <summary>
        /// Create mongoDb client
        /// </summary>
        /// <returns></returns>
        private static MongoClient GetMongoClient()
        {
            // Creates a MongoClientSettings object
            var settings = new MongoClientSettings()
            {
                Server = new MongoServerAddress(Host, Port),
                ConnectTimeout = ConnectTimeout,
                Credential = MongoCredential.CreateCredential(DatabaseName, UserName, Password),
            };
            // Creates a new client and connects to the server
            return new MongoClient(settings);
        }
        
        /// <summary>
        /// Create mongoDb client
        /// </summary>
        /// <returns></returns>
        private static MongoClient GetMongoCloudClient()
        {
            // Creates a MongoClientSettings object
            var settings = MongoClientSettings.FromConnectionString(ConnectionUri);
            settings.ConnectTimeout = ConnectTimeout;
            // Creates a new client
            return new MongoClient(settings);
        }

        /// <summary>
        /// Get mongoDb Collection
        /// </summary>
        /// <returns></returns>
        private static IMongoCollection<BsonDocument> GetMongoDbCollection()
        {
            var client = GetMongoClient();
            var db = client.GetDatabase(DatabaseName);
            var collection = db.GetCollection<BsonDocument>(CollectionName);


            return collection;
        }
        
        /// <summary>
        /// Get mongoDb Collection
        /// </summary>
        /// <returns></returns>
        private static IMongoCollection<BsonDocument> GetMongoCloudDbCollection()
        {
            var client = GetMongoCloudClient();
            var db = client.GetDatabase(CloudDatabaseName);
            var collection = db.GetCollection<BsonDocument>(CloudCollectionName);


            return collection;
        }


        /// <summary>
        ///  Insert document in collection
        /// </summary>
        /// <param name="collection"></param>
        private void InsertOneInCollection(IMongoCollection<BsonDocument> collection)
        {
            var document = new BsonDocument
            {
                { "student_id", 10000 },
                {
                    "scores",
                    new BsonArray
                    {
                        new BsonDocument { { "type", "exam" }, { "score", _rnd.Next(1, 100) } },
                        new BsonDocument { { "type", "quiz" }, { "score", _rnd.Next(1, 100) } },
                        new BsonDocument { { "type", "homework" }, { "score", _rnd.Next(1, 100) } },
                        new BsonDocument { { "type", "homework" }, { "score", _rnd.Next(1, 100) } }
                    }
                },
                { "class_id", _rnd.Next(1, 500) }
            };
            collection.InsertOne(document);
        }

        /// <summary>
        /// Read documents from collection
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<BsonDocument> ReadCollectionDocuments()
        {
            var mongoDbCollection = GetMongoDbCollection();
            //InsertOneInCollection(mongoDbCollection);
            var filter = Builders<BsonDocument>.Filter.Empty;
            var doc = mongoDbCollection.Find(filter).ToList();
            return doc;
        }
        
        private static IEnumerable<BsonDocument> ReadCloudCollectionDocuments()
        {
            var mongoDbCollection = GetMongoCloudDbCollection();
            var filter = Builders<BsonDocument>.Filter.Empty;
            var doc = mongoDbCollection.Find(filter).ToList();
            return doc;
        }


        /// <summary>
        /// Ping mongoDb
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool PingMongoDb(IMongoClient client)
        {
            // Sends a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase(DatabaseName).RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            }
            catch (Exception ex)
            {
                Log.AppendLine();
                Log.AppendLine(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Dump logs into file
        /// </summary>
        /// <param name="log"></param>
        private static void DumpLog(StringBuilder log)
        {
            Console.WriteLine(log);

            var fileName = $"Support.Log.{DateTime.Now.ToString("yyyyMMdd.hhmmss")}.txt";

            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(log);
            }
        }
    }
}