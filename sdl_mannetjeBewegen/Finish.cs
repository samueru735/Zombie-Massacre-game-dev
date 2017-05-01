using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public class Finish : SpecialObject
    {
        Surface finish;
        private Rectangle colRect;

        public Finish(Surface video, Point position) : base(video, position)
        {
            this.video = video;
            finish = new Surface(@"Assets\Sprites\finish.png");
            colRect = new Rectangle(position, finish.Size);
        }

        public override Rectangle ColRect
        {
            get { return colRect; }
            set { colRect = value; }
        }

        public override void Draw()
        {
            video.Blit(finish, position);
        }
    }
}
