using Godot;
using System;

public class Tsunami : Node2D
{
    public static Tsunami instance;

    [Export] private NodePath animPath;
    public AnimationPlayer anim;

    static public Tsunami GetInstance()
    {
        if (instance == null) instance = new Tsunami();
        return instance;
    }

    public override void _Ready()
    {
        if (instance != null)
        {
            QueueFree();
            return;
        }
        instance = this;

        anim = GetNode<AnimationPlayer>(animPath);

        base._Ready();
    }

    protected override void Dispose(bool pDisposing)
    {
        if (pDisposing && instance == this) instance = null;
        base.Dispose(pDisposing);
    }
}
