using Godot;
using System;

public partial class ArrowController : CharacterBody2D, IController
{
	[ExportGroup("Nodes")]
	[Export]
	public PcHurtBoxArea HurtBox { get; set; } = null!;
	
	[ExportGroup("Variables")]
	[Export]
	float Speed = 300.0f;
	[Export]
	float JumpVelocity = -400.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction.X != 0)
		{
			velocity.X = direction.X * Speed;
		}
		if (direction.Y != 0)
		{
			velocity.Y = direction.Y * Speed;
		}
		if (direction == Vector2.Zero)
		{
			velocity = Vector2.Zero;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
