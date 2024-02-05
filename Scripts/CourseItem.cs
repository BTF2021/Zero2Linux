using Godot;
using System;

public partial class CourseItem : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public int lesson = 0;
	[Export] public int complete = 0;
	[Export] public int lessonTag = 0;
	public override void _Ready()
	{ 	//GetNode<ProgressBar>("Percentage").Value = complete;
		//if(lessonTag == 1) GetNode<Label>("Label").Text = "Advanced Lesson " + lesson;
		//else if(lessonTag == 2) GetNode<Label>("Label").Text = "Special Lesson " + lesson;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_pressed() => GetTree().ChangeSceneToFile("res://Courses/Lesson_" + lesson + "/Lesson.tscn");
}
