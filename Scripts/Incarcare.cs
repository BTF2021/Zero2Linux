using Godot;
using System;

public partial class Incarcare : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{	var tip = (int)GD.RandRange(1, 11);
		switch(tip)
		{	case 1:
				GetNode<Label>("Tip").Text = "Fun fact: Nu se incarca nimic in acest moment";
				break;
			case 2:
				GetNode<Label>("Tip").Text = "E GNU/Linux";
				break;
			case 3:
				GetNode<Label>("Tip").Text = "Eu folosesc Arch by the way";
				break;
			case 4:
				GetNode<Label>("Tip").Text = "Viziteaza alternativeto.net pentru a vedea o lista de alternative pentru programe";
				break;
			case 5:
				GetNode<Label>("Tip").Text = "Fun fact: Android este bazat pe un kernel modificat de linux";
				break;
			case 6:
				GetNode<Label>("Tip").Text = "Fun fact: Majoritatea serverelor folosesc Linux";
				break;
			case 7:
				var name = "";
				switch(OS.GetName())
				{	case "Windows":
						name = "Poti incerca o distributie de linux intr-o masina virtuala sau intr-un container";
						break;
					case "Linux":
						name = "Nu uita sa-ti actualizezi periodic programele instalate!";
						break;
					case "Android":
						//Posibil build?
						name = "Fun fact: Se poate rula Linux si pe dispozitive cu procesoare ARM";
						break;
				}
				GetNode<Label>("Tip").Text = name;
				break;
			case 8:
				GetNode<Label>("Tip").Text = "Este cumva " + Time.GetDateStringFromSystem().Substr(0, 4) + " anul Linux?";
				break;
			case 9:
				GetNode<Label>("Tip").Text = "Daca nu stii ceva, exista intotdeauna manualul/wikiul";
				break;
			case 10:
				GetNode<Label>("Tip").Text = "Vim, Nano, Emacs... nu conteaza atata timp cat poti edita un fisier text";
				break;
			case 11:
				GetNode<Label>("Tip").Text = ":q + Enter pentru a iesi din Vim";
				break;
		}
		GetNode<ProgressBar>("Bara").Value = 0;
		Modulate = new Color(1, 1, 1, (float)0.5);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<Node2D>("."), "modulate", new Color(1, 1, 1, 1), 0.5);
		var timer = GetTree().CreateTimer(0.5);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<ProgressBar>("Bara"), "value", 100, GD.RandRange(2, 2.5));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(!GetNode<AnimationPlayer>("Icon/Animation").IsPlaying()) GetNode<AnimationPlayer>("Icon/Animation").Play("Rotate");
	}
	private void _on_bara_changed(float value)
	{	if(value == 100) 
		{	var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<Node2D>("."), "modulate", new Color(1, 1, 1, 0), 0.5);
		}
	}
}
