using Menagerie.Core.Services;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core;

public class ServicesDependencies
{
    public IFsService FsService { get; init; } = new FsService();
    public IGameProcessService GameProcessService { get; init; } = new GameProcessService();
    public IClientFileService ClientFileService { get; init; } = new ClientFileService();
    public ITextParserService TextParserService { get; init; } = new TextParserService();
    public ISettingsService SettingsService { get; init; } = new SettingsService();
    public IGameChatService GameChatService { get; init; } = new GameChatService();
}