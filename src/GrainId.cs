namespace GrainFramework;

public readonly record struct GrainId(string Value)
{
    public override string ToString() => Value;

    public static GrainId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("GrainId cannot be null or empty.", nameof(value));

        return new GrainId(value);
    }
}
