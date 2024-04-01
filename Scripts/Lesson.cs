using Godot;
using System;

public partial class Lesson : Node2D
{
	private DefaultData _data;

	[Export] public int lessonid = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_data = (DefaultData)GetNode("/root/DefaultData");
		if(!_data.isvideoavailable)           //Temporar
		{
			GetNode<TextureRect>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview").SelfModulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<Button>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/Watch").Disabled = true;
			GetNode<Label>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/Atentie").Show();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
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
