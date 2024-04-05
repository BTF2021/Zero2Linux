using Godot;
using System;

public partial class Lesson : Node2D
{
	private DefaultData _data;
	public Node _node;
	[Export] public int lessonid = 0;
	[Signal] public delegate void GetAnswersEventHandler(bool correct, int index);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_data = (DefaultData)GetNode("/root/DefaultData");
		_node = GetNode<VBoxContainer>("Panel/ScrollContainer/MarginContainer/Body/Content");
		GetAnswers += SendAnswers;
		if(!_data.isvideoavailable)
		{
			GetNode<TextureRect>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview").SelfModulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<Button>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/Watch").Disabled = true;
			GetNode<Label>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/Atentie").Show();
		}
		var countquestion = 0;
		for (int i = 0; i <= _node.GetChildCount()-1; i++)
		{	if(_node.GetChild(i).Name.ToString().Match("Quizitem"))
			{	_node.GetChild<Quizitem>(i).Index = i;
				countquestion++;
			}
		}
		showobjects(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
	private void showobjects(int index)
	{	var foundquestion = false;
		for (int i = index; i <= _node.GetChildCount()-1; i++)
		{	if(foundquestion) _node.GetChild<CanvasItem>(i).Visible = false;
			else _node.GetChild<CanvasItem>(i).Visible = true;
			if(_node.GetChild(i).Name.ToString().Match("Quizitem")) foundquestion = true;
		}
	}
	public void SendAnswers(bool correct, int index)
	{
		if(correct) 
		{
			showobjects(index+1);
			_node.GetChild<Quizitem>(index).Complete = true;
			_node.GetChild<Quizitem>(index)._disable();
		}
		else 
		{	var tween = GetTree().CreateTween();
			_node.GetChild<CanvasItem>(index).Modulate = new Color(1, (float)0.05, (float)0.05, 1);
			tween.TweenProperty(_node.GetChild<CanvasItem>(index), "modulate", new Color(1, 1, 1, 1), 0.5);
		}
	}
	private void _on_back_pressed()
	{	GD.Print("Pressed");
		GetTree().ChangeSceneToFile("res://Scenes/Courses.tscn");
	}
	private void _on_watch_pressed()
	{	var _video = (ResourceLoader.Load<PackedScene>("res://Scenes/VideoOverlay.tscn")).Instantiate();
		_video.GetNode<VideoStreamPlayer>("Panel/VideoStreamPlayer").Stream.File = "res://Courses/Lesson_" + lessonid + "/Video.webm";
		AddChild(_video);
	}
}
