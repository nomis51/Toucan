namespace Menagerie.Core.Services.Abstractions;

public interface ITextParserService
{
    void ParseClientFileLine(string line);
    void ParseClipboardLine(string line);
}