using Godot;
using System;

public partial class moving_hazard : AnimatableBody3D
{
	[ExportCategory("Movement Settings")]
	[Export] private Vector3 _destination;
	[Export] private double _duration;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tween = CreateTween();
		tween.SetLoops(); // will loop movement infinitely
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(this, "global_position", GlobalPosition + _destination, _duration);
		tween.TweenProperty(this, "global_position", GlobalPosition, _duration);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}
