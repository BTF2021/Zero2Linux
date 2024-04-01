using Godot;
using System;

public partial class Courses : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private DefaultData _data;
	private VBoxContainer _VBoxContainer;
	private PackedScene _courseitem;
	private CourseItem _panel;
	public int selected;
	public override async void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_VBoxContainer = (VBoxContainer)GetNode("Panel/ScrollContainer/VBoxContainer");
		_courseitem = GD.Load<PackedScene>("res://Scenes/CourseItem.tscn");
		int i=1;
		while(_data.lessonList.ContainsKey(i))
		{	AddItem(i);
			i++;
		}
		GD.Print("Left");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
	}
	//Daca pui aceasta functie in while, toate CourseItem o sa redirectioneze in Lesson_16 sau in _panel.lesson (Lesson_15)
	private void AddItem(int i)
	{	_panel =  _courseitem.Instantiate<CourseItem>();
		_panel.lesson = i;
		_panel.Name = "CourseItem" + i;
		_panel.lessonName = (string)_data.lessonList[i][0];
		_panel.GetNode<Label>("PanelContainer/Name").Text = _panel.lessonName;
		_panel.complete = (int)_data.currentStats.LessonCompletion[i];
		_panel.GetNode<ProgressBar>("PanelContainer/Percentage").Value = _panel.complete;
		_panel.lessonTag = (int)_data.lessonList[i][1];
		if(!_data.currentStats.Adv && (_panel.lessonTag == 1 || _panel.lessonTag == 3)) return;
		if(!_data.currentStats.Spc && (_panel.lessonTag == 2 || _panel.lessonTag == 3)) return;
		if(_panel.lessonTag == 1 || _panel.lessonTag == 3) _panel.GetNode<TextureRect>("PanelContainer/Adv").Show();
		if(_panel.lessonTag == 2 || _panel.lessonTag == 3) _panel.GetNode<TextureRect>("PanelContainer/Spc").Show();
		_VBoxContainer.AddChild(_panel);
		//Daca lectia cu nr i exista, conecteaza semnalul Pressed la functie. Altfel dezactiveaza
		if(ResourceLoader.Exists("res://Courses/Lesson_" + i + "/Lesson.tscn")) _panel.GetNode<Button>("Panel").Pressed += () => PanelPressed(i);
		else
		{	_panel.GetNode<Button>("Panel").Disabled = true;
			_panel.Modulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
		}
		GD.Print("Adaugat " + _panel.lessonName);
	}
	private void _on_back_pressed() => GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	private void PanelPressed(int index)
	{	_data.currentStats.CurrentLesson = index;
		GetTree().ChangeSceneToFile("res://Courses/Lesson_" + index + "/Lesson.tscn");
	}
}
