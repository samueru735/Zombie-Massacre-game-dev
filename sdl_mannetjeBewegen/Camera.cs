using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public class Camera
    {
        MoveableObject objectToFollow;
        private Rectangle rectCamera;
        private int level_width, level_height;

        public Camera(int width, int height, int level_width, int level_height, MoveableObject thingToFollow) // thing is de Hero
        {
            RectCamera = new Rectangle(0, 0, width, height);
            this.level_width = level_width;
            this.level_height = level_height;
            objectToFollow = thingToFollow;
        }

        public Rectangle RectCamera
        {
            get {return rectCamera;}
            set {rectCamera = value; }
        }

        public void Update()
        {   // volg de hero met hero als centraal punt, behalve wanneer ie bij de rand van het level is
            rectCamera.X = objectToFollow.Position.X + objectToFollow.Width / 2 - RectCamera.Width / 2;
            rectCamera.Y = objectToFollow.Position.Y + objectToFollow.Height / 2 - RectCamera.Height / 2;
            if (rectCamera.X < 0)
                rectCamera.X = 0;
            if (rectCamera.Y < 0)
                rectCamera.Y = 0;
            if (rectCamera.X > level_width - RectCamera.Width)
                rectCamera.X = level_width - rectCamera.Width;
            if (rectCamera.Y > level_height - RectCamera.Height)
                rectCamera.Y = level_height - rectCamera.Height;
        }
    }
}
