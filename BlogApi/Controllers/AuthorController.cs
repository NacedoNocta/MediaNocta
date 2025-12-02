using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("")]
public class AuthorController : ControllerBase
{
    private readonly ILogger<AuthorController> _logger;
    private readonly IAuthorService _authorService;

    public AuthorController(ILogger<AuthorController> logger, IAuthorService authorService)
    {
        _logger = logger;
        _authorService = authorService;
    }

    [HttpGet("Authors", Name = "GetAuthors")]
    public async Task<ActionResult<IEnumerable<IAuthor>>> GetAuthors()
    {
        try
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors");
            return StatusCode(500, "An error occurred while retrieving authors");
        }
    }

    [HttpGet("Authors/{id:guid}", Name = "GetAuthorById")]
    public async Task<ActionResult<IAuthor>> GetAuthorById(Guid id)
    {
        try
        {
            var author = await _authorService.GetAuthorByIdAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Author with ID {Id} not found", id);
                return NotFound($"Author with ID {id} not found");
            }

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving author with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the author");
        }
    }

    [HttpGet("Authors/name/{name}", Name = "GetAuthorByName")]
    public async Task<ActionResult<IAuthor>> GetAuthorByName(string name)
    {
        try
        {
            var author = await _authorService.GetAuthorByNameAsync(name);

            if (author == null)
            {
                _logger.LogWarning("Author with name {Name} not found", name);
                return NotFound($"Author with name {name} not found");
            }

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving author with name {Name}", name);
            return StatusCode(500, "An error occurred while retrieving the author");
        }
    }

    [HttpPost("Authors", Name = "CreateAuthor")]
    public async Task<ActionResult<IAuthor>> CreateAuthor([FromBody] Author author)
    {
        try
        {
            if (author == null)
            {
                return BadRequest("Author cannot be null");
            }

            var createdAuthor = await _authorService.CreateAuthorAsync(author);
            return CreatedAtRoute("GetAuthorById", new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author");
            return StatusCode(500, "An error occurred while creating the author");
        }
    }

    [HttpPut("Authors/{id:guid}", Name = "UpdateAuthor")]
    public async Task<ActionResult<IAuthor>> UpdateAuthor(Guid id, [FromBody] Author author)
    {
        try
        {
            if (author == null)
            {
                return BadRequest("Author cannot be null");
            }

            if (id != author.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedAuthor = await _authorService.UpdateAuthorAsync(author);

            if (updatedAuthor == null)
            {
                _logger.LogWarning("Author with ID {Id} not found for update", id);
                return NotFound($"Author with ID {id} not found");
            }

            return Ok(updatedAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the author");
        }
    }

    [HttpDelete("Authors/{id:guid}", Name = "DeleteAuthor")]
    public async Task<ActionResult> DeleteAuthor(Guid id)
    {
        try
        {
            var result = await _authorService.DeleteAuthorAsync(id);

            if (!result)
            {
                _logger.LogWarning("Author with ID {Id} not found for deletion", id);
                return NotFound($"Author with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the author");
        }
    }
}
