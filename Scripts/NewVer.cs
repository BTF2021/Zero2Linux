//Pentru fereastra care anunta ca o noua versiune a programului este disponibila
using Godot;
using System;
using System.Text;

public partial class NewVer : Node2D
{	
	private DefaultData _data;
	private HttpRequest request;	//Request http pentru descarcare
	private bool debug;		//Daca vrei extra detalii privind descarcarea sau nu
	private bool dwn;	//Daca se descarca sau nu
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		request = new HttpRequest();
		AddChild(request);
		request.RequestCompleted += OnRequestCompleted;                  //Cand se apeleaza Request => functia OnRequestCompleted

		#if GODOT_LINUXBSD || GODOT_WINDOWS
			GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title4").Text = (GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title4").Text).TrimEnd('.') + " (Un fisier zip o sa apara in acelasi folder cu executabilul)";
		#elif GODOT_ANDROID
			GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title4").Text = (GetNode<Label>("Window/Panel/ScrollContainer/VBoxContainer/Title4").Text).TrimEnd('.') + " (Necesita permisiunea de a instala noua aplicatie. Actualizarea va fi salvata in folderul Download)";
		#endif
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	//Daca se descarca
		if(dwn) 
		{	GetNode<TextureProgressBar>("Window/Download/VBoxContainer/Download/Bar/ProgressBar").Value = request.GetDownloadedBytes();
			GetNode<TextureProgressBar>("Window/Download/VBoxContainer/Download/Bar/ProgressBar").MaxValue = request.GetBodySize();
			GetNode<Label>("Window/Download/VBoxContainer/Download/Bar/ProgressBar/Text").Text = Math.Round(((float)(request.GetDownloadedBytes()) / 1048576), 1) + " MiB / " + Math.Round(((float)(request.GetBodySize()) / 1048576), 1) + " MiB";
			if(request.GetBodySize() == -1)
			{	GetNode<TextureProgressBar>("Window/Download/VBoxContainer/Download/Bar/ProgressBar").MaxValue = 100;
				GetNode<Label>("Window/Download/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Se pregateste descarcarea...";
			}
		}
	}
	//functie pentru anularea descarcarii
	private void _on_cancel_pressed()
	{	request.CancelRequest();
		switch(OS.GetName())	//Stergem fisierul descarcat
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
		GetNode<Panel>("Window/Download").Position = Position with { X = -372, Y = 250 };
		GetNode<Panel>("Window/Download").Size = Scale with { X = 745, Y = 55 };
		GetNode<Panel>("Window/Panel").Position = Position with { X = -372, Y = -258 };
		GetNode<Panel>("Window/Panel").Size = Scale with { X = 744, Y = 503 };
		GetNode<ScrollContainer>("Window/Panel/ScrollContainer").Size = Scale with { X = 752, Y = 504 };

		GetNode("/root").GetChild(-1).EmitSignal("Download", 0);
		GetNode<Label>("Window/Download/VBoxContainer/Info").Visible = false;
		GetNode<CanvasItem>("Window/Download/VBoxContainer/Download").Visible = false;
		GetNode<Button>("Window/Download/VBoxContainer/HBoxContainer/Download").Disabled = false;
		GetNode<CanvasItem>("Window/Download/VBoxContainer/HBoxContainer/Download").Show();
		GetTree().Paused = false;
		GetNode<Window>("Window").ExitButton(true);
	}
	//Functie pentru linkul catre pagina de Github
	private void _on_go_pressed()
	{	OS.ShellOpen("https://github.com/BTF2021/Zero2Linux/releases");
	}
	//Functie pentru checkboxul de debug
	private void _on_debug_pressed()
	{	debug = !debug;
		GetNode<CanvasItem>("Window/Download/VBoxContainer/Download/Log").Visible = debug;
	}
	//Functie pentru butonul de descarcare
	public void _on_download_pressed()
	{	GetNode<CanvasItem>("Window/Download/VBoxContainer/Download").Visible = true;
		GetNode<TextureProgressBar>("Window/Download/VBoxContainer/Download/Bar/ProgressBar").MaxValue = 100;
		GetNode<Label>("Window/Download/VBoxContainer/Info").Visible = true;
		GetNode<Label>("Window/Download/VBoxContainer/Info").Text = "Descarcarea este in desfasurare. Va rugam sa nu inchideti aplicatia";
		GetNode<Label>("Window/Download/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Pregatire descarcarea...";

		GetNode<Panel>("Window/Download").Position = Position with { X = -372, Y = 105 };
		GetNode<Panel>("Window/Download").Size = Scale with { X = 745, Y = 200 };
		GetNode<Panel>("Window/Panel").Position = Position with { X = -372, Y = -253 };
		GetNode<Panel>("Window/Panel").Size = Scale with { X = 745, Y = 353 };
		GetNode<Button>("Window/Download/VBoxContainer/HBoxContainer/Download").Disabled = true;
		GetNode<CanvasItem>("Window/Download/VBoxContainer/HBoxContainer/Download").Hide();
		GetNode<Window>("Window").ExitButton(false);
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
		GetNode<Label>("Window/Download/VBoxContainer/Download/Log/LogText").Text = linktosite.ToString();
	}
	//Daca a fost descarcat cu succes sau nu
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{	dwn = false;
		if(result == 0)
		{	GetNode<TextureButton>("Window/Download/VBoxContainer/Download/Bar/Cancel").Disabled = true;
			GetNode<CanvasItem>("Window/Download/VBoxContainer/Download/Bar/Cancel").Hide();
			GD.Print("Descarcat in " + OS.GetExecutablePath().GetBaseDir());
			GetNode<Label>("Window/Download/VBoxContainer/Download/Log/LogText").Text = "Descarcat in " + OS.GetExecutablePath().GetBaseDir();

			//In principal, procesul pentru toate platformele este acelasi
			//Creeam un fisier in care stocam rezultatul descarcarii (adica variabila body), iar, in cazul platformei Android, putem executa direct fisierul
			#if GODOT_ANDROID
				GetNode<Label>("Window/Download/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Executarea apk-ului...";
				GetNode<Label>("Window/Download/VBoxContainer/Download/Log/LogText").Text = "Executarea apk-ului /storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk";
				//NOTA: aplicatia necesita permisiunea android.permission.REQUEST_INSTALL_PACKAGES in custom permissions
				OS.ShellOpen("/storage/emulated/0/Download/Zero2Linux v" + (string)_data.newversion[0] + ".apk");
				
				//Desktop
				//Tbh, ar fi fost tare daca ar fi aplicat update-ul direct in loc sa puna arhiva intr-un folder.
				//Am incercat sa fac acest lucru, dar ar fi dat crash daca incerci sa suprascrii orice fisier din folder (data_Zero2Linux_ ...) sau
				//orice alt fisier in afara de executabil.
				//Executabilul in sine nu schimba versiunea si tot o sa ramana la versiunea veche, deci nu este de ajutor.
				//Mi-ar trebui o aplicatie extra pentru actualizari pentru desktop, dar nu are sens daca asta este tot ce face.
				//In viitor s-ar putea sa dezvolt aceasta aplicatie extra, dar pana atunci, avem acest cod de mai jos.
			#endif
			GetNode<Label>("Window/Download/VBoxContainer/Download/Bar/ProgressBar/Text").Text = "Gata :D";
			GetNode<Label>("Window/Download/VBoxContainer/Info").Text = "Descarcarea a fost finalizata cu succes.";

			#if GODOT_LINUXBSD || GODOT_WINDOWS
				GetNode<Label>("Window/Download/VBoxContainer/Info").Text = (GetNode<Label>("Window/Download/VBoxContainer/Info").Text).TrimEnd('.') + "\nNoua versiune ar trebui sa fie in acelasi folder cu executabilul";
			#elif GODOT_ANDROID
				GetNode<Label>("Window/Download/VBoxContainer/Info").Text = (GetNode<Label>("Window/Download/VBoxContainer/Info").Text).TrimEnd('.') + "\nNoua versiune este in folderul Download";
			#endif
			GetNode<CanvasItem>("Window/Download/VBoxContainer/Download/Log").Visible = false;
			GetNode<CanvasItem>("Window/Download/VBoxContainer/Download/Debug").Visible = false;
			GetNode<Window>("Window").ExitButton(true);
			debug = false;
			GetTree().Paused = false;
			GetNode("/root").GetChild(-1).EmitSignal("Download", 0);
		}
		else
		{	GD.Print("Eroare HttpRequest: " + (HttpRequest.Result)result);
			GetNode<Label>("Window/Download/VBoxContainer/Download/Log/LogText").Text = "Eroare HttpRequest: " + (HttpRequest.Result)result;
			return;
		}
		request.QueueFree();       //Nu mai e nevoie de HttpRequest. Putem sterge
	}
}
