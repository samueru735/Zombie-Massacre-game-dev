using SdlDotNet.Graphics;
using System.Drawing;


namespace Zombie_Massacre
{
    public class Door:SpecialObject
    {
        Surface door;
        private Rectangle colRect;

        public Door(Surface video, Point position):base(video, position)
        {
            this.video = video;
            door = new Surface(@"Assets\Sprites\door.png");
            colRect = new Rectangle(position, door.Size);
        }
        

        public override Rectangle ColRect
        {
            get { return colRect; }
            set { colRect = value; }
        }

        public override void Draw()
        {
            video.Blit(door, position);
        }
    }
}
