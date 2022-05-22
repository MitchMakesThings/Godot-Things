using CyberUnderground.Entities.Annotations;
using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class AnalyzerTool : Tool
    {
        [Export]
        private PackedScene DeleteAnnotationScene;

        [Export]
        private PackedScene DownloadAnnotationScene;

        protected override bool CanActivate(Entity target)
        {
            return target is FileEntity;
        }

        protected override void ToolFinished()
        {
            var file = this.GetAttachmentTarget() as FileEntity;
            base.ToolFinished();

            if (System.ObjectiveManager.ShouldDeleteFile(file))
            {
                var annotation = DeleteAnnotationScene.Instance();
                file?.AddChild(annotation);
                
                System.AudioManager.PlayEffect(SuccessSound);
            }
            else if (System.ObjectiveManager.ShouldDownloadFile(file))
            {
                var annotation = DownloadAnnotationScene.Instance();
                file?.AddChild(annotation);
                
                System.AudioManager.PlayEffect(SuccessSound);
            }
            else
            {
                file?.LowerIntensity();
                
                System.AudioManager.PlayEffect(FailureSound);
            }
        }
    }
}