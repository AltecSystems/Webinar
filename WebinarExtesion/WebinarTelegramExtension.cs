using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.DataModifier;
using Ascon.Pilot.ServerExtensions.SDK;
using Newtonsoft.Json;
using WebinarExtension.servext.Services;

namespace WebinarExtension.servext
{
    [Export(typeof(IServerActivity))]
    internal class SendTelegramNotification : IServerActivity
    {
        public string Name => "SendTelegramNotification";

        public Task RunAsync(IModifierBase modifier, IModifierBackend backend,
            IServerActivityContext serverActivityContext, IAutomationEventContext automationEventContext)
        {
            var type = backend.GetType("telegram");
            var rootObj = backend.GetObject(new Guid("00000001-0001-0001-0001-000000000001"));
            var child = rootObj.Children.FirstOrDefault(z => z.TypeId == type.Id);
            var obj = backend.GetObject(child.ObjectId);
            var json = obj.Attributes["json"].StrValue;
            var currentUsers = JsonConvert.DeserializeObject<List<Person>>(json);

            serverActivityContext.Params.TryGetValue("message", out var message);
            serverActivityContext.Params.TryGetValue("uri", out var uri);

            var resultMessage = message.ToString() + uri + automationEventContext.Source.Id;

            var executorIdToList = automationEventContext.Source.Attributes["executor"].Value;
            var executorId = ((IEnumerable<int>) executorIdToList).First();
            var peopleToMsg = backend.GetPersonOnPosition(executorId);

            new TelegramClient().SendMessage(peopleToMsg, currentUsers, resultMessage);

            return Task.CompletedTask;
        }
    }
}