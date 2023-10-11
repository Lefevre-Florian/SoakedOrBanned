using Godot;
using System;

public class ButtonOptionsWorldWide : Button
{
    [Export(PropertyHint.File)] string optionsPath;
    public override void _Ready()
    {
        Connect("button_up", this, "openOptions");
    }
    private void openOptions()
    {
        GetParent().GetParent().GetParent().AddChild(((PackedScene)GD.Load(optionsPath)).Instance());
        GetTree().Paused = true;
    }
}
