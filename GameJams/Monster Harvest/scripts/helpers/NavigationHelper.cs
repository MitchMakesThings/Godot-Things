using Godot;

public partial class NavigationHelper : NavigationRegion3D
{
    public static NavigationHelper Instance { get; private set; }
    private bool isBuildingNavigation = false;
    private bool isRebuildQueued = false;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;

        BakeFinished += () =>
        {
            isBuildingNavigation = false;
            if (isRebuildQueued)
            {
                Rebuild();
                isRebuildQueued = false;
            }
        };
    }

    public void Rebuild()
    {
        if (isBuildingNavigation)
        {
            isRebuildQueued = true;
            return;
        }
        
        isBuildingNavigation = true;
        BakeNavigationMesh();
    }
}
