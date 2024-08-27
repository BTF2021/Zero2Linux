using Godot;
using System;
using Newtonsoft.Json;

//clasa pentru a salva setari si progres. De ce am facut asta? Ca sa se salveze frumos in format JSON
public class stats
{
	public int version = 5;
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
		{3, 0}
	};
	public int Questionaires = 0;     //Nr chestionare facute
	public int Testsnum = 0;          //Nr teste facute
	public int goodtests = 0;         //Nr teste peste 5
	public int greattest = 0;         //Nr teste peste 7
	public int flawlesstests = 0;     //Nr teste fara greseli
	public bool Adv = true;
	public bool Spc = true;
	public float VideoVolume = 0;
	public bool QNumOnly = false;
	public bool AdvQ = true;
	public bool ChkUpdates = true;
}
public partial class DefaultData : Node
{
	//De aici vor fi accesate si salvate setarile si progresul
	public stats currentStats = new stats();
	private stats defaultStats = new stats();         //Duplicat pentru currentStats (Mai mult pentru versiune)

	public Godot.Collections.Dictionary<int, Godot.Collections.Array> lessonList = new Godot.Collections.Dictionary<int, Godot.Collections.Array>() 
	{	//structura vectorului este urmatoarea: numele lectiei, tipul lectiei(tag). Progresul a fost mutat in clasa stats
		{1, new Godot.Collections.Array{"Lectia 1: Ce este Linux?", 0}},
		{2, new Godot.Collections.Array{"Lectia 2: Distributii Linux", 0}},
		{3, new Godot.Collections.Array{"Linux pe masina virtuala", 2}}
	};
	public Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<int, Godot.Collections.Array>> questionList = new Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<int, Godot.Collections.Array>>() 
	{	//structura vectorului este urmatoarea: nr lectie, iar inauntru nr intrebari, rasp corect, cele 4 intrebari, explicatie
		{1, new Godot.Collections.Dictionary<int, Godot.Collections.Array>{
			{1, new Godot.Collections.Array{3, 2, "Ce este Linux?", "Un program", "Un kernel", "O aplicatie pentru web", "", "Linux este kernelul. Majoritatea programelor sunt parte din GNU Project"}},
			{2, new Godot.Collections.Array{4, 3, "In ce an a fost lansat Linux?", "1990", "1899", "1991", "1992", "Prima versiune de Linux a fost lansata pe 17 Septembrie 1991"}},
			{3, new Godot.Collections.Array{2, 1, "Cea mai mare parte din codul Linux este in C si Assembly. Adevarat sau fals?", "Adevarat", "Fals", "", "", "Cea mai mare parte din codul Linux este in C si Assembly"}},
			{4, new Godot.Collections.Array{2, 2, "Cea mai mare parte din codul Linux este in C++ si Python. Adevarat sau fals?", "Adevarat", "Fals", "", "", "Cea mai mare parte din codul Linux este in C si Assembly"}},
			{5, new Godot.Collections.Array{4, 3, "Cine este creatorul si dezvoltatorul principal al kernelului Linux?", "Ken Thompson", "Ari Lemmke", "Linux Torvalds", "Douglas McIlroy", "Linux Torvalds este creatorul si dezvoltatorl principal al Linux."}},
			{6, new Godot.Collections.Array{4, 4, "De la ce este prescurtat GPL", "Licenta Personala Generala", "Licenta Privata Generala", "Licenta Publica GNU", "Licenta Publica Generala", "Prescurtare GPL (in Romana) este Licenta Publica Generala"}}
		}},
		{2, new Godot.Collections.Dictionary<int, Godot.Collections.Array>{
			{1, new Godot.Collections.Array{4, 3, "Ce este inclus (in general) intr-o distributie?", "Kernelul Linux si un browser", "Browser si aplicatii", "Kernelul Linux si aplicatii", "Doar Kernelul Linux", "De obicei, o distributie include kernelul Linux, dar si aplicatii, majoritatea aplicatiilor fiind parte din proiectul GNU"}},
			{2, new Godot.Collections.Array{2, 1, "Se pot instala si actualiza aplicatii atat printr-o interfata grafica, cat si prin terminal", "Adevarat", "Fals", "", "", "Pentru a instala si actualiza aplicatii, utilizatorul are de obicei 2 moduri: grafic si prin terminal"}},
			{3, new Godot.Collections.Array{2, 1, "Linux Mint este derivat din Debian. Adevarat sau fals?", "Adevarat", "Fals", "", "", "Linux Mint este o distributie derivata din Ubuntu, care la randul lui este derivat din Debian. Deci Linux Mint este derivat din Debian"}},
			{4, new Godot.Collections.Array{4, 4, "Care este distributia care NU este bazata pe Debian", "Debian", "Ubuntu", "Linux Mint", "Fedora", "Fedora este distributia din cele patru care NU este bazata pe Debian"}},
			{5, new Godot.Collections.Array{4, 4, "Care este distributia bazata pe Ubuntu", "Arch", "Fedora", "Debian", "Linux Mint", "Linux Mint este o distributie bazata pe Ubuntu"}},
			{6, new Godot.Collections.Array{4, 1, "Care este distributia cea mai veche din cele patru", "Debian", "Fedora", "Linux Mint", "Ubuntu", "Debian este cea a doua cea mai veche distributie inca intretinuta"}}
		}}
	};
    //valori care nu ar trebui schimbate
	public string LoggedUser = " ";
    public bool verifiedver = false;
	public bool isvideoavailable = false;
	public int CurrentLesson = 0;
	public int questiontype = 0;
	public string loadtarget;
	//Vector pentru retinerea informatiilor privind versiunea noua de pe Github
	public Godot.Collections.Array<String> newversion = new Godot.Collections.Array<String>{};

    public override void _Ready()
	{	
		//Determinam daca putem reda videoclipuri. Daca nu (incercam sa) dezactivam optiunea
		#if GODOT_LINUXBSD
			#if TOOLS
				if(DirAccess.DirExistsAbsolute("res://addons/ffmpeg/linux64"))isvideoavailable=true;
			#else
				GD.Print(OS.GetExecutablePath().GetBaseDir());
				if(FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavcodec.so.60"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavfilter.so.9"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavformat.so.60"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libavutil.so.58"))
				&& (FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.linux.template_debug.x86_64.so"))
				|| FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.linux.template_release.x86_64.so")))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libswresample.so.4"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libswscale.so.7"))
				)isvideoavailable=true;
				else GDExtensionManager.UnloadExtension("res://addons/ffmpeg/ffmpeg.gdextension");
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
				&& (FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.windows.template_debug.x86_64.dll"))
				|| FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("libgdffmpeg.windows.template_release.x86_64.dll")))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("swresample-4.dll"))
				&& FileAccess.FileExists(OS.GetExecutablePath().GetBaseDir().PathJoin("swscale-7.dll"))
				)isvideoavailable=true;
				else GDExtensionManager.UnloadExtension("res://addons/ffmpeg/ffmpeg.gdextension");
			#endif
		#elif GODOT_ANDROID
			GDExtensionManager.UnloadExtension("res://addons/ffmpeg/ffmpeg.gdextension");
			isvideoavailable=false;
		#endif

		GD.Print("Videouri disponibile: " + isvideoavailable);
	}
	public bool SaveExists()
	{	System.Array filearray = DirAccess.GetFilesAt("user://");
		for (int i = 0; i < filearray.Length; i++)
		{	if(((string)(filearray.GetValue(i))).EndsWith("_save.json"))     //filearray.GetValue(i) trebuie sa fie string ca sa poate folosi EndsWith()
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
		if(currentStats.version < defaultStats.version) UpgradeSaveFile(user);
		if(currentStats.FullScr) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		if(currentStats.VSync) DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}
	//Functie pentru stergerea unui progres
	public void DeleteSave(string user)
	{
		DirAccess.RemoveAbsolute("user://" + user + "_save.json");             //Trebuie sa fie functie statica
		LoggedUser = " ";
		//Restul este mai mult pentru delogare
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		currentStats = new stats();
		GetTree().ChangeSceneToFile("res://Scenes/Logare.tscn");
	}
	//Functie pentru actualizarea progresului in functie de versiunea fisierului
	public void UpgradeSaveFile(string user)
	{	if(currentStats.version < 5)
		{	currentStats.goodtests = currentStats.flawlesstests;
			currentStats.greattest = currentStats.flawlesstests;
			currentStats.version = 5;
			WriteSave(user);
		}
		//if(currentStats.version < )
	}
	public void LoadScene(string target)
	{	//Incarcam Incarcare.tscn, dam valoare scena pe care vrem sa o incarcam, impachetam la loc si o incarcam ca scena principala
		loadtarget = target;
		GetTree().ChangeSceneToFile("res://Scenes/Incarcare.tscn");
	}
	public Godot.Collections.Array<Godot.Collections.Array> GenerateQuestionSet()
	{	GD.Randomize();     //Nu este necesar. Godot face asta de fiecare data cand deschizi aplicatia
		Godot.Collections.Array<Godot.Collections.Array> _list = new Godot.Collections.Array<Godot.Collections.Array>();
		int i, lesson, number;
		bool ok = false;
		lesson = (int)Mathf.Round(GD.RandRange(1, (int)lessonList.Count));       //Luam indexul unei lectii la intamplare. Daca lectia nu este terminata, repeta pana cand ajungem la o lectie terminata
		while((int)currentStats.LessonCompletion[lesson] != 100 || !questionList.ContainsKey(lesson) || ((int)lessonList[lesson][1] == 1 && !currentStats.AdvQ)) lesson = (int)Mathf.Round(GD.RandRange(1, (int)questionList.Count));      //Probabil se putea si mai bine
		number = (int)Mathf.Round(GD.RandRange(1, (int)questionList[lesson].Count));
		_list.Add((Godot.Collections.Array)questionList[lesson][number]);
		GD.Print("Adaugat o intrebare. Este din lectia nr" + lesson + ", intrebarea " + number);   //Pentru prima intrebare nu este necesara verificarea daca se repeta, atata timp cat respecta conditiile de mai sus
		for(i=2;i<=10;i++)
		{	while(!ok)
			{	//Se repeta ce am facut mai sus
				ok = true;
				lesson = (int)Mathf.Round(GD.RandRange(1, (int)lessonList.Count));
				while((int)currentStats.LessonCompletion[lesson] != 100 || !questionList.ContainsKey(lesson) || ((int)lessonList[lesson][1] == 1 && !currentStats.AdvQ)) lesson = (int)Mathf.Round(GD.RandRange(1, (int)questionList.Count));      //Probabil se putea si mai bine
				number = (int)Mathf.Round(GD.RandRange(1, (int)questionList[lesson].Count));
				if(_list.Contains((Godot.Collections.Array)questionList[lesson][number])) ok = false;
			}
			_list.Add((Godot.Collections.Array)questionList[lesson][number]);
			GD.Print("Adaugat a-" + i + "-a intrebare. Este din lectia nr" + lesson + ", intrebarea " + number);
			ok = false;
		}
		return _list;
	}
}