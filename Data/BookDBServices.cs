using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using dynamodb_sample.Models;

namespace dynamodb_sample.Data
{
    public class BookDBServices
    {
        IAmazonDynamoDB dynamoDBClient { get; set; }

        public BookDBServices(IAmazonDynamoDB dynamoDBClient)
        {
            this.dynamoDBClient = dynamoDBClient;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("Id", ScanOperator.NotEqual, 0);

            ScanOperationConfig soc = new ScanOperationConfig()
            {
                // AttributesToGet = new List { "Id", "Title", "ISBN", "Price" },
                Filter = scanFilter
            };
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            AsyncSearch<Book> search = context.FromScanAsync<Book>(soc, null);
            List<Book> documentList = new List<Book>();
            do
            {
                documentList = await search.GetNextSetAsync(default(System.Threading.CancellationToken));
            } while (!search.IsDone);

            return documentList;
        }

        public async Task<Book> GetBookAsync(string id)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            return await context.LoadAsync(new Book() {Id=id}, default(System.Threading.CancellationToken));
        }

        public async Task<Book> CreateBook(Book book)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            // Add a unique id for the primary key.
            book.Id = System.Guid.NewGuid().ToString();
            await context.SaveAsync(book, default(System.Threading.CancellationToken));
            return await context.LoadAsync(book, default(System.Threading.CancellationToken));
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.SaveAsync(book, default(System.Threading.CancellationToken));
            return await context.LoadAsync(book, default(System.Threading.CancellationToken));
        }

        public async Task DeleteBookAsync(Book book)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.DeleteAsync(book, default(System.Threading.CancellationToken));
        }
        
    }
}