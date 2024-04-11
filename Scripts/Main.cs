using Godot;
using System;

public partial class Main : Node2D
{
	private DefaultData _data;
	private HttpRequest request;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		GetNode<Label>("UI/Version").Text = "Ver " + (String)ProjectSettings.GetSetting("application/config/version");
		if(!_data.verifiedver)
		{	request = new HttpRequest();
			AddChild(request);
        	request.RequestCompleted += OnRequestCompleted;                                           //Cand se apeleaza Request => functia OnRequestCompleted
        	request.Request("https://raw.githubusercontent.com/BTF2021/Zero2Linux/main/version.txt"); //version.txt de pe Github
		}

		if(_data.currentStats.Anims)
		{	var pos = Position;
			pos.X = 490;
			pos.Y = 352;
			GetNode<Panel>("UI/Panel").Position = pos;
			pos.Y = 337;
			GetNode<Panel>("UI/Panel").Modulate = new Color(1, 1, 1, 0);
			var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<Panel>("UI/Panel"), "modulate", new Color(1, 1, 1, 1), 0.35);
			tween.Parallel().TweenProperty(GetNode<Panel>("UI/Panel"), "position", pos, 0.5);
		}
		else
		{	GetNode<Sprite2D>("UI/Bg/Sprite2D").Show();
			GetNode<VideoStreamPlayer>("UI/Bg/Bg").Paused = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	GetNode<Label>("UI/Bar/HBoxContainer/Time").Text = Time.GetDateStringFromSystem() + " " + Time.GetTimeStringFromSystem();
	}

    private void _on_quit_pressed() => GetTree().Quit(0);
    private void _on_course_pressed()
	{	//GetTree().ChangeSceneToFile("res://Scenes/Courses.tscn");
		GetNode<Control>("UI").AddChild((GD.Load<PackedScene>("res://Scenes/Courses.tscn")).Instantiate());
	}

	private void _on_settings_pressed()
	{	GetNode<Control>("UI").AddChild((GD.Load<PackedScene>("res://Scenes/Settings.tscn")).Instantiate());
	}
	private void _on_quizzes_pressed()
	{	AddChild((GD.Load<PackedScene>("res://Scenes/Quizzes.tscn")).Instantiate());
	}
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {	if(result == 0)
		{	string data = System.Text.Encoding.UTF8.GetString(body);                                                                               //tot ce este in version.txt
			_data.newversion.Add(data.Substring(0, data.IndexOf(";", 0)));                                                                         //Versiunea programului
			_data.newversion.Add(data.Substring(data.IndexOf("-", 0)));                                                                            //Textul fara versiunea programului
			if(!((String)ProjectSettings.GetSetting("application/config/version")).Contains(_data.newversion[0]) && _data.currentStats.ChkUpdates) //Daca versiunea programului corespunde cu versiunea din version.txt
			{
				GD.Print("Versiune veche");
				var newver = (GD.Load<PackedScene>("res://Scenes/NewVer.tscn")).Instantiate();
				newver.GetNode<Label>("Panel/ScrollContainer/VBoxContainer/Title2").Text = newver.GetNode<Label>("Panel/ScrollContainer/VBoxContainer/Title2").Text + (String)ProjectSettings.GetSetting("application/config/version") + "\nVersiunea actuala este: " + _data.newversion[0] + "\n ";
				newver.GetNode<Label>("Panel/ScrollContainer/VBoxContainer/Title3").Text = _data.newversion[1];
				AddChild(newver);
				GD.Print("Gata");
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
