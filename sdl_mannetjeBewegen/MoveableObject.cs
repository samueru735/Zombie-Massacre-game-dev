using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public abstract class MoveableObject
    {
        protected float xVelocity;
        protected float yVelocity;
        protected float gravity;
        protected int hp;
        protected Point position;
        protected Surface video;
        protected Surface image;
        protected Rectangle visibleRectangleStance;
        public Rectangle colRectangle;
        protected enum HorizontalDirection { left, right, none };
        protected enum VerticalDirection { down, up, none };
        protected int horizontalDirection, verticalDirection;
        protected int direction;
        protected int lastDirection;
        protected int positionYbeforeJump;
        protected int nuberOfWalkingFrames;
        protected Manager manager;
        protected int maxJumpHeight = 46;

        #region Properties
        public bool On_the_ground { get; internal set; }
        public int Height
        { get; set; }

        public Point Position
        {
            get { return position;}
            set { position = value; }            
        }
        public int Width
        { get; set; }

        public float YVelocity
        { get { return yVelocity; } set { yVelocity = value; } }

        public bool Dead { get; internal set; }

        public MoveableObject(Surface video, Manager manager)
        {
            this.video = video;
            this.manager = manager;
        }
        #endregion

        public abstract void Update();

        public abstract void Draw();

        internal abstract void HitWall(string direction, int distance);       
                              

        protected void SetMoveDirection()
        {
            bool hitScreen = HitScreenBorders(direction);
            if (direction != (int)HorizontalDirection.none)
            {
               
                if(!HitScreenBorders(direction)) // als het object de rand van het scherm niet raakt
                {
                    switch (direction)
                    {
                        case (int)HorizontalDirection.left:
                            if (xVelocity > 0)
                            {
                                xVelocity = -xVelocity;                                
                            }
                            lastDirection = (int)HorizontalDirection.left;
                            break;
                        case (int)HorizontalDirection.right:
                            if (xVelocity < 0)
                            {
                                xVelocity = -xVelocity;
                                lastDirection = (int)HorizontalDirection.right;
                            }
                            break;
                    }
                }
                
                if (direction != lastDirection)
                {
                    xVelocity = -xVelocity;     // omkeren
                }
                position.X += (int)xVelocity;
            }
            if (On_the_ground)
            {
                positionYbeforeJump = position.Y;
                yVelocity = 0;
            }

            if (verticalDirection == (int)VerticalDirection.up && position.Y > positionYbeforeJump - maxJumpHeight)
            {
                position.Y += (int)(yVelocity * gravity);
                On_the_ground = false;
            }
            else if (verticalDirection == (int)VerticalDirection.up &&
                position.Y < positionYbeforeJump - maxJumpHeight && position.Y > positionYbeforeJump - 1.5f * maxJumpHeight)
            {
                position.Y += (int)(yVelocity / 2 * gravity);
                On_the_ground = false;
            }                
            else
            {
                if (!On_the_ground) // falling
                {
                    yVelocity = 3;
                    position.Y += (int)(yVelocity * gravity);
                }
            }
        }

        internal abstract bool HitScreenBorders(int direction);
        public virtual void TakeDamage(int damage)
        {
            hp -= damage;
        }
    }
}
