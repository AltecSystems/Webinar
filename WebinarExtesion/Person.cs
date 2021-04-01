namespace WebinarExtension.servext
{
    internal class Person
    {
        public Person(string phoneNumber, long telegramChatId)
        {
            PhoneNumber = phoneNumber;
            TelegramChatId = telegramChatId;
        }

        public string PhoneNumber { get; set; }
        public long TelegramChatId { get; set; }
    }
}