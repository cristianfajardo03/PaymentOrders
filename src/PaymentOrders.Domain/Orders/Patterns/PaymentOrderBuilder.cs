using System.ComponentModel.DataAnnotations;

namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class PaymentOrderBuilder : IPaymentOrderBuilder
{
    private readonly OrderType _orderType;

    private Guid _id;
    private string? _sourceAccount;
    private string? _targetAccount;
    private decimal _amount;
    private string? _currency;
    private string? _swiftCode;
    private DateTimeOffset? _scheduledFor;
    private DateTimeOffset _createdAt;

    internal PaymentOrderBuilder(OrderType orderType)
    {
        _orderType = orderType;
    }

    public IPaymentOrderBuilder WithId(Guid id)
    {
        // El estudiante debe conservar el identificador de la orden para usarlo al construir el agregado.
        _id = id;
        return this;
    }

    public IPaymentOrderBuilder WithSourceAccount(string sourceAccount)
    {
        // El estudiante debe conservar la cuenta origen requerida por la orden.
        _sourceAccount = sourceAccount;
        return this;
    }

    public IPaymentOrderBuilder WithTargetAccount(string targetAccount)
    {
        // El estudiante debe conservar la cuenta destino requerida por la orden.
        _targetAccount = targetAccount;
        return this;
    }

    public IPaymentOrderBuilder WithAmount(decimal amount)
    {
        // El estudiante debe conservar el monto propuesto para la orden.
        _amount = amount;
        return this;
    }

    public IPaymentOrderBuilder WithCurrency(string currency)
    {
        // El estudiante debe conservar la moneda de la orden.
        _currency = currency;
        return this;
    }

    public IPaymentOrderBuilder WithSwiftCode(string? swiftCode)
    {
        // El estudiante debe conservar el SWIFT cuando el tipo de orden lo necesite.
        _swiftCode = swiftCode;
        return this;
    }

    public IPaymentOrderBuilder WithScheduledFor(DateTimeOffset? scheduledFor)
    {
        // El estudiante debe conservar la fecha de ejecución cuando el tipo de orden la necesite.
        _scheduledFor = scheduledFor;
        return this;
    }

    public IPaymentOrderBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        // El estudiante debe conservar la marca temporal de creación entregada por Application.
        _createdAt = createdAt;
        return this;
    }

    public PaymentOrder Build()
    {
        // El estudiante debe validar todos los campos obligatorios y sus reglas por tipo antes de crear el agregado configurado por la Factory.
        if (_id == Guid.Empty)
        {
            throw new InvalidOperationException("Id es requerido.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(_sourceAccount);
        ArgumentException.ThrowIfNullOrWhiteSpace(_targetAccount);
        ArgumentException.ThrowIfNullOrWhiteSpace(_currency);

        if (_createdAt == default)
        {
            throw new InvalidOperationException("Marca de tiempo de creacion obligatoria");
        }

        var draft = new PaymentOrderDraft(
            _id,
            _sourceAccount,
            _targetAccount,
            _amount,
            _currency,
            _swiftCode,
            _scheduledFor,
            _createdAt);

        return _orderType switch
        {
            OrderType.National => new NationalPaymentOrder(draft),
            OrderType.International => new InternationalPaymentOrder(draft),
            OrderType.Scheduled => new ScheduledPaymentOrder(draft),
            _ => throw new ArgumentOutOfRangeException(nameof(_orderType), _orderType, "Tipo de orden no manejado.")
        };
    }
}
