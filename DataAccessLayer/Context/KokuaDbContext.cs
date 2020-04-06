using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class KokuaDbContext : IKokuaDbContext
    {
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly IOptions<DbContextSettings> _options;

        public KokuaDbContext(IOptions<DbContextSettings> options)
        {
            _options = options;
            // Every command will be stored and it'll be processed at SaveChanges
            _commands = new List<Func<Task>>();
        }



        public async Task<int> SaveChanges()
        {
            ConfigureMongo();

            using (Session = await MongoClient.StartSessionAsync())
            {
                Session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync();
            }

            return _commands.Count;
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
                return;

            // Configure mongo (You can inject the config, just to simplify)
            MongoClient = new MongoClient("mongodb+srv://kadirogreten:89892dbc@kokuacluster-9i0hi.azure.mongodb.net/KokuaDb?retryWrites=true&w=majority");

            Database = MongoClient.GetDatabase("KokuaDb");

        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            ConfigureMongo();
            return Database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
    }

}
