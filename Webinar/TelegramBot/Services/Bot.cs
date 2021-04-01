using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebinarTelegram.TelegramBot.Services
{
    internal interface IBot
    {
        void Init();
    }

    internal class Bot : IBot
    {
        private readonly TelegramBotClient _botClient;
        private readonly IObjectManager _objectManager;

        private readonly ReplyKeyboardMarkup _replyKeyboardMarkup =
            new(new KeyboardButton[] {new() {Text = "Поделиться номером телефона", RequestContact = true}}, true, true);

        private readonly string _accessToken = "";

        public Bot(IObjectManager objectManager)
        {
            _botClient = new TelegramBotClient(_accessToken);

            _objectManager = objectManager;
        }

        public void Init()
        {
            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null && e.Message.Text == "/start")
                await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Привет!", replyMarkup: _replyKeyboardMarkup);
            if (e.Message.Type == MessageType.Contact)
            {
                _objectManager.SetPersonToObject(new Person(e.Message.Contact.PhoneNumber, e.Message.Chat.Id));
                await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Номер записан!");
            }
        }
    }
}