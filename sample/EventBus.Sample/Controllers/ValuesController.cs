using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventBus.Core;

namespace EventBus.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly SampleDbContext _sampleDbContext;

        public ValuesController(IEventPublisher eventPublisher, SampleDbContext sampleDbContext)
        {
            _eventPublisher = eventPublisher;
            _sampleDbContext = sampleDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            using (var transaction = await _sampleDbContext.Database.BeginTransactionAsync())
            {
                await _eventPublisher.PrepareAsync("eventbus.testtopic", new { value }, new { signature = "" });
                transaction.Commit();
            }
            await _eventPublisher.ConfirmAsync();

            return Ok();
        }
    }
}
