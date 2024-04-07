using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Parsers.Abstractions;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace Menagerie.Core.Parsers;

public abstract class Parser<T> : IParser
    where T : class
{
    #region Members

    private readonly Regex _regexCanParse;
    private readonly List<Token<T>> _tokens;

    #endregion

    #region Constructors

    protected Parser(Regex canParse, List<Token<T>> tokens)
    {
        _regexCanParse = canParse;
        _tokens = tokens;
    }

    #endregion

    #region Public methods

    public virtual bool CanParse(string text)
    {
        return _regexCanParse.IsMatch(text);
    }

    public T? Parse(string text)
    {
        var returnValue = (T)Activator.CreateInstance(typeof(T))!;

        foreach (var token in _tokens)
        {
            var index = token.Value == string.Empty
                ? text.Length
                : text.IndexOf(token.Value, StringComparison.Ordinal);

            var usedAlternativeValue = false;

            if (index == -1 || index >= text.Length)
            {
                if (!token.EndOfString)
                {
                    if (!string.IsNullOrEmpty(token.AlternateValue))
                    {
                        usedAlternativeValue = true;
                        index = text.IndexOf(token.AlternateValue, StringComparison.Ordinal);

                        if (index == -1 || index >= text.Length)
                        {
                            if (!token.BreakOnFail) continue;
                            break;
                        }
                    }
                    else
                    {
                        if (!token.BreakOnFail) continue;
                        break;
                    }
                }
                else
                {
                    index = text.Length;
                }
            }

            var value = text[..index];
            var offset = usedAlternativeValue ? token.AlternateValue.Length : token.Value.Length;
            if (offset + index > text.Length)
            {
                offset = text.Length - index;
            }

            text = text[(index + offset)..];

            if (!token.IsUseful || token.TargetType is null || returnValue is null) continue;

            token.SetProperty?.Invoke(returnValue, value);
        }

        return returnValue;
    }

    #endregion
}