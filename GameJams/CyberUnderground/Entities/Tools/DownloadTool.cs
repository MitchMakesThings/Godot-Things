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
            System.ObjectiveManager.FileDownloaded(this.GetAttachmentTarget() as FileEntity);
            base.ToolFinished();
        }
    }
}
