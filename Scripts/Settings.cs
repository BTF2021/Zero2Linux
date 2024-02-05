using Godot;
using System;
[Tool]
public partial class Settings : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) GetNode<CheckButton>("Settings/TabContainer/Video/Fullscreen/FullscreenButton").SetPressedNoSignal(true);
		if(DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled) GetNode<CheckButton>("Settings/TabContainer/Video/VSync/VSyncButton").SetPressedNoSignal(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_back_pressed()
	{	//Hide();
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}

	private void _on_fullscreen_pressed()
	{	GD.Print("Pressed");
		if(GetNode<CheckButton>("Settings/TabContainer/Video/Fullscreen/FullscreenButton").ButtonPressed) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		//GD.Print((int)ProjectSettings.GetSetting("display/window/size/mode"));
		//ProjectSettings.Save();
		//ProjectSettings.SaveCustom("res://override.cfg");
	}

	private void _on_v_sync_button_pressed()
	{
		GD.Print("Pressed");
		if(GetNode<CheckButton>("Settings/TabContainer/Video/VSync/VSyncButton").ButtonPressed) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		//GD.Print(ProjectSettings.GetSetting("display/window/vsync/vsync_mode"));
		//ProjectSettings.Save();
		//ProjectSettings.SaveCustom("res://override.cfg");
	}

	private void _on_antialiasing_selected(int index)
	{
		//Nu pare sa schimbe Msaa 2D
		/*
		GD.Print("Pressed " + (Viewport.Msaa)index);
		if(GetNode<OptionButton>("Settings/TabContainer/Video/Antialiasing/AntialiasingButton").ButtonPressed) GetViewport().Set("msaa_2d", index);
		GD.Print(GetViewport().Msaa2D); 
		Tot disabled  ^ */
	}
}
