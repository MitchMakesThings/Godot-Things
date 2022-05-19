using System;

namespace CyberUnderground.Entities.Tools
{
    public class DownloadTool : Tool
    {
        private Random _rnd = new Random();
        protected override bool CanActivate(Entity target)
        {
            return target is FileEntity;
        }

        protected override void ActivateTool(Entity target)
        {
            ActivationTime = _rnd.Next(4, 8);
            
            base.ActivateTool(target);
        }

        protected override void ToolFinished()
        {
            System.ObjectiveManager.FileDownloaded(this.GetAttachmentTarget() as FileEntity);
            base.ToolFinished();
        }
    }
}
