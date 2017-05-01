using SdlDotNet.Core;
using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SdlDotNet.Input;
using SdlDotNet.Audio;

namespace Zombie_Massacre
{
    public class Hero: MoveableObject
    {
        private Surface imageLeft, imageAirLeft;
        private Surface imageRight, imageAirRight;
        private Point originalPosition;                     
        private string controlMethod;
        private Weapon currentWeapon;
        private List<Weapon> weapons;
        private int angle, shotCount;
        private long updateCount;
        private bool shotFired;
        private Sound owwSound, aww;

        public Hero(Surface video, Point position, string controlMethod, Manager manager):base(video, manager)
        {
            this.controlMethod = controlMethod;     // in dit spel niet gebruikt, maar kan bv worden gebruikt voor 2 player mode
            this.position = position;
            originalPosition = position;
            this.manager = manager;
            AssignVariables();                       

            Events.KeyboardDown += Events_KeyboardDown;
            Events.KeyboardUp += Events_KeyboardUp;
            Events.MouseButtonUp += Events_MouseButtonUp;
            Events.MouseButtonDown += Events_MouseButtonDown;
        }

        private void Events_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.PrimaryButton:
                    if (currentWeapon.ReadyToShoot)
                        shotFired = true; break;
                case MouseButton.SecondaryButton:
                    CycleWeapons(); break;
            }
        }

        private void Events_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.WheelUp:
                    if (lastDirection == (int)HorizontalDirection.right)
                        angle -= 5;
                    else
                        angle += 5;
                    break;
                case MouseButton.WheelDown:
                    if (lastDirection == (int)HorizontalDirection.right)
                        angle += 5;
                    else
                        angle -= 5;
                    break;                           
            }
        }        

        private void AssignVariables()
        {
            imageLeft = new Surface(@"Assets\Sprites\hero\duke_walk_left.png");
            imageRight = imageLeft.CreateFlippedHorizontalSurface();           
            imageAirLeft = new Surface(@"Assets\Sprites\hero\duke_jump_l.png");
            imageAirRight = imageAirLeft.CreateFlippedHorizontalSurface();
            owwSound = new Sound(@"Assets\Sounds\oww.wav");
            aww = new Sound(@"Assets\Sounds\aww.wav");
            nuberOfWalkingFrames = 5;
            angle = 90;
            shotCount = 0;
            updateCount = 0;
            weapons = new List<Weapon>();
            weapons.Add(new Pistol(video, position, angle, manager));
            currentWeapon = weapons.First();                       

            hp = 10;
            gravity = 1.5f;
            Width = 32; 
            Height = 41; 
            xVelocity = 4;
            yVelocity = 0;
            direction = (int)HorizontalDirection.none;
            lastDirection = (int)HorizontalDirection.right;
            verticalDirection = (int)VerticalDirection.none;
            positionYbeforeJump = position.Y;
            visibleRectangleStance = new Rectangle(0, 0, Width, Height);
            colRectangle = new Rectangle(position.X, position.Y, Width, Height);
        }      

        public int HP {
            get { return hp; }
            set { hp = value; }
        }

        public Weapon CurrentWeapon
        {
            get { return currentWeapon; }
            set { currentWeapon = value; }
        }
        public List<Weapon> Weapons
        {
            get { return weapons; }
            set { weapons = value; }
        }
        public int Direction
        {
            get { return direction; }

        }

        public int Angle { get { return angle; } set { angle = value; } }

        public override void Update()
        {            
            if (hp <= 0)
            {
                Dead = true;
                try
                {
                    aww.Play();
                }
                catch (SdlDotNet.Core.SdlException e) //te weinig channels beschikbaar
                {
                    aww.Dispose();     // vernietig het geluid om channel weer vrij te maken
                    aww = new Sound(@"Assets\Sounds\aww.wav");
                }
                manager.GameState = (int)State.gameOver;
            }
                
            updateCount++;
            if (shotFired)
            {
                if (currentWeapon.GetType() == typeof(Smg))
                {
                    if (shotCount < 3)
                    {
                        if (updateCount % 5 == 0)
                        {
                            Shoot();
                            shotCount++;
                        }                        
                    }
                    else
                    {
                        shotCount = 0;
                        shotFired = false;
                        updateCount = 0;
                        currentWeapon.Cooldown();
                    }                        
                }
                else
                {
                    Shoot();
                    shotFired = false;
                    currentWeapon.Cooldown();
                } 
            }
            SetMoveDirection();
            if (direction != (int)HorizontalDirection.none && On_the_ground)
            {
                visibleRectangleStance.X += Width;
                if (visibleRectangleStance.X >= nuberOfWalkingFrames * Width)
                    visibleRectangleStance.X = 0;
            }
            if (! On_the_ground)
            {
                visibleRectangleStance.X = 0;
            }
            UpdateColRecangle();
            currentWeapon.Equip(position, angle, lastDirection);
            currentWeapon.Update();       
        }

        private void UpdateColRecangle()
        {
            colRectangle.X = position.X;
            colRectangle.Y = position.Y;
        }

        private void Events_KeyboardUp(object sender, KeyboardEventArgs e)
        {
            if (controlMethod == "arrows")
            {
            /*      */
            }
            else if (controlMethod == "keyboard")
            {
                switch (e.Key)
                {
                    case Key.A:
                        direction = (int)HorizontalDirection.none; 
                        break;
                    case Key.D:
                        direction = (int)HorizontalDirection.none; 
                        break;
                    case Key.LeftArrow:
                        direction = (int)HorizontalDirection.none;
                        break;
                    case Key.RightArrow:
                        direction = (int)HorizontalDirection.none;
                        break;
                    case Key.Space:
                        EndJump(); break;             
                }
            }
        }        

        private void EndJump()
        {
            if (yVelocity < -3)
                yVelocity = -3;
            verticalDirection = (int)VerticalDirection.down;
        }

        private void StartJump()
         {
            if (On_the_ground)
            {
                verticalDirection = (int)VerticalDirection.up;
                yVelocity = -6;                
                On_the_ground = false;
            }
            else
                Console.WriteLine("In the air");
        }

      private void Events_KeyboardDown(object sender, KeyboardEventArgs e)
        {
            if (controlMethod == "arrows")
            {
               /* */
            }
            else if (controlMethod == "keyboard")
            {
                switch (e.Key)
                {
                    case Key.A:
                        direction = (int)HorizontalDirection.left;
                        if (direction != lastDirection)
                            angle = 270; break;
                    case Key.LeftArrow:
                        direction = (int)HorizontalDirection.left;
                        if (direction != lastDirection)
                            angle = 270; break;
                    case Key.D:
                        direction = (int)HorizontalDirection.right;
                        if (direction != lastDirection)
                            angle = 90; break;
                    case Key.RightArrow:
                        direction = (int)HorizontalDirection.right;
                        if (direction != lastDirection)
                            angle = 90; break;
                    case Key.W:
                        if(lastDirection == (int)HorizontalDirection.right)
                            angle-=5; 
                        else
                            angle += 5;
                        break;
                    case Key.UpArrow:
                        if (lastDirection == (int)HorizontalDirection.right)
                            angle -= 5;
                        else
                            angle += 5;
                        break;

                    case Key.S:
                        if (lastDirection == (int)HorizontalDirection.right)
                            angle += 5;
                        else
                            angle -= 5; break;
                    case Key.DownArrow:
                        if (lastDirection == (int)HorizontalDirection.right)
                            angle += 5;
                        else
                            angle -= 5; break;
                    case Key.X:
                        CycleWeapons(); break;      
                    case Key.Space:
                         StartJump(); break;
                    case Key.LeftAlt:
                        if(currentWeapon.ReadyToShoot)
                            shotFired = true; break;
                }
            }            
        }
        public void RefreshWeapons()
        {
            List<Weapon> tempWeaponsList = new List<Weapon>();
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].GetType() == typeof(Pistol))
                    tempWeaponsList.Add(new Pistol(video, position, angle, manager));
                else if(weapons[i].GetType() == typeof(Smg))
                    tempWeaponsList.Add(new Smg(video, position, angle, manager));
                else if (weapons[i].GetType() == typeof(Shotgun))
                    tempWeaponsList.Add(new Shotgun(video, position, angle, manager));
            }
            weapons = tempWeaponsList;
        }

        private void CycleWeapons()
        {
            if (weapons.Count > 1 && currentWeapon != weapons.Last())
            {
                var index = weapons.IndexOf(currentWeapon);
                currentWeapon = weapons[index + 1];
            }
            else
                currentWeapon = weapons.First();
            
        }

        public override void Draw()
        {
            if(lastDirection == (int)HorizontalDirection.left)
            {
                if(On_the_ground)
                    video.Blit(imageLeft, position, visibleRectangleStance);
                else
                    video.Blit(imageAirLeft, position, visibleRectangleStance);                                
            }
                
            if (lastDirection == (int)HorizontalDirection.right)
            {
                if(On_the_ground)
                    video.Blit(imageRight, position, visibleRectangleStance);
                else
                    video.Blit(imageAirRight, position, visibleRectangleStance);                
            }
            currentWeapon.Draw();           
        }

        internal override bool HitScreenBorders(int direction)
        {
            switch (direction)
            {
                case (int)HorizontalDirection.left:
                    if (position.X - (int)xVelocity < 0) // bots tegen linkerkant scherm                        
                    {
                        position.X = 0;
                        return true;
                    }
                    break;
                case (int)HorizontalDirection.right:
                    if (position.X + (int)xVelocity > video.Width - Width) // bots tegen linkerkant scherm
                    {
                        position.X = video.Width - Width;
                        return true;
                    }
                    break;                   
            }
            if (position.Y + Height > video.Height)
            {
                TakeDamage(3);
                if (hp > 0)
                {
                    position = originalPosition;                    
                }
                return true;                    
            }
            return false;
        }
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            try
            {
                owwSound.Play();
            }
            catch (SdlException e) //       te weinig channels beschikbaar
            {
                owwSound.Dispose();     // vernietig het geluid om channel weer vrij te maken
                owwSound = new Sound(@"Assets\Sounds\bullet_hit.wav");
            }
        }

        internal override void HitWall(string direction, int distance)
        {
            if (direction == "right")
            {                
                position.X -= distance;
            }            
            else
            {                
                position.X += distance;
            }
            colRectangle.X = position.X;
        }
        private void Shoot()
        {
            currentWeapon.Shoot();
        }
    }
}
