using Godot;
using System;

public partial class Quizitem : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_send_pressed()
	{	//temporar
		GetParent().EmitSignal("GetAnswers", GetNode<CheckBox>("PanelContainer/Raspuns1").ButtonPressed, GetNode<CheckBox>("PanelContainer/Raspuns2").ButtonPressed, GetNode<CheckBox>("PanelContainer/Raspuns3").ButtonPressed, GetNode<CheckBox>("PanelContainer/Raspuns4").ButtonPressed);
	}
}
