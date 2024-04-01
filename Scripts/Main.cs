using Godot;
using System;

public partial class Main : Node
{
	private DefaultData _data;
	private HttpRequest request;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		GetNode<Label>("UI/Version").Text = "Ver " + (String)ProjectSettings.GetSetting("application/config/version");
		if(!_data.verifiedver && _data.currentStats.ChkUpdates)
		{	request = new HttpRequest();
			AddChild(request);
        	request.RequestCompleted += OnRequestCompleted;                                           //Cand se apeleaza Request => functia OnRequestCompleted
        	request.Request("https://raw.githubusercontent.com/BTF2021/Zero2Linux/main/version.txt"); //version.txt de pe Github
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void _on_quit_pressed() => GetTree().Quit(0);
    private void _on_course_pressed() => GetTree().ChangeSceneToFile("res://Scenes/Courses.tscn");

	private void _on_settings_pressed()
	{	AddChild((GD.Load<PackedScene>("res://Scenes/Settings.tscn")).Instantiate());
	}
	private void _on_quizzes_pressed()
	{	AddChild((GD.Load<PackedScene>("res://Scenes/Quizzes.tscn")).Instantiate());
	}
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {	if(result == 0)
		{	string data = System.Text.Encoding.UTF8.GetString(body);                                              //tot ce este in version.txt
			_data.newversion.Add(data.Substring(0, data.IndexOf(";", 0)));                                        //Versiunea programului
			_data.newversion.Add(data.Substring(data.IndexOf("-", 0)));                                           //Textul fara versiunea programului
			if(!((String)ProjectSettings.GetSetting("application/config/version")).Contains(_data.newversion[0])) //Daca versiunea programului corespunde cu versiunea din version.txt
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
