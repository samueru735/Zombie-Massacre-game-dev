using _2_ViewMapEditor;
using SdlDotNet.Graphics;


namespace Zombie_Massacre
{
    public class Level3 : Level
    {
        private MapModel level;               

        public Level3(Surface video) : base(video){}
        public Level3(Surface video, int blokSize) : base(video)
        {
            level = new MapModel(@"Assets\Maps\level3");
            this.blokSize = blokSize;
            byteTileArray = level.Map;
            terrain = new Terrain();
            CreateWorld();
        }
    }
}
