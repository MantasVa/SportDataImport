namespace SportDataImport.Enums;

public enum WeekType : byte
{
    Regular = 0,
    DoubleWeek1 = 1,
    DoubleWeek2 = 2,
}

internal static class WeekTypeExtensions
{
    public static float ToFloat(this WeekType weekType)
    {
        return weekType switch
        {
            WeekType.Regular => 0f,
            WeekType.DoubleWeek1 => 1f,
            WeekType.DoubleWeek2 => 2f,
            _ => throw new ArgumentOutOfRangeException(nameof(weekType)),
        };
    }
}