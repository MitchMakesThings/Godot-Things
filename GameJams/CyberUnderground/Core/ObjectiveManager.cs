using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public class ObjectiveManager
    {
        private readonly CoreSystem _system;
        private readonly EntityManager _entityManager;
        private HashSet<FileEntity> filesToDelete = new HashSet<FileEntity>();
        private HashSet<FileEntity> filesToDownload = new HashSet<FileEntity>();

        public ObjectiveManager(CoreSystem system, EntityManager entityManager)
        {
            _system = system;
            _entityManager = entityManager;
        }

        public void GenerateRandomObjectives()
        {
            var file = _entityManager.GetRandomEntity<FileEntity>();
            if (file == null) return;
            filesToDelete.Add(file);
            
            var file2 = _entityManager.GetRandomEntity<FileEntity>(new [] {file});
            if (file2 == null) return;
            filesToDownload.Add(file2);
            
            _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
        }

        public IEnumerable<string> GetObjectives()
        {
            var objectives = new List<string>();

            if (filesToDownload.Any())
            {
                objectives.Add($"Download {filesToDownload.Count()} pieces of incriminating evidence.");
            }

            if (filesToDelete.Any())
            {
                objectives.Add($"Delete {filesToDelete.Count()} log files");
            }

            return objectives;
        }
        
        public void FileDeleted(FileEntity file)
        {
            filesToDelete.Remove(file);

            if (!filesToDelete.Any())
            {
                // TODO check win conditions
                GD.Print("You've deleted them all!");
                _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
            }
        }

        public void FileDownloaded(FileEntity file)
        {
            filesToDownload.Remove(file);

            if (!filesToDownload.Any())
            {
                // TODO check win conditions
                GD.Print("You've downloaded them all!");
                _system.EmitSignal(nameof(CoreSystem.OnObjectivesUpdated));
            }
        }

        public bool ShouldDeleteFile(FileEntity file)
        {
            return filesToDelete.Contains(file);
        }

        public bool ShouldDownloadFile(FileEntity file)
        {
            return filesToDownload.Contains(file);
        }
    }
}