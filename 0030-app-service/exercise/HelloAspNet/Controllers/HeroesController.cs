using HelloAspNet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace HelloAspNet.Controllers
{
    [Route("api/heroes")]
    [ApiController]
    public class HeroesController : ControllerBase
    {
        private readonly IHeroesRepository repository;

        public HeroesController(IHeroesRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets all heroes from repository
        /// </summary>
        [HttpGet(Name = nameof(GetAllHeroes))]
        public ActionResult<IEnumerable<Hero>> GetAllHeroes() => Ok(repository.GetAll());

        /// <summary>
        /// Gets a hero by id
        /// </summary>
        /// <param name="id">ID of the hero to find</param>
        [HttpGet("{id}", Name = nameof(GetHeroById))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Hero))]
        public ActionResult<Hero> GetHeroById(int id)
        {
            if (!repository.TryGetById(id, out var hero)) return NotFound();
            return Ok(hero);
        }

        /// <summary>
        /// Add a new hero
        /// </summary>
        /// <param name="hero">Data of the new hero</param>
        [HttpPost(Name = nameof(AddHero))]
        [ProducesResponseType(typeof(Hero), (int)HttpStatusCode.Created)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public ActionResult<Hero> AddHero([FromBody] NewHero hero)
        {
            var newHero = repository.Add(hero);
            return CreatedAtRoute(nameof(GetHeroById), new { id = newHero.Id }, newHero);
        }

        /// <summary>
        /// Update an existing hero
        /// </summary>
        /// <param name="id">ID of the hero to update</param>
        /// <param name="patch">Fields to update</param>
        [HttpPatch("{id}", Name = nameof(PatchHero))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Hero))]
        public ActionResult<Hero> PatchHero(int id, [FromBody] HeroPatch patch)
        {
            if (!repository.TryPatch(id, patch, out var patchedHero)) return NotFound();
            return Ok(patchedHero);
        }

        /// <summary>
        /// Delete an existing hero
        /// </summary>
        /// <param name="id">ID of the hero to delete</param>
        [HttpDelete("{id}", Name = nameof(DeleteHeroById))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult DeleteHeroById(int id)
        {
            if (!repository.TryDeleteById(id)) return NotFound();
            return NoContent();
        }
    }
}
