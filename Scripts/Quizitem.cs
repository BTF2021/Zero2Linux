using Godot;
using System;

public partial class Quizitem : PanelContainer
{
	[Export] public int type = 0;         //0: Chestionar   1: Intrebari in lectii, optional
	[Export] public int answ = 0;
	public bool Complete = false;
	public int Index = 1;                 //Mai mult pentru GetChild()

	[Export] public string question = "";
	[Export] public string answer1 = "";
	[Export] public string answer2 = "";
	[Export] public string answer3 = "";
	[Export] public string answer4 = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	GetNode<Label>("PanelContainer/Name").Text = question;
		GetNode<CheckBox>("PanelContainer/Raspuns1").Text = answer1;
		GetNode<CheckBox>("PanelContainer/Raspuns2").Text = answer2;
		GetNode<CheckBox>("PanelContainer/Raspuns3").Text = answer3;
		GetNode<CheckBox>("PanelContainer/Raspuns4").Text = answer4;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void _disable()
	{
		GetNode<CheckBox>("PanelContainer/Raspuns1").Disabled = true; 
		GetNode<CheckBox>("PanelContainer/Raspuns2").Disabled = true;
		GetNode<CheckBox>("PanelContainer/Raspuns3").Disabled = true;
		GetNode<CheckBox>("PanelContainer/Raspuns4").Disabled = true;
		GetNode<Button>("PanelContainer/HBoxContainer/Send").Disabled = true; 
		GetNode<Button>("PanelContainer/HBoxContainer/Skip").Disabled = true;
	}
	private void _on_send_pressed()
	{	//if-ul este temporar, pana cand implementez complet Quizztime
		var a = new Godot.Collections.Array()
		{	GetNode<CheckBox>("PanelContainer/Raspuns1").ButtonPressed, 
			GetNode<CheckBox>("PanelContainer/Raspuns2").ButtonPressed,
			GetNode<CheckBox>("PanelContainer/Raspuns3").ButtonPressed,
			GetNode<CheckBox>("PanelContainer/Raspuns4").ButtonPressed
		};
		if (type == 0) GetParent().EmitSignal("GetAnswers", a[0], a[1], a[2], a[3]);
		else
		{
			var ok = true;
			for (int i = 1; i <=4; i++)
			{	if(answ == i && (bool)a[i-1] != true) ok = false;
				else if(answ != i && (bool)a[i-1] != false) ok = false; 
			}
			GetNode<Node2D>("/root/Lesson").EmitSignal("GetAnswers", ok, Index);
		}
	}
}
