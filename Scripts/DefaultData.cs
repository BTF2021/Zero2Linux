using Godot;
using System;
using System.Linq;

public partial class DefaultData : Node
{
	//Date care vor fi folosite in creearea lectiilor si in salvarea progresului
	public int currentLesson = 0;
	public Godot.Collections.Dictionary lessonList = new Godot.Collections.Dictionary()
	{
		//structura vectorului este urmatoarea: progress, tipul(tagul)
		{1, new Godot.Collections.Array{0, 0}}
		
	};
}