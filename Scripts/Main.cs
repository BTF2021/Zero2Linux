//Folosit pentru meniul principal
using Godot;
using System;

public partial class Main : Node2D
{
	private DefaultData _data;
	private HttpRequest request;	//Request http pentru verificarea unei noi versiuni a programului
	[Signal] public delegate void DownloadEventHandler(int mode);	//Semnal trimis de NewVer.cs atunci cand se descarca o noua versiune
	[Signal] public delegate void TutorialEventHandler(int mode);   //Semnal trimis de Tour.cs in timpul tutorialului
	bool mouseOverPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		Download += down;	//Conectam semnalul la functie
		Tutorial += BeginTutorial;

		//Pentru meniu
		GetNode<Label>("Bar/HBoxContainer/Logo/Panel/Account/BigLetter").Text = _data.currentStats.UsrName;
		GetNode<Label>("Bar/HBoxContainer/Logo/Panel/Account/Name").Text = _data.currentStats.UsrName;
		GetNode<Sprite2D>("Bar/HBoxContainer/Logo/Panel/Account/Bg").SelfModulate = _data.currentStats.FavColor;
		
		if(!_data.currentStats.ShowTutorial)
		{
			//Preluam de pe Github fisierul version.txt
			if(!_data.verifiedver)
			{	request = new HttpRequest();
				AddChild(request);
				request.RequestCompleted += OnRequestCompleted;                                           //Cand se apeleaza Request => functia OnRequestCompleted
				request.Request("https://raw.githubusercontent.com/BTF2021/Zero2Linux/main/version.txt"); //version.txt de pe Github
			}
			//Tranzitie
			if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
			else 
			{		GetNode<Sprite2D>("Bar").Position = Position with { X = 640, Y = 686 };
					GetNode<Node2D>("Bg").Scale = Scale with { X = 1.0f, Y = 1.0f};
			}
		}
		else
		{
			//Tutorial
			if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("TutorialIn");
			else 
			{
					GetNode<Node2D>("Bg").Scale = Scale with { X = 1.0f, Y = 1.0f};
			}
			BeginTutorial(0);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Pentru meniul de start ca sa dispara atunci cand se da click in alta parte
		if(Input.IsActionPressed("click") && !mouseOverPanel) GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible = false;
		MoveChild(GetNode("Bar"), -1);
		//Ceasul
		if(HasNode("Bar")) GetNode<Label>("Bar/HBoxContainer/Time").Text = Time.GetTimeStringFromSystem();
	}
	public override void _Notification(int what)
	{	//Daca dai inapoi pe Android
		if (what == NotificationWMGoBackRequest)
        	GetTree().Quit();
	}

	//Semnal pentru NewVer.cs ca sa nu poata utilizatorul sa paraseasca scena Main
	private void down(int mode)
	{	if(mode ==1)	//Descarcarea este in desfasurare
		{	
			GetNode<Node2D>("NewVer").ZIndex = 100;
			var tween = GetTree().CreateTween();
			tween.SetPauseMode((Tween.TweenPauseMode)2);	//Ca sa fie procesat chiar si daca SceneTree este in pauza
			tween.TweenProperty(GetNode<ColorRect>("Pause"), "self_modulate", new Color(1, 1, 1, 1), 0.5).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
			GetTree().Paused = true;
		}
		else	//Descarcarea s-a terminat
		{	
			GetNode<Node2D>("NewVer").ZIndex = 0;
			GetNode<ColorRect>("Pause").SelfModulate = new Color(1, 1, 1, 0);
		}
	}

	//Cand se apasa pe logo
	private void _on_logo_pressed()
	{
		if(GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible) CloseMenu();
		else
		{
			if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("Panel");
			else GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible = true;
			mouseOverPanel = true;
		}
	}
	//Incepe tutorialul
	private async void BeginTutorial(int step)
	{
		switch(step)
		{
			case 0:
				//Creeam un timer ca sa apara fereastra DUPA ce se termina animatia
				var timer = GetTree().CreateTimer(0.6);
				await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
				AddChild((GD.Load<PackedScene>("res://Scenes/Tour.tscn")).Instantiate());
				break;
			case 1:
				//Apare bara atunci cand ajungem sa vorbim despre aceasta
				if(_data.currentStats.Anims)
				{
					var tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode<Sprite2D>("Bar"), "position", Position with { X = 640, Y = 686 }, 0.5).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
				}
				else GetNode<Sprite2D>("Bar").Position = Position with { X = 640, Y = 686 };
				break;
		}
	}
	//Inchidere meniu
	private async void CloseMenu()
	{	var timer = GetTree().CreateTimer(0.01);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible = false;
		mouseOverPanel = false;
	}
	//Functii pentru detectarea cursorului pe iconita
	private void _on_menu_mouse_entered(bool icon)
	{
		if(GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible || icon) mouseOverPanel = true;
	}
	private void _on_menu_mouse_exited(bool icon)
	{
		if(GetNode<Panel>("Bar/HBoxContainer/Logo/Panel").Visible || icon) mouseOverPanel = false;
	}

	private void _on_quit_pressed() => GetTree().Quit(0);
	private void _on_logout_pressed() => _data.LogOut(_data.LoggedUser);
	private void _on_notification_pressed()	//Apare fereastra pentru o noua versiune
	{	if(!HasNode("NewVer"))
		{
			var newver = (GD.Load<PackedScene>("res://Scenes/NewVer.tscn")).Instantiate();
			newver.GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title2").Text = newver.GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title2").Text + (String)ProjectSettings.GetSetting("application/config/version") + "\nVersiunea actuala este: " + _data.newversion[0] + "\n ";
			newver.GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/HBoxContainer2/Title3").Text = _data.newversion[1];
			AddChild(newver);
		}
	}
	//Cele patru functii corespund celor patru butoane din meniu
	private void _on_lesson_pressed() { AddChild((GD.Load<PackedScene>("res://Scenes/Courses.tscn")).Instantiate()); CloseMenu(); }
	private void _on_settings_pressed() { AddChild((GD.Load<PackedScene>("res://Scenes/Settings.tscn")).Instantiate()); CloseMenu(); }
	private void _on_quizzes_pressed() { AddChild((GD.Load<PackedScene>("res://Scenes/Quizzes.tscn")).Instantiate()); CloseMenu(); }
	private void _on_stats_pressed() { AddChild((GD.Load<PackedScene>("res://Scenes/Progress.tscn")).Instantiate()); CloseMenu(); }
	private void _on_tour_pressed() { AddChild((GD.Load<PackedScene>("res://Scenes/Tour.tscn")).Instantiate()); CloseMenu(); }

	//Daca a fost primit un raspuns de la _request
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{	if(result == 0)
		{	string data = System.Text.Encoding.UTF8.GetString(body);		//tot ce este in version.txt
			_data.newversion.Clear();
			_data.newversion.Add(data.Substring(0, data.IndexOf(";", 0)));		//Versiunea programului
			_data.newversion.Add(data.Substring(data.IndexOf("-", 0)));		//Textul fara versiunea programului
			if(!((String)ProjectSettings.GetSetting("application/config/version")).Contains(_data.newversion[0]) && _data.currentStats.ChkUpdates) //Daca versiunea programului corespunde cu versiunea din version.txt
			{
				GD.Print("Versiune veche");
				GetNode<TextureButton>("Bar/HBoxContainer/Notification").Disabled = false;
				GetNode<TextureButton>("Bar/HBoxContainer/Notification").Visible = true;
			}
			else GD.Print("Mergem in continuare");
		}
		else
		{
			GD.Print("Eroare HttpRequest: " + (HttpRequest.Result)result);
			return;
		}
		_data.verifiedver = true;
		request.QueueFree();       //Nu mai e nevoie de HttpRequest. Putem sterge
	}
}
