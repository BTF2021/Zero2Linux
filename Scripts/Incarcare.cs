using Godot;
using System;

public partial class Incarcare : Node2D
{
	private DefaultData _data;
	[Export]public string target;
	private bool done;						//Pentru avertizarile legate de Tweenuri
	public Godot.Collections.Array progress;
	public override async void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		GetNode<RichTextLabel>("Tip").Text = "[center]";
		done = false;
		GD.Print(target);
		var tip = (int)GD.RandRange(1, 21);
		switch(tip)
		{	case 1:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Se incarca...";
				break;
			case 2:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "E GNU/Linux";
				break;
			case 3:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Eu folosesc Arch Linux (by the way)";
				break;
			case 4:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Viziteaza alternativeto.net pentru a vedea o lista de alternative pentru programe";
				break;
			case 5:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun fact: Android este bazat pe un kernel modificat de linux";
				break;
			case 6:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun fact: Majoritatea serverelor folosesc Linux";
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
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + name;
				break;
			case 8:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Este cumva " + Time.GetDateStringFromSystem().Substr(0, 4) + " anul Linux?";
				break;
			case 9:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Daca nu stii cum se utilizeaza un program, exista intotdeauna manualul/wikiul";
				break;
			case 10:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Vim, Nano, Emacs... nu conteaza atata timp cat poti edita un fisier text";
				break;
			case 11:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + ":q + Enter pentru a iesi din Vim";
				break;
			case 12:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun fact: Linux Mint este derivat dintr-o derivata a Debian";
				break;
			case 13:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Viziteaza pagina de Github al acestui proiect pentru noi actualizari";
				break;
			case 14:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun fact: Linux Torvalds foloseste Fedora";
				break;
			case 15:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Ai auzit de Justin Bieber Linux?";
				break;
			case 16:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Ai auzit de Hannah Montana Linux?";
				break;
			case 17:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Insereaza un text aici";
				break;
			case 18:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun Fact: Freax era numele oficial pentru Linux";
				break;
			case 19:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "[wave amp=45.0 freq=7.0 connected=1]<ooooooooooooooooooooooooooooooooooooooooooooooooooooe[/wave]";    //Ar trebui sa arate a sarpe
				break;
			case 20:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "Fun fact: Probabil ai mai multe dispozitive Linux decat crezi";
				break;
			case 21:
				GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "[i]Read the friendly manual[/i]";
				break;
		}
		//Easter Egg bazat pe data sistemului :)
		if(Time.GetDatetimeStringFromSystem().Find("09", 4) == 5 && Time.GetDatetimeStringFromSystem().Find("17", 7) == 8) GetNode<RichTextLabel>("Tip").Text = "[center]La multi ani Linux!";
		else if(Time.GetDatetimeStringFromSystem().Find("12", 4) == 5 && Time.GetDatetimeStringFromSystem().Find("28", 7) == 8) GetNode<RichTextLabel>("Tip").Text = "[center]La multi ani Linus Torvalds!";
		GetNode<RichTextLabel>("Tip").Text = GetNode<RichTextLabel>("Tip").Text + "[/center]";

		GetNode<ProgressBar>("Bara").Value = 0;
		ResourceLoader.LoadThreadedRequest(target);
		if(_data.currentStats.Anims)
		{	GetNode<Node2D>(".").Modulate = new Color(1, 1, 1, 0);
			var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<Node2D>("."), "modulate", new Color(1, 1, 1, 1), 0.25);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(!GetNode<AnimationPlayer>("Icon/Animation").IsPlaying()) GetNode<AnimationPlayer>("Icon/Animation").Play("Rotate");
		var status = ResourceLoader.LoadThreadedGetStatus(target, progress);
		switch(status)
		{
			case (ResourceLoader.ThreadLoadStatus)1:
				//In progress
				GetNode<ProgressBar>("Bara").Value = ((int)progress[0]) * 100;
				break;
			case (ResourceLoader.ThreadLoadStatus)3:
				//Incarcat
				if(!done)
				{	GetNode<ProgressBar>("Bara").Value = 100;
					ChangeScene(100);
				}
				done = true;
				break;
			case (ResourceLoader.ThreadLoadStatus)2:
				//Eroare
				GD.Print("Nu se poate incarca scena");
				break;
			
		}
	}
	private async void ChangeScene(float value)
	{	
		if(value == 100)
		{	var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<ProgressBar>("Bara"), "value", 120, 1.5);
			await ToSignal(tween, Tween.SignalName.Finished);
			tween.Stop();
			tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<Node2D>("."), "modulate", new Color(1, 1, 1, 0), 0.5);
			await ToSignal(tween, Tween.SignalName.Finished);
			tween.Stop(); 
			//Dupa multe incercari am reusit sa nu mai am erori de genul "Object reference not set to an instance of an object" in aceasta functie
			//Era nevoie doar de un if
			var packed = (PackedScene)(ResourceLoader.LoadThreadedGet(target));
			if(packed != null) GetTree().ChangeSceneToPacked(packed);
		}
	}
}
