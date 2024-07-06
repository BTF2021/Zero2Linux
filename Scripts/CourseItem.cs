using Godot;
using System;

public partial class CourseItem : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public int lesson = 0;
	[Export] public string lessonName = "";
	[Export] public int complete = 0;
	[Export] public int lessonTag = 0;
}
