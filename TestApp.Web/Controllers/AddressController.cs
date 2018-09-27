using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TestApp.Business.Domain;
using TestApp.Business.Services;
using TestApp.Core.Configuration;
using TestApp.Core.Helpers;
using TestApp.Web.Filters;

namespace TestApp.Web.Controllers
{
    [Route("api/customers")]
    public class AddressController : ApiController
    {
        public AddressController(
            IOptions<BasicAuthenticationConfiguration> authConfig,
            IAddressService addressService) : base(authConfig)
        {
            AddressService = addressService;
        }

        private IAddressService AddressService { get; }

        /// <summary>
        /// Get customer addresses.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/customers/AA001/addresses
        ///
        /// </remarks>
        /// <param name="customerId">Customer id.</param>
        /// <response code="200">Collection of customer addresses.</response>
        [HttpGet("{customerId}/addresses")]
        [ProducesResponseType(200, Type = typeof(List<Address>))]
        public async Task<IActionResult> GetCustomerAddresses(string customerId)
        {
            //security would typically happen here, but as we have none - we just delegate execution directly to service layer.
            var result = await AddressService.GetCustomerAddressesAsync(customerId, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Get customer address of given type.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/customers/AA001/addresses/invoice
        ///
        /// </remarks>
        /// <param name="customerId">Customer id.</param>
        /// <param name="type">Address type.</param>
        /// <response code="200">Customer address of given type.</response>
        /// <response code="404">In case address does not exist.</response>
        [HttpGet("{customerId}/addresses/{type}")]
        [ProducesResponseType(200, Type = typeof(Address))]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> GetAddress(string customerId, AddressType type)
        {
            //security would typically happen here, but as we have none - we just delegate execution directly to service layer.
            var result = await AddressService.GetCustomerAddressAsync(customerId, type, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Create a new address of given type.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/customers/AA001/addresses
        ///     {
        ///         "street": "Foobar 21",
        ///         "country": "pl",
        ///         "addressType": "invoice"
        ///     }
        ///
        /// Note that country code must in ISO 3166 standard (2 letters).
        /// </remarks>
        /// <param name="customerId">Customer id.</param>
        /// <param name="delta">Address to create.</param>
        /// <response code="201">Newly created address.</response>
        /// <response code="409">In case address of given type already exists.</response>
        /// <response code="422">In case of validation error.</response>
        [HttpPost("{customerId}/addresses")]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(409, Type = typeof(ApiError))]
        [ProducesResponseType(422, Type = typeof(ApiError))]
        public async Task<IActionResult> CreateAddress(string customerId, [FromBody] Delta<Address> delta)
        {
            var result = await AddressService.CreateAddressAsync(customerId, delta, AuthenticatedUser);
            return CreatedAtAction(nameof(GetAddress), new { customerId = result.CustomerId, type = result.AddressType}, result);
        }

        /// <summary>
        /// Updates an existing address.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/customers/AA001/addresses/delivery
        ///     {
        ///         "country": "gb"
        ///     }
        ///
        /// Note that country code must in ISO 3166 standard (2 letters).
        /// You can provide only fields you want to update on given address.
        /// </remarks>
        /// <param name="customerId">Customer id.</param>
        /// <param name="addressType">Address type.</param>
        /// <param name="delta">Updated address fields.</param>
        /// <response code="201">Updated address.</response>
        /// <response code="404">In case address does not exist.</response>
        /// <response code="422">In case of validation error.</response>
        [HttpPut("{customerId}/addresses/{addressType}")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(422, Type = typeof(ApiError))]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> UpdateAddress(string customerId, AddressType addressType, [FromBody] Delta<Address> delta)
        {
            delta.SetValue(s => s.AddressType, addressType);
            var result = await AddressService.UpdateAddressAsync(customerId, delta, AuthenticatedUser);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing address of given type.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/customers/AA001/addresses/delivery
        ///
        /// </remarks>
        /// <param name="customerId">Customer id.</param>
        /// <param name="addressType">Address type.</param>
        /// <response code="204">Successfully deleted.</response>
        /// <response code="404">In case address does not exist.</response>
        [HttpDelete("{customerId}/addresses/{addressType}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(ApiError))]
        public async Task<IActionResult> DeleteCustomer(string customerId, AddressType addressType)
        {
            await AddressService.DeleteAddressAsync(customerId, addressType, AuthenticatedUser);
            return NoContent();
        }
    }
}