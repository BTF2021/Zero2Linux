using Godot;
using System;
using Newtonsoft.Json;

//clasa pentru a salva setari si progres. De ce am facut asta? Ca sa se salveze frumos in format JSON
public class stats
{
	public int version = 1;
	public bool FullScr = false;
	public bool VSync = true;
	public int CurrentLesson = 0;
	public Godot.Collections.Dictionary<int, int> LessonCompletion = new Godot.Collections.Dictionary<int, int>() 
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
	public bool Adv = false;
	public bool Spc = true;
	public float VideoVolume = 0;
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
		{6, new Godot.Collections.Array{"Lectia a3:", 1}},
		{7, new Godot.Collections.Array{"Lectia 4: Test", 0}},
		{8, new Godot.Collections.Array{"Lectia 5: Test 2", 0}},
		{9, new Godot.Collections.Array{"Lectia s1: Teste", 2}},
		{10, new Godot.Collections.Array{"Lectia 6: Gol", 0}},
		{11, new Godot.Collections.Array{"Lectia a4:", 1}},
		{12, new Godot.Collections.Array{"Lectia 7:", 0}},
		{13, new Godot.Collections.Array{"Lectia s2:", 2}},
		{14, new Godot.Collections.Array{"Lectia s3:", 2}},
		{15, new Godot.Collections.Array{"Lectia 8:", 0}}
	};
    //valori care nu ar trebui schimbate
    public bool verifiedver = false;
	public bool isvideoavailable = false;
	//Vector pentru retinerea informatiilor privind versiunea noua de pe Github
	public Godot.Collections.Array<String> newversion = new Godot.Collections.Array<String>{};

    public override void _Ready()
	{	
		if(SaveExists()) ReadSave();
		else WriteSave();
		
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
		#endif

		GD.Print("Videouri disponibile: " + isvideoavailable);
	}

	public bool SaveExists()
	{	GD.Print("Exista fisier: " + FileAccess.FileExists("user://save.json"));
		return FileAccess.FileExists("user://save.json");
	}
	public void WriteSave()
	{	var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Write);
		if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
		file.StoreString(JsonConvert.SerializeObject(currentStats));
		file.Close();
	}
	public void ReadSave()
	{	var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Read);
		if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
		stats content = JsonConvert.DeserializeObject<stats>(file.GetAsText());
		file.Close();
		currentStats = content;
		if(currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		if(currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}
	public void PurgeSave()
	{
		currentStats = new stats();
		WriteSave();
		ReadSave();
	}
}