using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities;
using Godot;

namespace CyberUnderground.Core
{
    public class ObjectiveManager
    {
        private readonly EntityManager _entityManager;
        private HashSet<FileEntity> filesToDelete = new HashSet<FileEntity>();
        private HashSet<FileEntity> filesToDownload = new HashSet<FileEntity>();

        public ObjectiveManager(EntityManager entityManager)
        {
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
        }
        
        public void FileDeleted(FileEntity file)
        {
            filesToDelete.Remove(file);

            if (!filesToDelete.Any())
            {
                // TODO check win conditions
                GD.Print("You've deleted them all!");
            }
        }

        public void FileDownloaded(FileEntity file)
        {
            filesToDownload.Remove(file);

            if (!filesToDownload.Any())
            {
                // TODO check win conditions
                GD.Print("You've downloaded them all!");
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