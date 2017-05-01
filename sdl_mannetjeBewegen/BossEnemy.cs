using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlDotNet.Graphics;
using SdlDotNet.Audio;

namespace Zombie_Massacre
{
    public class BossEnemy : Enemy
    {
       // private Rectangle visibleRectWalk;
        private Random rndm;
        private int angle;
        private List<Bullet> fireballBulletList;

        public BossEnemy(Surface video, Point position, Manager manager, bool moving) : base(video, position, manager, moving)
        {
            this.position.X = position.X;
            this.position.Y = position.Y;
            this.moving = moving;
            AssignVariables();
        }
        public Sound BossSound{ get; set; }
        private void AssignVariables()
        {
            headsize = 40;
            hp = 100;
            Width = 100;
            Height = 110;
            currentStance = 1;
            imageLeft = new Surface(@"Assets\Sprites\zombies\zombie_boss.png");
            BossSound = new Sound(@"Assets\Sounds\boss_sound.wav");
            visibleRectangleStance = new Rectangle(0, 0, Width, Height);
            colRectangle = new Rectangle(position.X, position.Y, Width, Height);
            colRectleHead = new Rectangle(position.X, position.Y, Width, headsize);
            rndm = new Random();
            fireballBulletList = new List<Bullet>();
            outOfGround = true;
            angle = -90;
        }
        public List<Bullet> FireballBulletList
        {
            get { return fireballBulletList; }
            set { fireballBulletList = value; }
        }

        protected override void UpdateStance()
        {
            currentStance++;
            if (currentStance < 8)
                visibleRectangleStance.X = Width * currentStance + 1;
            else
            {
                visibleRectangleStance.X = 0;
                currentStance = 1;
            }
            colRectangle.Y = position.Y;
        }
        public override void Update()
        {
            updateCounter++;
            if (Dead)
            {
                Die();
                fireballBulletList.Clear();
            }                
            else
            {
                UpdateStance();                
                for (int i = 0; i < fireballBulletList.Count; i++)
                {
                    fireballBulletList[i].Update();
                }
                if(updateCounter %5 == 0)
                    ShootFireballs();
            }
        }
        public override void Draw()
        {
            if (deathAnimationCounter > 0)
                video.Blit(imageDeath, position, visibleRectDie);
            else
            {
                video.Blit(imageLeft, position, visibleRectangleStance);
                foreach (var fireball in fireballBulletList)
                    fireball.Draw();
            }                       
        }

        private void ShootFireballs()
        {
            for (int i = 0; i < 2; i++)
            {
                int angleDeviation = rndm.Next(40);
                int positiveOrNegativeDeviation = rndm.Next(2);
                Bullet bulletShot = new Bullet(video, manager);
                switch (positiveOrNegativeDeviation)
                {
                    case 0:
                        bulletShot = new Bullet(video, this, manager, angle + angleDeviation, direction, position);
                        break;
                    case 1:
                        bulletShot = new Bullet(video, this, manager, angle - angleDeviation, direction, position);
                        break;
                }
                fireballBulletList.Add(bulletShot);
                manager.MoveableObjects.Add(bulletShot);
            }          
        }
    }
}
