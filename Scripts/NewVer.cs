using Godot;
using System;
using System.Text;

public partial class NewVer : Control
{	
	private DefaultData _data;
	private HttpRequest request;
	private bool debug;
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	private bool dwn;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		request = new HttpRequest();
		AddChild(request);
		request.RequestCompleted += OnRequestCompleted;                  //Cand se apeleaza Request => functia OnRequestCompleted
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

		#if GODOT_LINUXBSD || GODOT_WINDOWS
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Text = (GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Text).TrimEnd('.') + " (Un fisier zip o sa apara in acelasi folder cu executabilul)";
		#elif GODOT_ANDROID
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Text = (GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Text).TrimEnd('.') + " (Necesita permisiunea de a instala noua aplicatie. Actualizarea va fi salvata in folderul Download)";
		#endif
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	if(dwn) 
		{	GetNode<TextureProgressBar>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar").Value = request.GetDownloadedBytes();
			GetNode<TextureProgressBar>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar").MaxValue = request.GetBodySize();
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar/Text").Text = Math.Round(((float)(request.GetDownloadedBytes()) / 1048576), 1) + " mb / " + Math.Round(((float)(request.GetBodySize()) / 1048576), 1) + " mb";
			if(request.GetBodySize() == -1)
			{	GetNode<TextureProgressBar>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar").MaxValue = 100;
				GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Se pregateste descarcarea...";
			}
		}
		
		mousepos = GetViewport().GetMousePosition();
		var winpos = GetNode<Sprite2D>("Panel").Position;
		var newpos = Position;
		newpos.X = Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1);
		newpos.Y = Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1);
		if(inputgrab)
		{	GetNode<Sprite2D>("Panel").Position = newpos;
		}
	}
	private void _on_skip_pressed()
	{	QueueFree();
	}

	private void _on_cancel_pressed()
	{	request.CancelRequest();
		switch(OS.GetName())
		{	case "Windows":
				DirAccess.RemoveAbsolute(OS.GetExecutablePath().GetBaseDir() + "/Zero2Linux v" + (string)_data.newversion[0] + ".zip");
				break;
			case "Linux":
				DirAccess.RemoveAbsolute(OS.GetExecutablePath().GetBaseDir() + "/Zero2Linux v" + (string)_data.newversion[0] + ".zip");
				break;
			case "Android":
				DirAccess.RemoveAbsolute("/storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk");
				break;
		}
		GetNode("/root").GetChild(-1).EmitSignal("Download", 0);
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Visible = true;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Visible = false;
		GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download").Visible = false;
		GetNode<Button>("Panel/Panel/ScrollContainer/VBoxContainer/HBoxContainer/Download").Disabled = false;
		GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/HBoxContainer/Download").Show();
		GetNode<TextureButton>("Panel/Skip").Disabled = false;
		GetNode<CanvasItem>("Panel/Skip").Visible = true;
	}

	private void _on_go_pressed()
	{	OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/releases");
	}
	private void _on_debug_pressed()
	{	debug = !debug;
		GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Log").Visible = debug;
	}
	public void _on_download_pressed()
	{	GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download").Visible = true;
		GetNode<TextureProgressBar>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar").MaxValue = 100;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title4").Visible = false;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Visible = true;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text = "\nDescarcarea este in desfasurare. Va rugam sa nu inchideti aplicatia";
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Pregatire descarcarea...";

		GetNode<Button>("Panel/Panel/ScrollContainer/VBoxContainer/HBoxContainer/Download").Disabled = true;
		GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/HBoxContainer/Download").Hide();
		GetNode<TextureButton>("Panel/Skip").Disabled = true;
		GetNode<CanvasItem>("Panel/Skip").Visible = false;
		GetNode("/root").GetChild(-1).EmitSignal("Download", 1);
		
		//Mai intai construim linkul catre Github in functie de versiunea noua, platforma si daca este Full sau Lite
		StringBuilder linktosite = new StringBuilder("https://github.com/BTF2021/Zero2Linux/releases/download/v" + (string)_data.newversion[0] + "/Z2L");
		request.DownloadFile = OS.GetExecutablePath().GetBaseDir() + "/Zero2Linux v" + (string)_data.newversion[0] + ".zip";
		switch(OS.GetName())
		{	case "Windows":
				linktosite.Append(".Win.");
				break;
			case "Linux":
				linktosite.Append(".Linux.");
				break;
			case "Android":
				linktosite = new StringBuilder("https://github.com/BTF2021/Zero2Linux/releases/download/v" + (string)_data.newversion[0] + "/Zero2Linux.apk");
				request.DownloadFile = "/storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk";
				break;
		}

		#if GODOT_LINUXBSD || GODOT_WINDOWS
			if(_data.isvideoavailable) linktosite.Append("Full.zip");
			else linktosite.Append("Lite.zip");
		#endif
		GD.Print(linktosite.ToString());

		//Descarcam arhiva/apkul
		request.RequestRaw(linktosite.ToString(), new string[] {});
		dwn = true;
		GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Log/LogText").Text = "Descarcare de pe site-ul " + linktosite;
	}
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{	dwn = false;
		if(result == 0)
		{	GetNode<TextureButton>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/Cancel").Disabled = true;
			GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/Cancel").Hide();
			GD.Print("Descarcat");
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Log/LogText").Text = "Descarcat";

			//In principal, procesul pentru toate platformele este acelasi
			//Cream un fisier in care stocam rezultatul descarcarii (adica variabila body), iar, in cazul platformei Android, putem executa direct fisierul
			#if GODOT_ANDROID
				GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Executarea apk-ului...";
				GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Log/LogText").Text = "Executarea apk-ului /storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk";
				//NOTA: aplicatia necesita permisiunea android.permission.REQUEST_INSTALL_PACKAGES in custom permissions
				OS.ShellOpen("/storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk");
				
				//Desktop
				//Tbh, ar fi fost tare daca ar fi aplicat update-ul direct in loc sa puna arhiva intr-un folder.
				//Am incercat sa fac acest lucru, dar ar fi dat crash daca incerci sa suprascrii orice fisier din folder (data_Zero2Linux_ ...) sau
				//orice alt fisier inafara de executabil.
				//Executabilul in sine nu schimba versiunea si tot o sa ramana la versiunea veche, deci nu este de ajutor.
				//Mi-ar trebui o aplicatie extra pentru actualizari pentru desktop, dar nu are sens daca asta este tot ce face.
				//In viitor s-ar putea sa dezvolt aceasta aplicatie extra, dar pana atunci, avem acest cod de mai jos.
			#endif
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Gata :D";
			GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text = "\nDescarcarea a fost finalizata cu succes.";

			#if GODOT_LINUXBSD || GODOT_WINDOWS
				GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text = (GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text).TrimEnd('.') + " Noua versiune ar trebui sa fie in acelasi folder cu executabilul";
			#elif GODOT_ANDROID
				GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text = (GetNode<Label>("Panel/Panel/ScrollContainer/VBoxContainer/Title5").Text).TrimEnd('.') + " In cazul in care inca nu ai instalat aplicatia, noua versiune este in folderul Download";
			#endif
			GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Log").Visible = false;
			GetNode<CanvasItem>("Panel/Panel/ScrollContainer/VBoxContainer/Download/Debug").Visible = false;
			GetNode<TextureButton>("Panel/Skip").Disabled = false;
			GetNode<CanvasItem>("Panel/Skip").Visible = true;
			debug = false;
			GetNode("/root").GetChild(1).EmitSignal("Download", 0);
		}
		else
		{	GD.Print("Eroare HttpRequest: " + (HttpRequest.Result)result);
			return;
		}
		request.QueueFree();       //Nu mai e nevoie de HttpRequest. Putem sterge
	}
	private void _on_drag_down()
	{	GD.Print("Hi");
		#if GODOT_ANDROID
			//Desi mousepos este preluat in _Proccess(), mousepos ramane aceeasi valoare dupa ce ecranul a fost atins
			//Presupun ca ii ia un frame ca sa proceseze noua pozitie, ceea ce nu este de ajuns pentru aceasta functie
			//Asa ca il actualizam acum mousepos
			mousepos = GetViewport().GetMousePosition();
		#endif
		var winpos = GetNode<Sprite2D>("Panel").Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		GD.Print(dif);
		inputgrab = true;
	}
	private void _on_drag_up()
	{	GD.Print("Bye");
		inputgrab = false;
	}
}
