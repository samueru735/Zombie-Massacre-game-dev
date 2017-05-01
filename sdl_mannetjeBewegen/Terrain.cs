using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public class Terrain
    {
        private List<Surface> imageTileList;
        private List<Rectangle> realTileList;
        private Surface image;
        private Rectangle imageTile;       
        private Surface tVideo;
        private Size size;
        private int blokSize;

        public Terrain()
        {
            imageTileList = new List<Surface>();
            for (int i = 0; i < 4000; i++)
            {
                imageTileList.Add(null);
            }
            realTileList = new List<Rectangle>();
            blokSize = 16;
            size = new Size { Height = blokSize, Width = blokSize};
            image = new Surface(@"Assets\Sprites\terrain.png");            
        }

        public List<Surface> ImageTileList
        {
            get { return imageTileList; }     // visuele representatie
        }
        public List<Rectangle> RealTileList
        {
            get { return realTileList; }    // gebruikt voor collision detectie
        }

 

        public void Add(int imageRow, int imageColumn, Point position, int i, int j)
        {
            int index = i + j * 100;
            tVideo = new Surface(size);
            imageTile = new Rectangle(imageRow*(blokSize+1), imageColumn*(blokSize+1), blokSize, blokSize); //bloksize +1, want tussen elk prentje is er 1 colom witte pixels
            tVideo.Blit(image, new Point(0, 0), imageTile);
            imageTileList[index] = tVideo;
            realTileList.Add( new Rectangle(position, size));           
        }
        public void Add(Surface extra, int i, int j)
        {
            int index = i + j * 100;
            imageTileList[index] = extra;
        }

        public void Draw(Surface video)
        {            
            for (int i = 0; i < imageTileList.Count; i++)
            {
                if (imageTileList[i] != null)
                {
                    if (i > 99)     // einde van de byte rij
                    {
                        int l = i / 100;
                        int k = i % 100;
                        video.Blit(imageTileList[i], new Point(k * blokSize, l * blokSize));
                    }
                    else
                        video.Blit(imageTileList[i], new Point(i * blokSize, 0));
                }
            }            
        }
    }
}
