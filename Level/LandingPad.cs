using Godot;
using System;

public partial class LandingPad : CsgBox3D
{
	/// <summary>
	/// File path set in the Inspector window of Godot
	/// </summary>
	[Export(PropertyHint.File, "*.tscn")] public string filePath; // filters for scene files only
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
