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
        Monster[] monsters = new Monster[1];
        Texture2D backgroundImage;
        Map gameMap;
        Random random = new Random();
        public int topOfScreen = 0;
        public int bottomOfScreen = 665;
        public int leftEdgeOfScreen = 0;
        public int rightEdgeOfScreen = 1255;
        

        int[,] layout = new int[40, 22];

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
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i] = new Monster(Content.Load<Texture2D>(@"Textures\monsterSpriteSheet"), 500, 300, 3);
            }
            backgroundImage = Content.Load<Texture2D>(@"Textures\backgroundImage");

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

            // TODO: Add your update logic here
            CheckPlayerMovement(keyboard, gamepad);
            MoveGuards();
            MoveMonsters();
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
                for (int i = 0; i < monsters.Length; i++)
                {
                    monsters[i].Reset();
                }
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i].Update(gameTime);
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
            gameMap.Draw(spriteBatch);
            protag.Draw(spriteBatch);
            for (int i = 0; i < guards.Length; i++)
            {
                guards[i].Draw(spriteBatch);
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i].Draw(spriteBatch);
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
         * MoveMonsters
         * 
         * Goes through the list of all monsters and moves them if necessary based on their facing
         * 
         */
        protected void MoveMonsters()
        {
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i].Move)
                {
                    if (monsters[i].Facing == 0)
                    {
                        if (monsters[i].Y > topOfScreen)
                        {
                            if (!gameMap.CollideWithWall(monsters[i].BoundingBox, 0, monsters[i].MovementRate))
                            {
                                monsters[i].Y -= monsters[i].MovementRate;
                                monsters[i].Facing = 0;
                                if (!gameMap.CollideWithWall(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate))
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

                    else if (monsters[i].Facing == 1)
                    {
                        if (monsters[i].X < rightEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithWall(monsters[i].BoundingBox, 1, monsters[i].MovementRate))
                            {
                                monsters[i].X += monsters[i].MovementRate;
                                monsters[i].Facing = 1;
                                if (!gameMap.CollideWithWall(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate+21))
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

                    else if (monsters[i].Facing == 2)
                    {
                        if (monsters[i].Y < bottomOfScreen)
                        {
                            if (!gameMap.CollideWithWall(monsters[i].BoundingBox, 2, monsters[i].MovementRate))
                            {
                                monsters[i].Y += monsters[i].MovementRate;
                                monsters[i].Facing = 2;
                                if (!gameMap.CollideWithWall(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate))
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

                    else if (monsters[i].Facing == 3)
                    {
                        if (monsters[i].X > leftEdgeOfScreen)
                        {
                            if (!gameMap.CollideWithWall(monsters[i].BoundingBox, 3, monsters[i].MovementRate))
                            {
                                monsters[i].X -= monsters[i].MovementRate;
                                monsters[i].Facing = 3;
                                if (!gameMap.CollideWithWall(monsters[i].BoundingBox, ((monsters[i].Facing + 3) % 4), monsters[i].MovementRate+21))
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
                if (guards[i].Facing % 2 == 0)
                {
                    if ((playerHitBox.Left <= guardHitBox.Right && playerHitBox.Left >= guardHitBox.Left) ||
                        (playerHitBox.Right <= guardHitBox.Right && playerHitBox.Right >= guardHitBox.Left))
                    {
                        return ((guards[i].Facing == 0 && playerHitBox.Bottom < guardHitBox.Top) ||
                                (guards[i].Facing == 2 && playerHitBox.Top > guardHitBox.Bottom)) && 
                                !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing);
                    }
                
                }
                else
                {
                    if ((playerHitBox.Top >= guardHitBox.Top && playerHitBox.Top <= guardHitBox.Bottom) ||
                        (playerHitBox.Bottom >= guardHitBox.Top && playerHitBox.Bottom <= guardHitBox.Bottom)) 
                    {
                        return ((guards[i].Facing == 1 && playerHitBox.Left >= guardHitBox.Right) ||
                                (guards[i].Facing == 3 && playerHitBox.Right <= guardHitBox.Left)) &&
                                !gameMap.WallBetween(guards[i].BoundingBox, protag.BoundingBox, guards[i].Facing);
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
            {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
            {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0},
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
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
            #endregion

            return layoutLevel;
        }
        
        #endregion
    }
}
