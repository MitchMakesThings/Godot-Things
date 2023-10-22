namespace BloodHarvest.scripts.interfaces;

public interface IInteractable
{
    string Title { get; set; }
    string Subtitle { get; }

    void Interact(InteractionSystem interactionSystem);
}