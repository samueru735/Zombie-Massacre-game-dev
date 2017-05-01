using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlDotNet.Graphics;
using System.Drawing;
using System.ComponentModel;
using SdlDotNet.Audio;

namespace Zombie_Massacre
{
    public abstract class Weapon:SpecialObject
    {
        protected int damage;
        protected int maxAmmo;
        protected int angle;     
        protected bool readyToShoot = true;
        public List<Bullet> bulletList ;
        protected int cooldown;
        protected int direction;
        protected Sound weaponSound;
        protected BackgroundWorker wb;
        protected Point posRelToHero;

        public Weapon(Surface video, Point position) : base(video, position)
        {
            this.video = video;
            this.position = position;
            wb = new BackgroundWorker();
            wb.DoWork += Wb_DoWork;
        }
        #region Properties
        public Surface Video
        {
            set { video = value; }
        }

        public List<Bullet> BulletList
        {
            get { return bulletList ; }
            set { bulletList = value; }
        }
        public int Damage {
            get { return damage; }            
        }
        public bool ReadyToShoot { get { return readyToShoot; } set { readyToShoot = value; } }
        #endregion

        protected enum HorizontalDirection { left, right, none };
        
        public void DoDamage(MoveableObject enemy)
        {
            enemy.TakeDamage(damage);
        }

        public virtual void Cooldown()
        {   // wacht een tijd vooraleer je wapen weer kan schieten
            readyToShoot = false;
            wb.RunWorkerAsync();
        }
        private void Wb_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(cooldown);
            readyToShoot = true;
            PostCooldownAction();   
        }

        protected Point BarrelExitPoint()
        {   // bepaal de plaats van waaruit de kogel moet vertrekken
            if (direction == (int)HorizontalDirection.left)
                return new Point(posRelToHero.X - 5, posRelToHero.Y);
            else
                return new Point(posRelToHero.X + 5, posRelToHero.Y);
        }

        internal virtual void PostCooldownAction()
        {   // eventuele actie wanneer het wapen klaar is voor het volgende schot            
        }        

        internal abstract void Equip(Point positionHero, int angle, int direction);
        internal abstract void Shoot();
        public abstract void Update();
        public override abstract void Draw();
    }
}
