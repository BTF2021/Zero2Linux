using Godot;
using System;
[Tool]
public partial class Settings : Node2D
{
	private DefaultData _data;
	private HSlider _slider;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_slider = (HSlider)GetNode("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeSlide");
		_slider.Value = _data.currentStats.VideoVolume;
		if(!_data.isvideoavailable)
		{	GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume").Modulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<HSlider>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeSlide").Editable = false;
		}
		if(_data.currentStats.FullScr) GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/Fullscreen/FullscreenButton").SetPressedNoSignal(true);
		if(_data.currentStats.VSync) GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/VSync/VSyncButton").SetPressedNoSignal(true);
		if(_data.currentStats.Adv) GetNode<CheckButton>("Settings/TabContainer/Lectii/VBoxContainer/Advanced/AdvancedButton").SetPressedNoSignal(true);
		if(_data.currentStats.Spc) GetNode<CheckButton>("Settings/TabContainer/Lectii/VBoxContainer/Special/SpecialButton").SetPressedNoSignal(true);
		if(_data.currentStats.ChkUpdates) GetNode<CheckButton>("Settings/TabContainer/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(_slider.Value == -11) GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Mut";
		else if(_slider.Value == 10) GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Max";
		else GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = (_slider.Value + 11).ToString();
	}

	private void _on_back_pressed()
	{	_data.WriteSave();
		QueueFree();
	}

	private void _on_fullscreen_pressed()
	{	GD.Print("Pressed");
		_data.currentStats.FullScr = !_data.currentStats.FullScr;
		if(_data.currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
	}

	private void _on_v_sync_button_pressed()
	{
		GD.Print("Pressed");
		_data.currentStats.VSync = !_data.currentStats.VSync;
		if(_data.currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}

	private void _on_video_volume_value_changed(float value)
	{	_data.currentStats.VideoVolume = value;
	}
	private void _on_advanced_pressed() => _data.currentStats.Adv = !_data.currentStats.Adv;
	private void _on_special_pressed() => _data.currentStats.Spc = !_data.currentStats.Spc;

	private void _on_updates_button_pressed()
	{
		GD.Print("Pressed");
		_data.currentStats.ChkUpdates = !_data.currentStats.ChkUpdates;
		GetNode<CheckButton>("Settings/TabContainer/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(_data.currentStats.ChkUpdates);
	}
	private void _on_delete_pressed() => GetNode<Node2D>("Confirma").Show();
}
