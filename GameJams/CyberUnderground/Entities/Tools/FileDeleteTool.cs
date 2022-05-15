using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class FileDeleteTool : Tool
    {
        protected override bool CanActivate(Entity target)
        {
            return base.CanActivate(target) && target is FileEntity;
        }

        protected override void ToolFinished()
        {
            var file = this.GetAttachmentTarget() as FileEntity;
            base.ToolFinished();
            
            if (file != null)
            {
                System.ObjectiveManager.FileDeleted(file);
                file.Delete();
            }
        }
    }
}
