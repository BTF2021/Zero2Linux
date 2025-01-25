//Pentru fereastra de setari
using Godot;
using System;
public partial class Settings : Control
{
	private DefaultData _data;
	private HSlider _slider;	//Sliderul pentru volum
	public int ypos;		//Variabila pentru semnalul _on_category_toggled sa redirectioneze la containerul potrivit
	//[TODO] Poate merge si fara variabila de mai sus. Asa poate mai reducem din numarul de linii
	public Godot.Collections.Array<int> containerpos;		//Vector pentru pozitiile containerelor in ScrollContainer-ul "Settings"
	bool animated;		//Pentru a ne asigura ca nu se schimba starea butoanelor in timpul tranzitiei catre o anumita categorie
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_slider = (HSlider)GetNode("Panel/Settings/VBoxContainer/Lectii/VideoVolume/VideoVolumeSlide");
		_slider.Value = _data.currentStats.VideoVolume;
		GetNode<Label>("Panel/Settings/VBoxContainer/Cont/Preview/Name").Text = _data.currentStats.UsrName;
		GetNode<Label>("Panel/Settings/VBoxContainer/Cont/Preview/BigLetter").Text = _data.currentStats.UsrName;
		GetNode<Sprite2D>("Panel/Settings/VBoxContainer/Cont/Preview/Bg").SelfModulate = _data.currentStats.FavColor;

		//Daca nu putem reda videoclipurile, ascundem tot ce este legat de videoclipuri
		if(!_data.isvideoavailable)
		{	GetNode<Label>("Panel/Settings/VBoxContainer/Lectii/VideoVolume").QueueFree();
			GetNode<Label>("Panel/Settings/VBoxContainer/Lectii/VideoVolume").Modulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<RichTextLabel>("Panel/Settings/Despre/VBoxContainer/Notice").Visible = false;
			_slider.Editable = false;
		}

		#if GODOT_ANDROID
			GetNode<Label>("Panel/Settings/VBoxContainer/Grafica/Fullscreen").QueueFree();
		#endif
		
		//Pur si simplu verificam daca optiunile sunt true
		if(_data.currentStats.FullScr) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Grafica/Fullscreen/FullscreenButton").SetPressedNoSignal(true);
		if(_data.currentStats.VSync) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Grafica/VSync/VSyncButton").SetPressedNoSignal(true);
		if(_data.currentStats.Anims) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Grafica/Animations/AnimationsButton").SetPressedNoSignal(true);
		if(_data.currentStats.Adv) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Lectii/Advanced/AdvancedButton").SetPressedNoSignal(true);
		if(_data.currentStats.Spc) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Lectii/Special/SpecialButton").SetPressedNoSignal(true);
		if(_data.currentStats.QNumOnly) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Lectii/ShowNumOnlyTest/SNTButton").SetPressedNoSignal(true);
		if(_data.currentStats.AdvQ) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Lectii/IncludeAdvQ/IAQButton").SetPressedNoSignal(true);
		GetNode<Label>("Panel/Settings/VBoxContainer/Despre/Version").Text = "Versiune: " + (String)ProjectSettings.GetSetting("application/config/version");
		if(_data.currentStats.ChkUpdates) GetNode<CheckButton>("Panel/Settings/VBoxContainer/Cont/GetUpdates/GetUpdatesButton").SetPressedNoSignal(true);

		//Initializa butoanele de pe partea stanga (conectam semnalul, facem vectorul de pozitii ale containerelor)
		animated = false;
		containerpos = new Godot.Collections.Array<int>();
		containerpos.Add(0);
		for(int i = 1; i < GetNode("Panel/SidePanel/VBoxContainer").GetChildCount(); i++) InitializeSideButtons(i, ypos);

		//Animatie
		if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	//Schimbam starea butoanelor in functie de valoarea ScrollBar-ului
		//Acest lucru trebuie sa se intample cand NU apasam pe unul dintre butoanele de pe partea din stanga
		if(!animated)
		{
			var scroll = GetNode<ScrollContainer>("Panel/Settings/").ScrollVertical;
			for(int i = 0; i < containerpos.Count; i++)
				if(scroll >= containerpos[i] && scroll < containerpos[i+1])
				{	//Ne asiguram ca numai un singur buton ramane apasat
					for(int j = 1; j < GetNode("Panel/SidePanel/VBoxContainer").GetChildCount(); j++)
					{
						if(GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(j).ButtonPressed == true && j != i+1)
						GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(j).ButtonPressed = false;
					else if(j == i+1) GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(j).ButtonPressed = true;
				}
			}
		}
		
		//Pentru miscarea ferestrei
		mousepos = GetViewport().GetMousePosition();
		var winpos = GetNode<Sprite2D>("Panel").Position;
		var newpos = Position;
		//Pozitia este raportata la centrul ferestrei
		newpos.X = Mathf.Clamp(Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1), 0, 1280);
		newpos.Y = Mathf.Clamp(Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1), 230, 860);
		if(inputgrab)
		{	GetNode<Sprite2D>("Panel").Position = newpos;
		}
	}

	//Functie pentru conectarea semnalului la buton
	//Puteam sa facem acelasi lucru si in _Ready daca nu redirectionau butoanele la acelasi container (A se vedea Courses.cs)
	private void InitializeSideButtons(int index, int position)
	{
		GD.Print(GetNode("Panel/SidePanel/VBoxContainer").GetChild(index).Name);
		GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(index).Pressed += () => _on_category_toggled(index, position);
		var pos = GetNode("Panel/Settings/VBoxContainer").GetChild<VBoxContainer>(index-1).Size;
		ypos += (int)pos.Y;
		containerpos.Add(ypos);
	}

	//Semnalul butonului
	private async void _on_category_toggled(int index, int position)
	{	
		//Daca am schimbat starea butonului in "Pressed"
		if(GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(index).ButtonPressed == true) 
		{	
			//Ne asiguram ca nu mai exista alte butoane apasate
			for(int i = 1; i < GetNode("Panel/SidePanel/VBoxContainer").GetChildCount(); i++)
			{
				if(GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(i).ButtonPressed == true && i != index)
					GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(i).ButtonPressed = false;
			}
			//Incepem tranzitia catre categoria dorita
			animated = true;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<ScrollContainer>("Panel/Settings/"), "scroll_vertical", position, 0.2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			await ToSignal(tween, Tween.SignalName.Finished);
			animated = false;
		}
		//Nu elibera butonul (Nu-l schimba starea din "Pressed")
		//Readucem utilizatorul la categoria dorita
		else 
		{
			GetNode("Panel/SidePanel/VBoxContainer").GetChild<Button>(index).ButtonPressed = true;
			//Incepem tranzitia catre categoria dorita
			animated = true;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<ScrollContainer>("Panel/Settings/"), "scroll_vertical", position, 0.2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			await ToSignal(tween, Tween.SignalName.Finished);
			animated = false;
		}
	}
	private void _on_back_pressed() => QueueFree();
	//Fullscreen
	private void _on_fullscreen_pressed()
	{	_data.currentStats.FullScr = !_data.currentStats.FullScr;
		if(_data.currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		_data.WriteSave(_data.LoggedUser);
	}
	//VSync
	private void _on_v_sync_button_pressed()
	{	_data.currentStats.VSync = !_data.currentStats.VSync;
		if(_data.currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		_data.WriteSave(_data.LoggedUser);
	}
	//Animatii
	private void _on_animations_pressed()
	{
		_data.currentStats.Anims = !_data.currentStats.Anims;
		_data.WriteSave(_data.LoggedUser);
	}
	//Volumul videoclipurilor
	private void _on_video_volume_value_changed(float value)
	{	_data.currentStats.VideoVolume = value;
		_data.WriteSave(_data.LoggedUser);
	}
	//Daca afisam lectiile avansate sau nu
	private void _on_advanced_pressed()
	{
		_data.currentStats.Adv = !_data.currentStats.Adv;
		_data.WriteSave(_data.LoggedUser);
	}
	//Daca afisam lectiile speciale sau nu
	private void _on_special_pressed()
	{
		_data.currentStats.Spc = !_data.currentStats.Spc;
		_data.WriteSave(_data.LoggedUser);
	}
	//Daca ascundem raspunsurile corecte si gresite in test sau nu
	private void _on_snt_button_pressed()
	{
		_data.currentStats.QNumOnly = !_data.currentStats.QNumOnly;
		_data.WriteSave(_data.LoggedUser);
	}
	//Daca includem lectiile avansate in chestionare sau nu
	private void _on_iaq_button_pressed()
	{
		_data.currentStats.AdvQ = !_data.currentStats.AdvQ;
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_github_pressed() => OS.ShellOpen("https://github.com/BTF2021/Zero2Linux");
	private void _on_issue_pressed() => OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/issues");
	//Daca verificam pentru o versiune noua sau nu
	private void _on_updates_button_pressed()
	{	_data.currentStats.ChkUpdates = !_data.currentStats.ChkUpdates;
		GetNode<CheckButton>("Panel/Settings/VBoxContainer/Cont/GetUpdates/GetUpdatesButton").SetPressedNoSignal(_data.currentStats.ChkUpdates);
		_data.WriteSave(_data.LoggedUser);
	}
	//Butonul pentru modificarea utilizatorului
	private void _on_modify_save_pressed()
	{
		var scene = (ConfigUser)GD.Load<PackedScene>("res://Scenes/ConfigUser.tscn").Instantiate();
		scene.mode = 1;
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
				GetNode<Label>("Panel/Settings/VBoxContainer/Cont/Preview/Name").Text = _data.currentStats.UsrName;
				GetNode<Label>("Panel/Settings/VBoxContainer/Cont/Preview/BigLetter").Text = _data.currentStats.UsrName;
				GetNode<Sprite2D>("Panel/Settings/VBoxContainer/Cont/Preview/Bg").SelfModulate = _data.currentStats.FavColor;
				break;
		}
	}
	//Pentru stergerea progresului. Apare o fereastra de confirmare
	private void _on_delete_pressed()
	{	var scene = (Confirm)GD.Load<PackedScene>("res://Scenes/Confirm.tscn").Instantiate();
		scene.reason = 0;
		AddChild(scene);
	}
	//Pentru linkuri
	private void _on_notice_meta_clicked(Variant meta) => OS.ShellOpen((string)meta);

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
