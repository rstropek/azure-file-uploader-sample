using HelloAspNet.Controllers;
using HelloAspNet.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HelloAspNet.Tests
{
    public class HeroesControllerTests
    {
        /// <summary>
        /// Verify that controller returns OK if hero was found in repository
        /// </summary>
        [Fact]
        public void GetHeroByIdSuccess()
        {
            // Setup mock object for repository
            var repository = new Mock<IHeroesRepository>();
            repository.Setup(r => r.TryGetById(42, out It.Ref<Hero>.IsAny)).Returns(true);

            // Create controller and call API
            var controller = new HeroesController(repository.Object);
            var result = controller.GetHeroById(42);

            // Make sure that result is OK and repository was called
            Assert.IsType<OkObjectResult>(result.Result);
            repository.VerifyAll();
        }

        /// <summary>
        /// Verify that controller returns NOT FOUND if hero was not found in repository
        /// </summary>
        [Fact]
        public void GetHeroByIdNotFound()
        {
            var repository = new Mock<IHeroesRepository>();
            repository.Setup(r => r.TryGetById(42, out It.Ref<Hero>.IsAny)).Returns(false);

            var controller = new HeroesController(repository.Object);
            var result = controller.GetHeroById(42);

            Assert.IsType<NotFoundResult>(result.Result);
            repository.VerifyAll();
        }
    }
}
