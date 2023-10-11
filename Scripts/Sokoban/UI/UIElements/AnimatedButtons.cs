using Godot;
using System;
using Com.IsartDigital.Utils.Events;

//Author : BOULIN Valère
public class AnimatedButtons : Button
{
    [Export] protected float MAX_SCALE = 1.2f;
    [Export] protected float SCALE_SPEED = .02f;

    protected Vector2 startRectScale;
    protected bool hovering;
    public static bool shouldReset = false;

    public override void _Ready()
    {
        startRectScale = RectScale;
        Connect(EventButton.MOUSE_ENTERED, this, nameof(OnHover));
        Connect(EventButton.MOUSE_EXITED, this, nameof(StopHover));
    }

    public override void _Process(float delta)
    {
        if (shouldReset)
        {
            Reset();
            shouldReset = false;
        }

        if (RectScale < new Vector2(MAX_SCALE, MAX_SCALE) && hovering)
            RectScale += new Vector2(SCALE_SPEED, SCALE_SPEED);
        else if (RectScale > new Vector2(startRectScale) && !hovering)
            RectScale -= new Vector2(SCALE_SPEED, SCALE_SPEED);
    }

    public virtual void Reset()
    {

    }

    protected void OnHover()
    {
        hovering = true;
    }

    protected void StopHover()
    {
        hovering = false;
    }
}
