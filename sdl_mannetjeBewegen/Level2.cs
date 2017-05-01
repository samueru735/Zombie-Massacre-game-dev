﻿using _2_ViewMapEditor;
using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public class Level2 : Level
    {
        private MapModel level;              

        public Level2(Surface video): base(video){}
        public Level2(Surface video, int blokSize) : base(video)
        {
            level = new MapModel(@"Assets\Maps\level2");
            this.blokSize = blokSize;
            byteTileArray = level.Map;
            terrain = new Terrain();
            CreateWorld();
        }
    }
}
