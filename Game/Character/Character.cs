using Godot;
using System;

public class Character : KinematicBody
{
	[Export]
	NodePath HeadNodePath;
	Spatial Head;

	[Export]
	NodePath CameraNodePath;
	Camera Camera;

	const float MouseSensitivity = 0.05f;

	#region Movement
	private Vector3 Velocity;
	private Vector3 Direction;
	
	[Export]
	private float JumpSpeed = 18.0f;

	[Export]
	private float MaxSpeed = 20f;

	[Export]
	private float AccelerationSpeed = 4.5f;

	[Export]
	private float DecelerationSpeed = 16.0f;

	private float Gravity;

	#endregion

	public override void _Ready()
	{
		Head = GetNode<Spatial>(HeadNodePath);
		Camera = GetNode<Camera>(CameraNodePath);

		// We could take the gravity vector from project settings, instead of * -1
		// However, we're not going to do any crazy non-standard physics, so nothing to worry about
		Gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * -1;
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		ProcessInput(delta);
		ProcessMovement(delta);
	}

	// TODO - extract this stuff out into a CharacterController - have it call a method to set Direction instead
	// That should be able to simplify AI / network puppets
	private void ProcessInput(float delta) {
		Direction = new Vector3();
		var cameraTransform = Camera.GlobalTransform;

		var inputMovementVector = new Vector2();
		if (Input.IsActionPressed("movement_forward")) {
			inputMovementVector.y += 1;
		}
		if (Input.IsActionPressed("movement_backward")) {
			inputMovementVector.y -= 1;
		}
		if (Input.IsActionPressed("movement_left")) {
			inputMovementVector.x -= 1;
		}
		if (Input.IsActionPressed("movement_right")) {
			inputMovementVector.x += 1;
		}

		// Theoretically this won't be necessary, since it was declared just before being set...
		inputMovementVector = inputMovementVector.Normalized();

		// Set our intended direction, which will be used to move us in ProcessMovement
		Direction += -GlobalTransform.basis.z * inputMovementVector.y;
		Direction += GlobalTransform.basis.x * inputMovementVector.x;

		if (IsOnFloor()) {
			if (Input.IsActionJustPressed("movement_jump")) {
				Direction.y = JumpSpeed;
			}
		}

		// Capture / free the camera on escape
		// TODO remove this stuff. Mouse capture could then all happen in the GameMode
		if (Input.IsActionJustPressed("ui_cancel")) {
			if (Input.GetMouseMode() == Input.MouseMode.Captured) {
			Input.SetMouseMode(Input.MouseMode.Visible);
			} else {
				Input.SetMouseMode(Input.MouseMode.Captured);
			}
		}
	}

	private void ProcessMovement(float delta) {
		Direction = Direction.Normalized();

		// Calculate vertical movement
		if (Direction.y != 0) {
			// Add jump power
			Velocity.y = Direction.y;
		}
		// Apply gravity
		Velocity.y += delta * Gravity;

		// Calculate horizontal movement
		var horizontalVelocity = Velocity;
		horizontalVelocity.y = 0;	// Strip out vertical component, as we're not interested

		var target = Direction;
		target *= MaxSpeed;

		// Work out whether we're speeding up or slowing down
		float acceleration;
		if (Direction.Dot(horizontalVelocity) > 0) {
			acceleration = AccelerationSpeed;
		} else {
			acceleration = DecelerationSpeed;
		}

		// Lerp toward intended target velocity
		horizontalVelocity = horizontalVelocity.LinearInterpolate(target, acceleration * delta);

		// Update the movement veloicty
		Velocity.x = horizontalVelocity.x;
		Velocity.z = horizontalVelocity.z;

		// Move! MoveAndSlide will also update KinematicBody flags for IsOnFloor etc
		Velocity = MoveAndSlide(Velocity, Vector3.Up, true);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		// Handle camera rotation
		if (Input.GetMouseMode() == Input.MouseMode.Captured) {
			InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
			if (mouseMotion != null) {
				HandleCameraRotation(mouseMotion);
			}
		}
	}

	private void HandleCameraRotation(InputEventMouseMotion mouseMotion) {
		// Rotate up / down
		Head.RotateX(Mathf.Deg2Rad(-mouseMotion.Relative.y * MouseSensitivity));
		Head.RotationDegrees = new Vector3(Mathf.Clamp(Head.RotationDegrees.x, -70, 70), Head.RotationDegrees.y, Head.RotationDegrees.z);

		// Rotate left / right
		RotateY(Mathf.Deg2Rad(-mouseMotion.Relative.x * MouseSensitivity));
	}
}
