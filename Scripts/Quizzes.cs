using Godot;
using System;

public partial class Quizzes : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_back_pressed()
	{	//Hide();
		QueueFree();
	}
	private void _on_random_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Quizztime.tscn");
	}
}
