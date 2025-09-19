//Pentru fereastra de progres
using Godot;
using System;

public partial class Progress : Node2D
{
	private DefaultData _data;
	private VBoxContainer _item;	//Itemul-model
	private int spc, unfinished;	//Variabile locale pentru numararea lectiilor speciale, respectiv cele neterminate
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_item = GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer/Item");
		GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Account/Name").Text = _data.currentStats.UsrName;
		GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Account/BigLetter").Text = _data.currentStats.UsrName;
		GetNode<Sprite2D>("Window/Panel/ScrollContainer/VBoxContainer/Account/Bg").SelfModulate = _data.currentStats.FavColor;

		//Calculam cate lectii sunt deja terminate
		_data.currentStats.FinishedLes = 0;
		int i = 1;
		while(_data.lessonList.ContainsKey(i))
		{	if((int)_data.currentStats.LessonCompletion[i] == 100 && (int)_data.lessonList[i][1] != 2) _data.currentStats.FinishedLes++;
			else if((int)_data.currentStats.LessonCompletion[i] == 100 && (int)_data.lessonList[i][1] == 2) spc++;
			else if((int)_data.currentStats.LessonCompletion[i] < 100 && (int)_data.currentStats.LessonCompletion[i] > 0) unfinished++;
			i++;
		}

		//Adaugam elemente in lista
		if(unfinished > 0)
		{	AddItem("Lectii incepute: " + unfinished, null);
			i=1;
			while(_data.lessonList.ContainsKey(i))
			{	if((int)_data.currentStats.LessonCompletion[i] < 100 && (int)_data.currentStats.LessonCompletion[i] > 0) AddItem((string)_data.lessonList[i][0], GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").GetChild<VBoxContainer>(-1));
				i++;
			}
		}
		else AddItem("Nu ai nicio lectie inceputa", null);
		if(_data.currentStats.FinishedLes > 0)
		{	AddItem("Lectii finalizate: " + (_data.currentStats.FinishedLes + spc), null);
			i=1;
			while(_data.lessonList.ContainsKey(i))
			{	if((int)_data.currentStats.LessonCompletion[i] == 100) AddItem((string)_data.lessonList[i][0], GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").GetChild<VBoxContainer>(-1));
				i++;
			}
		}
		else AddItem("Nu ai nicio lectie terminata", null);
		AddItem("Chestionare facute: " + _data.currentStats.Questionaires, null);
		AddItem("Teste facute: " + _data.currentStats.Testsnum, null);
		AddItem("Peste nota 5: " + _data.currentStats.goodtests, GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").GetChild<VBoxContainer>(-1));
		AddItem("Peste nota 7: " + _data.currentStats.greattest, GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").GetChild<VBoxContainer>(-1));
		AddItem("Nota 10: " + _data.currentStats.flawlesstests, GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").GetChild<VBoxContainer>(-1));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{	
	
	}*/
	//Buton pentru aratarea/ascunderea subcategoriilor
	private void _on_button_pressed(Node _node)
	{	_node.GetNode<HBoxContainer>("SubContainer").Visible = !_node.GetNode<HBoxContainer>("SubContainer").Visible;
		_node.GetNode<TextureRect>("Panel/Contents/Arrow").FlipV = !_node.GetNode<TextureRect>("Panel/Contents/Arrow").FlipV;
	}
	//Functie pentru crearea elementelor din lista
	//Ca sa putem face o sublista pentru o anumita categorie, ne trebuie si nodul respectiv
	private void AddItem(string text, VBoxContainer parent_node)
	{	var button = (VBoxContainer)_item.Duplicate(8);
		button.Visible = true;
		button.GetNode<Label>("Panel/Contents/Stat").Text = text;
		button.GetNode<Button>("Panel/Button").Pressed += () => _on_button_pressed(button);
		if(parent_node != null)
		{	parent_node.GetNode<Button>("Panel/Button").Disabled = false;
			parent_node.GetNode<Button>("Panel/Button").Visible = true;
			parent_node.GetNode<TextureRect>("Panel/Contents/Arrow").Visible = true;
			parent_node.GetNode<VBoxContainer>("SubContainer/Child").AddChild(button);
		}
		else GetNode<VBoxContainer>("Window/Panel/ScrollContainer/VBoxContainer").AddChild(button);
	}
}
