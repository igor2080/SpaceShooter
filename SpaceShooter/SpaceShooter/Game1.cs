using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;



namespace Shooter_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerShip, shooter, boom, menuArrow, enemyBullet, expBar, exp, speedy, bullet, enemyBoss, select, selected, UIBackground, chaser;
        SpriteFont font, scoreFont, UIfont, bigFont;
        Rectangle playerRect, arrowRect, expBarRect, expRect, selectedRect, UIrect;

        enum GameScreen
        {
            Menu,
            MenuCreateScreen,
            LoadScreen,
            Game,
            SaveScreen,
            Pause,
            Inactive,
        }
        enum Stage
        {
            One,
            Two,
            Three,
            Four,
            Five
        }
        enum CreatePlayer
        {
            None,
            NameScreen,
            SaveSlotScreen,

        }
        [Serializable]
        public struct SaveGameData
        {
            public string[] Name;
            public int[] Level;
            // public int[] Points;
            public SaveGameData(int max)
            {
                Name = new string[max];
                Level = new int[max];
                //Points = new int[max];
            }
        }

        static SaveGameData data = new SaveGameData(3);
        int width = 0, height = 0;
        KeyboardState Kstate, oldKstate;

        ///>Versions<
        ///0.8 - Monster HP update - released
        ///0.9 - Content update - released
        ///1.0 - UI update - released
        ///1.1 - Stage update - pre-released
        ///1.2 - Animation update
        ///1.3 - Upgrade update
        ///1.4 - Game mode update



        //game upgrades
        int Xspeed = 4, Yspeed = 4; //ship speed

        double spawnRate = 2.0;//enemy spawn rate
        int lives = 5;
        int enemyhp = 1;//enemy hit points
        int enemySpeed = 2;//moving
        double enemyfireMin = 1;///time between
        double enemyfireMax = 3;///enemy shots
        int bulletdamage = 1;//bullet damage
        int Nbullets = 1;//bullet upgrade
        double amp = 0.5;//bullet fire speed
        int score = 0;//scoring system
        int lastScore = 0;//last game score
        double[] maxExp = { 5, 12, 21, 30, 42, 55, 70, 85, 100, 100, 100, 100, 100, 100, 200, 200, 200, 200, 200, 500, 500, 500, 500, 500 };
        int a = 0;//current level marker
        int maxA;//overflow level check
        double expgainrate = 0.1;//exp bar move rate

        //player stats
        static string playerName = "";
        static int level = 1;
        static int points = 0;
        static StringBuilder name = new StringBuilder();
        static int playingSlot = 0;

        Random r = new Random();
        //
        Queue<Enemy> enemies = new Queue<Enemy>();
        Enemy[] enemiesDraw;
        Enemy[] enemiesTemp;
        //
        Queue<Bullet> bullets = new Queue<Bullet>();
        Bullet[] bulletsDraw;
        Bullet[] bulletsTemp;
        //
        Queue<Rectangle> booms = new Queue<Rectangle>();
        Rectangle[] boomsDraw;
        Queue<double> boomLength = new Queue<double>();
        //
        Queue<EnemyBullet> enemyBullets = new Queue<EnemyBullet>();
        EnemyBullet[] enemyBulletsTemp;
        EnemyBullet[] enemyBulletsDraw;
        //

        Queue<double> enemygt = new Queue<double>();
        double[] tempEnemygt;
        //
        double timer = 0;//used for seconds counter
        double Mtimer = 0;//used for minute counter
        double gt = 0;//compared to timer for the bullet speed
        double levelupgt = 0;//compared for level up message
        double stagegt = 0;//compared for stage complete message
        bool spawn = true;//used for stage complete message,will be false during stage complete message
        double expgt = 0;

        double spawnTime = 0;//static enemy spawn time
        GameScreen gs;
        GameScreen gsTemp;
        Stage st = Stage.One;
        CreatePlayer cp = CreatePlayer.None;
        int atmExp = 0;
        double leveltimer;
        int expgain = 0;
        double inputgt = 0;
        //object temp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gs = GameScreen.Menu;
            width = graphics.PreferredBackBufferWidth = 800;
            height = graphics.PreferredBackBufferHeight = 600;
            playerRect = new Rectangle(368, 400, 35, 35);
            arrowRect = new Rectangle(270, 110, 20, 20);
            maxA = maxExp.Length;
            selectedRect = new Rectangle(48, 148, 154, 204);
            UIrect = new Rectangle(0, height - 150, width - 1, height - 1);
            expBarRect = new Rectangle(70, UIrect.Top + 80, 108, 30);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            select = Content.Load<Texture2D>("select");
            selected = Content.Load<Texture2D>("selected");
            playerShip = Content.Load<Texture2D>("ship");
            playerShip.Name = "ship";
            bullet = Content.Load<Texture2D>("bullet");
            font = Content.Load<SpriteFont>("font");
            shooter = Content.Load<Texture2D>("shooter");
            shooter.Name = "shooter";
            boom = Content.Load<Texture2D>("explosion");
            menuArrow = Content.Load<Texture2D>("Arrow");
            scoreFont = Content.Load<SpriteFont>("Score");
            enemyBullet = Content.Load<Texture2D>("enemyBullet");
            expBar = Content.Load<Texture2D>("expbar");
            exp = Content.Load<Texture2D>("exp");
            speedy = Content.Load<Texture2D>("speedy");
            speedy.Name = "speedy";
            enemyBoss = Content.Load<Texture2D>("userBoss");
            enemyBoss.Name = "userBoss";
            UIBackground = Content.Load<Texture2D>("silverUI");
            UIfont = Content.Load<SpriteFont>("UIFont");
            bigFont = Content.Load<SpriteFont>("bigfont");
            chaser = Content.Load<Texture2D>("chaser");
            chaser.Name = "chaser";
        }

        public static void saveGame(SaveGameData data, int slot)
        {
            data.Name[slot] = playerName;
            data.Level[slot] = level;
            //data.Points[slot] = points;

            File.Delete(fullpath);
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                stream.Close();
            }

        }
        static string fullpath = @"C:\j\igor.sav";
        public static SaveGameData loadGame(string name)
        {
            try
            {
                Directory.CreateDirectory(@"c:\j");
            }
            catch
            {

            }
            FileStream stream;
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
            try
            {
                stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);

                data = (SaveGameData)serializer.Deserialize(stream);
                if (string.IsNullOrWhiteSpace(data.Name[0]))
                    data.Name[0] = "Empty";
                if (string.IsNullOrWhiteSpace(data.Name[1]))
                    data.Name[1] = "Empty";
                if (string.IsNullOrWhiteSpace(data.Name[2]))
                    data.Name[2] = "Empty";
                stream.Close();

                return data;
            }

            catch
            {

                stream = new FileStream(fullpath, FileMode.OpenOrCreate);// File.Create(fullpath);

                data.Name[0] = "Empty";
                data.Name[1] = "Empty";
                data.Name[2] = "Empty";
                serializer.Serialize(stream, data);
                stream.Close();

                return data;
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>


        public void ExpGain(GameTime gametime, ref int expgain)
        {
            expgainrate = 0.1 / expgain;
            if (expgain > 0)
            {
                if (timer > expgt)
                {
                    atmExp += 1;
                    expgt = timer + expgainrate;
                    expgain--;
                }
            }

            if (atmExp >= maxExp[a])
            {
                atmExp = 0;
                levelupgt = timer + 2;
                points += 5;
                level++;
                if (a < maxA - 1)
                    a++;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Kstate = Keyboard.GetState();
            if (!IsActive && gs != GameScreen.Inactive)
            {
                gsTemp = gs;
                gs = GameScreen.Inactive;

            }
            else if (IsActive && gs == GameScreen.Inactive)
            {
                gs = gsTemp;
            }

            enemyhp = (int)Math.Pow(2, Mtimer);

            if (gs == GameScreen.Menu)
            {
                Kstate = Keyboard.GetState();
                timer += gameTime.ElapsedGameTime.TotalSeconds;//time counter

                if (Kstate.IsKeyDown(Keys.Down) && oldKstate.IsKeyUp(Keys.Down))
                {
                    switch (arrowRect.Y)
                    {
                        case 110:
                            arrowRect.Y = 210;
                            break;
                        case 210:
                            arrowRect.Y = 310;
                            break;
                        case 310:
                            arrowRect.Y = 110;
                            break;
                    }

                }
                if (Kstate.IsKeyDown(Keys.Up) && oldKstate.IsKeyUp(Keys.Up))
                {
                    switch (arrowRect.Y)
                    {
                        case 310:
                            arrowRect.Y = 210;
                            break;
                        case 210:
                            arrowRect.Y = 110;
                            break;
                        case 110:
                            arrowRect.Y = 310;
                            break;
                    }
                }
                if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter))
                {
                    timer = 0;
                    switch (arrowRect.Y)
                    {
                        case 110://Start a new game
                            gs = GameScreen.MenuCreateScreen;
                            cp = CreatePlayer.NameScreen;
                            timer = 0;
                            oldKstate = Kstate;

                            break;
                        case 210://load game
                            gs = GameScreen.LoadScreen;
                            timer = 0;
                            data = loadGame("igor");

                            name.Clear();
                            oldKstate = Kstate;

                            break;
                        case 310://exit
                            this.Exit();
                            break;
                    }
                }


            }
            if (gs == GameScreen.MenuCreateScreen)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;//time counter
                if (cp == CreatePlayer.NameScreen)
                {

                    foreach (Keys key in Kstate.GetPressedKeys())
                    {
                        if ((key >= Keys.A && key <= Keys.Z) || (key >= Keys.D1 && key <= Keys.D9))
                        {
                            if (timer >= inputgt)
                            {
                                if (Kstate.IsKeyDown(key) && oldKstate.IsKeyUp(key))
                                {
                                    name.Append(key.ToString());
                                }
                                inputgt = timer;
                            }
                        }
                        else if (Kstate.IsKeyDown(Keys.Back) && name.Length != 0 && timer > inputgt)
                        {
                            name.Remove(name.Length - 1, 1);
                            inputgt = timer + 0.1;
                        }
                        else if (Kstate.IsKeyDown(Keys.Escape) && oldKstate.IsKeyUp(Keys.Escape))
                        {
                            name.Clear();
                            gs = GameScreen.Menu;
                            oldKstate = Kstate;
                        }


                        else if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter) && name.Length > 1)
                        {

                            playerName = name.ToString();
                            name.Clear();
                            cp = CreatePlayer.None;
                            gs = GameScreen.Game;
                            data = loadGame("igor");

                            oldKstate = Kstate;

                            oldKstate = Kstate;
                        }

                    }
                }
                else if (cp == CreatePlayer.SaveSlotScreen)
                {
                    if (Kstate.IsKeyDown(Keys.Right) && oldKstate.IsKeyUp(Keys.Right))
                    {
                        if (selectedRect.X < 225)
                            selectedRect.X += 175;
                        else
                            selectedRect.X = 48;
                    }
                    else if (Kstate.IsKeyDown(Keys.Left) && oldKstate.IsKeyUp(Keys.Left))
                    {
                        if (selectedRect.X > 48)
                            selectedRect.X -= 175;
                        else
                            selectedRect.X = 398;
                    }
                    else if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter))
                    {
                        switch (selectedRect.X)
                        {
                            case 48:
                                playingSlot = 0;

                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                                break;
                            case 223:
                                playingSlot = 1;
                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                                break;
                            case 398:
                                playingSlot = 2;
                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                                break;

                        }
                    }
                }
            }
            if (gs == GameScreen.LoadScreen)
            {
                if (Kstate.IsKeyDown(Keys.Escape) && oldKstate.IsKeyUp(Keys.Escape))
                {
                    gs = GameScreen.Menu;
                    oldKstate = Kstate;
                }
                if (Kstate.IsKeyDown(Keys.Right) && oldKstate.IsKeyUp(Keys.Right))
                {
                    if (selectedRect.X < 225)
                        selectedRect.X += 175;
                    else
                        selectedRect.X = 48;
                }
                else if (Kstate.IsKeyDown(Keys.Left) && oldKstate.IsKeyUp(Keys.Left))
                {
                    if (selectedRect.X > 48)
                        selectedRect.X -= 175;
                    else
                        selectedRect.X = 398;
                }
                else if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter))
                {
                    switch (selectedRect.X)
                    {
                        case 48:
                            if (data.Name[0] != "Empty")
                            {
                                playingSlot = 0;
                                playerName = data.Name[playingSlot];
                                level = data.Level[playingSlot];
                                if (level >= maxExp.Length - 1)
                                {
                                    a = maxExp.Length - 1;
                                }
                                else
                                    a = level;
                                //points = data.Points[playingSlot];
                                if (level == 2)
                                    points = 5;
                                else if (level == 1)
                                    points = 0;
                                else
                                    points = level * 5;

                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                            }
                            break;
                        case 223:
                            if (data.Name[1] != "Empty")
                            {
                                playingSlot = 1;
                                playerName = data.Name[playingSlot];
                                level = data.Level[playingSlot];
                                //points = data.Points[playingSlot];
                                if (level == 2)
                                    points = 5;
                                else if (level != 1)
                                    points = level * 5;
                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                            }
                            break;
                        case 398:
                            if (data.Name[2] != "Empty")
                            {
                                playingSlot = 2;
                                playerName = data.Name[playingSlot];
                                level = data.Level[playingSlot];
                                //points = data.Points[playingSlot];
                                if (level == 2)
                                    points = 5;
                                else if (level == 1)
                                    points = 0;
                                else
                                    points = level * 5;
                                gs = GameScreen.Game;
                                cp = CreatePlayer.None;
                            }
                            break;

                    }
                }
            }

            if (gs == GameScreen.Game)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;//time counter
                Mtimer += gameTime.ElapsedGameTime.TotalMinutes;//minute counter
                leveltimer += gameTime.ElapsedGameTime.TotalSeconds;
                Kstate = Keyboard.GetState();
                expRect = new Rectangle(74, UIrect.Top + 83, (int)((100 / maxExp[a]) * atmExp), 24);
                if (Kstate.IsKeyDown(Keys.Left))//player movement
                {
                    if (playerRect.Left > 5)
                        playerRect.X -= Xspeed;
                }
                if (Kstate.IsKeyDown(Keys.Right))
                {
                    if (playerRect.Right < width)
                        playerRect.X += Xspeed;
                }
                if (Kstate.IsKeyDown(Keys.Up))
                {
                    if (playerRect.Top > 5)
                        playerRect.Y -= Yspeed;
                }
                if (Kstate.IsKeyDown(Keys.Down))
                {
                    if (playerRect.Bottom < UIrect.Top)
                        playerRect.Y += Yspeed;
                }
                if (enemies.Count == 0 && !spawn && timer > stagegt + 2)
                {
                    spawn = true;
                }

                if (Kstate.IsKeyDown(Keys.LeftShift))
                {
                    switch (Nbullets)
                    {
                        case 1:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left + 17, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;
                        case 2:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;
                        case 3:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left + 17, playerRect.Top - 15, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;
                        case 4:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left + 17, playerRect.Top - 15, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Left));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Right));

                                gt = timer + amp;
                            }
                            break;
                        case 5:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Left));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Right));

                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;
                        case 6:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Left));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Right));

                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Left + 17, playerRect.Top - 15, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;
                        case 7:
                            if (timer > gt)
                            {
                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Left));

                                bullets.Enqueue(new Bullet(playerRect.Left + 21, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Left));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Right));

                                bullets.Enqueue(new Bullet(playerRect.Right - 21, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Right));

                                bullets.Enqueue(new Bullet(playerRect.Left, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Right, playerRect.Top - 5, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                bullets.Enqueue(new Bullet(playerRect.Left + 17, playerRect.Top - 15, 15, 15, bullet, bulletdamage, BulletType.Straight));

                                gt = timer + amp;
                            }
                            break;

                        default:
                            Nbullets = 2;
                            break;
                    }
                }


                if (spawn)//enemy spawner
                {
                    if (st == Stage.One)
                    {

                        if (timer >= spawnTime + spawnRate)
                        {

                            int num = r.Next(100);
                            if (num > 3 && num <= 80)
                            {
                                enemies.Enqueue(new Enemy(r.Next(20, 700), -41, 44, 41, shooter, enemyhp));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                            else if (num > 80)
                            {
                                enemies.Enqueue(new Enemy(-44, r.Next(20, 200), 44, 41, speedy, enemyhp));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                            else
                            {
                                enemies.Enqueue(new Enemy(r.Next(20, 700), -61, 64, 61, enemyBoss, enemyhp * 10));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                        }
                    }
                    else if (st == Stage.Two)
                    {
                        if (timer >= spawnTime + spawnRate)
                        {
                            int num = r.Next(100);
                            if (num > 6 && num <= 50)
                            {
                                enemies.Enqueue(new Enemy(r.Next(20, 700), -41, 44, 41, shooter, enemyhp));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                            else if (num > 50 && num <= 80)
                            {
                                enemies.Enqueue(new Enemy(-44, r.Next(20, 200), 44, 41, speedy, enemyhp));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                            else if (num < 7)
                            {
                                enemies.Enqueue(new Enemy(r.Next(20, 700), -61, 64, 61, enemyBoss, enemyhp * 10));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;

                            }
                            else
                            {
                                enemies.Enqueue(new Enemy(r.Next(20, 700), -41, 44, 41, chaser, Convert.ToInt32(enemyhp * 1.5)));
                                enemygt.Enqueue(timer);
                                spawnTime = timer;
                            }
                        }

                    }

                }



                enemiesTemp = enemies.ToArray();
                enemies.Clear();
                for (int i = 0; i < enemiesTemp.Length; i++)//enemy movement down
                {

                    if (enemiesTemp[i].type.Name == "shooter")
                    {
                        enemiesTemp[i].rect.Y += enemySpeed;
                    }
                    else if (enemiesTemp[i].type.Name == "speedy")
                    {
                        enemiesTemp[i].rect.X += 3;
                    }
                    else if (enemiesTemp[i].type.Name == "userBoss")
                    {
                        enemiesTemp[i].rect.Y += enemySpeed / 2;
                    }
                    else if (enemiesTemp[i].type.Name == "chaser")
                    {
                        enemiesTemp[i].rect.Y += enemySpeed / 2;
                        if (enemiesTemp[i].rect.Center.X > playerRect.Center.X)
                        {
                            enemiesTemp[i].rect.X -= 2;
                        }
                        else if (enemiesTemp[i].rect.Center.X < playerRect.Center.X)
                        {
                            enemiesTemp[i].rect.X += 2;
                        }
                    }

                    enemies.Enqueue(enemiesTemp[i]);
                }


                bulletsTemp = bullets.ToArray();
                bullets.Clear();
                for (int i = 0; i < bulletsTemp.Length; i++)//enemy bullets
                {
                    if (bulletsTemp[i].bulletType == BulletType.Straight)
                    {
                        bulletsTemp[i].rect.Y -= 5;
                        bool bflag = true;
                        if (bulletsTemp[i].rect.Y < 0)
                        {
                            bflag = false;
                        }
                        if (bflag)
                        {
                            bullets.Enqueue(bulletsTemp[i]);
                        }
                    }
                    else if (bulletsTemp[i].bulletType == BulletType.Left)
                    {
                        bulletsTemp[i].rect.Y -= 5;
                        bulletsTemp[i].rect.X -= 1;
                        bool bflag = true;
                        if (bulletsTemp[i].rect.Y < 0)
                        {
                            bflag = false;
                        }
                        if (bflag)
                        {
                            bullets.Enqueue(bulletsTemp[i]);
                        }
                    }
                    else if (bulletsTemp[i].bulletType == BulletType.Right)
                    {
                        bulletsTemp[i].rect.Y -= 5;
                        bulletsTemp[i].rect.X += 1;
                        bool bflag = true;
                        if (bulletsTemp[i].rect.Y < 0)
                        {
                            bflag = false;
                        }
                        if (bflag)
                        {
                            bullets.Enqueue(bulletsTemp[i]);
                        }
                    }
                }

                enemiesTemp = enemies.ToArray();
                enemies.Clear();
                for (int i = 0; i < enemiesTemp.Length; i++)//enemy despawner
                {
                    if (enemiesTemp[i].rect.Top > height || enemiesTemp[i].rect.Left > width)
                        continue;
                    enemies.Enqueue(enemiesTemp[i]);
                }

                bulletsTemp = bullets.ToArray();
                bullets.Clear();
                for (int b = 0; b < bulletsTemp.Length; b++)//enemy status
                {
                    bool bflag = true;
                    enemiesTemp = enemies.ToArray();
                    enemies.Clear();
                    for (int i = 0; i < enemiesTemp.Length; i++)
                    {

                        if (bulletsTemp[b].rect.Top <= enemiesTemp[i].rect.Bottom && bulletsTemp[b].rect.Bottom >= enemiesTemp[i].rect.Top && bulletsTemp[b].rect.Left <= enemiesTemp[i].rect.Right && bulletsTemp[b].rect.Right >= enemiesTemp[i].rect.Left)
                        {
                            enemiesTemp[i].deduct(bulletdamage);
                            if (enemiesTemp[i].life < 1)
                            {
                                expgain += enemiesTemp[i].getEnemyExp();
                                bflag = false;
                                booms.Enqueue(new Rectangle(enemiesTemp[i].rect.Left, (enemiesTemp[i].rect.Bottom + enemiesTemp[i].rect.Top) / 2, 64, 64));
                                boomLength.Enqueue(timer);
                                enemygt.Dequeue();
                                if (enemiesTemp[i].type.Name == shooter.Name)
                                {
                                    score += 10 * enemyhp;
                                }
                                else if (enemiesTemp[i].type.Name == speedy.Name)
                                {
                                    score += 20 * enemyhp;
                                }
                                else if (enemiesTemp[i].type.Name == enemyBoss.Name)
                                {
                                    score += 50 * enemyhp;
                                }
                                else if (enemiesTemp[i].type.Name == chaser.Name)
                                {
                                    score += 40 * enemyhp;
                                }
                                if (score > 99 && st == Stage.One)
                                {
                                    st = Stage.Two;
                                    stagegt = timer;
                                }

                                continue;
                            }
                            else bflag = false;
                        }
                        enemies.Enqueue(enemiesTemp[i]);
                    }
                    if (bflag)
                        bullets.Enqueue(bulletsTemp[b]);
                }


                enemiesTemp = enemies.ToArray();
                tempEnemygt = enemygt.ToArray();
                enemygt.Clear();
                for (int i = 0; i < enemies.Count; i++)//enemy fire rate
                {

                    if (enemiesTemp[i].type.Name == "shooter")
                    {

                        if (timer > tempEnemygt[i] + (enemyfireMin + r.NextDouble() * enemyfireMax))
                        {

                            if (enemiesTemp[i].rect.Bottom < height)
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Left + 15, enemiesTemp[i].rect.Bottom, EnemyBulletType.Straight));

                            tempEnemygt[i] = timer;

                        }
                    }
                    else if (enemiesTemp[i].type.Name == "speedy")
                    {
                        if (timer > tempEnemygt[i] + (enemyfireMin + r.NextDouble() * enemyfireMax))
                        {
                            if (enemiesTemp[i].rect.Left > 0)
                            {
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Left, enemiesTemp[i].rect.Bottom, EnemyBulletType.Straight));
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Right, enemiesTemp[i].rect.Bottom, EnemyBulletType.Straight));
                            }
                            tempEnemygt[i] = timer;

                        }
                    }
                    else if (enemiesTemp[i].type.Name == "userBoss")
                    {
                        if (timer > tempEnemygt[i] + (enemyfireMin + r.NextDouble() * enemyfireMax))
                        {
                            if (enemiesTemp[i].rect.Left > 0)
                            {
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Left, enemiesTemp[i].rect.Bottom, EnemyBulletType.Left));
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Left + 25, enemiesTemp[i].rect.Bottom, EnemyBulletType.Straight));
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Right, enemiesTemp[i].rect.Bottom, EnemyBulletType.Right));
                            }
                            tempEnemygt[i] = timer;
                        }

                    }
                    else if (enemiesTemp[i].type.Name == "chaser")
                    {

                        if (timer > tempEnemygt[i] + (enemyfireMin + r.NextDouble() * enemyfireMax))
                        {

                            if (enemiesTemp[i].rect.Bottom < height)
                            {
                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Left, enemiesTemp[i].rect.Bottom, EnemyBulletType.Left));

                                enemyBullets.Enqueue(new EnemyBullet(enemiesTemp[i], enemyBullet, enemiesTemp[i].rect.Right, enemiesTemp[i].rect.Bottom, EnemyBulletType.Right));
                            }

                            tempEnemygt[i] = timer;

                        }
                    }
                    enemygt.Enqueue(tempEnemygt[i]);
                }

                enemyBulletsTemp = enemyBullets.ToArray();
                enemyBullets.Clear();
                for (int i = 0; i < enemyBulletsTemp.Length; i++)//enemy bullet movement
                {
                    if (enemyBulletsTemp[i].rect.Y > height)
                        continue;
                    switch (enemyBulletsTemp[i].BulletType)
                    {
                        case EnemyBulletType.Straight:
                            enemyBulletsTemp[i].rect.Y += 3;
                            break;
                        case EnemyBulletType.Left:
                            enemyBulletsTemp[i].rect.X -= 1;
                            enemyBulletsTemp[i].rect.Y += 4;
                            break;
                        case EnemyBulletType.Right:
                            enemyBulletsTemp[i].rect.X += 1;
                            enemyBulletsTemp[i].rect.Y += 4;
                            break;
                    }


                    enemyBullets.Enqueue(enemyBulletsTemp[i]);
                }

                enemyBulletsTemp = enemyBullets.ToArray();
                enemyBullets.Clear();
                for (int i = 0; i < enemyBulletsTemp.Length; i++)//enemy bullet damage detect
                {

                    if (enemyBulletsTemp[i].rect.Bottom >= playerRect.Top && enemyBulletsTemp[i].rect.Top <= playerRect.Bottom && enemyBulletsTemp[i].rect.Left <= playerRect.Right && enemyBulletsTemp[i].rect.Right >= playerRect.Left)
                    {
                        lives--;
                        if (lives <= 0)
                        {
                            lastScore = score;
                            resetStats();
                            gs = GameScreen.Menu;

                            break;
                        }
                        continue;
                    }
                    enemyBullets.Enqueue(enemyBulletsTemp[i]);

                }

                ExpGain(gameTime, ref expgain);


                if (boomLength.Count != 0 && timer > boomLength.Peek() + 1.0)//boom length
                {
                    boomLength.Dequeue();
                    booms.Dequeue();
                }

                if (Kstate.IsKeyDown(Keys.P) && oldKstate.IsKeyUp(Keys.P))
                {
                    gs = GameScreen.Pause;
                    oldKstate = Kstate;
                    arrowRect.Location = new Point(120, 40);

                }
                else if (Kstate.IsKeyDown(Keys.S) && oldKstate.IsKeyUp(Keys.S))
                {
                    gs = GameScreen.SaveScreen;
                    oldKstate = Kstate;
                    selectedRect.X = 48;
                    data = loadGame("igor");
                    oldKstate = Kstate;
                }
            }

            if (gs == GameScreen.SaveScreen)
            {
                Kstate = Keyboard.GetState();
                if (Kstate.IsKeyDown(Keys.Left) && oldKstate.IsKeyUp(Keys.Left))
                {
                    if (selectedRect.X > 48)
                        selectedRect.X -= 175;
                    else
                        selectedRect.X = 398;
                }
                else if (Kstate.IsKeyDown(Keys.Right) && oldKstate.IsKeyUp(Keys.Right))
                {
                    if (selectedRect.X < 225)
                        selectedRect.X += 175;
                    else
                        selectedRect.X = 48;
                }
                else if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter))
                {

                    switch (selectedRect.X)
                    {
                        case 48:
                            data.Name[0] = playerName;
                            data.Level[0] = level;
                            //data.Points[0] = points;
                            saveGame(data, 0);
                            gs = GameScreen.Game;
                            break;
                        case 223:
                            data.Name[1] = playerName;
                            data.Level[1] = level;
                            //data.Points[1] = points;
                            saveGame(data, 1);
                            gs = GameScreen.Game;
                            break;
                        case 398:
                            data.Name[2] = playerName;
                            data.Level[2] = level;
                            //data.Points[2] = points;
                            saveGame(data, 2);
                            gs = GameScreen.Game;
                            break;

                    }
                }
                else if (Kstate.IsKeyDown(Keys.Escape) && oldKstate.IsKeyUp(Keys.Escape))
                {
                    gs = GameScreen.Game;
                }
                oldKstate = Kstate;
            }

            if (gs == GameScreen.Pause)
            {

                Kstate = Keyboard.GetState();
                if (Kstate.IsKeyDown(Keys.Down) && oldKstate.IsKeyUp(Keys.Down))
                {
                    switch (arrowRect.Y)
                    {
                        case 40:
                            arrowRect.Y = 80;
                            break;
                        case 80:
                            arrowRect.Y = 120;
                            break;
                        case 120:
                            arrowRect.Y = 160;
                            break;
                        case 160:
                            arrowRect.Y = 200;
                            break;
                        case 200:
                            arrowRect.Y = 240;
                            break;
                        case 240:
                            arrowRect.Y = 40;
                            break;
                    }
                }
                if (Kstate.IsKeyDown(Keys.Up) && oldKstate.IsKeyUp(Keys.Up))
                {
                    switch (arrowRect.Y)
                    {
                        case 240:
                            arrowRect.Y = 200;
                            break;
                        case 200:
                            arrowRect.Y = 160;
                            break;
                        case 160:
                            arrowRect.Y = 120;
                            break;
                        case 120:
                            arrowRect.Y = 80;
                            break;
                        case 80:
                            arrowRect.Y = 40;
                            break;
                        case 40:
                            arrowRect.Y = 240;
                            break;
                    }
                }
                if (Kstate.IsKeyDown(Keys.Enter) && oldKstate.IsKeyUp(Keys.Enter))
                {
                    switch (arrowRect.Y)
                    {
                        case 40://Xspeed
                            if (points > 0)
                            {
                                points--;
                                Xspeed++;
                            }
                            break;
                        case 80://Yspeed
                            if (points > 0)
                            {
                                points--;
                                Yspeed++;
                            }
                            break;
                        case 120://Bullet speed
                            if (points > 0)
                            {
                                points--;
                                amp /= 1.1;
                            }
                            break;
                        case 160://Spawn rate
                            if ((points > 0 && spawnRate > 0.1) || (points > 0 && maxExp[a] > 200 && spawnRate > 0))
                            {
                                points--;
                                spawnRate -= 0.1;
                            }
                            break;
                        case 200://Bullet level
                            if (points >= 5 && Nbullets < 7)
                            {
                                points -= 5;
                                Nbullets++;
                            }
                            break;
                        case 240://Damage
                            if (points >= 3)
                            {
                                points -= 3;
                                bulletdamage += 1;
                            }
                            break;
                    }
                }
                if (Kstate.IsKeyDown(Keys.P) && oldKstate.IsKeyUp(Keys.P) || Kstate.IsKeyDown(Keys.Escape) && oldKstate.IsKeyUp(Keys.Escape))
                {
                    gs = GameScreen.Game;
                    oldKstate = Kstate;
                }

            }
            oldKstate = Kstate;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)//draw method
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            if (gs == GameScreen.Menu)
            {
                spriteBatch.DrawString(font, "Start a new game", new Vector2(300, 100), Color.White);
                spriteBatch.DrawString(font, "Load a game", new Vector2(300, 200), Color.White);
                spriteBatch.DrawString(font, "Exit", new Vector2(300, 300), Color.White);


                spriteBatch.DrawString(scoreFont, "Last game score: " + lastScore, new Vector2(50, 50), Color.White);

                spriteBatch.Draw(menuArrow, arrowRect, Color.White);


            }
            else if (gs == GameScreen.MenuCreateScreen)
            {
                if (cp == CreatePlayer.NameScreen)
                {
                    spriteBatch.DrawString(scoreFont, "Enter your name:", new Vector2(300, 100), Color.White);
                    spriteBatch.DrawString(scoreFont, name, new Vector2(300, 150), Color.White);


                }
            }
            else if (gs == GameScreen.LoadScreen)
            {
                spriteBatch.Draw(selected, selectedRect, Color.Red);
                spriteBatch.Draw(select, new Rectangle(50, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[0], new Vector2(60, 160), Color.White);
                if (data.Name[0] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(60, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[0].ToString(), new Vector2(125, 190), Color.White);
                }
                spriteBatch.Draw(select, new Rectangle(225, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[1], new Vector2(235, 160), Color.White);
                if (data.Name[1] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(235, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[1].ToString(), new Vector2(300, 190), Color.White);
                }
                spriteBatch.Draw(select, new Rectangle(400, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[2], new Vector2(415, 160), Color.White);
                if (data.Name[2] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(410, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[2].ToString(), new Vector2(475, 190), Color.White);
                }
            }
            else if (gs == GameScreen.Game)
            {

                bulletsDraw = bullets.ToArray();
                for (int i = 0; i < bullets.Count; i++)
                {
                    spriteBatch.Draw(bullet, bulletsDraw[i].rect, Color.White);
                }
                boomsDraw = booms.ToArray();
                for (int i = 0; i < boomsDraw.Length; i++)
                {
                    spriteBatch.Draw(boom, boomsDraw[i], Color.White);
                }

                enemiesDraw = enemies.ToArray();
                for (int i = 0; i < enemies.Count; i++)
                {
                    spriteBatch.Draw(enemiesDraw[i].type, enemiesDraw[i].rect, Color.White);
                }


                enemyBulletsDraw = enemyBullets.ToArray();
                for (int i = 0; i < enemyBulletsDraw.Length; i++)
                {
                    spriteBatch.Draw(enemyBullet, enemyBulletsDraw[i].rect, Color.White);
                }
                if (timer < levelupgt)
                {
                    spriteBatch.DrawString(scoreFont, "Level up !", new Vector2(playerRect.X, playerRect.Top - playerRect.Height), Color.Yellow);
                }
                if (timer < stagegt + 2 && timer > 2)
                {
                    spriteBatch.DrawString(bigFont, "Stage complete", new Vector2(400, height / 3), Color.Yellow);
                    spawn = false;
                }

                spriteBatch.Draw(playerShip, playerRect, Color.White);

                spriteBatch.Draw(UIBackground, UIrect, Color.White);
                spriteBatch.Draw(exp, expRect, Color.White);
                spriteBatch.Draw(expBar, expBarRect, Color.CornflowerBlue);
                spriteBatch.DrawString(UIfont, "Exp:", new Vector2(expBarRect.X - 50, UIrect.Top + 83), Color.Black);

                spriteBatch.DrawString(UIfont, atmExp + "/" + maxExp[a], new Vector2(expBarRect.Center.X - 20, UIrect.Top + 83), Color.Black);
                spriteBatch.DrawString(UIfont, "Player name: " + playerName, new Vector2(UIrect.X + 20, UIrect.Top + 20), Color.Black);
                spriteBatch.DrawString(scoreFont, "Lives: " + lives, new Vector2(40, 50), Color.White);
                spriteBatch.DrawString(UIfont, "Score: " + score, new Vector2(20, UIrect.Top + 110), Color.Black);
                spriteBatch.DrawString(UIfont, "Level: " + level, new Vector2(UIrect.X + 20, UIrect.Top + 55), Color.Black);
                spriteBatch.DrawString(UIfont, "Press P to go to upgrade screen", new Vector2(UIrect.X + 300, UIrect.Top + 80), Color.Black);
                spriteBatch.DrawString(UIfont, "Press S to go to the save menu", new Vector2(UIrect.X + 300, UIrect.Top + 110), Color.Black);

            }
            else if (gs == GameScreen.SaveScreen)
            {
                spriteBatch.Draw(selected, selectedRect, Color.Red);
                spriteBatch.Draw(select, new Rectangle(50, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[0], new Vector2(60, 160), Color.White);
                if (data.Name[0] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(60, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[0].ToString(), new Vector2(125, 190), Color.White);
                    //spriteBatch.DrawString(scoreFont, "Points:", new Vector2(60, 220), Color.White);
                    //spriteBatch.DrawString(scoreFont, data.Points[0].ToString(), new Vector2(125, 220), Color.White);
                }
                spriteBatch.Draw(select, new Rectangle(225, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[1], new Vector2(235, 160), Color.White);
                if (data.Name[1] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(235, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[1].ToString(), new Vector2(300, 190), Color.White);
                    // spriteBatch.DrawString(scoreFont, "Points:", new Vector2(235, 220), Color.White);
                    // spriteBatch.DrawString(scoreFont, data.Points[1].ToString(), new Vector2(300, 220), Color.White);
                }
                spriteBatch.Draw(select, new Rectangle(400, 150, 150, 200), Color.Red);
                spriteBatch.DrawString(scoreFont, data.Name[2], new Vector2(415, 160), Color.White);
                if (data.Name[2] != "Empty")
                {
                    spriteBatch.DrawString(scoreFont, "Level:", new Vector2(410, 190), Color.White);
                    spriteBatch.DrawString(scoreFont, data.Level[2].ToString(), new Vector2(475, 190), Color.White);
                    //spriteBatch.DrawString(scoreFont, "Points:", new Vector2(410, 220), Color.White);
                    //spriteBatch.DrawString(scoreFont, data.Points[2].ToString(), new Vector2(475, 220), Color.White);
                }

            }
            else if (gs == GameScreen.Pause)
            {
                Kstate = Keyboard.GetState();

                spriteBatch.DrawString(scoreFont, "Upgrade horizontal speed by 1", new Vector2(150, 40), Color.White);
                spriteBatch.DrawString(scoreFont, "Upgrade vertical speed by 1", new Vector2(150, 80), Color.White);
                spriteBatch.DrawString(scoreFont, "Upgrade bullet speed", new Vector2(150, 120), Color.White);
                spriteBatch.DrawString(scoreFont, spawnRate>=0.1?"Upgrade spawn rate by 0.1": "Upgrade spawn rate by 0.1 (warning: will spawn mobs fast)", new Vector2(150, 160), Color.White);
                spriteBatch.DrawString(scoreFont, "Upgrade shots (Costs 5 points) "+Nbullets+"/7", new Vector2(150, 200), Color.White);
                spriteBatch.DrawString(scoreFont, "Upgrade damage (Costs 3 points) ", new Vector2(150, 240), Color.White);

                spriteBatch.DrawString(scoreFont, "Ship details:", new Vector2(150, 350), Color.White);
                spriteBatch.DrawString(scoreFont, "Ship vertical speed :" + Yspeed, new Vector2(150, 400), Color.White);
                spriteBatch.DrawString(scoreFont, "Ship horizontal speed :" + Xspeed, new Vector2(150, 425), Color.White);
                spriteBatch.DrawString(scoreFont, "Enemies appear once every " + Math.Round(spawnRate, 4) + " Seconds", new Vector2(150, 450), Color.White);
                spriteBatch.DrawString(scoreFont, "Ship bullet level: " + Nbullets, new Vector2(150, 475), Color.White);
                spriteBatch.DrawString(scoreFont, "Bullet damage: " + bulletdamage, new Vector2(150, 500), Color.White);
                spriteBatch.Draw(menuArrow, arrowRect, Color.White);
                oldKstate = Kstate;
                spriteBatch.DrawString(scoreFont, "Points: " + points, new Vector2(39, 540), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        public void resetStats()
        {
            arrowRect = new Rectangle(270, 110, 20, 20);
            playerRect = new Rectangle(368, 540, 35, 35);
            lives = 5;
            points = 0;
            Xspeed = 4;
            Yspeed = 4;
            spawnRate = 2.0;
            enemyhp = 1;
            enemySpeed = 2;
            bulletdamage = 1;
            Nbullets = 1;
            amp = 0.5;
            level = 1;
            a = 0;
            bullets.Clear();
            booms.Clear();
            boomLength.Clear();
            enemyBullets.Clear();
            enemies.Clear();
            booms.Clear();
            timer = 0;
            gt = 0;
            spawnTime = 0;
            atmExp = 0;
            expgain = 0;
            expgt = 0;
            Mtimer = 0;
            levelupgt = 0;
            leveltimer = 0;
            score = 0;
            st = Stage.One;
            spawn = true;
            stagegt = 0;
        }
    }
}//1418