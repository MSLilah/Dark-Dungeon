using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        Guard[] guards = new Guard[1];
        Texture2D backgroundImage;
        Texture2D darkBackgroundImage;
        Map gameMap;
        public int topOfScreen = 0;
        public int bottomOfScreen = 665;
        public int leftEdgeOfScreen = 0;
        public int rightEdgeOfScreen = 1255;

        int[,] layout = new int[40, 22];

        Boolean darkMode = false;

        float darkCheckElapsedTime = 1.0f;
        float darkTarget = 1.0f;

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
            protag = new Protagonist(Content.Load<Texture2D>(@"Textures\protagSpriteSheet"), 0, 0);

            for (int i = 0; i < guards.Length; i++)
            {
                guards[i] = new Guard(Content.Load<Texture2D>(@"Textures\guardSpriteSheet"), 660, 300, 3);
            }

            backgroundImage = Content.Load<Texture2D>(@"Textures\backgroundImage");
            darkBackgroundImage = Content.Load<Texture2D>(@"Textures\darkBackgroundImage");

            layout = createSimpleMap();
            gameMap = new Map(layout, Content.Load<Texture2D>(@"Textures\wall"));
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

            CheckPlayerMovement(keyboard, gamepad);

            CheckDarkMode(gameTime, keyboard, gamepad);

            MoveGuards();
            protag.Update(gameTime);
            for (int i = 0; i < guards.Length; i++)
            {
                guards[i].Update(gameTime);
            }
            if (GuardsSeeProtag())
            {
                protag.Reset();
                for (int i = 0; i < guards.Length; i++)
                {
                    guards[i].Reset();
                }
            }
            //player.Update(gameTime);

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
            //player.Draw(spriteBatch, 0, 0);
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
                    if (!gameMap.CollideWithWall(protag.BoundingBox, 0, protag.MovementRate))
                    {
                        protag.Y -= protag.MovementRate;
                    }
                    resetTimer = true;
                    protag.Facing = 0;
                }
            }

            //These elses restrict the player to only moving in four directions, which is what we want
            else if ((keyboard.IsKeyDown(Keys.Down)) || (gamepad.ThumbSticks.Left.Y < 0))
            {
                if (protag.Y < bottomOfScreen)
                {
                    if (!gameMap.CollideWithWall(protag.BoundingBox, 2, protag.MovementRate))
                    {
                        protag.Y += protag.MovementRate;
                    }
                    resetTimer = true;
                    protag.Facing = 2;
                }
            }

            else if ((keyboard.IsKeyDown(Keys.Right)) || (gamepad.ThumbSticks.Left.X > 0)) {
                if (protag.X < rightEdgeOfScreen) {
                    if (!gameMap.CollideWithWall(protag.BoundingBox, 1, protag.MovementRate))
                    {
                        protag.X += protag.MovementRate;
                    }
                    resetTimer = true;
                    protag.Facing = 1;
                }
            }

            else if ((keyboard.IsKeyDown(Keys.Left)) || (gamepad.ThumbSticks.Left.X < 0)) {
                if (protag.X > leftEdgeOfScreen) {
                    if (!gameMap.CollideWithWall(protag.BoundingBox, 3, protag.MovementRate))
                    {
                        protag.X -= protag.MovementRate;
                    }
                    resetTimer = true;
                    protag.Facing = 3;
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
         * Checks if the player is try to switch to/from dark mode
         * 
         * @param gametime - The current elapsed game time
         * @param keyboard - The current state of the keyboard
         * @param gamepad - The current state of hte gamepad
         */
        protected void CheckDarkMode(GameTime gametime, KeyboardState keyboard, GamePadState gamepad)
        {
            darkCheckElapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Space) || gamepad.IsButtonDown(Buttons.B))
            {
                if (darkCheckElapsedTime > darkTarget)
                {
                    darkMode = !darkMode;
                    darkCheckElapsedTime = 0.0f;
                }
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
                            if (!gameMap.CollideWithWall(guards[i].BoundingBox, 0, guards[i].MovementRate))
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
                            if (!gameMap.CollideWithWall(guards[i].BoundingBox, 1, guards[i].MovementRate))
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
                            if (!gameMap.CollideWithWall(guards[i].BoundingBox, 2, guards[i].MovementRate))
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
                            if (!gameMap.CollideWithWall(guards[i].BoundingBox, 3, guards[i].MovementRate))
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
                            return ((guards[i].Facing == 0 && playerHitBox.Bottom < guardHitBox.Top) ||
                                    (guards[i].Facing == 2 && playerHitBox.Top > guardHitBox.Bottom)) &&
                                    !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing);
                        }
                        // Otherwise, we're in dark mode and the player is not in range of the gaurd so she is unseen
                        else
                        {
                            return false;
                        }
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
                            return ((guards[i].Facing == 1 && playerHitBox.Left >= guardHitBox.Right) ||
                                    (guards[i].Facing == 3 && playerHitBox.Right <= guardHitBox.Left)) &&
                                    !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing);
                        }
                        // Otherwise, we're in dark mode and the player is not in range of the gaurd so she is unseen
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
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
            {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
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
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
            #endregion

            return layoutLevel;
        }
        
        #endregion
    }
}
