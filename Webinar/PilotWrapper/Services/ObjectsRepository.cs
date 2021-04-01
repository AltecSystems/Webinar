using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Microsoft.Extensions.Logging;

namespace WebinarTelegram.PilotWrapper.Services
{
    public interface IObjectsRepository
    {
        IEnumerable<DPerson> GetPeople();
        DPerson GetPerson(int id);
        DObject GetObjects(Guid id);

        MType GetType(string typeName);

        DPerson GetCurrentPerson();
    }

    public class ObjectsRepository : IObjectsRepository
    {
        private readonly IServerConnector _connector;
        private readonly ILogger<ObjectsRepository> _logger;

        public ObjectsRepository(ILogger<ObjectsRepository> logger, IServerConnector connector)
        {
            _logger = logger;
            _connector = connector;
        }

        public IEnumerable<DPerson> GetPeople()
        {
            _logger.LogInformation("Load people from database");
            return _connector.ServerApi.LoadPeople();
        }

        public DPerson GetPerson(int id)
        {
            _logger.LogInformation("Load people from database with id = {Id}", id);
            return _connector.ServerApi.LoadPeopleByIds(new[] {id}).FirstOrDefault();
        }

        public DObject GetObjects(Guid id)
        {
            return _connector.ServerApi.GetObjects(new[] {id}).FirstOrDefault();
        }

        public MType GetType(string typeName)
        {
            return _connector.ServerApi.GetMetadata(0).Types.FirstOrDefault(z => z.Name == typeName);
        }

        public DPerson GetCurrentPerson()
        {
            return _connector.GetDDatabaseInfo().Person;
        }
    }
}