using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public abstract class SpecialObject
    {
        protected Surface video, ownSurface;
        protected Point position;


        public SpecialObject(Surface video, Point position)
        {
            this.video = video;
            this.position = position;
        }
        public abstract Rectangle ColRect
        {
            get; set;
        }
        public abstract void Draw();
    }
}
