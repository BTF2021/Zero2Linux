//Folosit pentru creearea si modificarea utilizatorilor
//Pentru creearea acestei ferestre intr-o anumita scena, este necesara setarea modului inainte de creearea scenei, dar si o functie care va fi apelata dupa ce fereastra se inchide
//Un model pentru functia apelata dupa inchidere se afla pe ultimile linii de cod
using Godot;
using System;
using Newtonsoft.Json;

public partial class ConfigUser : Node2D
{
	private DefaultData _data;
	[Export]public int mode; //0: Creeaza un utilizator, 1: Modifica un utilizator existent

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	GetTree().Paused = true;
        _data = (DefaultData)GetNode("/root/DefaultData");

        if(mode == 0)
        {   GetNode<Label>("Create/Panel/Title").Text = "Creeaza un nou utilizator";
        }
        else if(mode == 1)
        {   
			 GetNode<Label>("Create/Panel/Title").Text = "Modifica utilizatorul " + _data.currentStats.UsrName;
            GetNode<LineEdit>("Create/Panel/Nume/Nume").Text = _data.currentStats.UsrName;
		    GetNode<ColorPicker>("Create/Panel/Culoare/Panel/ColorPicker").Color = _data.currentStats.FavColor;
        }

		//Animatii
		if(_data.currentStats.Anims)
		{	var scale = Scale;
			var pos = Position;
			scale.X = (float)0.75;
			scale.Y = (float)0.75;
			pos.X = 540;
			pos.Y = 255;
			GetNode<Control>("Create").Position = pos;
			GetNode<Control>("Create").Scale = scale;
			scale.X = (float)2.5;
			scale.Y = (float)2.5;
			pos.X = 227;
			pos.Y = 67;
			var tween = GetTree().CreateTween();
			tween.SetPauseMode((Tween.TweenPauseMode)2);	//Ca sa fie procesat chiar si daca SceneTree este in pauza
			tween.TweenProperty(GetNode<Control>("Create"), "position", pos, 0.5).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
			tween.Parallel().TweenProperty(GetNode<Control>("Create"), "scale", scale, 0.5).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Actualizam nodul de previzualizare("Preview") in functie de nume si de culoare
        GetNode<Label>("Create/Panel/Preview/Name").Text = GetNode<LineEdit>("Create/Panel/Nume/Nume").Text;
		GetNode<Label>("Create/Panel/Preview/BigLetter").Text = GetNode<LineEdit>("Create/Panel/Nume/Nume").Text;
		GetNode<Sprite2D>("Create/Panel/Preview/Bg").SelfModulate = GetNode<ColorPicker>("Create/Panel/Culoare/Panel/ColorPicker").Color;
	}

	//Functie de verificare daca numele este valid
    private void _on_nume_text_changed(string new_text)
	{	//Aici este partea unde eliminam caracterele interzise din nume
		if(new_text.IndexOf(" ") >= 0) new_text = new_text.Remove(new_text.IndexOf(" "), 1);
		if(new_text.IndexOf("/") >= 0) new_text = new_text.Remove(new_text.IndexOf("/"), 1);
		if(new_text.IndexOf(".") >= 0) new_text = new_text.Remove(new_text.IndexOf("."), 1);
		if(new_text.IndexOf(":") >= 0) new_text = new_text.Remove(new_text.IndexOf(":"), 1);
		if(new_text.IndexOf(",") >= 0) new_text = new_text.Remove(new_text.IndexOf(","), 1);
		if(new_text.IndexOf("@") >= 0) new_text = new_text.Remove(new_text.IndexOf("@"), 1);
		if(new_text.IndexOf("'") >= 0) new_text = new_text.Remove(new_text.IndexOf("'"), 1);
		if(new_text.IndexOf("%") >= 0) new_text = new_text.Remove(new_text.IndexOf("%"), 1);
		if(new_text.IndexOf('"') >= 0) new_text = new_text.Remove(new_text.IndexOf('"'), 1);
		var caret = GetNode<LineEdit>("Create/Panel/Nume/Nume").CaretColumn;	//Salveaza pozitia caretului inainte de a salva textul
		GetNode<LineEdit>("Create/Panel/Nume/Nume").Text = new_text;
		GetNode<LineEdit>("Create/Panel/Nume/Nume").CaretColumn = caret;
	}
	//Butonul de inchis fereastra
	private void _on_back_pressed()	//Atunci cand se anuleaza actiunea, dispare fereastra, indiferent de tip
	{	GetTree().Paused = false;
        GetParent().Call("ConfigFinished", 0);
		QueueFree();
	}
	//Butonul pentru creeare/modificare
	private void _on_create_pressed()	//Atunci cand confirmi iesirea din chestionar
	{	
        switch(mode)
        {
			//Creeare
            case 0:
                if(GetNode<LineEdit>("Create/Panel/Nume/Nume").Text.Length <= 0) 	//Daca numele nu exista
				{	GD.Print("Eroare: Nu exista nume");
		    		GetNode<Label>("Create/Panel/Eroare").Show();
		    		GetNode<Label>("Create/Panel/Eroare").Text = "Nu exista nume! Introduce un nume valid.";
            		return;
				}
				else
				{	var ok = true;
					var names = _data.GetSaves();
					//Verificam daca deja exista nume
					if(names != null)
					for (int i = 0; i< names.Length; i++) if(GetNode<LineEdit>("Create/Panel/Nume/Nume").Text == (string)names.GetValue(i)) ok = false;
					GD.Print(ok);
					if(ok)
					{	//Aici este partea unde cream un fisier folosind numele si culoarea deja date de utilizator
			    		_data.currentStats.UsrName = GetNode<LineEdit>("Create/Panel/Nume/Nume").Text;
			    		_data.currentStats.FavColor = GetNode<ColorPicker>("Create/Panel/Culoare/Panel/ColorPicker").Color;
					}
					else
					{	GD.Print("Deja exista un utilizator cu acel nume!");
			    		GetNode<Label>("Create/Panel/Eroare").Show();
			    		GetNode<Label>("Create/Panel/Eroare").Text = "Deja exista un utilizator cu acel nume!";
                		return;
					}
				}
				//Creearea fisierului propriu-zis
				var file = FileAccess.Open("user://" + _data.currentStats.UsrName +"_save.json", FileAccess.ModeFlags.Write);
				if (file == null) GD.Print("Nu se poate deschide fisierul. Eroare: " + FileAccess.GetOpenError());
				file.StoreString(JsonConvert.SerializeObject(_data.currentStats));
				file.Close();
				_data.currentStats = new stats();
                break;
			//Modificare
            case 1:
                //Daca am modificat numele
				if(_data.currentStats.UsrName != GetNode<LineEdit>("Create/Panel/Nume/Nume").Text)
                {  	if(GetNode<LineEdit>("Create/Panel/Nume/Nume").Text.Length <= 0) 	//Daca numele nu exista
					{	GD.Print("Eroare: Nu exista nume");
		    			GetNode<Label>("Create/Panel/Eroare").Show();
		    			GetNode<Label>("Create/Panel/Eroare").Text = "Nu exista nume! Introduce un nume valid.";
            			return;
					}
					else
					{	var ok = true;
						var names = _data.GetSaves();
						//Verificam daca deja exista nume
						if(names != null)
						for (int i = 0; i< names.Length; i++) if(GetNode<LineEdit>("Create/Panel/Nume/Nume").Text == (string)names.GetValue(i)) ok = false;
						GD.Print(ok);
						if(ok)
						{	_data.currentStats.UsrName = GetNode<LineEdit>("Create/Panel/Nume/Nume").Text;
				    		DirAccess.RenameAbsolute("user://" + _data.LoggedUser + "_save.json", "user://" + _data.currentStats.UsrName + "_save.json");
				    		_data.LoggedUser = _data.currentStats.UsrName;
						}
						else
						{	GD.Print("Deja exista un utilizator cu acel nume!");
			    			GetNode<Label>("Create/Panel/Eroare").Show();
			    			GetNode<Label>("Create/Panel/Eroare").Text = "Deja exista un utilizator cu acel nume!";
                			return;
						}
					}
                }
				//Daca am modificat culoarea
                if(_data.currentStats.FavColor != GetNode<ColorPicker>("Create/Panel/Culoare/Panel/ColorPicker").Color)
                    _data.currentStats.FavColor = GetNode<ColorPicker>("Create/Panel/Culoare/Panel/ColorPicker").Color;
				_data.WriteSave(_data.LoggedUser);
                break;
        }
        GetTree().Paused = false;
		GetParent().Call("ConfigFinished", 1);
		QueueFree();
	}
}

/*
//Functie apelata dupa ce fereastra a fost inchisa (stearsa din memorie prin QueueFree())
//Numele functiei trebuie sa ramana NESCHIMBAT. Fereastra se asteapta sa existe aceasta functie
//Functia trebuie adaptata in functie de scena.
public void ConfigFinished(int code)
	{
		switch(code)
		{	
			//Nu s-a creat/modificat utilizatorul
			case 0:
				//Cod
				break;
			
			//Utilizatorul a fost creat/modificat
			case 1:
				//Cod
				break;
		}
	}
*/
