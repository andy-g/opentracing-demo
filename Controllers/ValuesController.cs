using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;

namespace opentracing_demo.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {
        private readonly ITracer _tracer;
        public ValuesController (ITracer tracer) {
            _tracer = tracer;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get () {
            using (var scope = _tracer.BuildSpan ("publish-order-created-event").StartActive (true)) {
                return new string[] { "value1", "value2" };
            }
        }

        // GET api/values/5
        [HttpGet ("{id}")]
        public ActionResult<string> Get (int id) {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post ([FromBody] string value) { }

        // PUT api/values/5
        [HttpPut ("{id}")]
        public void Put (int id, [FromBody] string value) { }

        // DELETE api/values/5
        [HttpDelete ("{id}")]
        public void Delete (int id) { }
    }
}