using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Mapping;
using PaymentGateway.API.Models;
using PaymentGateway.Domain.Queries;

namespace PaymentGateway.API.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PaymentsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var command = _mapper.Map(request);
            var result = await _mediator.Send(command);

            var response = _mapper.Map(result);
            return new OkObjectResult(response);
        }

        [HttpGet("{paymentReference}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PaymentDetailsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayment(string paymentReference)
        {
            var query = new GetPaymentDetailsQuery(paymentReference);
            var result = await _mediator.Send(query);

            return result == null
                ? NotFound()
                : new OkObjectResult(_mapper.Map(result));
        }
    }
}
