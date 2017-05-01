using SdlDotNet.Graphics;
using System;
using System.Drawing;

namespace Zombie_Massacre
{
    public class Bullet:MoveableObject
    {
        private Surface video, bulletType;
        private float velocity;        
        private Weapon weapon;
        private BossEnemy bossEnemy;

        public Bullet(Surface video, Weapon weapon, Manager manager, int angle, int direction, Point position):base(video, manager)
        {
            this.video = video;
            this.manager = manager;
            this.weapon = weapon;
            this.position = position;

            Angle = angle;
            if(weapon.GetType() == typeof(Pistol))            
                bulletType = new Surface(@"Assets\Sprites\guns\pistol_bullet.png").CreateRotatedSurface(-angle);                                                                              
            else if (weapon.GetType() == typeof(Smg) || weapon.GetType() == typeof(Shotgun))
                bulletType = new Surface(@"Assets\Sprites\guns\smg_bullet.png").CreateRotatedSurface(-angle);
            Width = bulletType.Width;
            Height = bulletType.Height;
            colRectangle = new Rectangle(position.X, position.Y, Width, Height);
            velocity = 13;
        }
        public Bullet(Surface video, BossEnemy bossEnemy, Manager manager, int angle, int direction, Point position) : base(video, manager) // voor BossEnemy
        {
            this.video = video;
            this.manager = manager;            
            this.position = position;
            this.bossEnemy = bossEnemy;
            Angle = angle;
            bulletType = new Surface(@"Assets\Sprites\guns\fireball.png");              
            Width = bulletType.Width;
            Height = bulletType.Height;
            colRectangle = new Rectangle(position.X, position.Y, Width, Height);
            velocity = 8;
        }

        public Bullet(Surface video, Manager manager):base(video, manager)
        {
            this.video = video;
            this.manager = manager;
        }
        #region Properties
        public Surface BulletType
        {
            get { return bulletType; }
            set { bulletType = value; }
        }
        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public int Angle { get; set; }
        public int Direction { get; set; }
        #endregion
        public override void Update()
        {
            if (HitScreenBorders(direction) || Dead)
            {
                manager.MoveableObjects.Remove(this);   // kogel verdwijnt uit lijst
                if (weapon != null)
                    weapon.bulletList.Remove(this);
                else
                    bossEnemy.FireballBulletList.Remove(this);
            }                
                float x = (float)Math.Sin(Angle * (Math.PI / 180));     // bepaal kogeltraject naargelang de hoek dat ie wordt afgeschoten 
                float y = (float)(Math.Cos(Angle * (Math.PI / 180)));

                position.X += Convert.ToInt32(velocity * x);
                position.Y -= Convert.ToInt32(velocity * y);
            UpdateColRectangle();                          
        }

        private void UpdateColRectangle()
        {
            colRectangle.X = position.X;
            colRectangle.Y = position.Y;
        }

        public override void Draw()
        {
            video.Blit(bulletType, position);
        }

        internal override void HitWall(string direction, int distance)
        {
            Console.WriteLine("Bullet lost in space");
        }

        internal override bool HitScreenBorders(int direction)
        {
            switch (direction)
            {
                case (int)HorizontalDirection.left:
                    if (position.X - (int)velocity < 0) // bots tegen linkerkant scherm                        
                    {                        
                        return true;
                    }
                    break;
                case (int)HorizontalDirection.right:
                    if (position.X + (int)velocity > video.Width - Width) // bots tegen linkerkant scherm
                    {                        
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
    }
}