namespace Ordering.Domain.ValueObjects;

public record OrderName
{
    private const int DetfaultLength = 5;
    public string Value {  get; }
    private OrderName(string value) => Value = value;

    public static OrderName Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DetfaultLength);

        return new OrderName(value);
    }
}
