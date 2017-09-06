using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventBus.Publish;
using EventBus.Sample.Events;
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

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
