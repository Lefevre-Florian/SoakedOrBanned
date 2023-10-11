using Godot;
using System;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.UI {

	public enum LandType
    {
		Grass,
		Path,
		Water
    }

	public class LandMap : TileMap
	{
		private static LandMap instance;

		private LandMap() : base() { }

		public override void _Ready()
		{
			if (instance != null)
            {
				QueueFree();
				return;
            }

			instance = this;
		}

		public static LandMap GetInstance()
        {
			if (instance == null) instance = new LandMap();
			return instance;
        }

        protected override void Dispose(bool pDisposing)
        {
			if (pDisposing && instance != null) instance = null;
            base.Dispose(pDisposing);
        }

    }

}