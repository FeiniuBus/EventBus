using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventBus.Publish;
using EventBus.Sample.Events;

namespace EventBus.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IPublisher _publisher;

        public ValuesController(IPublisher publisher)
        {
            _publisher = publisher;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        //public Task PublishAsync(IDbTransic tran, Action<string> businessMethod)
        //{
        //    try
        //    {
                
        //        businessMethod.Invoke();

        //        var success = Publish(message);
        //        if (!success) throw new Exception("");
        //    } catch (Exception ex)
        //    {
        //        dbCont
        //    }

        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
