using Godot;
using System;

public partial class VideoOverlay : Node2D
{
	double videocurrenttime, videotime;          //Simpilficate VideoStreamPlayer.StreamPosition si  VideoStreamPlayer.GetStreamLength()
	bool paused;                                 //Pentru a verifica daca utilizatorul a pus pauza
	TimeSpan vct, vt;                            //Pentru formatare text
	private DefaultData _data;                   //DefaultData
	private VideoStreamPlayer _stream;           //Video
	private HSlider _slider;                     //Seekbar
	private HSlider _volume;                     //Volum
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		_stream = (VideoStreamPlayer)GetNode("Panel/VideoStreamPlayer");
		_slider = (HSlider)GetNode("Controls/Seekbar");
		_volume = (HSlider)GetNode("Controls/Volum/Volume");
		_stream.Stop();
		videotime = _stream.GetStreamLength();
		_slider.MaxValue = videotime;
		_volume.Value = _data.videovolume;
		if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) GetNode<TextureButton>("Controls/Fullscr").TextureNormal = GD.Load<Texture2D>("res://Sprites/winscr.png");
		vt = TimeSpan.FromSeconds(videotime);
		paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	videocurrenttime = _stream.StreamPosition;
		//Pentru volum. NU DA MAI MULT DE 10db
		if(_data.videovolume != -11) _stream.VolumeDb = _data.videovolume;
		else _stream.VolumeDb = -100;
		vct = TimeSpan.FromSeconds(videocurrenttime);
		if(_stream.IsPlaying())
			if(!_stream.Paused) _slider.Value = videocurrenttime;
			else _stream.StreamPosition = _slider.Value;
		else GetNode<TextureButton>("Controls/Play").TextureNormal = GD.Load<Texture2D>("res://Sprites/Play.png");
		//Text
		if(_stream.GetStreamLength() >= 3600) GetNode<Label>("Controls/Time").Text = vct.ToString("hh\\:mm\\:ss") + "/" + vt.ToString("hh\\:mm\\:ss");
		else GetNode<Label>("Controls/Time").Text = vct.ToString("mm\\:ss") + "/" + vt.ToString("mm\\:ss");

		//Pnetru volum
		if(_volume.Value == -11) GetNode<Label>("Controls/Volum/VolumeNum").Text = "Volum: Mut";
		else if(_volume.Value == 10)GetNode<Label>("Controls/Volum/VolumeNum").Text = "Volum: Max";
		else GetNode<Label>("Controls/Volum/VolumeNum").Text = "Volum: " + (_volume.Value + 11).ToString();
	}
	private void _on_play_pressed()
	{
		//Daca nu a dat play, porneste. Altfel pauza
		if(!_stream.IsPlaying())
		{	_stream.Stop();
			_stream.Play();
			GetNode<TextureButton>("Controls/Play").TextureNormal = GD.Load<Texture2D>("res://Sprites/Pause.png");
		}
		else _stream.Paused = !_stream.Paused;
		paused = _stream.Paused;
		if(!_stream.Paused) GetNode<TextureButton>("Controls/Play").TextureNormal = GD.Load<Texture2D>("res://Sprites/Pause.png");
		else GetNode<TextureButton>("Controls/Play").TextureNormal = GD.Load<Texture2D>("res://Sprites/Play.png");
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
		GetNode<TextureButton>("Controls/Play").TextureNormal = GD.Load<Texture2D>("res://Sprites/Play.png");
		GetNode<TextureButton>("Controls/Play").SetPressedNoSignal(false);
		QueueFree();
	}
	//Cand tii apasat pe Seekbar...
	private void _on_drag_started()
	{	_stream.Paused = true;
	}
	//...si cand te opresti
	private void _on_drag_ended(bool value_changed)
	{	if(!paused)_stream.Paused = false;
	}
	private void _on_volume_changed(float value)
	{	_data.videovolume = value;
	}
	//Butonul de fullscreen
	private void _on_fullscr_pressed()
	{	if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
		{	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			GetNode<TextureButton>("Controls/Fullscr").TextureNormal = GD.Load<Texture2D>("res://Sprites/winscr.png");
		}
		else
		{	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			GetNode<TextureButton>("Controls/Fullscr").TextureNormal = GD.Load<Texture2D>("res://Sprites/fullscr.png");
		}
	}
}
