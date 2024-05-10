using Godot;
using System;

public partial class Progress : Control
{
	private DefaultData _data;
	private Label _text;
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_text = GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/HBoxContainer/Stats");
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Player/Account/Name").Text = _data.currentStats.UsrName;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Player/Account/BigLetter").Text = _data.currentStats.UsrName;
		GetNode<Sprite2D>("Panel/Panel/ScrollContainer/VBoxContainer/Player/Account/Bg").SelfModulate = _data.currentStats.FavColor;

		_data.currentStats.FinishedLes = 0;
		int i = 1;
		while(_data.lessonList.ContainsKey(i))
		{	if((int)_data.currentStats.LessonCompletion[i] == 100 && (int)_data.lessonList[i][1] != 2) _data.currentStats.FinishedLes++;
			i++;
		}

		_text.Text = "Lectii finalizate: " + _data.currentStats.FinishedLes +
		"\nChestionare facute: " + _data.currentStats.Questionaires +
		"\nLista lectiilor finalizate: ";
		i=1;
		while(_data.lessonList.ContainsKey(i))
		{	if((int)_data.currentStats.LessonCompletion[i] == 100) _text.Text = _text.Text + "\n   " + (string)_data.lessonList[i][0];
			i++;
		}
		if(_data.currentStats.Anims)
		{
			var tween = GetTree().CreateTween();
			GetNode<Sprite2D>("Panel").Modulate = new Color(1, 1, 1, 0);
			var pos = Position;
			pos.X = 645;
			pos.Y = 339 - 25;
			GetNode<Sprite2D>("Panel").Position = pos;
			pos.Y = 339;
			tween.TweenProperty(GetNode<Sprite2D>("Panel"), "modulate", new Color(1, 1, 1, 1), 0.15);
			tween.Parallel().TweenProperty(GetNode<Sprite2D>("Panel"), "position", pos, 0.15);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	mousepos = GetViewport().GetMousePosition();
		var winpos = GetNode<Sprite2D>("Panel").Position;
		var newpos = Position;
		newpos.X = Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1);
		newpos.Y = Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1);
		if(inputgrab) GetNode<Sprite2D>("Panel").Position = newpos;
	}
	private void _on_back_pressed() => QueueFree();
	private void _on_drag_down()
	{	GD.Print("Hi");
		#if GODOT_ANDROID
			//Desi mousepos este preluat in _Proccess(), mousepos ramane aceeasi valoare dupa ce ecranul a fost atins
			//Presupun ca ii ia un frame ca sa proceseze noua pozitie, ceea ce nu este de ajuns pentru aceasta functie
			//Asa ca il actualizam acum mousepos
			mousepos = GetViewport().GetMousePosition();
		#endif
		var winpos = GetNode<Sprite2D>("Panel").Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		GD.Print(dif);
		inputgrab = true;
	}
	private void _on_drag_up()
	{	GD.Print("Bye");
		inputgrab = false;
	}
}
