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
            var file = this.GetAttachmentTarget() as FileEntity;
            base.ToolFinished();
            
            if (file != null)
            {
                System.ObjectiveManager.FileDeleted(file);
                file.Delete();
            }

            if (this.GetAttachmentTarget() is EntryPoint)
            {
                // TODO back out a step, we've aborted / finished this run!
            }
        }
    }
}
