namespace SportDataImport.Domain.Enums;

public enum WeekType : byte
{
    Regular = 0,
    DoubleWeekFirst = 1,
    DoubleWeekSecond = 2,
}

public static class WeekTypeExtensions
{
    public static float ToFloat(this WeekType weekType)
    {
        return weekType switch
        {
            WeekType.Regular => 0f,
            WeekType.DoubleWeekFirst => 1f,
            WeekType.DoubleWeekSecond => 2f,
            _ => throw new ArgumentOutOfRangeException(nameof(weekType)),
        };
    }
}