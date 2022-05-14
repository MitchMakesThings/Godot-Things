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
            (Target as FileEntity)?.Delete();
            
            base.ToolFinished();
        }
    }
}
