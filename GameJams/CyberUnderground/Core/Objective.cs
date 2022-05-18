using CyberUnderground.Entities;

namespace CyberUnderground.Core
{
    public enum ObjectiveType
    {
        Delete,
        Download,
        Upload // TODO
    }
    
    public class Objective
    {
        public bool Complete { get; set; }
        public ObjectiveType Type { get; set; }
        public Entity Target { get; set; }
    }
}