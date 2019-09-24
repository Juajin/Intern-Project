using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HelperLibrary.DynamoDB
{
    public class DynamoDBHelper : IDynamoDBHelper
    {
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly DynamoDBContext _dynamoDBContext;
        private string tableName = "Anime";
        private static string token = string.Empty;
        private static string pseudoDeleteAttributeName = "Available";

        public DynamoDBHelper()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000/"
            };
            this.dynamoDBClient = new AmazonDynamoDBClient("fake", "fake", config);
            _dynamoDBContext = new DynamoDBContext(this.dynamoDBClient);

        }
        public void Save<T>(T item)
        {
            try
            {
                PropertyInfo[] propArray = item.GetType().GetProperties();
                var prop = propArray.First(a => a.CustomAttributes.Any(b => b.AttributeType.Name == "PseudoDeleteAttribute"));
                pseudoDeleteAttributeName = prop.Name;
                _dynamoDBContext.SaveAsync<T>(item);
            }
            catch
            {
            }

        }
        public void SaveList<T>(IEnumerable<T> items)
        {
            var itemBatch = _dynamoDBContext.CreateBatchWrite<T>();
            foreach (var item in items)
            {
                itemBatch.AddPutItem(item);
            }
            itemBatch.ExecuteAsync();
        }
        public List<T> GetPage<T>(string TableName, int pageSize, bool sort)
        {
            TableName = TableName != null ? TableName : tableName;
            //List<ScanCondition> cond = new List<ScanCondition>();
            //cond.Add(new ScanCondition("Name", ScanOperator.IsNotNull));
            Table animeTable = Table.LoadTable(dynamoDBClient, TableName);
            ScanOperationConfig config = new ScanOperationConfig();
            if (token != string.Empty)
            {
                config.Limit = pageSize;
                config.Select = SelectValues.AllAttributes;
                config.PaginationToken = token;
                
            }
            else
            {
                config = new ScanOperationConfig() { Limit = pageSize, Select = SelectValues.AllAttributes };
            }
            var search = animeTable.Scan(config);
            var items = search.GetNextSetAsync().Result;
            token = search.PaginationToken;
            List<T> result = _dynamoDBContext.FromDocuments<T>(items).ToList();
            return result;
        }
        public List<T> GetItem<T>(string filterType, string filterValue)
        {
            List<AttributeValue> eqListAvailable = new List<AttributeValue>();
            eqListAvailable.Add(new AttributeValue() { N = "1" });
            QueryFilter queryFilter = new QueryFilter();
            List<AttributeValue> eqList = new List<AttributeValue>();
            eqList.Add(new AttributeValue() { S = filterValue });
            queryFilter.AddCondition(filterType, new Condition()
            {
                ComparisonOperator = ComparisonOperator.EQ,
                AttributeValueList = eqList
            });
            queryFilter.AddCondition(pseudoDeleteAttributeName, new Condition()
            {
                AttributeValueList = eqListAvailable,
                ComparisonOperator = ComparisonOperator.EQ
            });

            QueryOperationConfig config = filterType == "Name" ? new QueryOperationConfig() { Filter = queryFilter} : new QueryOperationConfig() { IndexName = filterType, Filter = queryFilter };

            var list = _dynamoDBContext.FromQueryAsync<T>(config).GetRemainingAsync().Result;
            return list;
            //Easy way to query by primary(hash) key
            //IEnumerable<T> list = _dynamoDBContext.QueryAsync<T>(name).GetRemainingAsync().Result;
            //return list;
        }
        public List<T> GetItemByDate<T>(DateTime date1, DateTime date2)
        {
            List<T> list = new List<T>();
            string date01 = date1 == null ? DateTime.MinValue.Year.ToString() : "" + date1.Year;
            string date02 = date2 == null ? DateTime.MinValue.Year.ToString() : "" + date2.Year;
            if (date1 > date2)
            {
                string temp;
                temp = date01;
                date01 = date02;
                date02 = temp;
            }
            List<AttributeValue> eqListAvailable = new List<AttributeValue>();
            eqListAvailable.Add(new AttributeValue() { N = "1" });
            while (Int32.Parse(date01) < Int32.Parse(date02))
            {
                List<T> tempList = new List<T>();
                try
                {
                    QueryFilter queryFilter = new QueryFilter();
                    List<AttributeValue> eqListDates = new List<AttributeValue>();
                    eqListDates.Add(new AttributeValue() { S = date01 });
                    queryFilter.AddCondition("ReleaseDateYear", new Condition()
                    {
                        AttributeValueList = eqListDates,
                        ComparisonOperator = ComparisonOperator.EQ
                    });
                    queryFilter.AddCondition(pseudoDeleteAttributeName, new Condition()
                    {
                        AttributeValueList = eqListAvailable,
                        ComparisonOperator = ComparisonOperator.EQ
                    });
                    date01 = (Int32.Parse(date01) + 1).ToString();
                    QueryOperationConfig config = new QueryOperationConfig() { IndexName = "Date", Filter = queryFilter };
                    tempList = _dynamoDBContext.FromQueryAsync<T>(config).GetRemainingAsync().Result;
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
            //queryFilter.AddCondition("Available", new Condition() {
            //    AttributeValueList= eqListAvailable,
            //    ComparisonOperator=ComparisonOperator.EQ
            //});
            //queryFilter.AddCondition("ReleaseDate", new Condition()
            //{
            //    AttributeValueList = eqListDates,
            //    ComparisonOperator = ComparisonOperator.BETWEEN
            //});
            //
            //return list;
        }
        public void DeleteItem<T>(T item)
        {

            try
            {
                PropertyInfo[] propArray = item.GetType().GetProperties();
                var prop = propArray.First(a => a.CustomAttributes.Any(b => b.AttributeType.Name == "PseudoDeleteAttribute"));
                pseudoDeleteAttributeName = prop.Name;
                prop.SetValue(item, false);
                Save<T>(item);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        
        //REPOSSSSSSSS \\
        //public IEnumerable<T> GetItemByCategory<T>(string category)
        //{
        //    QueryFilter queryFilter = new QueryFilter();
        //    List<AttributeValue> eqList = new List<AttributeValue>();
        //    eqList.Add(new AttributeValue() { S = category });
        //    queryFilter.AddCondition("Category", new Condition()
        //    {
        //        ComparisonOperator = ComparisonOperator.EQ,
        //        AttributeValueList = eqList
        //    });
        //    QueryOperationConfig config = new QueryOperationConfig()
        //    {
        //        IndexName = "Category",
        //        Filter = queryFilter
        //    };

        //    IEnumerable<T> list = _dynamoDBContext.FromQueryAsync<T>(config).GetRemainingAsync().Result;
        //    return list;
        //}
        //public IEnumerable<T> GetItemByWriter<T>(string writer)
        //{
        //    QueryFilter queryFilter = new QueryFilter();
        //    List<AttributeValue> eqList = new List<AttributeValue>();
        //    eqList.Add(new AttributeValue() { S = writer });
        //    queryFilter.AddCondition("Writer", new Condition()
        //    {
        //        ComparisonOperator = ComparisonOperator.EQ,
        //        AttributeValueList = eqList
        //    });
        //    QueryOperationConfig config = new QueryOperationConfig()
        //    {
        //        IndexName = "Writer",
        //        Filter = queryFilter
        //    };

        //    IEnumerable<T> list = _dynamoDBContext.FromQueryAsync<T>(config).GetRemainingAsync().Result;
        //    return list;
        //}
        //public IEnumerable<T> GetItemByDate<T>(DateTime date1, DateTime date2)
        //{
        //    date1 = date1 < DateTime.MinValue ? DateTime.MinValue : date1;
        //    date2 = date2 < DateTime.MinValue ? DateTime.MinValue : date2;
        //    QueryFilter queryFilter = new QueryFilter();
        //    List<AttributeValue> eqList = new List<AttributeValue>();
        //    eqList.Add(new AttributeValue { S = date1.ToShortDateString() });
        //    eqList.Add(new AttributeValue { S = date2.ToShortDateString() });
        //    queryFilter.AddCondition("ReleaseDate", new Condition()
        //    {
        //        ComparisonOperator = ComparisonOperator.BETWEEN,
        //        AttributeValueList = eqList
        //    });
        //    QueryOperationConfig config = new QueryOperationConfig()
        //    {
        //        IndexName = "ReleaseDate",
        //        Filter=queryFilter
        //    };
        //    IEnumerable<T> list = _dynamoDBContext.FromQueryAsync<T>(config).GetRemainingAsync().Result;
        //    return list;
        //}
    }
}
