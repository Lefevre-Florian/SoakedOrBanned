using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.Utils.Elems;

//Author : VERDIER Thomas 

namespace Com.IsartDigital.Sokoban.TerrainGeneration {

	public class Ambiance : Node
	{
		[Export(PropertyHint.Dir)] private string dir;


		private const string WALL= "Wall.tscn";
		private const string WATER_BLOCK = "TileWaterBlock.tscn";
		private const string DIRT_BLOCK = "TileDirtBlock.tscn";
		private const string TARGER_WATER = "TargetWaterBlock.tscn";
		private const string CRATE_WATER = "CrateWater.tscn";
		private const string PLAYER_WATER = "WaterPlayer.tscn";
		private const string TARGET_DIRT = "TargetDirtBlock.tscn";
		private const string CRATE_DIRT = "CrateDirt.tscn";
	    private const string PLAYER_FIRT = "PlayerDirt.tscn";

		public Dictionary<string, string> elements = new Dictionary<string, string>();

		public override void _Ready()
		{
			AddToList(ListElemTiles.WALL_STRING,WALL);
			AddToList(ListElemTiles.WATER_BLOCK_STRING, WATER_BLOCK);
			AddToList(ListElemTiles.DIRT_BLOCK_STRING, DIRT_BLOCK);
			AddToList(ListElemTiles.TARGET_WATER_BLOCK_STRING, TARGER_WATER);
			AddToList(ListElemTiles.CRATE_WATER_STRING, CRATE_WATER);
			AddToList(ListElemTiles.PLAYER_WATER_STRING, PLAYER_WATER);
			AddToList(ListElemTiles.TARGET_DIRT_STRING, TARGET_DIRT);
			AddToList(ListElemTiles.CRATE_DIRT_STRING, CRATE_DIRT);
			AddToList(ListElemTiles.PLAYER_DIRT_STRING, PLAYER_FIRT);

			AmbianceManager.ambiances.Add(this);		
		}

		private void AddToList(string pElem, string pPath)
        {
			elements.Add(pElem, dir + '/' + pPath);

		}

	}

}