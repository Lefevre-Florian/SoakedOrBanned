using Godot;
using System;
using Com.IsartDigital.Tools.Resources;
using Com.IsartDigital.Sokoban.Managers;

// Author : BENCTEUX Pierre-Antoine
namespace Com.IsartDigital.Sokoban.Abstracts
{
	public static class Controller
	{

		public static bool canPlay = false;

		static public Vector2 Move()
		{
			if (!canPlay)
				return Vector2.Zero;

			if (Input.IsActionJustPressed(Resources.LEFT_COMMAND))
				return Vector2.Left;
			else if (Input.IsActionJustPressed(Resources.UP_COMMAND))
				return Vector2.Up;
			else if (Input.IsActionJustPressed(Resources.RIGHT_COMMAND))
				return Vector2.Right;
			else if (Input.IsActionJustPressed(Resources.DOWN_COMMAND))
				return Vector2.Down;
			else
				return Vector2.Zero;
		}

		static public void SwapCharacter()
        {
			if (Input.IsActionJustPressed(Resources.SWAP_CHARACTER_CONTROL))
				GameManager.GetInstance().ChangeCharacterSelected();
		}

		static public bool ReloadScene()
        {
			if (Input.IsActionJustPressed(Resources.RELOAD_SCENE_CONTROL))
				return true;
			else
				return false;
		}
	}
}