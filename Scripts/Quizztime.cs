using Godot;
using System;

public partial class Quizztime : Node2D
{
	private DefaultData _data;
	public PackedScene _item;
	int total = 10;
	int intrebari = 0;
	int corecte = 0;
	int gresite = 0;
	Godot.Collections.Array chars = new Godot.Collections.Array(){"/", "%", "&", "$", "@", "$", "#", "(", ")", "-", "0", "O"};
	string randomstr = "";
	Timer timer;
	[Signal] public delegate void GetAnswersEventHandler(bool correct, int index);
	[Signal] public delegate void NextEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_item = GD.Load<PackedScene>("res://Scenes/Quizitem.tscn");
		GetAnswers += SendAnswers;
		Next += NextQuestion;
		NextQuestion();
		if(_data.questiontype == 1)
		{	GetNode<Label>("Title").Text = "Test";
			GetNode<RichTextLabel>("Correct").TooltipText = "Eroare: nr_intrebari_corecte nu poate fi afisat";
			GetNode<RichTextLabel>("Wrong").TooltipText = "Eroare: nr_intrebari_gresite nu poate fi afisat";
			if(!_data.currentStats.QNumOnly) _timeout();
		}
		if(!_data.currentStats.QNumOnly && _data.currentStats.Anims)
		{	timer = GetNode<Timer>("Timer");
			timer.Autostart = true;
			timer.Timeout += _timeout;
			timer.Start((float)0.15);
		}
		else if(_data.currentStats.QNumOnly && _data.questiontype == 1)
		{	GetNode<RichTextLabel>("Correct").Hide();
			GetNode<RichTextLabel>("Wrong").Hide();
			var pos = GetNode<Label>("Number").Position;
			pos.X = 456;
			GetNode<Label>("Number").Position = pos;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	GetNode<Label>("Number").Text = "Intrebari: " + intrebari + "/" + total;
		if(_data.questiontype == 0)
		{	GetNode<RichTextLabel>("Correct").Text = "Corecte: " + corecte;
			GetNode<RichTextLabel>("Wrong").Text = "Gresite: " + gresite;
		}
	}
	private void _on_continue_pressed()
	{	GetNode<Button>("Continue").Hide();
		NextQuestion();
	}

	private void _on_back_pressed()
	{	GD.Print("Pressed");
		_data.questiontype = 0;
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}
	public void SendAnswers(bool correct, int index)
	{
		if(correct)
		{	corecte++;
			if(_data.questiontype == 0) GetNode<Button>("Continue").Show();
		}
		else 
		{	gresite++;
			if(intrebari<total) NextQuestion(); //Nu conteaza tipul. Treci mai departe
			return;
		}
		if(intrebari<total)
		{
			if(_data.questiontype == 1) NextQuestion();   //Nu arata explicatia. Treci mai departe
		}
		else 
		{	var name = (String)GetChild(-1).Name;
			if(name.Contains("Quizitem")) GetChild(-1).QueueFree();
			_finished();
		}
	}
	public void _timeout()
	{	randomstr = "";
		var rand = new Godot.Collections.Array(){(string)chars[GD.RandRange(0, 11)], (string)chars[GD.RandRange(0, 11)]};
		randomstr = String.Join("", rand);
		if(_data.questiontype == 1) GetNode<RichTextLabel>("Correct").Text = "Corecte: [shake rate=100.0 level=20 connected=1][color=#e5e5e5]" + randomstr  + "[/color][/shake]";
		randomstr = "";
		rand = new Godot.Collections.Array(){(string)chars[GD.RandRange(0, 11)], (string)chars[GD.RandRange(0, 11)]};
		randomstr = String.Join("", rand);
		if(_data.questiontype == 1) GetNode<RichTextLabel>("Wrong").Text = "Gresite: [shake rate=100.0 level=20 connected=1][color=#e5e5e5]" + randomstr + "[/color][/shake]";
	}
	public async void NextQuestion()
	{	if(intrebari<total)
		{	intrebari++;
			var name = (String)GetChild(-1).Name;
			if(name.Contains("Quizitem")) GetChild(-1).QueueFree();
			var _panel =  _item.Instantiate<Quizitem>();
			var pos = Position;
			pos.X = 296;
			pos.Y = 192;
			_panel.Position = pos;
			_panel.type = _data.questiontype + 1;
			AddChild(_panel);
			_panel.Name = "Quizitem";
		}
		else
		{	var name = (String)GetChild(-1).Name;
			if(name.Contains("Quizitem")) GetChild(-1).QueueFree();
		}
	}
	private void _finished()
	{	if(_data.questiontype == 0) GetNode<Button>("Continue").Hide();
		GD.Print("Done");
	}
}
