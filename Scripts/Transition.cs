using Godot;
using System;

public partial class Transition : Sprite2D
{
	//Tranzitia pentru logare este motivul pentru care aceasta scena exista
	//Arata scena doar daca suntem la ecranul de logare
	//Nu trebuie sa fie aratata in celelalte tranzitii (cum ar fi cand se incarca o lectie sau un chestionar)
	public override void _Process(double delta)
	{	if(GetTree().CurrentScene != null)
			if(GetTree().CurrentScene.Name == "Logare") Show();
			else Hide();
	}
}
