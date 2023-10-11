using Godot;
using System;
using System.Collections.Generic;

//Author : VERDIER Thomas

namespace Com.IsartDigital.Sokoban.TerrainGeneration
{
    public class AmbianceManager : Node
    {
        private static RandomNumberGenerator rand = new RandomNumberGenerator();
        public static List<Ambiance> ambiances = new List<Ambiance>();

        public static Ambiance GetAnAmbiance()
        {
            rand.Randomize();
            int i = rand.RandiRange(0, ambiances.Count - 1);
            return ambiances[i];
        }

        protected override void Dispose(bool disposing)
        {
            ambiances.Clear();
            base.Dispose(disposing);
        }
    }
}
