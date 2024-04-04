namespace Menagerie.Models.IncomingTradeViewModels;

public class TooltipLine
{
    public string Label { get; set; }
    public string Value { get; set; }

    public TooltipLine(string label, string value)
    {
        Label = label;
        Value = value;
    }
}