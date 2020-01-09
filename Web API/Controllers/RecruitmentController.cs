using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentController : ControllerBase
    {
        public static List<Candidate> candidates = new List<Candidate> {
            new Candidate { Id = new Guid("18c46c62-3f33-4e6c-a2b2-49f7d9887051"), FirstName =  "John", LastName = "Smith" }
        };

        [NonAction]
        public void SetCandidates(List<Candidate> _candidates)
        {
            candidates.Clear();
            candidates.AddRange(_candidates);
        }

        // GET: api/Recruitment
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(candidates);
        }

        // GET: api/Recruitment/{{id}}
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id)
        {
            var candidate = candidates.Find(i => i.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }

        // POST: api/Recruitment
        [HttpPost]
        public IActionResult Post([FromBody] Candidate candidate)
        {
            candidate.Id = Guid.NewGuid();

            candidates.Add(candidate);

            return CreatedAtAction("Get", new { id = candidate.Id }, candidate);
        }

        // PUT: api/Recruitment/{{id}}
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Candidate updatedCandidate)
        {
            if (id != updatedCandidate.Id)
            {
                return BadRequest();
            }

            var index = candidates.FindIndex(i => i.Id == id);

            if (index == -1)
            {
                return NotFound();
            }

            candidates[index] = updatedCandidate;

            return NoContent();
        }

        // DELETE: api/Recruitment/{{id}}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var candidate = candidates.Find(i => i.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            var index = candidates.FindIndex(i => i.Id == id);

            candidates.RemoveAt(index);

            return Ok(candidate);
        }
    }
}