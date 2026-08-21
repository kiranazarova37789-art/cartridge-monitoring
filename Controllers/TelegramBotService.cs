using Folivora.Scaffold;
using Telegram.Bot;

public class TelegramBotService : BackgroundService
{
    private CartridgeDbContext _context;
    public TelegramBotService(CartridgeDbContext context, ITelegramBotClient botClient)
    {
        _context = context;
        _botClient = botClient;
    }
    private readonly ITelegramBotClient _botClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(-1, stoppingToken); // Держим бота работающим в фоне
    }
}
