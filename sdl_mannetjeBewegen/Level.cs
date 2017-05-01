using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Zombie_Massacre
{
    public abstract class Level
    {
        protected byte[,] byteTileArray;        
        protected Surface video;
        protected int gravity;
        protected Terrain terrain;
        protected int blokSize;
        private List<Point> enemyPositions = new List<Point>();
        private List<Point> idleEnemyPositions = new List<Point>();
        private List<SpecialObject> specialObjects = new List<SpecialObject>();
        private Point bossPosition;

        public Point HeroPosition { get; internal set; }        

        public Level(Surface video)
        {
            this.video = video;
        }
        public Point BossPosition
        {
            get { return bossPosition; }
        }
        public void DrawWorld()
        {   
            if(terrain != null)         
                terrain.Draw(video);
        }
        public List<Point> EnemyPositions
        {
            get { return enemyPositions; }
        }
        

        public List<Point> IdleEnemyPositions
        {
            get { return idleEnemyPositions; }   
        }
        public List<SpecialObject> SpecialObjects
        {
            get { return specialObjects; }
            set { specialObjects = value; }
        }

        protected void CreateWorld()
        {
            for (int j = 0; j < byteTileArray.GetLength(0); j++)
            {
                for (int i = 0; i < byteTileArray.GetLength(1); i++)
                {
                    switch ((byteTileArray[j, i]))
                    {
                        case 7:
                            HeroPosition = new Point(i * blokSize, j * blokSize - 26);
                            break;
                        case 11:
                            terrain.Add(0, 1, new Point(i * blokSize, j * blokSize), i, j);
                            break;
                        case 2:
                            //terrain.Add(new Surface(@"Assets\Sprites\guns\smg_r.png"), i, j);
                            specialObjects.Add(new Smg(video, new Point(i * blokSize, j * blokSize)));                            
                            break;
                        case 8:
                            specialObjects.Add(new Shotgun(video, new Point(i * blokSize, j * blokSize)));
                            break;
                        case 9:
                            specialObjects.Add(new Door(video, new Point(i*blokSize, j*blokSize)));
                            break;
                        case 19:
                            bossPosition = new Point(i * blokSize, j * blokSize - 90);
                            break;
                        case 45:
                            specialObjects.Add(new Finish(video, new Point(i * blokSize, j * blokSize - 55)));
                            break;
                        case 50:
                            terrain.Add(2, 1, new Point(i * blokSize, j * blokSize), i, j);
                            break;
                        case 51:
                            terrain.Add(3, 3, new Point(i * blokSize, j * blokSize), i, j);
                            break;
                        case 112:
                            idleEnemyPositions.Add(new Point(i * blokSize, j * blokSize - 31));
                            break;
                        case 113:
                            enemyPositions.Add(new Point(i * blokSize, j * blokSize - 31));
                            break;
                        
                    }
                }
            }
        }

        public Terrain GetTerrain()
        {
            return terrain;
        }

        internal int GetTile(int left, int bottom)
        {
            int xTile = (left / blokSize);
            int yTile = ((bottom + 2) / blokSize);
            if (xTile + 1 < byteTileArray.GetLength(1) && yTile + 1 < byteTileArray.GetLength(0))
            {
                if (byteTileArray[yTile + 1, xTile + 1] == 0)
                    return bottom + 1;
            }
            return 0;
        }
    }

}
