using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.DataClasses;

namespace WebinarTelegram.PilotWrapper.Services
{
    public interface IObjectBuilder
    {
        DObject DataObject { get; }
        IObjectBuilder SetAttribute(string attributeName, string value);
    }

    internal class ObjectBuilder : IObjectBuilder
    {
        private readonly Dictionary<Guid, DChange> _dChanges = new();
        private readonly IObjectsRepository _repository;

        public ObjectBuilder(IObjectsRepository repository)
        {
            _repository = repository;
        }

        public DObject DataObject { get; set; }

        public IObjectBuilder SetAttribute(string attributeName, string value)
        {
            if (!_dChanges.ContainsKey(DataObject.Id)) return this;

            if (_dChanges[DataObject.Id].New.Attributes.ContainsKey(attributeName))
                _dChanges[DataObject.Id].New.Attributes.Remove(attributeName);

            _dChanges[DataObject.Id].New.Attributes.Add(attributeName, new DValue {StrValue = value});

            return this;
        }

        public DChangesetData GetDChangeSetData()
        {
            if (!_dChanges.Any()) return new DChangesetData();

            var dChangeSetData = new DChangesetData {Identity = Guid.NewGuid()};

            foreach (var change in _dChanges) dChangeSetData.Changes.Add(change.Value);
            _dChanges.Clear();

            return dChangeSetData;
        }

        public IObjectBuilder CreateObject(Guid objectGuid, Guid parentGuid, MType objectType)
        {
            DataObject = new DObject();
            DataObject.CreatorId = _repository.GetCurrentPerson().Id;
            DataObject.Created = DateTime.Now;
            DataObject.Id = objectGuid;
            DataObject.ParentId = parentGuid;
            DataObject.TypeId = objectType.Id;
            DataObject.Context.Add(DataObject.Id);
            DataObject.Context.Add(DObject.RootId);
            DataObject.Context.Add(DObject.GlobalRootId);

            var parentObjOld = _repository.GetObjects(DataObject.ParentId);
            var parentObjNew = parentObjOld.Clone();
            parentObjNew.Children.Add(new DChild {ObjectId = DataObject.Id, TypeId = DataObject.TypeId});
            var changedParent = new DChange();
            changedParent.Old = parentObjOld;
            changedParent.New = parentObjNew;

            ChangeEditObject(parentObjNew, parentObjOld);

            ChangeNewObject(DataObject);

            return this;
        }

        private void ChangeEditObject(DObject newObj, DObject oldObj)
        {
            if (!_dChanges.ContainsKey(newObj.Id))
            {
                var change = new DChange {Old = oldObj, New = newObj};
                _dChanges.Add(newObj.Id, change);
            }
            else
            {
                _dChanges[newObj.Id].New = newObj;
                _dChanges[newObj.Id].Old = oldObj;
            }
        }

        private void ChangeNewObject(DObject dObject)
        {
            if (_dChanges.ContainsKey(dObject.Id)) return;

            var change = new DChange {Old = null, New = dObject};
            _dChanges.Add(dObject.Id, change);
        }

        public IObjectBuilder EditObject(Guid objectId)
        {
            if (_dChanges.ContainsKey(objectId)) return this;

            DataObject = _repository.GetObjects(objectId);
            var newObj = DataObject.Clone();
            ChangeEditObject(newObj, DataObject);
            return this;
        }
    }
}