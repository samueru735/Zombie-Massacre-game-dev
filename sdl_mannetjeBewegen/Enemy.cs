using SdlDotNet.Audio;
using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public class Enemy : MoveableObject
    {

        protected int currentStance = 0, headsize, deathAnimationCounter, crawlOutOfGroundAnimationCounter, animationWidth;
        private Sound hitSound, headshotSound;
        protected Rectangle visibleRectWalk, visibleRectFall, visibleRectDie, visibleRectCrawl, colRectleHead;
        protected Surface imageLeft, imageRight, imageAirLeft, imageAirRight, imageDeath, imageCrawlOut, imageStance;
        protected long updateCounter;
        private long hitwallCounter;
        protected bool outOfGround, moving;
        private Random rndm;

        public Enemy (Surface video, Point position, Manager manager, bool moving) : base(video, manager)
        {
            //this.manager = manager;
            this.position.X = position.X;
            this.position.Y = position.Y;
            this.moving = moving;
            AssignVariables();
            if (moving)
            {                
                nuberOfWalkingFrames = 3;
                rndm = new Random();
                imageLeft = new Surface(@"Assets\Sprites\zombies\zombie_walk_l.png");
                imageRight = new Surface(@"Assets\Sprites\zombies\zombie_walk_r.png");
                //imageAirLeft = new Surface(@"Assets\Sprites\zombie_fall_l.png");
                imageAirRight = new Surface(@"Assets\Sprites\zombies\zombie_fall_r.png");
                imageAirLeft = imageAirRight.CreateFlippedHorizontalSurface();
                ChooseInitialDirection(rndm.Next(2));                
                visibleRectWalk = new Rectangle(0, 0, Width, Height);//(120, 144, Width, Height);
                visibleRectFall = new Rectangle(0, 0, 45, 40);
            }
            else
            {
                xVelocity = 0;
                imageStance = new Surface(@"Assets\Sprites\zombies\Zombies_2.png");
                UpdateStance();
            }            
        }
        private void AssignVariables()
        {
            headsize = 10;
            yVelocity = 0;
            gravity = 2.5f;
            hp = 10;
            outOfGround = false;
            crawlOutOfGroundAnimationCounter = 0;
            deathAnimationCounter = 0;
            imageDeath = new Surface(@"Assets\Sprites\zombies\zombie_die.png");
            imageCrawlOut = new Surface(@"Assets\Sprites\zombies\zombie_crawl_out.png");
            hitSound = new Sound(@"Assets\Sounds\bullet_hit.wav");
            headshotSound = new Sound(@"Assets\Sounds\headshot_sound.wav");
            Height = 45; 
            Width = 28;            
            animationWidth = 34;
            visibleRectDie = new Rectangle(0, 0, animationWidth, Height);
            visibleRectCrawl = new Rectangle(0, 0, animationWidth, Height);
            colRectangle = new Rectangle(position.X, position.Y, Width, Height);
            colRectleHead = new Rectangle(position.X, position.Y, Width, headsize);

        }

        private void ChooseInitialDirection(int directionNumber)
        {
            switch (directionNumber)
            {
                case 0: direction = (int)HorizontalDirection.left;
                    break;
                case 1: direction = (int)HorizontalDirection.right;
                    break;
            }
        }


        public Rectangle ColRectangleHead
        {   // voor headshots
            get { return colRectleHead; }
        }
       
        public override void Update()
        {
            if (Dead)
                Die();
            else
            {
                if (!outOfGround)
                    CrawlOutOfGround();
                else
                {
                    if(moving)
                    {
                        SetMoveDirection();
                        if (direction != (int)HorizontalDirection.none && On_the_ground)
                        {
                            visibleRectWalk.X += Width;
                            if (visibleRectWalk.X >= nuberOfWalkingFrames * Width)
                                visibleRectWalk.X = 0;
                        }
                        UpdateColRectangle();
                    }
                    else
                    {
                        UpdateStance();
                    }
                }
            }            
        }

        protected virtual void Die()
        {
            xVelocity = 0;
            colRectangle = new Rectangle(0,0,0,0);    
            if (deathAnimationCounter <= 6)
            {
                visibleRectDie.X = deathAnimationCounter * 34;                
                deathAnimationCounter++;
            }                                        
        }
        private void CrawlOutOfGround()
        {
            xVelocity = 0;            
            if (crawlOutOfGroundAnimationCounter <= 4)
            {
                visibleRectCrawl.X = crawlOutOfGroundAnimationCounter * 34;
                crawlOutOfGroundAnimationCounter++;
            }
            else
            {
                outOfGround = true;
                if(moving)
                    xVelocity = 1;
            }                
        }

        protected virtual void UpdateStance()
        {
            currentStance++;
            if (currentStance < 6)
                visibleRectangleStance = new Rectangle(Width * currentStance + 1, 86, Width, Height);
            else
            {
                visibleRectangleStance = new Rectangle(2, 86, Width, Height);            
                currentStance = 1;
            }          
        }

        protected void UpdateColRectangle()
        {
            colRectangle.X = position.X;
            colRectangle.Y = position.Y;
            colRectleHead.X = position.X;
            colRectleHead.Y = position.Y;
        }

        public override void Draw()
        {
            if (!outOfGround)
                video.Blit(imageCrawlOut, position, visibleRectCrawl);
            else
            {
                if (deathAnimationCounter > 0)
                    video.Blit(imageDeath, position, visibleRectDie);
                else
                {
                    if (moving)
                    {
                        if (lastDirection == (int)HorizontalDirection.left)
                        {                            
                            video.Blit(imageLeft, position, visibleRectWalk);                            
                        }

                        if (lastDirection == (int)HorizontalDirection.right)
                        {
                            video.Blit(imageRight, position, visibleRectWalk);
                        }
                    }
                    else
                    {
                        video.Blit(imageStance, position, visibleRectangleStance);
                    }                    
                }
            }                        
        }

        internal override bool HitScreenBorders(int direction)
        {
                switch (direction)
                {
                    case (int)HorizontalDirection.left:
                        if (position.X - (int)xVelocity < 0) // bots tegen linkerkant scherm                        
                        {
                            position.X = 0;
                            this.direction = (int)HorizontalDirection.right;
                            return true;
                        }
                        break;
                    case (int)HorizontalDirection.right:
                        if (position.X + (int)xVelocity > video.Width - Width) // bots tegen linkerkant scherm
                        {
                            position.X = video.Width - Width;
                            this.direction = (int)HorizontalDirection.left;
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
                hitwallCounter = updateCounter + 1;            
            return false;
        }

        internal void TakeHeadShot()
        {                       
            hp -= manager.CurrentWeapon.Damage * 2;
            ControlDeath();
            try
            {
                headshotSound.Play();
            }
            catch (SdlDotNet.Core.SdlException e) //te weinig channels beschikbaar
            {
                headshotSound.Dispose();     // vernietig het geluid om channel weer vrij te maken
                headshotSound = new Sound(@"Assets\Sounds\headshot_sound.wav");
            }
        }

        private void ControlDeath()
        {
            if (hp <= 0)
            {
                if (outOfGround != true)
                    hp = 1;
                else
                    Dead = true;
            }               
        }

        internal override void HitWall(string direction, int distance)
        {   // bots tegen muur en reageer

            if (direction == "right")
            {
                this.direction = (int)HorizontalDirection.left;
                position.X -= distance;
            }               
            else
            {
                this.direction = (int)HorizontalDirection.right;
                position.X += distance;
            }
            colRectangle.X = position.X;
        }
        public override void TakeDamage(int damage)
        {
            hp -= damage;
            try
            {
                hitSound.Play();
            }
            catch (SdlDotNet.Core.SdlException e) //te weinig channels beschikbaar
            {                
                hitSound.Dispose();     // vernietig het geluid om channel weer vrij te maken
                hitSound = new Sound(@"Assets\Sounds\bullet_hit.wav");
            }            
            ControlDeath();
        }
    } 
}
