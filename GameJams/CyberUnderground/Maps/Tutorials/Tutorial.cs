namespace CyberUnderground.Maps.Tutorials
{
    public class Tutorial : Level
    {

        public override void _Ready()
        {
            base._Ready();
            ObjectiveManager.GenerateRandomObjectives();
        }

    }
}
