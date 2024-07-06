using Godot;
using System;

public partial class Incarcare : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{	var tip = (int)GD.RandRange(1, 17);
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
				GetNode<Label>("Tip").Text = "Escape + :q + Enter pentru a iesi din Vim";
				break;
			case 12:
				GetNode<Label>("Tip").Text = "Fun fact: Linux Mint este derivat dintr-o derivata a Debian";
				break;
			case 13:
				GetNode<Label>("Tip").Text = "Viziteaza pagina de Github al acestui proiect pentru noi actualizari";
				break;
			case 14:
				GetNode<Label>("Tip").Text = "fun fact: Linux Torvalds foloseste Fedora";
				break;
			case 15:
				GetNode<Label>("Tip").Text = "Ai auzit de Justin Bieber Linux?";
				break;
			case 16:
				GetNode<Label>("Tip").Text = "Ai auzit de Hannah Montana Linux?";
				break;
			case 17:
				GetNode<Label>("Tip").Text = "Insereaza un text aici";
				break;
		}
		//Easter Egg bazat pe data sistemului :)
		if(Time.GetDatetimeStringFromSystem().Find("09", 4) == 5 && Time.GetDatetimeStringFromSystem().Find("17", 7) == 8) GetNode<Label>("Tip").Text = "La multi ani Linux!";
		else if(Time.GetDatetimeStringFromSystem().Find("12", 4) == 5 && Time.GetDatetimeStringFromSystem().Find("28", 7) == 8) GetNode<Label>("Tip").Text = "La multi ani Linus Torvalds!";
		
		GetNode<ProgressBar>("Bara").Value = 0;
		Modulate = new Color(1, 1, 1, (float)0.5);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<Node2D>("."), "modulate", new Color(1, 1, 1, 1), 0.2);
		var timer = GetTree().CreateTimer(0.3);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<ProgressBar>("Bara"), "value", 100, GD.RandRange(1.5, 2)).SetTrans(Tween.TransitionType.Quad);
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
