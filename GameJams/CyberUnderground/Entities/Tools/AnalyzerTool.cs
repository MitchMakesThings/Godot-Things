using CyberUnderground.Entities.Annotations;
using Godot;

namespace CyberUnderground.Entities.Tools
{
    public class AnalyzerTool : Tool
    {
        [Export]
        private PackedScene AnnotationScene;

        [Export]
        private Color _deleteTargetColor;

        [Export]
        private Color _downloadTargetColor;
        
        protected override bool CanActivate(Entity target)
        {
            return target is FileEntity;
        }
        
        protected override void ToolFinished()
        {
            var file = this.GetAttachmentTarget() as FileEntity;
            base.ToolFinished();
            
            // TODO
            if (System.ObjectiveManager.ShouldDeleteFile(file))
            {
                var annotation = AnnotationScene.Instance<Annotation>();
                file?.AddChild(annotation);
                annotation.SetColor(_deleteTargetColor);
            }

            if (System.ObjectiveManager.ShouldDownloadFile(file))
            {
                var annotation = AnnotationScene.Instance<Annotation>();
                file?.AddChild(annotation);
                annotation.SetColor(_downloadTargetColor);
            }
        }
    }
}
