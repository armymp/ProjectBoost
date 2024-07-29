using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Flight Control Variables")]
	[Export(PropertyHint.Range, "750, 1500")] private float _thrust = 1000.0f;
	[Export(PropertyHint.Range, "100, 250")] private float _torqueThrust = 250.0f;

	private bool _isTransitioning = false;
	private string _nextLevelFile;
	private AudioStreamPlayer _explosionAudio;
	private AudioStreamPlayer _successAudio;
	private AudioStreamPlayer3D _rocketAudio;
	private GpuParticles3D _boosterParticles;
	private GpuParticles3D _rightBoosterParticles;
	private GpuParticles3D _leftBoosterParticles;
	private GpuParticles3D _explosionParticles;
	private GpuParticles3D _successParticles;

	public override void _Ready()
	{
		_explosionAudio = GetNode<AudioStreamPlayer>("ExplosionAudio");
		_successAudio = GetNode<AudioStreamPlayer>("SuccessAudio");
		_rocketAudio = GetNode<AudioStreamPlayer3D>("RocketAudio");
		_boosterParticles = GetNode<GpuParticles3D>("BoosterParticles");
		_rightBoosterParticles = GetNode<GpuParticles3D>("RightBoosterParticles");
		_leftBoosterParticles = GetNode<GpuParticles3D>("LeftBoosterParticles");
		_explosionParticles = GetNode<GpuParticles3D>("ExplosionParticles");
		_successParticles = GetNode<GpuParticles3D>("SuccessParticles");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// 3D nodes store their rotation in a matrix called a basis
		// Vector3.Up will move directly upwards, but if we want to move forward in relation to the direction we're
		// facing we use Basis.Y
		if (Input.IsActionPressed("Boost"))
		{
			ApplyCentralForce(Basis.Y * (float)delta * _thrust);
			_boosterParticles.Emitting = true;
			if (_rocketAudio.Playing == false)
			{
				_rocketAudio.Play();
			}
		}

		if (Input.IsActionJustReleased("Boost"))
		{
			_rocketAudio.Stop();
			_boosterParticles.Emitting = false;
		}

		// Rotate ship counter-clockwise around Z axis
		if (Input.IsActionPressed("Rotate_Left"))
		{
			ApplyTorque(new Vector3(0.0f, 0.0f, _torqueThrust * (float)delta));
			_rightBoosterParticles.Emitting = true;
		}
		else
		{
			_rightBoosterParticles.Emitting = false;
		}
		

		// Rotate ship clockwise around Z axis
		if (Input.IsActionPressed("Rotate_Right"))
		{
			ApplyTorque(new Vector3(0.0f, 0.0f, -_torqueThrust * (float)delta));
			_leftBoosterParticles.Emitting = true;
		}
		else
		{
			_leftBoosterParticles.Emitting = false;
		}

		if (Input.IsActionPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
		
	}
	
	private void _on_body_entered(Node body)
	{
		if (_isTransitioning == false)
		{
			if (body.GetGroups().Contains("Goal"))
			{
				CompleteLevel(((LandingPad)body).filePath);
			}

			if (body.GetGroups().Contains("Hazard"))
			{
				CrashSequence();
			}
		}
	}

	private async void CrashSequence()
	{
		_explosionParticles.Emitting = true;
		StopBoosterParticles();
		StopRocketSound();
		_explosionAudio.Play();
		
		SetProcess(false); // disables _Process after crashing. Player loses control of movement.
		_isTransitioning = true;
		await ToSignal(_explosionAudio, AudioStreamPlayer.SignalName.Finished); // since the audio duration is short we wait for it before reloading
		
		ReloadScene();
		//GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
		//var tween = CreateTween();
		// tween.TweenInterval(1.0f);
		//tween.TweenCallback(new Callable(this,nameof(ReloadScene)));
	}

	private async void CompleteLevel(string nextLevelFile)
	{
		_successParticles.Emitting = true;
		StopRocketSound();
		StopBoosterParticles();
		_successAudio.Play();
		SetProcess(false);
		_isTransitioning = true;
		_nextLevelFile = nextLevelFile;
		await ToSignal(_successAudio, AudioStreamPlayer.SignalName.Finished);
		LoadNextScene();
		
		// var tween = CreateTween();
		// tween.TweenInterval(1.0f);
		// tween.TweenCallback(new Callable(this, nameof(LoadNextScene)));
	}
	
	// alternative way to set timer using async await
	// private async void CompleteLevel(string nextLevelFile)
	// {
	// 	GD.Print("You win!");
	// 	SetProcess(false);
	// 	_isTransitioning = true;
	// 	_nextLevelFile = nextLevelFile;
	//
	// 	Timer timer = GetNode<Timer>("TransitionTimer");
	// 	timer.WaitTime = 1.0f;
	// 	timer.Start();
	// 	await ToSignal(timer, "timeout");
	// 	
	// 	LoadNextScene();
	// }

	private void ReloadScene()
	{
		GetTree().ReloadCurrentScene();
	}

	private void LoadNextScene()
	{
		GetTree().ChangeSceneToFile(_nextLevelFile);
	}
	
	private void StopBoosterParticles()
	{
		_boosterParticles.Emitting = false;
		_leftBoosterParticles.Emitting = false;
		_rightBoosterParticles.Emitting = false;
	}

	private void StopRocketSound()
	{
		if(_rocketAudio.Playing)
			_rocketAudio.Stop();
	}
}



