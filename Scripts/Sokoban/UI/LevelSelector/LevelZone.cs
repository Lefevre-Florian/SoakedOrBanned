using Godot;
using System;
using Com.IsartDigital.Sokoban.Managers;
using Com.IsartDigital.Sokoban.Architectures;
using System.Collections.Generic;
//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class LevelZone : Node2D
	{
		public static List<LevelZone> levelZones = new List<LevelZone>();

		[Export] NodePath linkedLevelPath;
		private FlagLevel linkedLevel;

		[Export] NodePath mapCloudsPath;
		private TileMap mapClouds;

		[Export] public int numLevel;

		[Export] float timeForCloudsToDisappear = 1;
		[Export] float travelingTime = 0.5f;
		Tween tween = new Tween();

		public override void _Ready()
		{
			AddChild(tween);
			mapClouds = (TileMap)GetNode(mapCloudsPath);
			linkedLevel = (FlagLevel)GetNode(linkedLevelPath);
			linkedLevel.numLevel = numLevel;
			linkedLevel.VisualSetUp();
			if (DatabaseManager.User != null)
			{
				User lUser = DatabaseManager.User.Value;
				if (lUser.levelUnlocked > numLevel)
					ClearCells();
			}
			else
			{
				if (Map.unlockedLevel > numLevel)
					ClearCells();
			}
			levelZones.Add(this);
		}

		public void ClearSequence()
        {
			SelectorPlayer.GetInstance().SetActionIdle();
			CameraMovementIn();
		}

		private void CameraMovementIn()
		{
			tween.InterpolateProperty(SelectorPlayer.GetInstance().camera, "global_position", SelectorPlayer.GetInstance().GlobalPosition, GlobalPosition, travelingTime);
			tween.InterpolateCallback(this, travelingTime, nameof(RemoveClouds));
			tween.Start();
		}

		private void RemoveClouds()
        {
			tween.InterpolateProperty(mapClouds, "modulate:a", 1, 0, timeForCloudsToDisappear);
			tween.InterpolateCallback(this, timeForCloudsToDisappear, nameof(ClearCells));
			tween.InterpolateCallback(this, timeForCloudsToDisappear, nameof(CameraMovementOut));
			tween.Start();
		}

		public void ClearCells()
        {
			mapClouds.Clear();
		}

		private void CameraMovementOut()
		{
			tween.InterpolateProperty(SelectorPlayer.GetInstance().camera, "global_position",GlobalPosition, SelectorPlayer.GetInstance().GlobalPosition, travelingTime);
			tween.InterpolateCallback(SelectorPlayer.GetInstance(), travelingTime, "SetActionMove");
			tween.InterpolateCallback(SelectorPlayer.GetInstance(), travelingTime, "IdleJuice");
			tween.Start();
		}

        protected override void Dispose(bool disposing)
        {
			levelZones.Remove(this);
            base.Dispose(disposing);
        }

    }

}