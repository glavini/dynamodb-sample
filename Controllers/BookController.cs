using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dynamodb_sample.Models;
using dynamodb_sample.Data;
using Amazon.DynamoDBv2;
using System;
using Microsoft.AspNetCore.Http;

namespace dynamodb_sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private IAmazonDynamoDB dynamoDBClient;

        public BookController(IAmazonDynamoDB dynamoDBClient)
        {
            //_context = context;
            this.dynamoDBClient = dynamoDBClient;
        }

        // GET: Books
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            BookDBServices service = new BookDBServices(dynamoDBClient);
            List<Book> books = await service.GetBooksAsync();
            return Ok(books);
        }

        // GET: Books/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Book>> GetBook([FromRoute] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BookDBServices service = new BookDBServices(dynamoDBClient);
            Book book = await service.GetBookAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // POST: Books
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Book>> Create([FromBody][Bind("Id,ISBN,InPublication,PageCount,Price,ProductCategory,Title")] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                BookDBServices service = new BookDBServices(dynamoDBClient);
                Book newBook = await service.CreateBook(book);
                return StatusCode(StatusCodes.Status201Created, book);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // PUT: Books/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]        
        public async Task<ActionResult<Book>> Update([FromRoute] string id, [FromBody][Bind("Id,ISBN,InPublication,PageCount,Price,ProductCategory,Title")] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id || id == null)
            {
                return BadRequest();
            }

            try
            {
                BookDBServices service = new BookDBServices(dynamoDBClient);
                Book auxBook = await service.GetBookAsync(id);

                if (auxBook == null)
                {
                    return NotFound();
                }
                Book newBook = await service.UpdateBookAsync(book);
                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE: Books/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Book>> Delete([FromRoute] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                BookDBServices service = new BookDBServices(dynamoDBClient);
                Book book = await service.GetBookAsync(id);

                if (book == null)
                {
                    return NotFound();
                }
                await service.DeleteBookAsync(book);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        
    }
}

