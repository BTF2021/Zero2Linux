using Godot;
using System;

public partial class Confirma : Node2D
{
	private DefaultData _data;
	public Timer timer;
	public bool timeout;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		timer = GetNode<Timer>("timer");
		timeout = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if (!timer.IsStopped()) GetNode<ProgressBar>("Settings/Bar").Value = 3 - timer.TimeLeft;
		else GetNode<ProgressBar>("Settings/Bar").Value = 0;
		if(timeout) GetNode<ProgressBar>("Settings/Bar").Value = 3;
	}

	private void _on_cancel_pressed() => Hide();
	private void _on_hold_down()
	{	timer.Start(3.0);
	}
	private void _on_hold_up()
	{	if(timer.IsStopped() && timeout) _data.DeleteSave(_data.LoggedUser);
		else timer.Stop();
	}
	private void _on_timeout()
	{	timeout = true;
		GetNode<Label>("Settings/Bar/Text").Text = "Elibereaza butonul";
	}
}
