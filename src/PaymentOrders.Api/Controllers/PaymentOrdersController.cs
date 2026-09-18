using Microsoft.AspNetCore.Mvc;
using PaymentOrders.Application.Contracts;
using PaymentOrders.Application.Services;
using PaymentOrders.Domain.Orders;

namespace PaymentOrders.Api.Controllers;

[ApiController]
[Route("api/payment-orders")]
public sealed class PaymentOrdersController(PaymentOrderService paymentOrderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentOrderResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatePaymentOrderResult>> Create(
        [FromBody] CreatePaymentOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            ModelState.AddModelError("Idempotency-Key", "The Idempotency-Key header is required.");
            return ValidationProblem(ModelState);
        }

        if (idempotencyKey.Length > 128)
        {
            ModelState.AddModelError("Idempotency-Key", "The Idempotency-Key header must contain at most 128 characters.");
            return ValidationProblem(ModelState);
        }

        var result = await paymentOrderService.CreateAsync(request, idempotencyKey, cancellationToken);
        if (result.IsIdempotentReplay)
        {
            return Ok(result);
        }

        return CreatedAtAction(nameof(GetById), new { orderId = result.OrderId }, result);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentOrderResponse>> GetById(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await paymentOrderService.GetAsync(orderId, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PaymentOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PaymentOrderResponse>>> List(
        [FromQuery] OrderType? orderType,
        [FromQuery] PaymentOrderStatus? status,
        [FromQuery] DateTimeOffset? createdFrom,
        [FromQuery] DateTimeOffset? createdTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            ModelState.AddModelError("pagination", "Page must be positive and pageSize must be between 1 and 100.");
            return ValidationProblem(ModelState);
        }

        var filter = new PaymentOrderFilter(orderType, status, createdFrom, createdTo, page, pageSize);
        return Ok(await paymentOrderService.ListAsync(filter, cancellationToken));
    }

    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var cancelled = await paymentOrderService.CancelAsync(orderId, cancellationToken);
        return cancelled ? NoContent() : NotFound();
    }

    [HttpPost("{orderId:guid}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Process(Guid orderId, CancellationToken cancellationToken)
    {
        var processed = await paymentOrderService.ProcessAsync(orderId, cancellationToken);
        return processed ? NoContent() : NotFound();
    }

    [HttpGet("{orderId:guid}/events")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentOrderEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PaymentOrderEventResponse>>> GetEvents(Guid orderId, CancellationToken cancellationToken)
    {
        var events = await paymentOrderService.GetEventsAsync(orderId, cancellationToken);
        return events is null ? NotFound() : Ok(events);
    }
}
