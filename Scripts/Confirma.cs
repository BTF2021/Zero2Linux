using Godot;
using System;

public partial class Confirma : Node2D
{
	private DefaultData _data;
	public Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		timer = GetNode<Timer>("timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if (!timer.IsStopped()) GetNode<ProgressBar>("Settings/Bar").Value = 3 - timer.TimeLeft;
		else GetNode<ProgressBar>("Settings/Bar").Value = 0;
	}

	private void _on_cancel_pressed() => Hide();
	private void _on_hold_down()
	{	timer.Start(3.0);
	}
	private void _on_hold_up()
	{	if(timer.IsStopped())
		{	_data.PurgeSave();
			GetParent().QueueFree();
		}
		else timer.Stop();
	}
}
