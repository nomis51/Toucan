using System.Linq.Expressions;

namespace Menagerie.Core.Models.Parsing;

public class Token<T>
    where T : class
{
    public string Value { get; }
    public Action<T, string>? SetProperty { get; }
    public string AlternateValue { get; }
    public bool IsUseful { get; }
    public bool BreakOnFail { get; }
    public Type? TargetType { get; }
    public bool EndOfString { get; }

    public Token(string value, bool isUseful, Action<T, string>? setProperty = null, Type? targetType = null, string alternateValue = "",
        bool breakOnFail = true, bool endOfString = false)
    {
        Value = value;
        SetProperty = setProperty;
        AlternateValue = alternateValue;
        IsUseful = isUseful;
        BreakOnFail = breakOnFail;
        EndOfString = endOfString;

        if (IsUseful && targetType is null) throw new Exception("Invalid parsing token");
        TargetType = targetType;
    }
}