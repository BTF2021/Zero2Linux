//Pentru fereastra cu chestionare si teste
using Godot;
using System;

public partial class Quizzes : Node2D
{
	private DefaultData _data;
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	_data = (DefaultData)GetNode("/root/DefaultData");
		//Calculam numarul de lectii terminate
		_data.currentStats.FinishedLes = 0;
		int i = 1;
		while(_data.lessonList.ContainsKey(i))
		{	if((int)_data.currentStats.LessonCompletion[i] == 100 && (int)_data.lessonList[i][1] != 2) _data.currentStats.FinishedLes++;
			i++;
		}
		GD.Print(_data.currentStats.FinishedLes);
		//Daca nu indeplineste conditia, nu lasa utilizatorul sa faca chestionare si teste
		if(_data.currentStats.FinishedLes >= 2) 
		{	GetNode<Label>("Window/Requirement").Hide();
			GetNode<Panel>("Window/Settings").Show();
		}
		else
		{	GetNode<Label>("Window/Requirement").Show();
			GetNode<Panel>("Window/Settings").Hide();
		}
		GetNode<Label>("Window/Settings/Stats").Text = "Chestionare terminate: " + _data.currentStats.Questionaires + "\nLectii terminate: " + _data.currentStats.FinishedLes;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{

	}*/
	//Cele doua functii sunt pentru modul Antrenament
	private async void _on_training_pressed()
	{	_data.questiontype = 0;
		_data.LoadScene("res://Scenes/Quizztime.tscn");
	}
	private void _on_training_mouse_entered() => GetNode<Label>("Window/Settings/Description2").Text = "In modul antrenament, se genereaza un chestionar aleatoriu." + 
	"\nNu conteaza daca ai raspuns corect sau gresit.";
	//Cele doua functii sunt pentru modul Test
	private void _on_test_pressed()
	{	_data.questiontype = 1;
		_data.LoadScene("res://Scenes/Quizztime.tscn");
	}
	private void _on_test_entered() => GetNode<Label>("Window/Settings/Description2").Text = "In modul test, se genereaza un chestionar aleatoriu." +
	"\nAi doua minute pentru rezolvare" +
	"\nNumarul raspunsurilor corecte si gresite se vor arata la finalul testului" + 
	"\nMult noroc";
}
