//Pentru meniul de logare
using Godot;
using System;
using Newtonsoft.Json;

public partial class Logare : Node2D
{
	private DefaultData _data;
	private Button _profile;	//Iconul pentru user
	private System.Array _names;         //Vector pentru numele profilelor
	private int ListLenght;		//Lungimea listei de utilizatori
	private int Index;
	private bool profilespresent;        //Daca exista profile
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_profile = GetNode<Button>("Profiles/SidePanel/ScrollContainer/List/>,,<");	//butonul-model are un nume considerat de aplicatie invalid
		_names = new string[101];	//vectori pentru numele utilizatorilor
		CheckUsers();
		GetNode<Control>("Profiles").Visible = false;
		GetNode<Control>("NoUser").Visible = false;
		//Daca nu sunt utilizatori existenti, atunci trimite-i la ecranul de bun venit. Altfel, trimite-i la lista de utilizatori
		if(!profilespresent) GetNode<AnimationPlayer>("AnimationPlayer").Play("NoUser");
		else GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	GetNode<Label>("Profiles/Time").Text = Time.GetTimeStringFromSystem().Substring(0, 5);	//Ceasul
		GetNode<Label>("Profiles/Time/Date").Text = Time.GetDateStringFromSystem();	//Calendarul
	}
	public override void _Notification(int what)
	{	//Daca dai inapoi pe Android
		if (what == NotificationWMGoBackRequest)
        	GetTree().Quit();
	}
	//Butonul de inchidere a aplicatiei
	private void _on_quit_pressed() => GetTree().Quit(0);

	//Functie pentru primul buton, atunci cand nu exista utilizator
	private void _on_continue_pressed()
	{
		GetNode<TabContainer>("NoUser/TabContainer").CurrentTab = 1;
		GetNode<TabContainer>("NoUser/TabContainer").Modulate = new Color(1, 1, 1, 0);
		var pos = GetNode<TabContainer>("NoUser/TabContainer").Position;
		pos.X = pos.X + 90;
		GetNode<TabContainer>("NoUser/TabContainer").Position = pos;
		pos.X = pos.X - 90;
		var tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<TabContainer>("NoUser/TabContainer"), "modulate", new Color(1, 1, 1, 1), 0.25);
		tween.Parallel().TweenProperty(GetNode<TabContainer>("NoUser/TabContainer"), "position", pos, 0.25);
	}
	//Functie pentru ultimul buton, atunci cand nu exista utilizator
	private void _on_finish_pressed() => GetNode<AnimationPlayer>("AnimationPlayer").Play("Finish");
	//Butoanele pentru creearea unui nou utilizator.
	private void _on_createuser_pressed()
	{
		var scene = (ConfigUser)GD.Load<PackedScene>("res://Scenes/ConfigUser.tscn").Instantiate();
		scene.mode = 0;
		AddChild(scene);
	}
	//Functie apelata dupa ce fereastra a fost inchisa (stearsa din memorie prin QueueFree())
	public void ConfigFinished(int code)
	{
		switch(code)
		{	
			//Nu s-a creat/modificat utilizatorul
			case 0:
				break;
			//Utilizatorul a fost creat/modificat
			case 1:
				if(GetNode<Control>("NoUser").Visible)
				{	GetNode<TabContainer>("NoUser/TabContainer").CurrentTab = 2;
					GetNode<TabContainer>("NoUser/TabContainer").Modulate = new Color(1, 1, 1, 0);
					var pos = GetNode<TabContainer>("NoUser/TabContainer").Position;
					pos.X = pos.X + 90;
					GetNode<TabContainer>("NoUser/TabContainer").Position = pos;
					pos.X = pos.X - 90;
					var tween = GetTree().CreateTween();
					tween.TweenProperty(GetNode<TabContainer>("NoUser/TabContainer"), "modulate", new Color(1, 1, 1, 1), 0.25);
					tween.Parallel().TweenProperty(GetNode<TabContainer>("NoUser/TabContainer"), "position", pos, 0.25);
				}
				CheckUsers();
				break;
		}
	}
	//Pur si simplu pentru refacerea listei de utilizatori in meniu
	private void CheckUsers()
	{	for(int i = 0; i< GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").GetChildCount(); i++)
			if(GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").GetChild(i).Name != (StringName)">,,<")
				GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").GetChild(i).QueueFree();
		
		if(!_data.SaveExists()) profilespresent = false;
		else
		{	ListLenght = _data.GetSaves().Length;
			profilespresent = true;
			System.Array.Copy(_data.GetSaves(), 0, _names, 0, ListLenght);
			if(ListLenght == 100)
			{	if(_names.GetValue(99) != null)
				{	GetNode<TextureButton>("Profiles/ToolPanel/HBoxContainer/Create").Disabled = true;
					GetNode<TextureButton>("Profiles/ToolPanel/HBoxContainer/Create").TooltipText = "Nu se pot creea noi utilizatori.\nVa rugam sa stergeti un utilizator pentru a creea altul";
				}
				else GetNode<TextureButton>("Profiles/ToolPanel/HBoxContainer/Create").TooltipText = "Creeaza un utilizator nou";
			}
			else GetNode<TextureButton>("Profiles/ToolPanel/HBoxContainer/Create").TooltipText = "Creeaza un utilizator nou";
			//Adaugam iconitele utilizatorilor
			for(int i = 0; i< _names.Length; i++) 
				if(_names.GetValue(i) != null) AddProfiles(i);
		}
	}
	//Functia pentru logare
	private void logging(string name)
	{	_data.LogIn(name);
		for(int i = 0; i< GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").GetChildCount(); i++)
			GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").GetChild<Button>(i).Disabled = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Logging");
		var timer = GetTree().CreateTimer(0.6);
		timer.Timeout += () => GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}
	
	//Functie pentru adaugarea profilelor existente
	private void AddProfiles(int index)
	{	//Facem iconita
		Index = index;
		var button = (Button)_profile.Duplicate(8);
		var file = FileAccess.Open("user://" + (string)_names.GetValue(index) + "_save.json", FileAccess.ModeFlags.Read);
		if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
		stats content = JsonConvert.DeserializeObject<stats>(file.GetAsText());
		file.Close();
		button.Name = content.UsrName;
		button.TooltipText = button.TooltipText + content.UsrName;
		button.GetNode<Label>("Name").Text = content.UsrName;
		button.GetNode<Label>("BigLetter").Text = content.UsrName;
		button.GetNode<Sprite2D>("Bg").SelfModulate = content.FavColor;
		button.Show();
		GetNode<VBoxContainer>("Profiles/SidePanel/ScrollContainer/List").AddChild(button);
		button.Pressed += () => logging(content.UsrName);	//Conectam functia logging ca sa stim cine se logheaza
	}
}
