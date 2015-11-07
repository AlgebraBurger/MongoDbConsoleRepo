using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("CustomerDb");
            var CustColl = db.GetCollection<Customer>("Customer");
            
            // query customer
            var customerID = new ObjectId("xxx");

            var customers = CustColl
                            .Find(c => c.Id == customerID)
                            .SortBy(c => c.fullName)
                            .Limit(3)
                            .ToListAsync()
                            .Result;

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.fullName);
            }

            //Update Customer
            var cust = customers.First();
            cust.fullName = cust.fullName.ToUpper();

            CustColl.SaveAsync(cust);


        }

        private static async Task<ReplaceOneResult> SaveAsync<T>(
            this IMongoCollection<T> collection, T entity) where T : IId
        {
            return await collection.ReplaceOneAsync(
                i => i.Id == entity.Id,
                entity, 
                new UpdateOptions { IsUpsert = true });            
        }

        public interface IId
        {
            ObjectId Id { get; }
        }
        public class Customer : IId
        {
            public ObjectId Id { get; set; }
            public string fullName { get; set; }

        }
    }
}
