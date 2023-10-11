using Godot;
using System;

// Author : Group
namespace Com.IsartDigital.Tools.Resources
{
	static public class Resources
	{
		// Commands
		public const string LEFT_COMMAND = "Left";
		public const string UP_COMMAND = "Up";
		public const string RIGHT_COMMAND = "Right";
		public const string DOWN_COMMAND = "Down";
		public const string LEFT_CLICK = "LeftClick";
		public const string SWAP_CHARACTER_CONTROL = "SwapCharacter";
		public const string PAUSE_CONTROL = "Pause";
		public const string RELOAD_SCENE_CONTROL = "ReloadScene";

		// Signals
		public const string AREA_ENTERED = "area_entered";
		public const string BUTTON_UP = "button_up";
		public const string BUTTON_DOWN = "button_down";
		public const string PRESSED = "pressed";
		public const string FINISHED = "finished";

		// Methods
		public const string ADD_CHILD = "add_child";

		// Paths
		public const string TITLE_SCREEN_PATH = "res://Scenes/TitleScreen.tscn";
		public const string MAIN_SCREEN_PATH = "res://Scenes/Main.tscn";
		public const string LEVELCREATOR_SCREEN_NAME = "LevelCreator";

		// Properties
		public const string POSITION = "position";
		public const string GLOBAL_POSITION = "global_position";
		public const string COLOR = "color";
		public const string MODULATE = "modulate";
		public const string SELF_MODULATE = "self_modulate";
		public const string SCALE = "scale";
		public const string ROTATION_DEGREES = "rotation_degrees";
		public const string RECT_POSITION = "rect_position";
		public const string RECT_ROTATION = "rect_rotation";
		public const string RECT_SCALE= "rect_scale";
		public const string VISIBLE = "visible";
		public const string AMOUNT = "amount";
		public const string RING_INNER_RADIUS = "emission_ring_inner_radius";
		public const string RING_EXTERN_RADIUS = "emission_ring_radius";
		public const string INITIAL_VELOCITY = "initial_velocity";
		public const string ORBIT_VELOCITY = "orbit_velocity";
		public const string SPREAD = "spread";
		public const string TIMEOUT = "timeout";
		public const string OFFSET = "offset";
		public const string EMITTING = "emitting";
	}
}