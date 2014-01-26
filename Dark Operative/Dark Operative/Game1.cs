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
        Texture2D backgroundImage;
        Sprite player;
        public int topOfScreen = 0;
        public int bottomOfScreen = 665;
        public int leftEdgeOfScreen = 0;
        public int rightEdgeOfScreen = 1255;

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
            protag = new Protagonist(Content.Load<Texture2D>(@"Textures\protagSpriteSheet"));
            backgroundImage = Content.Load<Texture2D>(@"Textures\backgroundImage");
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
            protag.Update(gameTime);
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
            protag.Draw(spriteBatch);
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
                    protag.Y -= protag.MovementRate;
                    resetTimer = true;
                    protag.Facing = 0;
                }
            }

            //These elses restrict the player to only moving in four directions, which is what we want
            else if ((keyboard.IsKeyDown(Keys.Down)) || (gamepad.ThumbSticks.Left.Y < 0))
            {
                if (protag.Y < bottomOfScreen)
                {
                    protag.Y += protag.MovementRate;
                    resetTimer = true;
                    protag.Facing = 2;
                }
            }

            else if ((keyboard.IsKeyDown(Keys.Right)) || (gamepad.ThumbSticks.Left.X > 0)) {
                if (protag.X < rightEdgeOfScreen) {
                    protag.X += protag.MovementRate;
                    protag.Facing = 1;
                }
            }

            else if ((keyboard.IsKeyDown(Keys.Left)) || (gamepad.ThumbSticks.Left.X < 0)) {
                if (protag.X > leftEdgeOfScreen) {
                    protag.X -= protag.MovementRate;
                    protag.Facing = 3;
                }
            }

            if (resetTimer)
            {
                protag.MovementCount = 0f;
                protag.Move();
            }
            else
            {
                protag.Stand();
            }
        }

        #endregion
    }
}
