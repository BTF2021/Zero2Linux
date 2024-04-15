using Godot;
using System;

public partial class Lesson : Node2D
{
	private DefaultData _data;
	public Node _node;
	public Godot.Collections.Array<Quizitem> _questions;
	[Export] public int lessonid = 0;
	private int totalquestioncount;    //Nr total de intrebari din lectie
	private int totallessonblocks;	   //Nr total de blocuri din lectie
	private int questionansw;          //Nr de intrebari deja raspunse
	private int blocksread;			   //Nr de blocuri deja vazute
	private int percent;               //Procentul progresului
	[Signal] public delegate void GetAnswersEventHandler(bool correct, int index);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_data = (DefaultData)GetNode("/root/DefaultData");
		_node = GetNode<VBoxContainer>("Panel/ScrollContainer/MarginContainer/Body/Content");
		_questions = new Godot.Collections.Array<Quizitem>{};
		GetAnswers += SendAnswers;
		if(!_data.isvideoavailable)
		{
			GetNode<TextureRect>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview").SelfModulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<Sprite2D>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/textureRect").QueueFree();
			GetNode<Label>("Panel/ScrollContainer/MarginContainer/Body/VideoPreview/Atentie").Show();
		}

		//Calculeaza nr de intrebari si blocuri
		percent = (int)_data.currentStats.LessonCompletion[lessonid];
		totalquestioncount = 0;
		totallessonblocks = 0;
		for (int i = 0; i <= _node.GetChildCount()-1; i++)
		{	if(_node.GetChild(i).Name.ToString().Match("Quizitem"))
			{	_node.GetChild<Quizitem>(i).Index = i;
				_node.GetChild<Quizitem>(i).Complete = false;
				_questions.Add((Quizitem)_node.GetChild<Quizitem>(i));
				totalquestioncount++;
			}
			else if(_node.GetChild(i).Name.ToString().Match("Block")) totallessonblocks++;
		}

		//Aici aratam lectia in fuctie de progresul salvat
		//TODO: clean up
		if(_data.currentStats.LessonCompletion[lessonid] < 100)
		{
			//0%
			if((questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks) == _data.currentStats.LessonCompletion[lessonid])
					showobjects(0);
			
			//Luam tot ce este in Content si adaugam nr de intrebari si blocuri si calculam progresul
			//Pana cand acesta este egal cu progresul salvat (Daca nu a fost modificat save.json)
			else
			{	GD.Print(questionansw + "/" + totalquestioncount + " " + blocksread + "/" + totallessonblocks + " " + ((questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks)));
				for (int i = 0; i <= _node.GetChildCount()-1; i++)
				{	if(_node.GetChild(i).Name.ToString().Match("Quizitem")) 
					{	_node.GetChild<Quizitem>(i).Complete = true;
						questionansw++;
					}
					else if(_node.GetChild(i).Name.ToString().Match("Block")) blocksread++;
					if((questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks) == _data.currentStats.LessonCompletion[lessonid])
					{
						showobjects(i);
						i = _node.GetChildCount();     //Opreste for
					}
					GD.Print(questionansw + "/" + totalquestioncount + " " + blocksread + "/" + totallessonblocks + " " + ((questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks)));
				}
			}
		}
		//100%
		else
		{
			for (int i = 0; i <= _node.GetChildCount()-1; i++)
				if(_node.GetChild(i).Name.ToString().Match("Quizitem")) 
					_node.GetChild<Quizitem>(i).Complete = true;              //Daca nu sunt setate la true, se poate suprascrie progresul si se strica totul
			_node.GetParent().GetChild<CanvasItem>(_node.GetParent().GetChildCount()-2).Visible = true;
			_node.GetParent().GetChild<CanvasItem>(_node.GetParent().GetChildCount()-1).Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void showobjects(int index)
	{	var foundquestion = false;
		for (int i = index; i <= _node.GetChildCount()-1; i++)
		{	if(foundquestion) _node.GetChild<CanvasItem>(i).Visible = false;
			else 
			{
				if(_node.GetChild(i).Name.ToString().Match("Block")) blocksread++;
				_node.GetChild<CanvasItem>(i).Visible = true;
			}
			if(_node.GetChild(i).Name.ToString().Match("Quizitem")) foundquestion = true;
		}
		if (!foundquestion)
		{
			_node.GetParent().GetChild<CanvasItem>(_node.GetParent().GetChildCount()-2).Visible = true;
			_node.GetParent().GetChild<CanvasItem>(_node.GetParent().GetChildCount()-1).Visible = true;
		}
	}
	public void SendAnswers(bool correct, int index)
	{
		if(correct)
		{	//Daca nu a fost deja raspuns
			if(!_node.GetChild<Quizitem>(index).Complete)
			{
				showobjects(index+1);
				_node.GetChild<Quizitem>(index).Complete = true;
				_node.GetChild<Quizitem>(index)._disable();

				questionansw = 0;
				for (int i = 0; i <= _questions.Count-1; i++)
				{
					if((bool)_questions[i].Complete)questionansw++;
				}
				GD.Print(questionansw + "/" + totalquestioncount + " " + blocksread + "/" + totallessonblocks + " " + ((questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks)));
				_data.currentStats.LessonCompletion[lessonid] = (questionansw + blocksread) * 100 / (totalquestioncount + totallessonblocks);
				_data.WriteSave(_data.LoggedUser);
			}
			else
			{	_node.GetChild<Quizitem>(index)._disable();
				var tween = GetTree().CreateTween();
				_node.GetChild<CanvasItem>(index).Modulate = new Color((float)0.05, 1, (float)0.05, 1);  //Culoare verde
				tween.TweenProperty(_node.GetChild<CanvasItem>(index), "modulate", new Color(1, 1, 1, 1), 0.5);
			}
		}
		else 
		{	var tween = GetTree().CreateTween();
			_node.GetChild<CanvasItem>(index).Modulate = new Color(1, (float)0.05, (float)0.05, 1);      //Culoare rosie
			tween.TweenProperty(_node.GetChild<CanvasItem>(index), "modulate", new Color(1, 1, 1, 1), 0.5);
		}
	}
	private void _on_back_pressed()
	{	GD.Print("Pressed");
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}
	private void _on_watch_pressed()
	{	var _video = (ResourceLoader.Load<PackedScene>("res://Scenes/VideoOverlay.tscn")).Instantiate();
		_video.GetNode<VideoStreamPlayer>("Panel/VideoStreamPlayer").Stream.File = "res://Courses/Lesson_" + lessonid + "/Video.webm";
		AddChild(_video);
	}
}
