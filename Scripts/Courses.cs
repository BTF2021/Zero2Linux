using Godot;
using System;
using System.Linq;

public partial class Courses : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private DefaultData _data;
	private VBoxContainer _VBoxContainer;
	public override async void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_VBoxContainer = (VBoxContainer)GetNode("Panel/ScrollContainer/VBoxContainer");
		GD.Print(_VBoxContainer.GetChildCount());
		for(int i = 1; i<=_VBoxContainer.GetChildCount(); i++)
		{
			GetNode<CourseItem>("Panel/ScrollContainer/VBoxContainer/CourseItem" + i).lesson = i;
			//Nu stiu cum sa iau o valoare din vector intr-un dicitonar
			//GetNode<CourseItem>("Panel/ScrollContainer/VBoxContainer/CourseItem" + i).complete = _data.lessonList.ElementAt(i);
			//GetNode<CourseItem>("Panel/ScrollContainer/VBoxContainer/CourseItem" + i).lessonTag = i;
		}
		GD.Print("Left");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
	private void _on_back_pressed() => GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	
	private void _on_pressed1() => GetTree().ChangeSceneToFile("res://Courses/Lesson_1/Lesson.tscn");
}
