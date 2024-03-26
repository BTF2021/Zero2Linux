using Godot;
using System;

public partial class Quizztime : Node2D
{
	int total = 15;
	int intrebari = 0;
	int corecte = 0;
	int gresite = 0;
	[Signal]
	public delegate void GetAnswersEventHandler(bool a1, bool a2, bool a3, bool a4);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.GetAnswers += Answer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	GetNode<Label>("Status/Number").Text = intrebari + "/" + total;
		GetNode<Label>("Status/Correct").Text = "Corecte: " + corecte;
		GetNode<Label>("Status/Wrong").Text = "Gresite: " + gresite;
	}

	private void _on_back_pressed()
	{	GD.Print("Pressed");
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}
	//Functie temporara pana cand fac lista de intrebari si generez un Quizzitem
	public void Answer(bool a1, bool a2, bool a3, bool a4)
	{
		if(a1 && !a2 && !a3 && !a4)corecte++;
		else gresite++;
		if(intrebari<total)intrebari++;
	}
}
