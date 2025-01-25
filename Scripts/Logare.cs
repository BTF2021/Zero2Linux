//Pentru meniul de logare
using Godot;
using System;
using Newtonsoft.Json;                           //Pentru culoarea cercului (in AddProfiles())

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
		_profile = GetNode<Button>("Profiles/Square/Vlist/List/>,,<");	//butonul-model are un nume considerat de aplicatie invalid
		_names = new string[101];	//vectori pentru numele utilizatorilor
		GetNode<Label>("Bg/Version").Text = "Zero2Linux Ver " + (String)ProjectSettings.GetSetting("application/config/version");
		GetNode<CanvasItem>("/root/Transition").Show(); //A se vedea funtia logging
		CheckUsers();
		GetNode<Control>("Profiles").Visible = false;
		GetNode<Control>("NoUser").Visible = false;
		//Daca nu sunt utilizatori existenti, atunci trimite-i la ecranul de bun venit. Altfel, trimite-i la lista de utilizatori
		if(!profilespresent)
		{
			GetNode<Control>("NoUser").Visible = true;
			GetNode<AnimationPlayer>("AnimationPlayer").Play("NoUser");
		}
		else
		{	GetNode<Control>("Profiles").Visible = true;
			GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	GetNode<Label>("Bg/Time").Text = Time.GetTimeStringFromSystem();	//Ceasul
	}
	public override void _Notification(int what)
	{	//Daca dai inapoi pe Android
		if (what == NotificationWMGoBackRequest)
        	GetTree().Quit();
	}
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
				GetNode<Control>("NoUser").Visible = false;
				GetNode<Control>("Profiles").Visible = true;
				CheckUsers();
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Profiles");
				break;
		}
	}
	//Pur si simplu pentru refacerea listei de utilizatori in meniu
	private void CheckUsers()
	{	//Stergem toate iconitele care nu sunt model
		for(int i = 0; i< GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChildCount(); i++) 
		{	for(int j = 0; j< GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild(i).GetChildCount(); j++)
			{	if(GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild(i).GetChild<Node>(j).Name != (StringName)">,,<")
					GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild(i).GetChild<Node>(j).QueueFree();
			}
		}
		if(!_data.SaveExists()) profilespresent = false;
		else
		{	ListLenght = _data.GetSaves().Length;
			profilespresent = true;
			if(ListLenght >= 13)GetNode<Panel>("Profiles/Panel").Visible = true;	//Daca lista trebuie afisata pe mai mult de o linie
			else GetNode<Panel>("Profiles/Panel").Visible = false;
			System.Array.Copy(_data.GetSaves(), 0, _names, 0, ListLenght);
			if(ListLenght == 100) 
			{	if(_names.GetValue(99) != null)
				{	GetNode<Button>("Profiles/Create").Disabled = true;
					GetNode<Button>("Profiles/Create").TooltipText = "Nu se pot creea noi utilizatori.\nVa rugam sa stergeti un utilizator pentru a creea altul";
				}
				else GetNode<Button>("Profiles/Create").TooltipText = "Creeaza un utilizator nou";
			}
			else GetNode<Button>("Profiles/Create").TooltipText = "Creeaza un utilizator nou";
			//Adaugam iconitele utilizatorilor
			for(int i = 0; i< _names.Length; i++) 
				if(_names.GetValue(i) != null) AddProfiles(i);
		}
	}
	//Functia pentru logare
	private void logging(string name)
	{	_data.LoggedUser = name;
		GD.Print("Logat in: " + name);
		
		//Dezactiveaza toate butoanele
		for(int i = 0; i< GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChildCount(); i++) 
			for(int j = 0; j< GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild(i).GetChildCount(); j++)
				GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild(i).GetChild<Button>(j).Disabled = true;
		
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Logging");
		var timer = GetTree().CreateTimer(0.3);
		//Aceasta instructiune (ChangeSceneToFile) este motivul pentru care exista o alta scena in Autoload
		//In timpul in care Godot a eliberat din memorie aceasta scena si incarca scena Main, o sa apara gri
		//De aceea aratam scena Transition in _Ready()
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
		//Aici este partea unde plasam iconita pe un anumit rand
		if(index % 6 == 0 && index > 0 && GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChildCount() != (index / 6) + 1)
		{	//Duplicam randul si-l punem ca child in VList, dar stergem iconitele din acea lista
			Node list = GetNode<HBoxContainer>("Profiles/Square/Vlist/List").Duplicate(8);
			int count = list.GetChildCount();
			for(int i = 0; i < count; i++) 
				list.GetChild(i).QueueFree();
			GetNode<VBoxContainer>("Profiles/Square/Vlist").AddChild(list);
		}
		GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild((index / 6)).AddChild(button);
		GD.Print("Adaugat " + content.UsrName + " cu indexul " + index + " in " + GetNode<VBoxContainer>("Profiles/Square/Vlist").GetChild((index / 6)).Name);
		button.Pressed += () => logging(content.UsrName);	//Conectam functia logging ca sa stim cine se logheaza
	}
}
