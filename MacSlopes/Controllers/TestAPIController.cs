using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MacSlopes.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestAPIController : ControllerBase
    {
        private readonly IBlogRepository _repository;

        public TestAPIController(IBlogRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var posts = _repository.GetPosts();
            if (posts == null)
            {
                return BadRequest(posts);
            }
            return Ok(posts);
        }

        [HttpGet("{Id}")]
        public IActionResult GetPost(string Id)
        {
            var post = _repository.GetPost(Id);
            if (post== null)
            {
                return BadRequest();
            }

            return Ok(post);
        }
    }
}