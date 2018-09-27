using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TestApp.Business.Domain;
using TestApp.Business.Services;
using TestApp.Core.Configuration;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Web.Filters;

namespace TestApp.Web.Controllers
{
    [Route("api/customers")]
    public class CustomersController : ApiController
    {
        public CustomersController(
            IOptions<BasicAuthenticationConfiguration> authConfig,
            ICustomerService customerService) : base(authConfig)
        {
            CustomerService = customerService;
        }

        private ICustomerService CustomerService { get; }

        /// <summary>
        /// Get customers by given query.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/customers?sortBy=name&amp;sortOrder=descending&amp;pageIndex=0&amp;pageSize=40
        ///
        /// </remarks>
        /// <param name="query">Query used for paging, filtering and sorting.</param>
        /// <response code="200">Paged collection of customers.</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedResource<Customer>))]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerQuery query)
        {
            //security would typically happen here, but as we have none - we just delegate execution directly to service layer.
            var result = await CustomerService.GetCustomersAsync(query, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Get customer by id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/customers/AA001
        ///
        /// </remarks>
        /// <param name="id">Id of the customer.</param>
        /// <response code="200">Customer with related addresses.</response>
        /// <response code="404">In case customer does not exist.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var result = await CustomerService.GetCustomerAsync(id, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Create a new customer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/customers
        ///     {
        ///         "name": "Joe Doe",
        ///         "country": "pl"
        ///     }
        ///
        /// Note that country code must in ISO 3166 standard (2 letters).
        /// </remarks>
        /// <param name="delta">Customer to create.</param>
        /// <response code="201">Newly created customer.</response>
        /// <response code="422">In case of validation error.</response>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(422, Type = typeof(ApiError))]
        public async Task<IActionResult> CreateCustomer([FromBody] Delta<Customer> delta)
        {
            var result = await CustomerService.CreateCustomerAsync(delta, AuthenticatedUser);
            return CreatedAtAction(nameof(GetCustomerById), new {result.Id}, result);
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/customers/AA001
        ///     {
        ///         "country": "gb"
        ///     }
        ///
        /// Note that country code must in ISO 3166 standard (2 letters).
        /// You can provide only fields you want to update on given customer.
        /// </remarks>
        /// <param name="id">Customer id.</param>
        /// <param name="delta">Updated customer fields.</param>
        /// <response code="201">Updated customer.</response>
        /// <response code="404">In case customer does not exist.</response>
        /// <response code="422">In case of validation error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(422, Type = typeof(ApiError))]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> UpdateCustomer(string id, [FromBody] Delta<Customer> delta)
        {
            var result = await CustomerService.UpdateCustomerAsync(id, delta, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing customer and related resources.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/customers/AA001
        ///
        /// </remarks>
        /// <param name="id">Customer id.</param>
        /// <response code="204">Successfully deleted.</response>
        /// <response code="404">In case customer does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await CustomerService.DeleteCustomerAsync(id, AuthenticatedUser);
            return NoContent();
        }
    }
}