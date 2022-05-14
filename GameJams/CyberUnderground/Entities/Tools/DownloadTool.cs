namespace CyberUnderground.Entities.Tools
{
    public class DownloadTool : Tool
    {
        protected override bool CanActivate(Entity target)
        {
            return target is FileEntity;
        }

        protected override void ToolFinished()
        {
            base.ToolFinished();
            
            System.ObjectiveManager.FileDownloaded(AttachmentTarget as FileEntity);
        }
    }
}
