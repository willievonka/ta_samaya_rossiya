namespace Domain.Enums;

public enum LineStyle
{
    Solid = 0,
    Dashed = 1,
    Dotted = 2,
}

public static class LineStyleExtensions
{
    public static string ParseToString(this LineStyle lineStyle)
    {
        return lineStyle switch
        {
            LineStyle.Dashed => "dashed",
            LineStyle.Dotted => "dotted",
            LineStyle.Solid => "solid",
            _ => throw new ArgumentOutOfRangeException(nameof(lineStyle), lineStyle, null)
        };
    }

    public static LineStyle ParseToEnum(string str)
    {
        return str switch
        {
            "dashed" => LineStyle.Dashed,
            "dotted" => LineStyle.Dotted, 
            "solid" => LineStyle.Solid,
            _ => LineStyle.Solid
        };
    } 
}