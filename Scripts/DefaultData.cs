using Godot;
using System;
using System.Collections.Generic;

public partial class DefaultData : Node
{
	//Date care vor fi folosite in creearea lectiilor si in salvarea progresului
	public int currentLesson = 0;
	public Godot.Collections.Dictionary<int, Godot.Collections.Array> lessonList = new Godot.Collections.Dictionary<int, Godot.Collections.Array>() 
	{	//structura vectorului este urmatoarea: numele lectiei, progress, tipul lectiei(tag)
		{1, new Godot.Collections.Array{"Lectia 1: Introducere", 15, 0}},
		{2, new Godot.Collections.Array{"Lectia 2: Ce sunt distrourile", 23, 0}},
		{3, new Godot.Collections.Array{"Lectia a1: Desktop environments", 3, 1}},
		{4, new Godot.Collections.Array{"Lectia a2: GPU de la NVidia", 6, 1}},
		{5, new Godot.Collections.Array{"Lectia 3: Repos si Package Managers", 68, 0}},
		{6, new Godot.Collections.Array{"Lectia a3:", 73, 1}},
		{7, new Godot.Collections.Array{"Lectia 4: Test", 31, 0}},
		{8, new Godot.Collections.Array{"Lectia 5: Test 2", 15, 0}},
		{9, new Godot.Collections.Array{"Lectia s1: Teste", 15, 2}},
		{10, new Godot.Collections.Array{"Lectia 6: Gol", 15, 0}},
		{11, new Godot.Collections.Array{"Lectia a4:", 15, 1}},
		{12, new Godot.Collections.Array{"Lectia 7:", 15, 0}},
		{13, new Godot.Collections.Array{"Lectia s2:", 15, 2}},
		{14, new Godot.Collections.Array{"Lectia s3:", 15, 2}},
		{15, new Godot.Collections.Array{"Lectia 8:", 15, 0}}
	};

	//valori pentru lessons
	public bool includeadvanced = false;
	public bool includespecial = true;
	public float videovolume = 0;
	//valori sin misc
	public bool checkupdates = true;
	//valori care nu ar trebui schimbate
	public bool verifiedver = false;
	public bool isvideoavailable = false;

	public override void _Ready()
	{	
		#if TOOLS && GODOT_LINUXBSD
			if(DirAccess.DirExistsAbsolute("res://addons/ffmpeg/linux64"))isvideoavailable=true;
		#elif TOOLS && GODOT_WINDOWS
			if(DirAccess.DirExistsAbsolute("res://addons/ffmpeg/win64"))isvideoavailable=true;
		#endif
		GD.Print(isvideoavailable);
	}
}