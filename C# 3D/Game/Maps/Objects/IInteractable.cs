using Godot;

public interface IInteractable {
    void Interact(Node caller);

    bool CanInteract(Node caller);

    string InteractionText { get; }
}