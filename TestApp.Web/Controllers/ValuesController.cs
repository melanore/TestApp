using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.Data;
using TestApp.Data.Entities;
using TestApp.Data.Repositories;

namespace TestApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        public ValuesController(ITestAppUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        private ITestAppUnitOfWork UnitOfWork { get; }

        /// <summary>
        ///     Returns foobar.
        /// </summary>
        /// <remarks>
        ///     Here is a sample remarks placeholder.
        /// </remarks>
        /// <returns>foobar</returns>
        [HttpGet]
        public async Task<Customer> Get()
        {
            return await UnitOfWork.CustomerRepository.GetByIdAsync("123");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}