using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    // TODO actually make an extensible thing, so we could have quests/quest chains.
    // Hardcoded delete vs download should do enough for the game jam.
    public class ObjectiveManager
    {
        private readonly CoreSystem _system;
        private readonly EntityManager _entityManager;

        private readonly Dictionary<Entity, Objective> _objectives = new Dictionary<Entity, Objective>();

        public ObjectiveManager(CoreSystem system, EntityManager entityManager)
        {
            _system = system;
            _entityManager = entityManager;
        }

        public void GenerateRandomObjectives()
        {
            var rnd = new System.Random();
            var deleteObjective = new Objective()
            {
                Type = ObjectiveType.Delete
            };
            for (int i = 0; i < rnd.Next(5); i++)
            {
                var file = _entityManager.GetRandomEntity<FileEntity>(_objectives.Select(o => o.Key as FileEntity));
                if (file != null)
                {
                    _objectives.Add(file, deleteObjective);
                    deleteObjective.AddTarget(file);
                }
            }

            // TODO fold into loop above, looping over each objective type
            var downloadObjective = new Objective()
            {
                Type = ObjectiveType.Download
            };
            for (int i = 0; i < rnd.Next(3); i++)
            {
                var file = _entityManager.GetRandomEntity<FileEntity>(_objectives.Select(o => o.Key as FileEntity));
                if (file != null)
                {
                    _objectives.Add(file, downloadObjective);
                    downloadObjective.AddTarget(file);
                }
            }

            _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return _objectives.Values.Distinct();
        }

        public void Clear()
        {
            _objectives.Clear();
        }

        public void FileDeleted(FileEntity file)
        {
            if (!_objectives.ContainsKey(file))
            {
                return;
            }

            var objective = _objectives[file];
            if (objective.Type != ObjectiveType.Delete) return;
            
            objective.CompleteTarget(file);
            _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
        }

        public void FileDownloaded(FileEntity file)
        {
            if (!_objectives.ContainsKey(file))
            {
                return;
            }

            var objective = _objectives[file];
            if (objective.Type != ObjectiveType.Download) return;
            
            objective.CompleteTarget(file);
            _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
        }

        public bool ShouldDeleteFile(FileEntity file)
        {
            return _objectives.ContainsKey(file) && _objectives[file].Type == ObjectiveType.Delete;
        }

        public bool ShouldDownloadFile(FileEntity file)
        {
            return _objectives.ContainsKey(file) && _objectives[file].Type == ObjectiveType.Download;
        }
    }
}