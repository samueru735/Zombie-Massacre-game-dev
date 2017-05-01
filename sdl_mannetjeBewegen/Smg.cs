using System;
using System.Drawing;
using SdlDotNet.Graphics;
using System.Collections.Generic;

namespace Zombie_Massacre
{
    internal class Smg : Weapon
    {
        private Manager manager;
        private Surface smgRight, smgLeft, smgToUse;
        private Random rndm;

        public Smg(Surface video, Point positionHero, int angle, Manager manager): base(video, positionHero)
        {
            this.video = video;
            this.angle = angle;
            this.manager = manager;
            weaponSound = new SdlDotNet.Audio.Sound(@"Assets\Sounds\smg_sound.wav");
            posRelToHero = positionHero;
            rndm = new Random();
            bulletList = new List<Bullet>();
            cooldown = 250;
            damage = 2;
            smgRight = new Surface(@"Assets\Sprites\guns\smg_r.png");
            smgLeft = smgRight.CreateFlippedHorizontalSurface();
            smgToUse = smgRight;        // zichtbare smg
        }
        public Smg(Surface video, Point position): base(video,position)  // smg als speciaal object op speelveld
        {
            this.video = video;
            posRelToHero = position;
            smgRight = new Surface(@"Assets\Sprites\guns\smg_r.png");
            smgToUse = smgRight;
            ColRect = new Rectangle(position, smgRight.Size);
        }
        public override Rectangle ColRect
        {
            get; set;
        }

        public override void Draw()
        {
            video.Blit(smgToUse, posRelToHero);
            if(bulletList != null)
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
                posRelToHero.Y += 13 - Math.Abs((-angle + 90)/5);
                smgToUse = smgRight.CreateRotatedSurface(-angle + 90);
            }
            else
            {
                posRelToHero.X -= 14 - Math.Abs((angle - 90) / 50);
                posRelToHero.Y += 45 - Math.Abs((angle - 90) / 5);
                smgToUse = smgLeft.CreateRotatedSurface(-angle - 90);
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                bulletList[i].Update();
            }
        }

        internal override void Equip(Point positionHero, int angle, int direction)
        {   // verandert positie wapen naargelang de positie van de hero
            this.direction = direction;
            this.posRelToHero = positionHero;
            this.angle = angle;

            switch (direction)
            {
                case (int)HorizontalDirection.left:
                    smgToUse = smgLeft; break;                
                case (int)HorizontalDirection.right:
                    smgToUse = smgRight; break;                
            }
        }

        internal override void Shoot()
        {
            int angleDeviation = rndm.Next(5);          // kogels gaat niet altijd rechtdoor
            int positiveOrNegativeDeviation = rndm.Next(2); // afwijking naar boven of beneden toe
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
            bulletList.Add(bulletShot);                 // voeg de afgevuurde kogel toe aan bulletList
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
    }
}