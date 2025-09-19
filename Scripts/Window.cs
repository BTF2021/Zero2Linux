//Pentru ferestre
using Godot;
using System;

public partial class Window : Sprite2D
{   
	private DefaultData _data;
	[Export] public string windowtitle;
	//Pentru miscarea ferestrei
	private Vector2 mousepos;
	private bool inputgrab;
	private Vector2 dif;
	 public override void _Ready()
	 {
		_data = (DefaultData)GetNode("/root/DefaultData");
		GetNode<Label>("Label").Text = windowtitle;
		if(_data.currentStats.Anims) GetNode<AnimationPlayer>("AnimationPlayer").Play("In");
	 }
	public override void _Process(double delta)
	{	//Pentru miscarea ferestrei
		mousepos = GetViewport().GetMousePosition();
		var winpos = Position;
		var newpos = Position;
		//Pozitia este raportata la centrul ferestrei
		newpos.X = Mathf.Clamp(Mathf.Lerp(winpos.X, mousepos.X + dif.X, 1), 0, 1280);
		newpos.Y = Mathf.Clamp(Mathf.Lerp(winpos.Y, mousepos.Y + dif.Y, 1), 250, 896);
		if(inputgrab)
		{	Position = newpos;
		}
	}
	//Daca sa apara butonul de inchidere a ferestrei sau nu
	public void ExitButton(bool toggled)
	{
		GetNode<TextureButton>("Back").Disabled = !toggled;
		GetNode<TextureButton>("Back").Visible = toggled;
	}
	private void _on_back_pressed() => GetParent().QueueFree();	//Inchide fereastra
	private void _on_drag_down()	//Cand partea de sus a ferestrei este apasata
	{
		#if GODOT_ANDROID
			//Desi mousepos este preluat in _Proccess(), mousepos ramane aceeasi valoare dupa ce ecranul a fost atins
			//Presupun ca ii ia un frame ca sa proceseze noua pozitie, ceea ce nu este de ajuns pentru aceasta functie
			//Asa ca il actualizam acum
			mousepos = GetViewport().GetMousePosition();
		#endif
		var winpos = Position;
		dif.X = winpos.X - mousepos.X;
		dif.Y = winpos.Y - mousepos.Y;
		inputgrab = true;
		GetParent().GetParent().MoveChild(GetNode(".."), -1);
	}
	private void _on_drag_up()	//Cand partea de sus a ferestrei nu mai este apasata
	{	inputgrab = false;
	}
}
