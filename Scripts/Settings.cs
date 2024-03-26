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
		_slider.Value = _data.videovolume;
		if(!_data.isvideoavailable)
		{	GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume").Modulate = new Color((float)0.6, (float)0.6, (float)0.6, 1);
			GetNode<HSlider>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeSlide").Editable = false;
		}
		if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/Fullscreen/FullscreenButton").SetPressedNoSignal(true);
		if(DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled) GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/VSync/VSyncButton").SetPressedNoSignal(true);
		if(_data.includeadvanced) GetNode<CheckButton>("Settings/TabContainer/Lectii/VBoxContainer/Advanced/AdvancedButton").SetPressedNoSignal(true);
		if(_data.includespecial) GetNode<CheckButton>("Settings/TabContainer/Lectii/VBoxContainer/Special/SpecialButton").SetPressedNoSignal(true);
		if(_data.checkupdates) GetNode<CheckButton>("Settings/TabContainer/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(_slider.Value == -11) GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Mut";
		else if(_slider.Value == 10) GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = "Max";
		else GetNode<Label>("Settings/TabContainer/Lectii/VBoxContainer/VideoVolume/VideoVolumeText").Text = (_slider.Value + 11).ToString();
	}

	private void _on_back_pressed()
	{	//Hide();
		QueueFree();
	}

	private void _on_fullscreen_pressed()
	{	GD.Print("Pressed");
		if(GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/Fullscreen/FullscreenButton").ButtonPressed) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
	}

	private void _on_v_sync_button_pressed()
	{
		GD.Print("Pressed");
		if(GetNode<CheckButton>("Settings/TabContainer/Grafica/VBoxContainer/VSync/VSyncButton").ButtonPressed) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}

	private void _on_video_volume_value_changed(float value)
	{	_data.videovolume = value;
	}
	private void _on_advanced_pressed() => _data.includeadvanced = !_data.includeadvanced;
	private void _on_special_pressed() => _data.includespecial = !_data.includespecial;

	private void _on_updates_button_pressed()
	{
		GD.Print("Pressed");
		_data.checkupdates = !_data.checkupdates;
		GetNode<CheckButton>("Settings/TabContainer/Altele/VBoxContainer/GetUpdates/GetUpdatesButton").SetPressedNoSignal(_data.checkupdates);
	}
}
