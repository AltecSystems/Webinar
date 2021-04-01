using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Telegram.Bot;
using WebinarExtension.servext.Extensions;

namespace WebinarExtension.servext.Services
{
    internal class TelegramClient
    {
        private readonly string _accessToken = "1614942357:AAGTIPtppMjgKHYWPdoMlfQ0M-YPs8ZIdjQ";

        public void SendMessage(INPerson person, List<Person> currentUsers, string message)
        {
            var botClient = new TelegramBotClient(_accessToken);
            botClient.SendTextMessageAsync(
                currentUsers.First(x => x.PhoneNumber.PhoneNumberDigits() == person.Phone.PhoneNumberDigits())
                    .TelegramChatId,
                message);
        }
    }
}