using Amazon.DynamoDBv2.DataModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeeforFreedom.Models
{
    [DynamoDBTable("Anime")]
    public class Anime : Content
    {
        [DynamoDBHashKey]
        public override string Name { get; set; }
        [DynamoDBRangeKey]
        public override string ReleaseDate { get; set; }
        [DynamoDBGlobalSecondaryIndexHashKey]
        public override string Category { get; set; }
        [DynamoDBGlobalSecondaryIndexHashKey]
        public override string Writer { get; set; }
        [BsonId]
        public Guid Id;
        [PseudoDeleteAttribute]
        public bool Available { get; set; }
        [DynamoDBGlobalSecondaryIndexHashKey]
        public string ReleaseDateYear { get; set; }
    }


}