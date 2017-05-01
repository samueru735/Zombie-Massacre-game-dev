using System;
using System.Drawing;
using SdlDotNet.Graphics;
using System.Collections.Generic;
using SdlDotNet.Audio;

namespace Zombie_Massacre
{
    internal class Shotgun : Weapon
    {
        //private int angle, direction;
        //private List<Bullet> bulletList;
        private Manager manager;
       // private Point posRelToHero;
        private Surface  shotgunRight, shotgunLeft, shotgunToUse;
        private Random rndm;
        private Sound reloadSound;
        //private Rectangle colRectangle;

        public Shotgun(Surface video, Point positionHero, int angle, Manager manager) : base(video, positionHero)
        {
            this.video = video;
            this.angle = angle;
            this.manager = manager;
            weaponSound = new Sound(@"Assets\Sounds\shotgun_sound.wav");
            reloadSound = new Sound(@"Assets\Sounds\shotgun_ready_sound.wav");
            posRelToHero = positionHero;
            rndm = new Random();
            bulletList = new List<Bullet>();
            cooldown = 1000;
            damage = 1;
            maxAmmo = 8;
            shotgunRight = new Surface(@"Assets\Sprites\guns\shotgun_r.png");
            shotgunLeft = shotgunRight.CreateFlippedHorizontalSurface();
            shotgunToUse = shotgunRight;
        }
        public Shotgun(Surface video, Point position) : base(video, position)  // shotgun als speciaal object op speelveld
        {
            this.video = video;
            posRelToHero = position;
            shotgunRight = new Surface(@"Assets\Sprites\guns\shotgun_r.png");
            shotgunToUse = shotgunRight;
            ColRect = new Rectangle(position, shotgunRight.Size);
        }
        public override Rectangle ColRect
        {
            get; set;
        }

        public override void Draw()
        {
            video.Blit(shotgunToUse, posRelToHero);
            if (bulletList != null)
            {
                foreach (var bullet in bulletList)
                    bullet.Draw();
            }
        }

        public override void Update()
        {
            if (direction == (int)HorizontalDirection.right)
            {
                posRelToHero.X += 13 - Math.Abs((-angle + 90) / 50);
                posRelToHero.Y += 17 - Math.Abs((-angle + 90) / 5);
                shotgunToUse = shotgunRight.CreateRotatedSurface(-angle + 90);
            }
            else
            {
                posRelToHero.X -= 14 - Math.Abs((angle - 90) / 50);
                posRelToHero.Y += 49 - Math.Abs((angle - 90) / 5);
                shotgunToUse = shotgunLeft.CreateRotatedSurface(-angle - 90);
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                bulletList[i].Update();
            }
        }

        internal override void Equip(Point positionHero, int angle, int direction)
        {
            this.direction = direction;
            posRelToHero = positionHero;
            this.angle = angle;

            switch (direction)
            {
                case (int)HorizontalDirection.left:
                    shotgunToUse = shotgunLeft; break;
                case (int)HorizontalDirection.right:
                    shotgunToUse = shotgunRight; break;                    
            }
        }

        internal override void Shoot()
        {            
            for(int i = 0; i < 12; i++)
            {
                int angleDeviation = rndm.Next(35);
                int positiveOrNegativeDeviation = rndm.Next(2);
                Bullet bulletShot = new Bullet(video, manager);
                switch (positiveOrNegativeDeviation)
                {
                    case 0:
                        bulletShot = new Bullet(video, this, manager, angle + angleDeviation, direction, BarrelExitPoint());
                        break;
                    case 1:
                        bulletShot = new Bullet(video, this, manager, angle - angleDeviation, direction, BarrelExitPoint());
                        break;
                }
                bulletList.Add(bulletShot);
                manager.MoveableObjects.Add(bulletShot);
            }
            try
            {
                weaponSound.Play();
            }
            catch (SdlDotNet.Core.SdlException e) //te weinig channels beschikbaar
            {
                weaponSound.Dispose();     // vernietig het geluid om channel weer vrij te maken                
            }
        }

        internal override void PostCooldownAction()
        {
            reloadSound.Play();
        }
    }
}