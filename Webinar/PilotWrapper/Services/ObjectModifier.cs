using System;
using System.Collections.Concurrent;
using System.Linq;
using Ascon.Pilot.DataClasses;

namespace WebinarTelegram.PilotWrapper.Services
{
    public interface IObjectModifier
    {
        IObjectBuilder EditById(Guid objectId);
        void Apply();

        IObjectBuilder CreateById(Guid objectGuid, Guid parentGuid, MType objectType);
    }

    public class ObjectModifier : IObjectModifier
    {
        private readonly IServerConnector _connector;

        private readonly ConcurrentDictionary<Guid, ObjectBuilder> _dChanges = new();
        private readonly IObjectsRepository _repository;

        public ObjectModifier(IServerConnector connector, IObjectsRepository repository)
        {
            _connector = connector;
            _repository = repository;
        }

        public IObjectBuilder EditById(Guid objectGuid)
        {
            if (_dChanges.ContainsKey(objectGuid)) return _dChanges[objectGuid];

            var builder = new ObjectBuilder(_repository).EditObject(objectGuid);
            _dChanges.TryAdd(objectGuid, (ObjectBuilder) builder);
            return builder;
        }

        public void Apply()
        {
            foreach (var builder in _dChanges.Values)
            {
                var changeSetData = builder.GetDChangeSetData();

                if (!changeSetData.Changes.Any())
                    return;

                _connector.ServerApi.Change(changeSetData);
            }

            _dChanges.Clear();
        }

        public IObjectBuilder CreateById(Guid objectGuid, Guid parentGuid, MType objectType)
        {
            if (_dChanges.ContainsKey(objectGuid)) return _dChanges[objectGuid];

            var builder = new ObjectBuilder(_repository).CreateObject(objectGuid, parentGuid, objectType);
            _dChanges.TryAdd(objectGuid, (ObjectBuilder) builder);
            return builder;
        }
    }
}