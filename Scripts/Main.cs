using Godot;
using System;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	//ConfigFile config;
	public override void _Ready()
	{	GetNode<Node2D>("Settings").Hide();
		//config.Load("res://override.cfg");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void _on_quit_pressed() => GetTree().Quit(0);
    private void _on_course_pressed() => GetTree().ChangeSceneToFile("res://Scenes/Courses.tscn");

	private void _on_settings_pressed()
	{	GetNode<Node2D>("Settings").Show();
	}
	private void _on_quizzes_pressed()
	{	GetNode<Node2D>("Quizzes").Show();
	}
}
