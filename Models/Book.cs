using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;
namespace dynamodb_sample.Models
{
    [DynamoDBTable("ProductTable")]
    public class Book
    {
        [DynamoDBHashKey]    //Partition key
        public string Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public double Price { get; set; }

        public string PageCount { get; set; }

        public string ProductCategory { get; set; }

        public bool InPublication { get; set; }
    }
}
