//Folosit pentru fereastra de tur.
using Godot;
using System;

public partial class Tour : Node2D
{
    private DefaultData _data;
    public override void _Ready()
    {   _data = (DefaultData)GetNode("/root/DefaultData");
        if(_data.currentStats.ShowTutorial) GetNode<Window>("Window").ExitButton(false);
        GetNode<CheckBox>("Window/TabContainer/End/Panel/DoNotShow").ButtonPressed = !_data.currentStats.ShowTutorial;

        GetNode<Label>("Window/TabContainer/Lesson/Clip/Panel/Account/BigLetter").Text = _data.currentStats.UsrName;
		GetNode<Label>("Window/TabContainer/Lesson/Clip/Panel/Account/Name").Text = _data.currentStats.UsrName;
		GetNode<Sprite2D>("Window/TabContainer/Lesson/Clip/Panel/Account/Bg").SelfModulate = _data.currentStats.FavColor;
    }
    /*public override void _Process(double delta)
    {

    }
    */

    public void _on_do_not_show_pressed() => _data.currentStats.ShowTutorial = !GetNode<CheckBox>("Window/TabContainer/End/Panel/DoNotShow").ButtonPressed;
    public void _on_next_pressed () 
    {
        GetNode<TabContainer>("Window/TabContainer").CurrentTab += 1;
        switch(GetNode<TabContainer>("Window/TabContainer").CurrentTab)
        {
            case 1:
                //Tranzitia pentru bara
                GetNode("/root").GetChild(-1).EmitSignal("Tutorial", 1);
                break;
            case 4:
                //Turul s-a terminat
                GetNode<Window>("Window").ExitButton(true);
                _data.currentStats.ShowTutorial = false;
                GetNode<CheckBox>("Window/TabContainer/End/Panel/DoNotShow").ButtonPressed = !_data.currentStats.ShowTutorial;
                break;
        }
    }
}
