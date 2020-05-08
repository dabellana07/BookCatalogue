using AutoMapper;
using BookCatalogue.API.DTO;
using BookCatalogue.Contracts.Data.Repositories;
using BookCatalogue.Contracts.ElasticSearch.Services;
using BookCatalogue.ElasticSearch.Documents;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookCatalogue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookElasticService _bookElasticService;
        private readonly IMapper _mapper;

        public BookController(
            IBookRepository bookRepository,
            IBookElasticService bookElasticService,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _bookElasticService = bookElasticService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string name)
        {
            var bookDocuments = await _bookElasticService.SearchBooks(name);
            return Ok(bookDocuments.Select(d => _mapper.Map<BookDTO>(d)).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookEntities = await _bookRepository.GetAllAsync();
            var books = bookEntities.Select(b => _mapper.Map<BookDTO>(b)).ToList();
            return Ok(books);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var bookEntity = await _bookRepository.GetAsync(id);

            if (bookEntity == null)
                return NotFound();

            return Ok(_mapper.Map<BookDTO>(bookEntity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]BookDTO bookDTO)
        {
            var bookEntity = _mapper.Map<Models.Book>(bookDTO);

            _bookRepository.Add(bookEntity);
            await _bookRepository.SaveAsync();
            await _bookElasticService.AddBook(_mapper.Map<Book>(bookEntity));

            return CreatedAtAction(nameof(Get), new { id = bookEntity.Id },
                _mapper.Map<BookDTO>(bookEntity));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]BookDTO bookDTO)
        {
            if (bookDTO == null || id != bookDTO.Id)
                return BadRequest();

            var bookEntity = await _bookRepository.GetAsync(id);

            if (bookEntity == null)
                return NotFound();

            var updatedBook = _mapper.Map<Models.Book>(bookDTO);

            _bookRepository.Update(updatedBook);
            await _bookRepository.SaveAsync();
            await _bookElasticService.UpdateBook(_mapper.Map<Book>(updatedBook));

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bookEntity = await _bookRepository.GetAsync(id);

            if (bookEntity == null)
                return NotFound();

            _bookRepository.Remove(bookEntity);
            await _bookRepository.SaveAsync();
            await _bookElasticService.RemoveBook(id);

            return Ok();
        }
    }
}