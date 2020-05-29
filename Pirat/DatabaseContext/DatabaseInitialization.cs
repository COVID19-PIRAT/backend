using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;

namespace Pirat.DatabaseContext
{

    /// <summary>
    /// The related tables if an offer is inserted to the database
    /// </summary>
    public class OfferContext
    {
        [JsonProperty]
        public AddressEntity Address { get; set; }

        [JsonProperty]
        public OfferEntity Offer { get; set; }

        [JsonProperty]
        public List<ConsumableEntity> Consumables { get; set; }

        [JsonProperty]
        public List<DeviceEntity> Devices { get; set; }

        [JsonProperty]
        public List<PersonalEntity> Personals { get; set; }
    }

    public class OffersInitialization
    {
        [JsonProperty]
        public List<OfferContext> OfferContexts { get; set; }
    }

    public static class DatabaseInitialization
    {

        private static readonly string DatabaseInitializationFilesLocation =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Database");

        private static readonly string DatabaseTableInitializationFile =
            Path.Combine(DatabaseInitializationFilesLocation, "init.sql");

        /// <summary>
        /// Checks if connection to database can be established. Otherwise exception is thrown.
        /// </summary>
        public static void CheckConnectionToDatabase()
        {
            var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("PIRAT_CONNECTION"));
            connection.Open();
            connection.Dispose();
        }

        /// <summary>
        /// Create new tables in database if not exist so far. Table create commands are read from init.sql.
        /// </summary>
        public static void InitDatabaseTables()
        {
            CheckConnectionToDatabase();

            var commandsInput = File.ReadAllText(DatabaseTableInitializationFile, Encoding.UTF8);

            var commands = commandsInput.Split(';');

            var tablesBefore = FindExistingTables();
            Console.Out.WriteLine($"Tables in database:");
            foreach (var table in tablesBefore)
            {
                Console.Out.WriteLine($"{table}");
            }

            using (NpgsqlConnection connection =
                new NpgsqlConnection(Environment.GetEnvironmentVariable("PIRAT_CONNECTION")))
            {
                connection.Open();

                foreach (var command in commands)
                {
                    using NpgsqlCommand sqlCommand = new NpgsqlCommand(command + ";", connection);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            var tablesAfter = FindExistingTables();
            foreach (var table in tablesAfter.Except(tablesBefore))
            {
                Console.Out.WriteLine($"Created table: {table}");
            }
        }

        /// <summary>
        /// The database that is found by using the connection string is initialized with dummy data.
        /// Dummy data will only be inserted if associated tables are empty.
        /// </summary>
        public static async void InitDatabaseWithDummyData()
        {
            CheckConnectionToDatabase();

            DbContextOptions<ResourceContext> options =
            new DbContextOptionsBuilder<ResourceContext>().UseNpgsql(Environment.GetEnvironmentVariable("PIRAT_CONNECTION")).Options;

            await using var context = new ResourceContext(options);
            //Insert only dummy values if offer table empty
            if (!context.offer.Any())
            {
                var dummyData = Path.Combine(DatabaseInitializationFilesLocation, "init_dummy_offers.json");
                var content = File.ReadAllText(dummyData);
                var offerInit = JsonConvert.DeserializeObject<OffersInitialization>(content);
                foreach (var offerContext in offerInit.OfferContexts)
                {
                    var address = (AddressEntity) await offerContext.Address.InsertAsync(context);
                    offerContext.Offer.address_id = address.Id;
                    var offer = (OfferEntity) await offerContext.Offer.InsertAsync(context);

                    if (offerContext.Consumables != null)
                    {
                        foreach (var consumable in offerContext.Consumables)
                        {
                            consumable.offer_id = offer.id;
                            await consumable.InsertAsync(context);
                        }
                    }

                    if (offerContext.Devices != null)
                    {
                        foreach (var device in offerContext.Devices)
                        {
                            device.offer_id = offer.id;
                            await device.InsertAsync(context);
                        }
                    }

                    if (offerContext.Personals != null)
                    {
                        foreach (var personal in offerContext.Personals)
                        {
                            personal.offer_id = offer.id;
                            await personal.InsertAsync(context);
                        }
                    }
                }
            }
            //Insert other dummy data contexts here
        }

        private static List<string> FindExistingTables()
        {
            var existingTables = new List<string>();
            using (NpgsqlConnection connection =
                new NpgsqlConnection(Environment.GetEnvironmentVariable("PIRAT_CONNECTION")))
            {
                connection.Open();
                using NpgsqlCommand c = new NpgsqlCommand("SELECT table_name " +
                                                          "FROM information_schema.tables " +
                                                          "WHERE table_schema = 'public' " +
                                                          "AND table_type = 'BASE TABLE';", connection);

                using NpgsqlDataReader rdr = c.ExecuteReader();
                while (rdr.Read())
                {
                    existingTables.Add(rdr.GetString(0));
                }
            }

            return existingTables;
        }
    }
}
