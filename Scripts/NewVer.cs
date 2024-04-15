using Godot;
using System;

public partial class NewVer : Control
{	
	private DefaultData _data;
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
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
		if(inputgrab)
		{	GetNode<Sprite2D>("Panel").Position = newpos;
		}
	}
	private void _on_skip_pressed()
	{	QueueFree();
	}

	private void _on_go_pressed()
	{	OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/releases");
	}
	private void _on_drag_down()
	{	GD.Print("Hi");
		inputgrab = true;
		var winpos = GetNode<Sprite2D>("Panel").Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		GD.Print(dif);
	}
	private void _on_drag_up()
	{	GD.Print("Bye");
		inputgrab = false;
	}
}
