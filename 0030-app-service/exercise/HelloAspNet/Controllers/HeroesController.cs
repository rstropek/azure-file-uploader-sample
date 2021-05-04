using HelloAspNet.Services;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Hero))]
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
        [ProducesResponseType(typeof(Hero), StatusCodes.Status201Created)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Hero))]
        public ActionResult<Hero> PatchHero(int id, [FromBody] HeroPatch patch)
        {
            if (!repository.TryPatch(id, patch, out var patchedHero)) return NotFound();
            return Ok(patchedHero);
        }

        /// <summary>
        /// Update existing heroes
        /// </summary>
        /// <param name="patches">Patches</param>
        [HttpPatch(Name = nameof(PatchHeroes))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Hero>))]
        public ActionResult<IEnumerable<Hero>> PatchHeroes([FromBody] IEnumerable<HeroWithIdPatch> patches)
        {
            repository.Patch(patches, out var patchedHeroes);
            return Ok(patchedHeroes);
        }

        /// <summary>
        /// Delete an existing hero
        /// </summary>
        /// <param name="id">ID of the hero to delete</param>
        [HttpDelete("{id}", Name = nameof(DeleteHeroById))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteHeroById(int id)
        {
            if (!repository.TryDeleteById(id)) return NotFound();
            return NoContent();
        }
    }
}
