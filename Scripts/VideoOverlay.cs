using Godot;
using System;

public partial class VideoOverlay : Node2D
{
	double videocurrenttime, videotime;          //Simpilficate VideoStreamPlayer.StreamPosition si  VideoStreamPlayer.GetStreamLength()
	bool paused;                                 //Pentru a verifica daca utilizatorul a pus pauza
	bool dragging;                               //Pentru a verifica daca utilizatorul foloseste oricare slider (Mai mult pentru animatie)
	TimeSpan vct, vt;                            //Pentru formatare text
	private DefaultData _data;                   //DefaultData
	private VideoStreamPlayer _stream;           //Video
	private HSlider _slider;                     //Seekbar
	private HSlider _volume;                     //Volum
	private Timer _timer;
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_stream = (VideoStreamPlayer)GetNode("Panel/VideoStreamPlayer");
		_slider = (HSlider)GetNode("ControlsTint/Controls/Seekbar");
		_volume = (HSlider)GetNode("ControlsTint/Controls/Volum/Volume");
		_timer = GetNode<Timer>("Timer");
		_stream.Stop();
		videotime = _stream.GetStreamLength();
		_slider.MaxValue = videotime;
		_volume.Value = _data.currentStats.VideoVolume;
		if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) GetNode<TextureButton>("ControlsTint/Controls/Fullscr").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Winscr.png");
		vt = TimeSpan.FromSeconds(videotime);
		paused = true;
		dragging = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	videocurrenttime = _stream.StreamPosition;
		//Pentru volum. NU DA MAI MULT DE 10db
		if(_data.currentStats.VideoVolume != -11) _stream.VolumeDb = _data.currentStats.VideoVolume;
		else _stream.VolumeDb = -100;
		vct = TimeSpan.FromSeconds(videocurrenttime);
		if(_stream.IsPlaying())
			if(!_stream.Paused) _slider.Value = videocurrenttime;
			else _stream.StreamPosition = _slider.Value;
		else GetNode<TextureButton>("ControlsTint/Controls/Play").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Play.png");
		//Text
		if(_stream.GetStreamLength() >= 3600) GetNode<Label>("ControlsTint/Controls/Time").Text = vct.ToString("hh\\:mm\\:ss") + "/" + vt.ToString("hh\\:mm\\:ss");
		else GetNode<Label>("ControlsTint/Controls/Time").Text = vct.ToString("mm\\:ss") + "/" + vt.ToString("mm\\:ss");

		//Pnetru volum
		if(_volume.Value == -11) GetNode<Label>("ControlsTint/Controls/Volum/VolumeNum").Text = "Volum: Mut";
		else if(_volume.Value == 10)GetNode<Label>("ControlsTint/Controls/Volum/VolumeNum").Text = "Volum: Max";
		else GetNode<Label>("ControlsTint/Controls/Volum/VolumeNum").Text = "Volum: " + (_volume.Value + 11).ToString();
	}
	private void _on_play_pressed()
	{
		//Daca nu a dat play, porneste. Altfel pauza
		if(!_stream.IsPlaying())
		{	_stream.Stop();
			_stream.Play();
			GetNode<TextureButton>("ControlsTint/Controls/Play").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Pause.png");
			_on_mouse_exited();
		}
		else _stream.Paused = !_stream.Paused;
		paused = _stream.Paused;
		if(!_stream.Paused) GetNode<TextureButton>("ControlsTint/Controls/Play").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Pause.png");
		else GetNode<TextureButton>("ControlsTint/Controls/Play").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Play.png");
	}
	//Scade StreamPosition cu 10s
	private void _on_backward_pressed()
	{	if(videocurrenttime < 10) _stream.StreamPosition = 0;
		else _stream.StreamPosition = _stream.StreamPosition - 10;
	}
	//Creste StreamPosition cu 10s
	private void _on_forward_pressed()
	{	if(videocurrenttime > _stream.GetStreamLength() - 10) _stream.StreamPosition = _stream.GetStreamLength();
		else _stream.StreamPosition = _stream.StreamPosition + 10;
	}
	private void _on_back_pressed()
	{	_slider.Value = 0; //Daca nu-l setezi la 0, cand iesi si reintrii, o sa inceapa de unde a ramas
		_stream.Stop();
		_stream.Paused = false;
		GetNode<TextureButton>("ControlsTint/Controls/Play").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Play.png");
		GetNode<TextureButton>("ControlsTint/Controls/Play").SetPressedNoSignal(false);
		QueueFree();
	}
	//Cand tii apasat pe Seekbar...
	private void _on_drag_started()
	{	_stream.Paused = true;
		dragging = true;
		_timer.Stop();
	}
	private void _on_volume_drag_started()
	{	dragging = true;
		_timer.Stop();
	}
	//...si cand te opresti
	private void _on_drag_ended(bool value_changed)
	{	if(!paused)_stream.Paused = false;
		dragging = false;
	}

	private void _on_volume_changed(float value)
	{	dragging = false;
		_data.currentStats.VideoVolume = value;
		_data.WriteSave(_data.LoggedUser);
	}
	//Butonul de fullscreen
	private void _on_fullscr_pressed()
	{	if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
		{	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			_data.currentStats.FullScr = true;
			GetNode<TextureButton>("ControlsTint/Controls/Fullscr").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Winscr.png");
		}
		else
		{	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			_data.currentStats.FullScr = false;
			GetNode<TextureButton>("ControlsTint/Controls/Fullscr").TextureNormal = GD.Load<CompressedTexture2D>("res://Sprites/Fullscr.png");
		}
		_data.WriteSave(_data.LoggedUser);
	}
	private void _on_timer_timeout()
	{	var pos = Position;
		pos.X = 0;
		pos.Y = 50;
		var tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(GetNode<HBoxContainer>("ControlsTint/Controls"), "position", pos, 0.25);
		pos.X = -50;
		pos.Y = 0;
		tween.Parallel().TweenProperty(GetNode<TextureButton>("BackTint/Back"), "position", pos, 0.25);
		tween.Parallel().TweenProperty(GetNode<ColorRect>("ControlsTint"), "self_modulate", new Color(1, 1, 1, 0), 0.25);
		tween.Parallel().TweenProperty(GetNode<ColorRect>("BackTint"), "self_modulate", new Color(1, 1, 1, 0), 0.25);
	}
	private async void _on_mouse_exited()
	{	GD.Print("Bye");
		if(!paused && !dragging) _timer.Start(1.5);
	}
	private void _on_mouse_entered()
	{	GD.Print("Hi");
		_timer.Stop();
		var pos = Position;
		pos.X = 0;
		pos.Y = 8;
		var tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(GetNode<HBoxContainer>("ControlsTint/Controls"), "position", pos, 0.25);
		pos.X = -5;
		pos.Y = 0;
		tween.Parallel().TweenProperty(GetNode<TextureButton>("BackTint/Back"), "position", pos, 0.25);
		tween.Parallel().TweenProperty(GetNode<ColorRect>("ControlsTint"), "self_modulate", new Color(1, 1, 1, 1), 0.25);
		tween.Parallel().TweenProperty(GetNode<ColorRect>("BackTint"), "self_modulate", new Color(1, 1, 1, 1), 0.25);
	}
}
