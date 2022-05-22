using System;
using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities.Tools;

namespace CyberUnderground.Entities
{
    public class EntityManager
    {
        private HashSet<Entity> _entities = new HashSet<Entity>();
        private Random _rnd = new Random();
        
        public void Add(Entity e)
        {
            if (e is Tool) return; // Ignore tools
            _entities.Add(e);
        }

        public void Remove(Entity e)
        {
            _entities.Remove(e);
        }

        public void Clear()
        {
            _entities.Clear();
        }

        public T GetRandomEntity<T>() where T : class
        {
            return GetRandomEntity<T>(new T[]{});
        }

        public T GetRandomEntity<T>(IEnumerable<T> excluded) where T : class
        {
            var workingSet = _entities.Where(e => e is T t && !excluded.Contains(t));
            if (!workingSet.Any()) return default;
            
            return workingSet.ElementAt(_rnd.Next(workingSet.Count())) as T;
        }

        public Entity GetRandomEntity()
        {
            return GetRandomEntity(new Entity[]{});
        }
        public Entity GetRandomEntity(Entity exclude)
        {
            return GetRandomEntity(new[] { exclude });
        }

        public Entity GetRandomEntity(IEnumerable<Entity> excluded)
        {
            var workingSet = _entities.Except(excluded);
            if (!workingSet.Any()) return null;

            return workingSet.ElementAt(_rnd.Next(workingSet.Count()));
        }
    }
}