using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HelperLibrary.MongoDB
{
    public class MongoDBHelper : IMongoDBHelper
    {
        IMongoDatabase db;
        public string table;
        public static int count = 0;
        public MongoDBHelper(string database, string table)
        {
            this.table = table;
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void Save<T>(T item)
        {
            var collection = db.GetCollection<T>(table);

            var document = item.ToBsonDocument();
            var result = collection.ReplaceOne(
                new BsonDocument() {
                        {"Name", document.GetValue("Name").AsString },
                        {"ReleaseDate", document.GetValue("ReleaseDate").AsString }
                },
                item,
                new UpdateOptions { IsUpsert = true }
            );
            count = 0;
        }

        public void SaveList<T>(IEnumerable<T> items)
        {
            var collection = db.GetCollection<T>(table);
            try
            {
                foreach (var item in items)
                {
                    var document = item.ToBsonDocument();
                    var result = collection.ReplaceOne(
                        new BsonDocument() {
                        {"Name", document.GetValue("Name").AsString },
                        {"ReleaseDate", document.GetValue("ReleaseDate").AsString }
                        },
                        item,
                        new UpdateOptions { IsUpsert = true }
                    );
                }
            }
            catch
            {
            }
            count = 0;
        }

        public List<T> GetPage<T>(string TableName, int pageSize, bool sort)
        {

            var collection = db.GetCollection<T>(table);
            var anim = collection.Find(name => true).Sort(new BsonDocument("ReleaseDate", sort ? 1 : -1)).Skip(count).Limit(pageSize).ToList<T>();
            count += pageSize;
            return anim;
        }

        public List<T> GetItem<T>(string filterType, string filterValue)
        {
            var collection = db.GetCollection<T>(table);
            var list = collection.Find<T>(new BsonDocument(filterType, filterValue)).ToList<T>();
            return list;
        }

        public List<T> GetItemByDate<T>(DateTime date1, DateTime date2)
        {
            List<T> list = new List<T>();
            string date01 = date1 == null ? DateTime.MinValue.Year.ToString() : "" + date1.Year;
            string date02 = date2 == null ? DateTime.MinValue.Year.ToString() : "2017" /*+ date2.Year*/;
            if (date1 > date2)
            {
                string temp;
                temp = date01;
                date01 = date02;
                date02 = temp;
            }
            var collection = db.GetCollection<T>(table);
            while (Int32.Parse(date01) <= Int32.Parse(date02))
            {
                List<T> tempList = new List<T>();
                try
                {
                    tempList = collection.Find<T>(date01).ToList<T>();
                    date01 = (Int32.Parse(date01) + 1).ToString();
                }
                catch
                {
                    break;
                }
                foreach (var item in tempList)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public void DeleteItem<T>(T item)
        {
            Type ItemType = item.GetType();
            PropertyInfo[] propArray = ItemType.GetProperties();
            foreach (var prop in propArray)
            {
                try
                {
                    prop.CustomAttributes.First(a => a.AttributeType.Name == "PseudoDeleteAttribute");
                    prop.SetValue(item, false);
                    break;
                }
                catch
                {
                }
            }
            Save<T>(item);
        }
    }
}
