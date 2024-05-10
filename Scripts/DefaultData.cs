using Godot;
using System;
using Newtonsoft.Json;

//clasa pentru a salva setari si progres. De ce am facut asta? Ca sa se salveze frumos in format JSON
public class stats
{
	public int version = 3;
	public string UsrName = " ";
	public Color FavColor = new Color(1, 1, 1, 1);
	public bool FullScr = false;
	public bool VSync = true;
	public bool Anims = true;
	public int FinishedLes = 0;
	public Godot.Collections.Dictionary<int, double> LessonCompletion = new Godot.Collections.Dictionary<int, double>() 
	{
		{1, 0},
		{2, 0},
		{3, 0},
		{4, 0},
		{5, 0},
		{6, 0},
		{7, 0},
		{8, 0},
		{9, 0},
		{10, 0},
		{11, 0},
		{12, 0},
		{13, 0},
		{14, 0},
		{15, 0}
	};
	public int Questionaires = 0;     //Nr chestionare facute
	public bool Adv = false;
	public bool Spc = true;
	public float VideoVolume = 0;
	public bool QNumOnly = false;
	public bool ChkUpdates = true;
}
public partial class DefaultData : Node
{
	//De aici vor fi accesate si salvate setarile si progresul
	public stats currentStats = new stats();

	public Godot.Collections.Dictionary<int, Godot.Collections.Array> lessonList = new Godot.Collections.Dictionary<int, Godot.Collections.Array>() 
	{	//structura vectorului este urmatoarea: numele lectiei, tipul lectiei(tag). Progresul a fost mutat in clasa stats
		{1, new Godot.Collections.Array{"Lectia 1: Introducere", 0}},
		{2, new Godot.Collections.Array{"Lectia 2: Ce sunt distrourile", 0}},
		{3, new Godot.Collections.Array{"Lectia a1: Desktop environments", 1}},
		{4, new Godot.Collections.Array{"Lectia a2: GPU de la NVidia", 1}},
		{5, new Godot.Collections.Array{"Lectia 3: Repos si Package Managers", 0}},
		{6, new Godot.Collections.Array{"Lectia a3: Pachete", 1}},
		{7, new Godot.Collections.Array{"Lectia 4: Test", 0}},
		{8, new Godot.Collections.Array{"Lectia 5: Test 2", 0}},
		{9, new Godot.Collections.Array{"Lectia s1: Teste", 2}},
		{10, new Godot.Collections.Array{"Lectia 6: Gol", 0}},
		{11, new Godot.Collections.Array{"Lectia a4: Tot gol", 1}},
		{12, new Godot.Collections.Array{"Lectia 7: Mai multe teste", 0}},
		{13, new Godot.Collections.Array{"Lectia s2: Test special", 2}},
		{14, new Godot.Collections.Array{"Lectia s3: Test special 2", 2}},
		{15, new Godot.Collections.Array{"Lectia 8: Ultima lectie", 0}}
	};
    //valori care nu ar trebui schimbate
	public string LoggedUser = " ";
    public bool verifiedver = false;
	public bool isvideoavailable = false;
	public int CurrentLesson = 0;
	public int questiontype = 0;
	//Vector pentru retinerea informatiilor privind versiunea noua de pe Github
	public Godot.Collections.Array<String> newversion = new Godot.Collections.Array<String>{};

    public override void _Ready()
	{	
		#if GODOT_LINUXBSD
			#if TOOLS
				if(DirAccess.DirExistsAbsolute("res://addons/ffmpeg/linux64"))isvideoavailable=true;
			#else
				GD.Print(OS.GetExecutablePath().GetBaseDir());
				if(FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavcodec.so.60"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavfilter.so.9"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavformat.so.60"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavutil.so.58"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.linux.template_debug.x86_64.so"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libswresample.so.4"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libswscale.so.7"))
				)isvideoavailable=true;
			#endif
		#elif GODOT_WINDOWS
			#if TOOLS
				if(DirAccess.DirExistsAbsolute("res://addons/ffmpeg/win64"))isvideoavailable=true;
			#else
				GD.Print(OS.GetExecutablePath().GetBaseDir());
				if(FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("avcodec-60.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("avfilter-9.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("avformat-60.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("avutil-58.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.windows.template_debug.x86_64.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("swresample-4.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("swscale-7.dll"))
				)isvideoavailable=true;
			#endif
		#elif GODOT_ANDROID
			isvideoavailable=false;
		#endif
		
		GD.Print("Videouri disponibile: " + isvideoavailable);
	}

	public bool SaveExists()
	{	System.Array filearray = DirAccess.GetFilesAt("user://");
		for (int i = 0; i < filearray.Length; i++)
		{	if(((string)(filearray.GetValue(i))).EndsWith("_save.json"))     //Ceva stupid. filearray.GetValue(i) trebuie sa fie string ca sa poate folosi EndsWith()
			{	GD.Print("Exista fisier: true");
				return true;
			}
		}
		GD.Print("Exista fisier: false. Creaza un nou save.json");
		return false;
	}
	public System.Array GetSaves()
	{	System.Array filearray = DirAccess.GetFilesAt("user://");
		var length = filearray.Length;
		if(length > 7) length = 7;
		System.Array savenames = new string[7];
		if(!SaveExists()) return null;
		for (int i = 0; i < length; i++)
			if(((string)(filearray.GetValue(i))).EndsWith("_save.json"))     					 //Ceva stupid. filearray.GetValue(i) trebuie sa fie string ca sa poate folosi Contains()
			{	var pos = ((string)(filearray.GetValue(i))).Find("_save.json");					 //Aflam pozitia _save.json
				savenames.SetValue(((string)(filearray.GetValue(i))).Substr(0, pos), i);         //Facem un subsir pana la _save.json si-l bagam in vector
				GD.Print(savenames.GetValue(i));
			}
		return savenames;
	}
	public void WriteSave(string user)
	{	int i = 1;
		while(lessonList.ContainsKey(i))
		{	if((int)currentStats.LessonCompletion[i] == 100 && (int)lessonList[i][1] != 2) currentStats.FinishedLes++;
			i++;
		}
		var file = FileAccess.Open("user://" + user + "_save.json", FileAccess.ModeFlags.Write);
		if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
		file.StoreString(JsonConvert.SerializeObject(currentStats));
		file.Close();
	}
	public void ReadSave(string user)
	{	var file = FileAccess.Open("user://" + user + "_save.json", FileAccess.ModeFlags.Read);
		if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
		stats content = JsonConvert.DeserializeObject<stats>(file.GetAsText());
		file.Close();
		currentStats = content;
		if(currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		if(currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}
	public void PurgeSave(string user)
	{
		DirAccess.RemoveAbsolute("user://" + user + "_save.json");             //Trebuie sa fie functie statica
		LoggedUser = " ";
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		currentStats = new stats();
		GetTree().ChangeSceneToFile("res://Scenes/Logare.tscn");
	}
}