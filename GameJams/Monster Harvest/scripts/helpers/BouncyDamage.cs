using Godot;
using System;

public partial class BouncyDamage : RigidBody3D
{
    [Export]
    private Label3D Label { get; set; } = null!;

    [Export]
    protected float LaunchStrength = 15;
    
    public void Launch(Vector3 globalPosition, int damage)
    {
        Label.Text = damage.ToString();
        GlobalPosition = globalPosition;

        var randomDirection = new Vector3(Random.Shared.NextSingle() * .4f, 1, Random.Shared.NextSingle() * .4f);
        ApplyImpulse(randomDirection * LaunchStrength);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(Label, "scale", Vector3.Zero, 1);
        tween.TweenCallback(Callable.From(ReturnToPool));
    }

    private void ReturnToPool()
    {
        // Reset state
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Label.Scale = Vector3.One;
        GlobalPosition = Vector3.Zero + (Vector3.Down * 100);

        DamageHelper.Instance.ReturnToPool(this);
    }

}
