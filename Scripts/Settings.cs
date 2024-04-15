using Godot;
using System;
[Tool]
public partial class Settings : Control
{
	private DefaultData _data;
	private HSlider _slider;
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_slider = (HSlider)GetNode("Panel/Settings/Lectii/VBoxContainer/VideoVolume/VideoVolumeSlide");
		_slider.Value = _data.currentStats.VideoVolume;
		GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").Text = _data.currentStats.UsrName;
		GetNode<ColorPickerButton>("Panel/Settings/Altele/VBoxContainer/FavColour/ColorButton").Color = _data.currentStats.FavColor;
		if(!_data.isvideoavailable)
		{	GetNode<Label>("Panel/Settings/Lectii/VBoxContainer/VideoVolume").Modulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			_slider.Editable = false;
		}
		if(_data.currentStats.FullScr) GetNode<CheckButton>("Panel/Settings/Grafica/VBoxContainer/Fullscreen/FullscreenButton").SetPressedNoSignal(true);
		if(_data.currentStats.VSync) GetNode<CheckButton>("Panel/Settings/Grafica/VBoxContainer/VSync/VSyncButton").SetPressedNoSignal(true);
		if(_data.currentStats.Anims) GetNode<CheckButton>("Panel/Settings/Grafica/VBoxContainer/Animations/AnimationsButton").SetPressedNoSignal(true);
		if(_data.currentStats.Adv) GetNode<CheckButton>("Panel/Settings/Lectii/VBoxContainer/Advanced/AdvancedButton").SetPressedNoSignal(true);
		if(_data.currentStats.Spc) GetNode<CheckButton>("Panel/Settings/Lectii/VBoxContainer/Special/SpecialButton").SetPressedNoSignal(true);
		GetNode<Label>("Panel/Settings/Altele/VBoxContainer/Version").Text = "Versiune: " + (String)ProjectSettings.GetSetting("application/config/version");
		if(!((String)ProjectSettings.GetSetting("application/config/version")).Contains(_data.newversion[0])) GetNode<Label>("Panel/Settings/Altele/VBoxContainer/Version").Text = GetNode<Label>("Panel/Settings/Altele/VBoxContainer/Version").Text + ". Exista versiune noua";
		if(_data.currentStats.ChkUpdates) GetNode<CheckButton>("Panel/Settings/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(true);

		if(_data.currentStats.Anims)
		{
			var tween = GetTree().CreateTween();
			GetNode<Sprite2D>("Panel").Modulate = new Color(1, 1, 1, 0);
			var pos = Position;
			pos.X = 645;
			pos.Y = 339 - 25;
			GetNode<Sprite2D>("Panel").Position = pos;
			pos.Y = 339;
			tween.TweenProperty(GetNode<Sprite2D>("Panel"), "modulate", new Color(1, 1, 1, 1), 0.15);
			tween.Parallel().TweenProperty(GetNode<Sprite2D>("Panel"), "position", pos, 0.15);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(_slider.Value == -11) GetNode<Label>("Panel/Settings/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Mut";
		else if(_slider.Value == 10) GetNode<Label>("Panel/Settings/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Max";
		else GetNode<Label>("Panel/Settings/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = (_slider.Value + 11).ToString();

		mousepos = GetViewport().GetMousePosition();
		var winpos = GetNode<Sprite2D>("Panel").Position;
		var newpos = Position;
		newpos.X = Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1);
		newpos.Y = Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1);
		if(inputgrab)
		{	GetNode<Sprite2D>("Panel").Position = newpos;
		}
	}

	private void _on_back_pressed() => QueueFree();

	private void _on_fullscreen_pressed()
	{	_data.currentStats.FullScr = !_data.currentStats.FullScr;
		if(_data.currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		_data.WriteSave(_data.LoggedUser);
	}

	private void _on_v_sync_button_pressed()
	{	_data.currentStats.VSync = !_data.currentStats.VSync;
		if(_data.currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		_data.WriteSave(_data.LoggedUser);
	}

	private void _on_animations_pressed()
	{
		_data.currentStats.Anims = !_data.currentStats.Anims;
		_data.WriteSave(_data.LoggedUser);
	}

	private void _on_video_volume_value_changed(float value)
	{	_data.currentStats.VideoVolume = value;
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_advanced_pressed()
	{
		_data.currentStats.Adv = !_data.currentStats.Adv;
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_special_pressed()
	{
		_data.currentStats.Spc = !_data.currentStats.Spc;
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_version_pressed() => OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/releases");
	private void _on_updates_button_pressed()
	{	_data.currentStats.ChkUpdates = !_data.currentStats.ChkUpdates;
		GetNode<CheckButton>("Panel/Settings/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(_data.currentStats.ChkUpdates);
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_name_changed(string new_text)
	{	new_text = new_text.Replace(" ", "_");
		new_text = new_text.Replace("/", "_");
		new_text = new_text.Replace(".", "_");
		new_text = new_text.Replace(":", "_");
		new_text = new_text.Replace(",", "_");
		new_text = new_text.Replace("@", "_");
		new_text = new_text.Replace("'", "_");
		new_text = new_text.Replace("%", "_");
		GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").Text = new_text;
		GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").CaretColumn = new_text.Length;
	}
	private void _on_name_submitted(string new_text)
	{
		if(new_text.Length <= 0)
		{	GD.Print("Nu se poate creea utilizator: Nu exista nume");
			GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").SelfModulate = new Color(1, (float)0.05, (float)0.05, 1);
			var tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit"), "self_modulate", new Color(1, 1, 1, 1), 0.5);
			GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").Text = _data.currentStats.UsrName;
		}
		else
		{	var ok = true;
			var names = _data.GetSaves();
			for (int i = 0; i< names.Length; i++) if(new_text == (string)names.GetValue(i)) ok = false;
			GD.Print(ok);
			if(ok)
			{	GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").Text = new_text;
				_data.currentStats.UsrName = new_text;
				DirAccess.RenameAbsolute("user://" + _data.LoggedUser + "_save.json", "user://" + new_text + "_save.json");
				_data.LoggedUser = new_text;
				_data.WriteSave(_data.LoggedUser);
				GD.Print("Changed");
			}
			else 
			{	GD.Print("Nu se poate creea utilizator: Deja exista un user cu acel nume");
				GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").SelfModulate = new Color(1, (float)0.05, (float)0.05, 1);
				var tween = GetTree().CreateTween();
				tween.TweenProperty(GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit"), "self_modulate", new Color(1, 1, 1, 1), 0.5);
				GetNode<LineEdit>("Panel/Settings/Altele/VBoxContainer/Name/NameEdit").Text = _data.currentStats.UsrName;
			}
		}
	}
	private void _on_color_button(Color color)
	{
		_data.currentStats.FavColor = color;
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_delete_pressed() => AddChild((GD.Load<PackedScene>("res://Scenes/Confirma.tscn")).Instantiate());

	private void _on_drag_down()
	{	GD.Print("Hi");
		inputgrab = true;
		var winpos = GetNode<Sprite2D>("Panel").Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		GD.Print(dif);
	}
	private void _on_drag_up()
	{	GD.Print("Bye");
		inputgrab = false;
	}
}
