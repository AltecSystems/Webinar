using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Newtonsoft.Json;
using WebinarTelegram.PilotWrapper.Services;

namespace WebinarTelegram.TelegramBot.Services
{
    internal interface IObjectManager
    {
        void SetPersonToObject(Person person);
    }

    internal class ObjectManager : IObjectManager
    {
        private readonly IObjectModifier _modifier;
        private readonly IObjectsRepository _repository;
        private List<Person> _persons;
        private Guid _telegramObjId;

        public ObjectManager(IObjectsRepository repository, IObjectModifier modifier)
        {
            _repository = repository;
            _modifier = modifier;
        }

        public void SetPersonToObject(Person person)
        {
            var obj = GetTelegramObject();
            var json = obj.Attributes["json"].StrValue;

            if (string.IsNullOrWhiteSpace(json))
            {
                _persons ??= new List<Person>();
                _persons.Add(person);
            }
            else
            {
                _persons = JsonConvert.DeserializeObject<List<Person>>(json);
                if (_persons.All(s => s.PhoneNumber != person.PhoneNumber)) _persons.Add(person);
            }

            var serializedPersons = JsonConvert.SerializeObject(_persons);
            _modifier.EditById(_telegramObjId).SetAttribute("json", serializedPersons);
            _modifier.Apply();
        }

        private DObject GetTelegramObject()
        {
            if (_telegramObjId != Guid.Empty) return _repository.GetObjects(_telegramObjId);

            var type = _repository.GetType("telegram");
            var rootObj = _repository.GetObjects(new Guid("00000001-0001-0001-0001-000000000001"));
            var child = rootObj.Children.FirstOrDefault(z => z.TypeId == type.Id);
            var obj = child.ObjectId == Guid.Empty ? CreateTelegramObject() : _repository.GetObjects(child.ObjectId);
            _telegramObjId = obj.Id;
            return obj;
        }

        private DObject CreateTelegramObject()
        {
            var type = _repository.GetType("telegram");
            var rootObj = _repository.GetObjects(new Guid("00000001-0001-0001-0001-000000000001"));
            _telegramObjId = Guid.NewGuid();
            var obj = _modifier.CreateById(_telegramObjId, rootObj.Id, type).SetAttribute("json", "").DataObject;
            _modifier.Apply();
            return obj;
        }
    }
}