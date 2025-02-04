//Folosit pentru fereastra cu lectiile
using Godot;
using System;

public partial class Courses : Control
{
	// Called when the node enters the scene tree for the first time.
	private DefaultData _data;
	private VBoxContainer _VBoxContainer;	//Punem lectiile in acest container
	private PackedScene _courseitem;	//Scena pentru item
	private CourseItem _panel;	//Itemul propriu-zis, dupa ce setam indexul lectiei, numele, tagul,...	
	private bool normal;	//Filtrul pentru lectii normale
	private bool advanced;	//Filtrul pentru lectii avansate
	private bool special;	//Filtrul pentru lectii speciale
	private string namefilter;	//Filtrul de nume

	//Ultimele trei variante sunt pentru miscarea ferestrei
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	public override async void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_VBoxContainer = (VBoxContainer)GetNode("Panel/VBoxContainer/ScrollContainer/VBoxContainer");
		_courseitem = GD.Load<PackedScene>("res://Scenes/CourseItem.tscn");

		//Initializam filtrele
		normal = true;
		advanced = true;
		if(!_data.currentStats.Adv)	GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Avansat").ButtonPressed = true;
		special = true;
		if(!_data.currentStats.Spc) GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Special").ButtonPressed = true;
		namefilter = "";

		//Aici preluam lista cu toate lectiile din DefaultData
		int i=1;
		while(_data.lessonList.ContainsKey(i))
		{	AddItem(i);
			i++;
		}
		FilterCourses();

		if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	//Pentru miscarea ferestrei
		mousepos = GetViewport().GetMousePosition();
		var winpos = GetNode<Sprite2D>("Panel").Position;
		var newpos = Position;
		//Pozitia este raportata la centrul ferestrei
		newpos.X = Mathf.Clamp(Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1), 0, 1280);
		newpos.Y = Mathf.Clamp(Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1), 279, 920);
		if(inputgrab)
		{	GetNode<Sprite2D>("Panel").Position = newpos;
		}
	}
	//Aceasta functie este pentru adaugarea lectiilor.
	//Daca pui aceste instructiuni in structura while din _Ready(), toate CourseItem-urile o sa redirectioneze in Lesson_<ultimul index + 1> sau in _panel.lesson (Lesson_<ultimul index>)
	private void AddItem(int i)
	{	_panel =  _courseitem.Instantiate<CourseItem>();
		//Pur si simplu setam valorile din _panel pentru lectia dorita
		_panel.lesson = i;
		_panel.Name = "CourseItem" + i;
		_panel.lessonName = (string)_data.lessonList[i][0];
		_panel.GetNode<Label>("PanelContainer/Name").Text = _panel.lessonName;
		_panel.complete = (int)_data.currentStats.LessonCompletion[i];
		_panel.GetNode<ProgressBar>("PanelContainer/Percentage").Value = _panel.complete;
		_panel.lessonTag = (int)_data.lessonList[i][1];
		//Afiseaza tagul corespunzator tipului de lectie
		if(_panel.lessonTag == 1 || _panel.lessonTag == 3) _panel.GetNode<TextureRect>("PanelContainer/Adv").Show();
		if(_panel.lessonTag == 2 || _panel.lessonTag == 3) _panel.GetNode<TextureRect>("PanelContainer/Spc").Show();
		_VBoxContainer.AddChild(_panel);	//Adauga lectia in lista
		//Daca lectia cu nr i exista, conecteaza semnalul Pressed la functie. Altfel dezactiveaza
		if(ResourceLoader.Exists("res://Courses/Lesson_" + i + "/Lesson.tscn")) _panel.GetNode<Button>("Panel").Pressed += () => PanelPressed(i);
		else
		{	_panel.GetNode<Button>("Panel").Disabled = true;
			_panel.Modulate = new Color((float)0.6, (float)0.6, (float)0.6, (float)0.5);
		}
		GD.Print("Adaugat " + _panel.lessonName);
	}
	//Functie de afisare a lectiilor in functie de anumite filtre
	private void FilterCourses()
	{	CourseItem panel;
		for(int i = 0; i < GetNode("Panel/VBoxContainer/ScrollContainer/VBoxContainer").GetChildCount(); i++)
			GetNode("Panel/VBoxContainer/ScrollContainer/VBoxContainer").GetChild<CourseItem>(i).Visible = true;
		for(int i = 0; i < GetNode("Panel/VBoxContainer/ScrollContainer/VBoxContainer").GetChildCount(); i++)
		{	panel = GetNode("Panel/VBoxContainer/ScrollContainer/VBoxContainer").GetChild<CourseItem>(i);
			if(namefilter != "")
				if(!panel.GetNode<Label>("PanelContainer/Name").Text.Contains(namefilter))
					panel.Visible = false;
			if(!normal && panel.lessonTag == 0) panel.Visible = false;
			if(!advanced && (panel.lessonTag == 1 || panel.lessonTag == 3)) panel.Visible = false;
			if(!special && (panel.lessonTag == 2 || panel.lessonTag == 3)) panel.Visible = false;
		}
	}
	//Fucntie pentru momentul in care se schimba textul in LineEdit
	private void _on_line_edit_text_changed(string new_text)
	{
		namefilter = new_text;
		FilterCourses();
	}
	//Functie pentru butonul "Normal"
	private void _on_normal_toggled(bool toggled_on)
	{
		normal = !toggled_on;
		if(!toggled_on) GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Normal").Modulate = new Color(1, 1, 1, 1);
		else GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Normal").Modulate = new Color("646464");
		FilterCourses();
	}
	//Functie pentru butonul "Avansat"
	private void _on_avansat_toggled(bool toggled_on)
	{
		advanced = !toggled_on;
		if(!toggled_on) GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Avansat").Modulate = new Color(1, 1, 1, 1);
		else GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Avansat").Modulate = new Color("646464");
		FilterCourses();
	}
	//Functie pentru butonul "Special"
	private void _on_special_toggled(bool toggled_on)
	{
		special = !toggled_on;
		if(!toggled_on) GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Special").Modulate = new Color(1, 1, 1, 1);
		else GetNode<TextureButton>("Panel/VBoxContainer/Panel/HBoxContainer/Special").Modulate = new Color("646464");
		FilterCourses();
	}
	private void _on_back_pressed() => QueueFree();	//Inchide fereastra
	private void PanelPressed(int index)	//Incarca lectia
	{	_data.CurrentLesson = index;
		_data.LoadScene("res://Scenes/Lesson.tscn");
	}
	private void _on_drag_down()	//Cand partea de sus a ferestrei este apasata
	{	GD.Print("Hi");
		#if GODOT_ANDROID
			//Desi mousepos este preluat in _Proccess(), mousepos ramane aceeasi valoare dupa ce ecranul a fost atins
			//Presupun ca ii ia un frame ca sa proceseze noua pozitie, ceea ce nu este de ajuns pentru aceasta functie
			//Asa ca il actualizam acum
			mousepos = GetViewport().GetMousePosition();
		#endif
		var winpos = GetNode<Sprite2D>("Panel").Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		GD.Print(dif);
		inputgrab = true;
		GetParent().MoveChild(this, -1);
	}
	private void _on_drag_up()	//Cand partea de sus a ferestrei nu mai este apasata
	{	GD.Print("Bye");
		inputgrab = false;
	}
}
