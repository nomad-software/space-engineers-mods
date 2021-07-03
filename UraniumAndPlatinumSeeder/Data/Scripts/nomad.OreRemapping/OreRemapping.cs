using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Weapons;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace nomad.OreRemapping
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class OreRemapping : MySessionComponentBase
    {
        public override void LoadData()
        {
            var allPlanets = MyDefinitionManager.Static.GetPlanetsGeneratorsDefinitions();

            foreach (var def in allPlanets)
            {
                var planet = def as MyPlanetGeneratorDefinition;
                var oreList = new List<MyPlanetOreMapping>(planet.OreMappings.ToList());

                if (planet.Id.SubtypeName == "EarthLike")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        // if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        if (oreMap.Value == 164 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }
                    }
                }

                if (planet.Id.SubtypeName == "Alien")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        // if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        if (oreMap.Value == 164 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }
                    }
                }

                if (planet.Id.SubtypeName == "Mars")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        // if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        if (oreMap.Value == 164 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }
                    }
                }

                if (planet.Id.SubtypeName == "Moon")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        // if (oreMap.Value == 164 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 96; oreMap.Depth = 11; }
                    }
                }

                if (planet.Id.SubtypeName == "Europa")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        // if (oreMap.Value == 164 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 96; oreMap.Depth = 11; }
                    }
                }

                if (planet.Id.SubtypeName == "Titan")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        if (oreMap.Value == 152 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 38; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 80; oreMap.Depth = 8; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        // if (oreMap.Value == 164 && oreMap.Type.Contains("Ice_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 96; oreMap.Depth = 11; }

                    }
                }

                if (planet.Id.SubtypeName == "Triton")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 1 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 10; oreMap.Depth = 3; }
                        // if (oreMap.Value == 24 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 10; oreMap.Depth = 3; }
                        // if (oreMap.Value == 48 && oreMap.Type.Contains("Silicon_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 9; oreMap.Depth = 3; }
                        // if (oreMap.Value == 72 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 20; oreMap.Depth = 3; }
                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        if (oreMap.Value == 152 && oreMap.Type.Contains("Silicon_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        if (oreMap.Value == 156 && oreMap.Type.Contains("Silicon_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        if (oreMap.Value == 160 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 41; oreMap.Depth = 3; }
                        if (oreMap.Value == 164 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }
                    }
                }

                if (planet.Id.SubtypeName == "Pertam")
                {
                    for (int i = 0; i < oreList.Count; i++)
                    {
                        var oreMap = planet.OreMappings[i];

                        // if (oreMap.Value == 1 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 10; oreMap.Depth = 3; }
                        // if (oreMap.Value == 24 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 10; oreMap.Depth = 3; }

                        // I've replaced cobalt here because the values 152 and over don't seem to be seeded in Pertam.
                        // There also seems to be some sort of weighting going on with the ores, so I've used the least common cobalt values to replace.
                        if (oreMap.Value == 72 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        if (oreMap.Value == 84 && oreMap.Type.Contains("Cobalt_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }

                        // if (oreMap.Value == 96 && oreMap.Type.Contains("Silver_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 39; oreMap.Depth = 3; }
                        // if (oreMap.Value == 144 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 32; oreMap.Depth = 2; }
                        // if (oreMap.Value == 148 && oreMap.Type.Contains("Iron_02") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 35; oreMap.Depth = 3; }
                        // if (oreMap.Value == 152 && oreMap.Type.Contains("Silicon_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 35; oreMap.Depth = 5; }
                        // if (oreMap.Value == 156 && oreMap.Type.Contains("Silicon_01") == true) { oreMap.Type = "Uraninite_01"; oreMap.Start = 85; oreMap.Depth = 10; }
                        // if (oreMap.Value == 160 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 41; oreMap.Depth = 5; }
                        // if (oreMap.Value == 164 && oreMap.Type.Contains("Nickel_01") == true) { oreMap.Type = "Platinum_01"; oreMap.Start = 96; oreMap.Depth = 10; }
                    }
                }

                planet.OreMappings = oreList.ToArray();
            }
        }
    }
}
