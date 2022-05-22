using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class FileDeleteTool : Tool
    {
        protected override bool CanActivate(Entity target)
        {
            return target is FileEntity || target is EntryPoint;;
        }

        protected override void ToolFinished()
        {
            var attachment = this.GetAttachmentTarget();
            var file = attachment as FileEntity;
            base.ToolFinished();
            
            if (file != null)
            {
                System.ObjectiveManager.FileDeleted(file);
                file.Delete();
                
                System.AudioManager.PlayEffect(SuccessSound);
            }

            if (attachment is EntryPoint)
            {
                System.Disconnect(true);
            }
        }
    }
}
