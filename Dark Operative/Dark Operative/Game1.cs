using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/**
 * Game1.cs
 * 
 * The main class for Dark Dungeon
 * 
 * @author Lilah Ingvaldsen
 * @author Hailee Kenney
 * @author Justice Nichols
 */
namespace Dark_Operative
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Declarations
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Protagonist protag;
        Guard[] guards;
        Monster[] monsters = new Monster[1];
        Texture2D backgroundImage;
        Texture2D darkBackgroundImage;
        Texture2D exclamationPoint;
        SpriteFont font;
        Map gameMap;
        Random random = new Random();
        public int topOfScreen = 0;
        public int bottomOfScreen = 599;
        public int leftEdgeOfScreen = 0;
        public int rightEdgeOfScreen = 1169;
        

        int[,] layout = new int[40, 22];

        bool darkMode = false;

        float darkCheckElapsedTime = 1.0f;
        float darkTarget = 1.0f;
        bool darkPressed = false;

        float pauseCheckElapsedTime = 0.0f;
        float pauseTarget = 1.0f;
        bool pausePressed = false;

        float loseElapsedTime = 0.0f;
        float loseTarget = 1.0f;

        bool pause = false;
        bool lose = false;
        bool wonLevel = false;

        int lives = 3;
        int guardWhoSaw = -1;

        int timer = 400;
        int score = 0;
        float elapsedTimerTime = 0.0f;
        float targetTimerTime = 1.0f;

        //Text locations
        Vector2 PauseTextLoc = new Vector2(500, 330);
        Vector2 GameOverTextLoc = new Vector2(480, 330);
        Vector2 CaughtTextLoc = new Vector2(330, 330);
        Vector2 LevelCompleteTextLoc = new Vector2(460, 330);
        Vector2 ScoreWinLocation = new Vector2(500, 360);
        Vector2 TimerWinLocation = new Vector2(500, 390);
        Vector2 ScoreLocation = new Vector2(1050, 650);
        Vector2 TimerLocation = new Vector2(1050, 670);
        Vector2 LivesLocation = new Vector2(1050, 630);

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
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

            // TODO: use this.Content to load your game content here
            layout = createSimpleMap();
            gameMap = new Map(layout, Content.Load<Texture2D>(@"Textures\wall"), Content.Load<Texture2D>(@"Textures\Treasure"));
            font = Content.Load<SpriteFont>(@"Fonts\emulogic");
            exclamationPoint = Content.Load<Texture2D>(@"Textures\spotted");

            //Create and place the protagonist
            Vector3 protagCoords = gameMap.ProtagStartCoords;
            protag = new Protagonist(Content.Load<Texture2D>(@"Textures\protagSpriteSheet"), (int)protagCoords.X, (int)protagCoords.Y);

            //Create and place the guards
            ArrayList enemyCoordList = gameMap.GuardCoords;
            Vector3 enemyCoords;
            guards = new Guard[enemyCoordList.ToArray().Length];
            for (int i = 0; i < enemyCoordList.ToArray().Length; i++)
            {
                enemyCoords = (Vector3)enemyCoordList[i];
                guards[i] = new Guard(Content.Load<Texture2D>(@"Textures\guardSpriteSheet"), (int)enemyCoords.X, (int)enemyCoords.Y, (int)enemyCoords.Z);
            }

            //Create and place the monsters
            enemyCoordList = gameMap.MonsterCoords;
            monsters = new Monster[enemyCoordList.ToArray().Length];
            for (int i = 0; i < enemyCoordList.ToArray().Length; i++)
            {
                enemyCoords = (Vector3)enemyCoordList[i];
                monsters[i] = new Monster(Content.Load<Texture2D>(@"Textures\monsterSpriteSheet"), (int)enemyCoords.X, (int)enemyCoords.Y, (int)enemyCoords.Z);
            }

            backgroundImage = Content.Load<Texture2D>(@"Textures\backgroundImage");
            darkBackgroundImage = Content.Load<Texture2D>(@"Textures\darkBackgroundImage");
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
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            CheckPause(gameTime, keyboard, gamepad);

            if (!pause && !lose && !wonLevel)
            {
                elapsedTimerTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedTimerTime >= targetTimerTime)
                {
                    timer--;
                    elapsedTimerTime = 0.0f;
                }
                if (timer <= 0)
                {
                    lose = true;
                }
                #region Gameplay
                CheckPlayerMovement(keyboard, gamepad);

                CheckDarkMode(gameTime, keyboard, gamepad);

                MoveGuards();
                if (darkMode)
                {
                    MoveMonsters();
                }
                protag.Update(gameTime);
                for (int i = 0; i < guards.Length; i++)
                {
                    guards[i].Update(gameTime);
                }
                if (GuardsSeeProtag())
                {
                    lose = true;
                    lives--;
                }
                else if (EnemyTouchesPlayer())
                {
                    lose = true;
                    lives--;
                }
                for (int i = 0; i < monsters.Length; i++)
                {
                    monsters[i].Update(gameTime);
                }
                //player.Update(gameTime);
                #endregion
            }
            else if (lose)
            {
                loseElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (loseElapsedTime > loseTarget)
                {
                    loseElapsedTime = 0;
                    ResetGame();
                }
            }
            else if (wonLevel)
            {
                if (timer == 0)
                {
                    loseElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (loseElapsedTime > loseTarget)
                    {
                        loseElapsedTime = 0;
                        ResetGame();
                    }
                }
                else
                {
                    timer--;
                    score += 5;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, 1280, 720),
                new Rectangle(0, 0, 1280, 720), Color.White);

            if (darkMode)
            {
                spriteBatch.Draw(darkBackgroundImage, new Rectangle(0, 0, 1280, 720),
                new Rectangle(0, 0, 1280, 720), Color.White);
            }

            gameMap.Draw(spriteBatch);
            protag.Draw(spriteBatch);
            for (int i = 0; i < guards.Length; i++)
            {
                guards[i].Draw(spriteBatch);
            }
            if (darkMode)
            {
                for (int i = 0; i < monsters.Length; i++)
                {
                    monsters[i].Draw(spriteBatch);
                }
            }

            if (pause)
            {
                spriteBatch.DrawString(font, "P A U S E D", PauseTextLoc, Color.White);
            }
            else if (lose)
            {
                if (lives >= 0)
                {
                    spriteBatch.DrawString(font, "Y O U  W E R E  C A U G H T !", CaughtTextLoc, Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font, "G A M E  O V E R", GameOverTextLoc, Color.White);
                }

                //The player was caught by a guard, so draw the exclamation point above their head
                if (guardWhoSaw > -1)
                {
                    Rectangle guardBox = guards[guardWhoSaw].BoundingBox;
                    spriteBatch.Draw(exclamationPoint, new Rectangle(guardBox.Left, guardBox.Top - 25, 20, 20), Color.White);
                }
            }
            else if (wonLevel)
            {
                spriteBatch.DrawString(font, "L E V E L  C O M P L E T E", CaughtTextLoc, Color.White);
                spriteBatch.DrawString(font, "Score: " + score, ScoreWinLocation, Color.White);
                spriteBatch.DrawString(font, "Timer: " + timer, TimerWinLocation, Color.White);
                
            }

            spriteBatch.DrawString(font, "LIVES: " + lives, LivesLocation, Color.White);

            if (!wonLevel)
            {
                spriteBatch.DrawString(font, "SCORE: " + score, TimerLocation, Color.White);
                spriteBatch.DrawString(font, "TIME: " + timer, ScoreLocation, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Helper Methods
        
        /**
         * CheckPlayerMovement
         * 
         * Checks keyboard/gamepad and moves the player if the 
         * movement keys are pressed
         * 
         * @param keyboard - The current state of the keyboard
         * @param gamepad - The current state of hte gamepad
         * 
         */
        protected void CheckPlayerMovement(KeyboardState keyboard, GamePadState gamepad)
        {
            bool resetTimer = false;

            if ((keyboard.IsKeyDown(Keys.Up)) || (gamepad.ThumbSticks.Left.Y > 0))
            {
                if (protag.Y > topOfScreen)
                {
                    resetTimer = true;
                }
                protag.Facing = 0;
            }

            //These elses restrict the player to only moving in four directions, which is what we want
            else if ((keyboard.IsKeyDown(Keys.Down)) || (gamepad.ThumbSticks.Left.Y < 0))
            {
                if (protag.Y < bottomOfScreen)
                {
                    resetTimer = true;                   
                }
                protag.Facing = 2;
            }

            else if ((keyboard.IsKeyDown(Keys.Right)) || (gamepad.ThumbSticks.Left.X > 0)) {
                if (protag.X < rightEdgeOfScreen) {
                    resetTimer = true;                   
                }
                protag.Facing = 1;
            }

            else if ((keyboard.IsKeyDown(Keys.Left)) || (gamepad.ThumbSticks.Left.X < 0)) {
                if (protag.X > leftEdgeOfScreen) {
                    resetTimer = true;
                }
                protag.Facing = 3;
            }

            if (gameMap.CollideWithElement(protag.BoundingBox, protag.Facing, protag.MovementRate, Map.GOAL) && resetTimer)
            {
                wonLevel = true;
            }

            else if (!gameMap.CollideWithElement(protag.BoundingBox, protag.Facing, protag.MovementRate, Map.WALL) && resetTimer)
            {
                switch (protag.Facing)
                {
                    case 0:
                        protag.Y -= protag.MovementRate;
                        break;
                    case 1:
                        protag.X += protag.MovementRate;
                        break;
                    case 2:
                        protag.Y += protag.MovementRate;
                        break;
                    case 3:
                        protag.X -= protag.MovementRate;
                        break;
                    default:
                        break;
                }
            }

            if (resetTimer)
            {
                protag.MovementCount = 0f;
                protag.StartMovement();
            }
            else
            {
                protag.Stand();
            }
        }

        /**
         * CheckDarkMode
         * 
         * Checks if the player is trying to switch to/from dark mode
         * 
         * @param gametime - The current elapsed game time
         * @param keyboard - The current state of the keyboard
         * @param gamepad - The current state of hte gamepad
         */
        protected void CheckDarkMode(GameTime gametime, KeyboardState keyboard, GamePadState gamepad)
        {
            darkCheckElapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            if ((keyboard.IsKeyDown(Keys.Space) || gamepad.IsButtonDown(Buttons.B)) && !darkPressed)
            {
                if (darkCheckElapsedTime > darkTarget)
                {
                    darkMode = !darkMode;
                    darkCheckElapsedTime = 0.0f;
                    darkPressed = true;
                }
            }
            else if (keyboard.IsKeyUp(Keys.Space) && gamepad.IsButtonUp(Buttons.B))
            {
                darkPressed = false;
            }
        }

        /**
         * CheckPause
         * 
         * Checks if the player is trying to pause
         * 
         * @param gametime - The current elapsed game time
         * @param keyboard - The current state of the keyboard
         * @param gamepad - The current state of the gamepad 
         */
        protected void CheckPause(GameTime gametime, KeyboardState keyboard, GamePadState gamepad)
        {
            if ((keyboard.IsKeyDown(Keys.Escape) || gamepad.IsButtonDown(Buttons.Start)) && !pausePressed) {
                pause = !pause;
                pausePressed = true;
            }
            else if (keyboard.IsKeyUp(Keys.Escape) && gamepad.IsButtonUp(Buttons.Start))
            {
                pausePressed = false;
            }
        }

        /**
         * MoveGuards
         * 
         * Goes through the list of all guards and moves them if necessary based on their facing
         * 
         */
        protected void MoveGuards()
        {
            for (int i = 0; i < guards.Length; i++)
            {
                if (guards[i].Move)
                {
                    if (guards[i].Facing == 0)
                    {
                        if (guards[i].Y > topOfScreen)
                        {
                            if (!gameMap.CollideWithElement(guards[i].BoundingBox, 0, guards[i].MovementRate, Map.WALL))
                            {
                                guards[i].Y -= guards[i].MovementRate;
                                guards[i].Facing = 0;
                            }
                            else
                            {
                                guards[i].Stand(false);
                            }
                        }
                    }

                    else if (guards[i].Facing == 1)
                    {
                        if (guards[i].X < rightEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithElement(guards[i].BoundingBox, 1, guards[i].MovementRate, Map.WALL))
                            {
                                guards[i].X += guards[i].MovementRate;
                                guards[i].Facing = 1;
                            }
                            else
                            {
                                guards[i].Stand(false);
                            }
                        }
                    }

                    else if (guards[i].Facing == 2)
                    {
                        if (guards[i].Y < bottomOfScreen)
                        {
                            if (!gameMap.CollideWithElement(guards[i].BoundingBox, 2, guards[i].MovementRate, Map.WALL))
                            {
                                guards[i].Y += guards[i].MovementRate;
                                guards[i].Facing = 2;
                            }
                            else
                            {
                                guards[i].Stand(false);
                            }
                        }
                    }

                    else if (guards[i].Facing == 3)
                    {
                        if (guards[i].X > leftEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithElement(guards[i].BoundingBox, 3, guards[i].MovementRate, Map.WALL))
                            {
                                guards[i].X -= guards[i].MovementRate;
                                guards[i].Facing = 3;
                             }
                            else
                            {
                                guards[i].Stand(false);
                            }
                        }
                    }
                }
            }
        }

        /**
         * MoveMonsters
         * 
         * Goes through the list of all monsters and moves them. They track the left wall.
         * If they do not start on the left wall theymove until they find and intersection and then follow it.
         */
        protected void MoveMonsters()
        {
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i].Move)
                {
                    #region Facing Up
                    if (monsters[i].Facing == 0)
                    {
                        if (monsters[i].Y > topOfScreen)
                        {
                            if (!gameMap.CollideWithElement(monsters[i].BoundingBox, 0, monsters[i].MovementRate, Map.WALL))
                            {
                                monsters[i].Y -= monsters[i].MovementRate;
                                monsters[i].Facing = 0;
                                if (!gameMap.CollideWithElement(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate, Map.WALL))
                                {
                                    monsters[i].Facing = (monsters[i].Facing + 3) % 4;
                                }
                            
                            }
                            else
                            {
                                monsters[i].Stand(true);
                            monsters[i].Facing = (monsters[i].Facing + 1) % 4;
                            }
                        }
                    }
                    #endregion
                    #region Facing Right
                    else if (monsters[i].Facing == 1)
                    {
                        if (monsters[i].X < rightEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithElement(monsters[i].BoundingBox, 1, monsters[i].MovementRate, Map.WALL))
                            {
                                monsters[i].X += monsters[i].MovementRate;
                                monsters[i].Facing = 1;
                                if (!gameMap.CollideWithElement(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate+21, Map.WALL))
                                {
                                    monsters[i].Facing = (monsters[i].Facing + 3) % 4;
                                }
                            }
                            else
                            {
                                monsters[i].Stand(true);
                                monsters[i].Facing = (monsters[i].Facing + 1) % 4;
                                
                            }
                        }
                    }
                    #endregion
                    #region Facing Down
                    else if (monsters[i].Facing == 2)
                    {
                        if (monsters[i].Y < bottomOfScreen)
                        {
                            if (!gameMap.CollideWithElement(monsters[i].BoundingBox, 2, monsters[i].MovementRate, Map.WALL))
                            {
                                monsters[i].Y += monsters[i].MovementRate;
                                monsters[i].Facing = 2;
                                if (!gameMap.CollideWithElement(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate, Map.WALL))
                                {
                                    monsters[i].Facing = (monsters[i].Facing + 3) % 4;
                                }
                            }
                            else
                            {
                                monsters[i].Stand(true);
                                monsters[i].Facing = (monsters[i].Facing + 1) % 4;
                            }
                        }
                    }
                    #endregion
                    #region Facing Left
                    else if (monsters[i].Facing == 3)
                    {
                        if (monsters[i].X > leftEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithElement(monsters[i].BoundingBox, 3, monsters[i].MovementRate, Map.WALL))
                            {
                                monsters[i].X -= monsters[i].MovementRate;
                                monsters[i].Facing = 3;
                                if (!gameMap.CollideWithElement(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate+21, Map.WALL))
                                {
                                    monsters[i].Facing = (monsters[i].Facing + 3) % 4;
                                }
                            }
                            else
                            {
                                monsters[i].Stand(true);
                                monsters[i].Facing = (monsters[i].Facing + 1) % 4;
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        /**
         * GuardsSeeProtag
         * 
         * Goes through the list of all guards and checks if any of them can see the protagonist
         * 
         */
        protected bool GuardsSeeProtag() {
            //TODO: Modify this method to include walls and account for darkness
            Rectangle playerHitBox = protag.BoundingBox;
            for (int i = 0; i < guards.Length; i++)
            {
                Rectangle guardHitBox = guards[i].BoundingBox;

                //Check for LOS and return true if the guard can see the protagonist
                
                // If the gaurd is facing up/down or if this is a horizontal collision
                if (guards[i].Facing % 2 == 0)
                {
                    // Check for a horizontal collision
                    if ((playerHitBox.Left <= guardHitBox.Right && playerHitBox.Left >= guardHitBox.Left) ||
                        (playerHitBox.Right <= guardHitBox.Right && playerHitBox.Right >= guardHitBox.Left))
                    {
                        // If we're not in dark mode or we are and the play is less than 50 pixles infront of the gaurd
                        if (!darkMode || 
                            (darkMode && ((Math.Abs(guardHitBox.Bottom - playerHitBox.Top) < 50) || 
                            (Math.Abs(guardHitBox.Top - playerHitBox.Bottom) < 50))))
                        {
                            // Check if the player is the gaurd's line of sight, if so return true
                            if (((guards[i].Facing == 0 && playerHitBox.Bottom < guardHitBox.Top) ||
                                    (guards[i].Facing == 2 && playerHitBox.Top > guardHitBox.Bottom)) &&
                                    !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing))
                            {
                                guardWhoSaw = i;
                                return true;
                            }
                        }
                        // Otherwise, we're in dark mode and the player is not in range of the gaurd so she is unseen
                    }
                
                }
                // If the gaurd is facing left/right or if there is a vertical collision
                else
                {
                    // Check for a vertical collision
                    if ((playerHitBox.Top >= guardHitBox.Top && playerHitBox.Top <= guardHitBox.Bottom) ||
                        (playerHitBox.Bottom >= guardHitBox.Top && playerHitBox.Bottom <= guardHitBox.Bottom)) 
                    {
                        // If we're not in dark mode or we are and the play is less than 50 pixles infront of the gaurd
                        if (!darkMode || 
                            (darkMode && ((Math.Abs(guardHitBox.Left - playerHitBox.Right) < 50) || 
                            (Math.Abs(guardHitBox.Right - playerHitBox.Left) < 50))))
                        {
                            // Check if the player is the gaurd's line of sight, if so return true
                            if (((guards[i].Facing == 1 && playerHitBox.Left >= guardHitBox.Right) ||
                                    (guards[i].Facing == 3 && playerHitBox.Right <= guardHitBox.Left)) &&
                                    !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing))
                            {
                                guardWhoSaw = i;
                                return true;
                            }
                        }
                        // Otherwise, we're in dark mode and the player is not in range of the gaurd so she is unseen
                    }
                }
            }
            guardWhoSaw = -1;
            return false;
        }

         /**
         * EnemyTouchesPlayer
         * 
         * Goes through the list of all monsters and guards and checks if any of them are touching the protagonist
         * 
         * @return True if an enemy touches the player, else false
         * 
         */
        protected bool EnemyTouchesPlayer()
        {

            Rectangle playerBox = protag.BoundingBox;
            Rectangle enemyBox;

            if (darkMode)
            {
                for (int i = 0; i < monsters.Length; i++)
                {
                    enemyBox = monsters[i].BoundingBox;
                    if (Collision(playerBox, enemyBox))
                    {
                        return true;
                    }

                }
            }

            //Only check for collision with monsters if Dark Mode is active
            for (int j = 0; j < guards.Length; j++)
            {
                enemyBox = guards[j].BoundingBox;
                if (Collision(playerBox, enemyBox))
                {
                    return true;
                }

            }
            return false;
        }

        /**
         * Collision
         * 
         * Checks whether two hitboxes have collided with each other
         * 
         * @param box1 The first box
         * @param box2 The second box
         * @return True if the two boxes have collided, else false
         * 
         */
        protected bool Collision(Rectangle box1, Rectangle box2)
        {
            return (box1.Right > box2.Left && box1.Left < box2.Right &&
                    box1.Bottom > box2.Top && box1.Top < box2.Bottom);
        }

        /**
         * ResetGame
         * 
         * Resets the game to either the beginning of the level or 
         * the title screen, depending on the number of lives remaining
         */
        protected void ResetGame()
        {
            protag.Reset();
            for (int i = 0; i < guards.Length; i++)
            {
                guards[i].Reset();
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i].Reset();
            }
            darkMode = false;
            darkPressed = false;
            pausePressed = false;
            lose = false;
            wonLevel = false;
            timer = 400;
            //TODO: Go back to the title screen if lives are equal to 0
        }

        /**
         * createSimpleMap
         * 
         * Generates a very basic array to represent the maze map
         */
        private int[,] createSimpleMap()
        {
            #region Define Simple Map
            int[,] layoutLevel = {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,3,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
            {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,2,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,11,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,7,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
            #endregion

            #region Define Level 3
            int[,] layoutLevel3 = {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,3,0,0,0,0,0,0,1,0,0,0,0,1,0,0,1,1,1,0,2,1},
            {1,0,0,0,0,0,0,0,1,0,0,0,8,1,0,0,1,1,1,0,0,1},
            {1,1,1,1,1,1,0,0,1,0,0,1,1,1,0,0,1,1,1,0,0,1},
            {1,6,10,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,4,0,1},
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,0,0,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,1,1,1,0,0,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,1,1,1,7,0,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,1,1,1,1,1,1,0,0,1,0,0,1,0,0,1,1,1,1,1},
            {1,0,0,1,6,0,0,0,0,0,0,1,0,0,1,6,0,0,0,0,0,1},
            {1,0,0,1,0,0,0,0,0,0,0,1,7,0,1,0,0,0,0,0,0,1},
            {1,0,0,1,1,1,1,0,0,1,1,1,1,1,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
            {1,0,0,0,0,8,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
            {1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,0,0,1},
            {1,6,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,4,0,1,0,0,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,1,0,0,1},
            {1,0,0,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,0,0,1},
            {1,7,0,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,0,0,1},
            {1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,0,0,1,1,0,0,1},
            {1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,4,0,1,1,1,1,0,0,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,0,0,1},
            {1,1,1,1,1,0,0,1,1,1,1,1,6,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,0,0,1},
            {1,1,1,1,1,0,0,0,0,0,0,0,5,0,0,0,0,0,1,0,0,1},
            {1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
            {1,1,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,1,1,0,0,1},
            {1,6,0,0,0,0,0,1,1,1,0,0,0,0,1,10,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1,0,0,0,0,1,0,0,0,0,0,0,1},
            {1,1,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,1,1,0,0,1},
            {1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};
            #endregion

            //return layoutLevel;
            return layoutLevel3;
        }
        
        #endregion
    }
}
