using SdlDotNet.Core;
using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Zombie_Massacre
{
    public class Pistol: Weapon
    {
        private Surface pistolRight, pistolLeft, pistolToUse;
        private Manager manager;
        private Random rndm;
       
        public Pistol(Surface video, Point positionHero, int angle, Manager manager):base(video, positionHero)
        {
            this.video = video;
            this.angle = angle;
            this.manager = manager;
            weaponSound = new SdlDotNet.Audio.Sound(@"Assets\Sounds\pistol_sound.wav");
            posRelToHero = positionHero;
            rndm = new Random();
            bulletList = new List<Bullet>();
            cooldown = 500; // in ms            
            damage = 4;            
            pistolRight = new Surface(@"Assets\Sprites\guns\pistol_r.png");
            pistolLeft = pistolRight.CreateFlippedHorizontalSurface();
        }
        #region Properties
        private void Wb_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(750);          
            readyToShoot = true;
        }

        public Point PosRelToHero
        {
            get { return posRelToHero; }
            set { posRelToHero = value; }
        }

        public override Rectangle ColRect
        {
            get; set;
        }
        #endregion
        public override void Update()
        {
            if(direction == (int)HorizontalDirection.right)
            {
                posRelToHero.X += 18 - Math.Abs((-angle + 90) / 100);
                posRelToHero.Y += 13 - Math.Abs((-angle + 90) / 5);
                pistolToUse = pistolRight.CreateRotatedSurface(-angle+90);                
            }
            else
            {
                posRelToHero.X -= 12 - Math.Abs((angle - 90) / 100);
                posRelToHero.Y += 45 - Math.Abs((angle - 90) / 5);
                pistolToUse = pistolLeft.CreateRotatedSurface(-angle-90);
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                    bulletList[i].Update();
            }                           
        }

        internal override void Equip(Point positionHero, int angle, int direction)
        {  // verandert positie wapen naargelang de positie van de hero
            this.direction = direction;
            posRelToHero = positionHero;
            this.angle = angle;
            
            switch (direction)
            {
                case (int)HorizontalDirection.left:
                    pistolToUse = pistolLeft; break;               
                case (int)HorizontalDirection.right:
                    pistolToUse = pistolRight; break;                
                default: pistolToUse = null; break;//return null;
            }            
        }

        internal override void Shoot()
        {
            int angleDeviation;
            angleDeviation = rndm.Next(4);
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
            try
            {
                weaponSound.Play();
            }
            catch (SdlDotNet.Core.SdlException e) //te weinig channels beschikbaar
            {
                weaponSound.Dispose();     // vernietig het geluid om channel weer vrij te maken                
            }
        }

        public override void Draw()
        {
            video.Blit(pistolToUse, posRelToHero);
            foreach (var bullet in bulletList)
                bullet.Draw();
        }
    }
}
