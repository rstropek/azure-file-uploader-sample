using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace HelloAspNet.Services
{
    public record NewHero([Required][MinLength(1)] string Name, bool CanFly);
    public record Hero(int Id, string Name, bool CanFly) : NewHero(Name, CanFly);
    public record HeroPatch([MinLengthIfNotNull(1)] string? Name, bool? CanFly);

    public interface IHeroesRepository
    {
        IEnumerable<Hero> GetAll();

        bool TryGetById(int id, [NotNullWhen(true)] out Hero? hero);

        Hero Add(NewHero h);

        bool TryPatch(int id, HeroPatch patch, out Hero? hero);

        bool TryDeleteById(int id);
    }

    public class HeroesRepository : IHeroesRepository
    {
        private readonly ConcurrentDictionary<int, Hero> Heroes = new();
        private readonly ILogger<HeroesRepository> logger;
        private int NextHeroId = 0;

        public HeroesRepository(ILogger<HeroesRepository> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Hero> GetAll() => Heroes.Values.ToArray();

        public bool TryGetById(int id, [NotNullWhen(true)] out Hero? hero)
        {
            var result = Heroes.TryGetValue(id, out hero);
            if (!result) logger.LogWarning("Search for hero with ID {HeroId} failed.", id);
            return result;
        }

        public Hero Add(NewHero h)
        {
            var id = Interlocked.Increment(ref NextHeroId);
            var newHero = new Hero(id, h.Name, h.CanFly);
            Heroes[id] = newHero;

            logger.LogTrace("Added new hero {HeroId}.", id);
            return newHero;
        }

        public bool TryPatch(int id, HeroPatch patch, out Hero? hero)
        {
            // Note: Patching is not done in a "transaction". Last one wins.

            if (!TryGetById(id, out hero))
            {
                logger.LogWarning("Patching of hero with ID {HeroId} failed because ID was not found.", id);
                return false;
            }

            if (patch.Name != null) hero = hero with { Name = patch.Name };
            if (patch.CanFly.HasValue) hero = hero with { CanFly = patch.CanFly.Value };

            Heroes[id] = hero;
            return true;
        }

        public bool TryDeleteById(int id)
        {
            var result = Heroes.TryRemove(id, out var _);
            if (!result) logger.LogWarning("Removing hero with ID {HeroId} failed because ID was not found.", id);
            return result;
        }
    }
}
