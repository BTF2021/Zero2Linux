using Godot;
using System;

public partial class NewVer : Node2D
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_skip_pressed()
	{	QueueFree();
	}

	private void _on_go_pressed()
	{	OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/releases");
	}
}
