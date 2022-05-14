using System.Collections.Generic;
using System.Linq;
using CyberUnderground.Entities;

namespace CyberUnderground.Core
{
    public class ObjectiveManager
    {
        private HashSet<FileEntity> filesToDelete = new HashSet<FileEntity>();
        private HashSet<FileEntity> filesToDownload = new HashSet<FileEntity>();

        public void FileDeleted(FileEntity file)
        {
            filesToDelete.Remove(file);

            if (!filesToDelete.Any())
            {
                // TODO check win conditions
            }
        }

        public void FileDownloaded(FileEntity file)
        {
            filesToDownload.Remove(file);

            if (!filesToDownload.Any())
            {
                // TODO check win conditions
            }
        }
    }
}