using SdlDotNet.Audio;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zombie_Massacre
{
    public enum State { init, homeScreen, buildLevel, game, gameOver, quit, finish }

    public class Manager
    {        
        // variabelen
        private Surface m_Video, m_Level, background, heart, game_over, font_start, font_exit, font_finish, font_finish2;
        private List<Surface> m_menuItemsList;
        private Rectangle rectStart, rectExit, temp;
        private List<Rectangle> rectMenuItemsList;        
        private SdlDotNet.Graphics.Font font;
        private const int SCREEN_WIDTH = 1000;
        private const int SCREEN_HEIGHT = 480;
        private const int LEVEL_WIDTH = 1600;
        private const int LEVEL_HEIGHT = 640;
        private Hero hero;
        private Weapon currentWeapon;
        private List<Enemy> enemies = new List<Enemy>();
        private BossEnemy bossEnemy;
        private Level activeLevel;
        private Level1 level1;
        private Level2 level2;
        private Level3 level3;
        private Terrain terrain;
        private int tileSize = 16;
        private List<MoveableObject> moveableObjects;
        private List<SpecialObject> specialObjects;
        
        private Camera camera;
        private bool done;

                
        public Manager()
        {                        
            GameState = (int)State.init;    
            Events.Tick += new EventHandler<TickEventArgs>(Events_Tick);
            Events.Quit += Events_Quit;
            Events.Fps = 60;
            Events.Run();
        }
        private static void AudioPlayBackThread()
        {
            Music backGroundMusic = new Music(@"Assets\Sounds\zombie_music_1.wav");
            MusicPlayer.Volume = 100;
            MusicPlayer.Load(backGroundMusic);
            MusicPlayer.Play(true);         // true voor loop
        }
        #region Properties
        public int GameState { get; set; }
        public List<MoveableObject> MoveableObjects
        {
            get { return moveableObjects; }
            set { moveableObjects = value; }
        }
        public Weapon CurrentWeapon
        {
            get { return currentWeapon; }
        }
        #endregion

        #region Gamestate_methods
        private void Init()
        {   // initialiseer m_Video, levels en Event Listeners
            m_Video = Video.SetVideoMode(SCREEN_WIDTH, SCREEN_HEIGHT,false,false,false,true);
            background = new Surface(@"Assets\Sprites\background_1.jpg");
            heart = new Surface(@"Assets\Sprites\stats\heart.png");            
            m_menuItemsList = new List<Surface>();      
            rectMenuItemsList = new List<Rectangle>();
            specialObjects = new List<SpecialObject>();     // lijst met speciale objecten in het level (wapens, deuren)    
            level1 = new Level1(m_Video);
            level2 = new Level2(m_Video);
            level3 = new Level3(m_Video);
            activeLevel = level1;                        
            Events.MouseMotion += Events_MouseMotion;           
            Events.MouseButtonDown += Events_MouseButtonDown;
            Events.MouseButtonUp += Events_MouseButtonUp;            
            Thread audioThread = new Thread(new ThreadStart(AudioPlayBackThread));
            audioThread.Start();            
            done = true;
        }
        private void HomeScreen()
        {            
            if (!done)  // build homescreen 
            {
                font = new SdlDotNet.Graphics.Font(@"Assets\Fonts\Xeranthemum.ttf", 45);    //      font type, font size 
                font_start = font.Render("START", Color.White);
                font_exit = font.Render("EXIT", Color.White);
                m_menuItemsList.Add(font_start);                
                m_menuItemsList.Add(font_exit);
                rectStart = new Rectangle(new Point(m_Video.Width / 2 - font_start.Width / 2, m_Video.Height / 3), font_start.Size);    // startknop aanmaken
                rectExit = new Rectangle(new Point(m_Video.Width / 2 - font_exit.Width / 2, m_Video.Height - m_Video.Height / 3), font_exit.Size); // exitknop
                rectMenuItemsList.Add(rectStart);       // knoppen worden in een rectangle lijst gestoken om te kunnen reageren op mouse input
                rectMenuItemsList.Add(rectExit);
                m_Video.Blit(background);                
                done = true;
            }                        
            m_Video.Blit(font_start, new Point(rectStart.X, rectStart.Y));
            m_Video.Blit(font_exit, new Point(rectExit.X, rectExit.Y));
        }
        private void Game(TickEventArgs e)
        {
            m_Level.Blit(background);
            m_Level.Update(camera.RectCamera);
            UpdateAll(e);
            WorldAwareness();
            HitDetection();
            DrawAll(e);
        }

        private void BuildLevel()
        {

            if (activeLevel.GetType() == typeof(Level1))
            {
                m_Level = new Surface(LEVEL_WIDTH, LEVEL_HEIGHT);
                activeLevel = new Level1(m_Level, tileSize);
            }
            else if (activeLevel.GetType() == typeof(Level2))
            {
                m_Level = new Surface(LEVEL_WIDTH, LEVEL_HEIGHT);
                activeLevel = new Level2(m_Level, tileSize);
            }
            else if (activeLevel.GetType() == typeof(Level3))
            {
                m_Level = new Surface(LEVEL_WIDTH, LEVEL_HEIGHT);
                activeLevel = new Level3(m_Level, tileSize);
                background = new Surface(@"Assets\Sprites\background_lvl1.jpg");
            }
            specialObjects = activeLevel.SpecialObjects;
            terrain = activeLevel.GetTerrain();

            if (hero != null && hero.Dead != true)
            {
                Hero tempHero = hero;
                moveableObjects = new List<MoveableObject>();
                hero = new Hero(m_Level, activeLevel.HeroPosition, "keyboard", this) { HP = tempHero.HP, Weapons = tempHero.Weapons, On_the_ground = true };
                hero.RefreshWeapons();
                hero.CurrentWeapon = hero.Weapons.Last();
            }
            else
            {
                moveableObjects = new List<MoveableObject>();
                hero = new Hero(m_Level, activeLevel.HeroPosition, "keyboard", this) { On_the_ground = true };
            }
            moveableObjects.Add(hero);
            camera = new Camera(SCREEN_WIDTH, SCREEN_HEIGHT, LEVEL_WIDTH, LEVEL_HEIGHT, hero);
            currentWeapon = hero.CurrentWeapon;
            foreach (var enemyPosition in activeLevel.EnemyPositions)
            {
                enemies.Add(new Enemy(m_Level, enemyPosition, this, true)); // moving
            }
            foreach (var idleEnemyPosition in activeLevel.IdleEnemyPositions)
            {
                enemies.Add(new Enemy(m_Level, idleEnemyPosition, this, false));  // not moving
            }
            if (activeLevel.BossPosition != null && activeLevel.BossPosition != new Point(0, 0))
            {
                bossEnemy = new BossEnemy(m_Level, activeLevel.BossPosition, this, false);
                enemies.Add(bossEnemy);
                bossEnemy.BossSound.Play();
            }
            moveableObjects.AddRange(enemies);
            done = true;
        }
        private void GameOver()
        {
            if (activeLevel.GetType() != typeof(Level3))    // level 3 is boss level, die mag je opnieuw proberen zonder de andere levels te moeten doorlopen
                activeLevel = level1;                       // anders bij game over terug naar level 1
            game_over = new Surface(@"Assets\Sprites\game_over.png");
            game_over = game_over.CreateStretchedSurface(new Size(m_Video.Width, m_Video.Height));
            ClearAllLists();
            
            m_Video.Fill(Color.Red);
            m_Video.Blit(game_over);
        }
        private void ShowFinish()
        {
            font = new SdlDotNet.Graphics.Font(@"Assets\Fonts\Xeranthemum.ttf", 20);
            font_finish = font.Render("Congratulations, you've saved yourself", Color.White);
            font_finish2 = font.Render("from this horrible appocalypse!", Color.White);
            rectStart = new Rectangle(new Point(m_Video.Width / 2 - font_finish.Width / 2, m_Video.Height / 3), font_finish.Size);
            rectExit = new Rectangle(new Point(m_Video.Width / 2 - font_finish2.Width / 2, m_Video.Height - m_Video.Height / 3), font_finish2.Size);
            m_Video.Fill(Color.Black);
            done = true;
            m_Video.Blit(font_finish, new Point(rectStart.X, rectStart.Y));
            m_Video.Blit(font_finish2, new Point(rectExit.X, rectExit.Y));
        }
        #endregion

        #region Mouse_input
        private void Events_MouseMotion(object sender, SdlDotNet.Input.MouseMotionEventArgs e)
        {
            if (GameState == (int)State.homeScreen)
            {
                temp = new Rectangle(e.Position, new Size(1, 1));       // maak van je muispositie een kleine rechthoek
                for (int i = 0; i < rectMenuItemsList.Count; i++)
                {
                    if (temp.IntersectsWith(rectMenuItemsList[i]))      // muis komt op de menu tekst
                        m_menuItemsList[i].ReplaceColor(Color.White, Color.Yellow);
                    else
                        m_menuItemsList[i].ReplaceColor(Color.Yellow, Color.White);
                }
            }
        }
        private void Events_MouseButtonDown(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (GameState == (int)State.homeScreen)
            {
                for (int i = 0; i < rectMenuItemsList.Count; i++)
                {
                    if (temp.IntersectsWith(rectMenuItemsList[i]))
                        m_menuItemsList[i].ReplaceColor(Color.Yellow, Color.Red);
                    else
                        m_menuItemsList[i].ReplaceColor(Color.Red, Color.White);
                }
            }
        }
        private void Events_MouseButtonUp(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            switch (GameState)
            {
                case (int)State.homeScreen:     // check voor het aanklikken van knoppen op homescreen
                    for (int i = 0; i < rectMenuItemsList.Count; i++)
                    {
                        if (temp.IntersectsWith(rectMenuItemsList[i]))
                            switch (i)
                            {
                                case 0: // start
                                    GameState = (int)State.buildLevel; done = false;
                                    break;
                                case 1: // exit
                                    GameState = (int)State.quit;
                                    break;
                            }
                    }
                    break;
                case (int)State.gameOver:       // bij game over klik je om terug naar het hoofdscherm te gaan
                    GameState = (int)State.homeScreen;
                    done = false;
                    break;
                case (int)State.finish:         
                    GameState = (int)State.homeScreen;
                    background = new Surface(@"Assets\Sprites\background_1.jpg");
                    activeLevel = level1;
                    done = false;
                    break;
            }
        }
        #endregion
     

        private void ClearAllLists()
        {
            moveableObjects.Clear();
            m_menuItemsList.Clear();
            rectMenuItemsList.Clear();
            enemies.Clear();
        }

        

        private void Events_Tick(object sender, TickEventArgs e)
        {
            CheckGameState(e);
            m_Video.Update();               
        }

        private static void Events_Quit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }

        private void CheckGameState(TickEventArgs e)
        {
            switch (GameState)
            {
                case (int)State.init: if (!done) Init();
                    else {GameState = (int)State.homeScreen;
                        done = false; }
                    break;
                case (int)State.homeScreen: HomeScreen();                    
                    break;
                case (int)State.buildLevel: if(!done)BuildLevel();
                    else { GameState = (int)State.game; done = false; }
                    break;
                case (int)State.game: Game(e);
                    break;
                case (int)State.gameOver: GameOver();
                    break;
                case (int)State.finish: ShowFinish();
                    break;
                case (int)State.quit: Events_Quit(null, null);
                    break;
            }
        }        

        private void DrawAll(TickEventArgs e)
        {
            hero.Draw();
            foreach (var enemy in enemies)
                enemy.Draw();
            foreach (var specialObject in specialObjects)
            {
                specialObject.Draw();
            }
            activeLevel.DrawWorld();
            m_Video.Blit(m_Level, new Point(0, 0), camera.RectCamera);
            for (int i = 0; i < hero.HP; i++)
            {
                m_Video.Blit(heart, new Point(m_Video.Width - heart.Width * i - heart.Width, 5));
            }            
        }

        private void UpdateAll(TickEventArgs e)
        {
            hero.Update();
            camera.Update();
            currentWeapon = hero.CurrentWeapon;            
            if (e.Tick % 5 == 0)        // om de 5 ticks zombies updaten, zombies bewegen van nature toch houterig 
            {
                foreach (var enemy in enemies)
                    enemy.Update();
            }
        }

        private void WorldAwareness()       // het voelen van de blokken onder/boven, links/rechts
        {            
            foreach (var moveableObject in moveableObjects)
            {
                bool onTheGround = false;
                foreach (var tile in terrain.RealTileList)
                {
                    if (moveableObject.colRectangle.IntersectsWith(tile))
                    {
                        if (moveableObject.GetType() == typeof(Bullet))
                        {
                            moveableObject.Dead = true;
                            break;
                        }                            
                        if (tile.Y >= moveableObject.colRectangle.Bottom -5 && tile.Y <= moveableObject.colRectangle.Bottom + 5 
                            && moveableObject.GetType() != typeof(Bullet)) // op de grond
                        {
                            moveableObject.On_the_ground = true;
                            onTheGround = true;
                        }
                        else 
                        {
                            if (tile.X <= moveableObject.colRectangle.X + moveableObject.colRectangle.Width && tile.X > moveableObject.colRectangle.X + moveableObject.colRectangle.Width/3 )
                            {
                                moveableObject.HitWall("right", moveableObject.colRectangle.X + moveableObject.colRectangle.Width - tile.X);
                            }
                            else if(tile.X + tile.Width >= moveableObject.colRectangle.X && tile.X + tile.Width < moveableObject.colRectangle.X + moveableObject.Width/3)
                            {
                                moveableObject.HitWall("left", tile.X + tile.Width - moveableObject.colRectangle.X);
                            }
                        }
                    }
                }
                if (onTheGround == false)
                    moveableObject.On_the_ground = false;
            }
            for (int i = 0; i < specialObjects.Count; i++)      // controlleer op collisions met speciale level objecten
            {
                if (hero.colRectangle.IntersectsWith(specialObjects[i].ColRect)){
                    ControlSpecial(specialObjects[i]); 
                }
            }            
        }

        private void ControlSpecial(SpecialObject specialObject)
        {
            if (specialObject is Weapon)
            {
                if(specialObject.GetType() == typeof(Smg))
                    hero.Weapons.Add(new Smg(m_Level, hero.Position, hero.Angle, this));
                if(specialObject.GetType() == typeof(Shotgun))
                    hero.Weapons.Add(new Shotgun(m_Level, hero.Position, hero.Angle, this));
                hero.CurrentWeapon = hero.Weapons.Last();
                hero.CurrentWeapon.Equip(hero.Position, hero.Angle, hero.Direction);                
            }
            else if(specialObject.GetType() == typeof(Door))
            {                
                if(activeLevel.GetType() == typeof(Level1))
                {
                    activeLevel = level2;
                    GameState = (int)State.buildLevel;                   
                }
                else if (activeLevel.GetType() == typeof(Level2))
                {
                    activeLevel = level3;
                    GameState = (int)State.buildLevel; 
                }
                ClearAllLists();
            }
            else if (specialObject.GetType() == typeof(Finish))
            {
                GameState = (int)State.finish;
            }
            specialObjects.Remove(specialObject);
        }

        private void HitDetection()     // kogels en andere projectielen detectie
        {
            foreach (var enemy in enemies)
            {                
                if (enemy.colRectangle.IntersectsWith(hero.colRectangle))
                {
                    Console.WriteLine("patat");
                    hero.TakeDamage(1);
                }
                foreach (var bullet in currentWeapon.BulletList)
                {
                    if (bullet.colRectangle.IntersectsWith(enemy.colRectangle))
                    {
                        currentWeapon.DoDamage(enemy);
                        Console.WriteLine("Enemy hit");
                        if (bullet.colRectangle.IntersectsWith(enemy.ColRectangleHead))
                            enemy.TakeHeadShot();
                        bullet.Dead = true;                        
                        break;
                    }                                          
                }
            }
            if(bossEnemy != null)
            {
                foreach (var fireball in bossEnemy.FireballBulletList)
                {
                    if (fireball.colRectangle.IntersectsWith(hero.colRectangle)) {
                        hero.TakeDamage(1);
                        Console.WriteLine("hit");
                        fireball.Dead = true;
                        break;
                    }
                }
            }            
        }
    }
}
