using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * Guard.cs
 * 
 * A class representing a guard in the maze
 * 
 * @authors Lilah Ingvaldsen
 *  
 */

namespace Dark_Operative
{
    class Guard
    {
        #region Declarations

        //The guard's sprite
        Sprite guardSprite;

        //Movement control variables
        int moveRate = 1;
        float moveCount = 0.0f;
        float moveDelay = 0.01f;

        //Determines the amount of time for which a guard will wait
        //in a single location
        float waitCount = 0.0f;
        float waitTime = 1.0f;
        bool move = false;
        
        //Determines whether or not this guard is stationary
        bool stationary = false;

        //Determines how long a guard will walk for
        //TODO: Guard should stop when it reaches the end of a corridor.
        //This should be removed later.
        float walkCount = 0.0f;
        float walkTime = 7.0f;

        //The direction in which the guard is facing
        int facing = 0;

        //Position variables
        int xPos = 0;
        int yPos = 0;

        //Starting position
        int xStart = 0;
        int yStart = 0;
        int startFacing = 0;

        #endregion

        #region Properties

        public int X
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public int Y
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public int Facing
        {
            get { return facing; }
            set { facing = value % 4; }
        }

        public int MovementRate
        {
            get { return moveRate; }
            set { moveRate = value; }
        }

        public float MovementCount
        {
            get { return moveCount; }
            set { moveCount = value; }
        }

        public float MovementDelay
        {
            get { return moveDelay; }
            set { moveDelay = value; }
        }

        //States whether or not a guard is moving at the current
        //moment
        public bool Move
        {
            get { return move; }
        }

        //States whether or not the current guard is a stationary
        //guard
        public bool Stationary
        {
            get { return stationary; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(xPos, yPos, 21, 51); }
        }

        #endregion

        #region Methods

        /**
         * Guard
         * 
         * Constructor for the Guard class
         * 
         */
        public Guard(Texture2D spriteToSet, int x, int y, int directionToFace, bool stationaryGuard)
        {
            facing = directionToFace;
            startFacing = facing;
            guardSprite = new Sprite(spriteToSet, 0, 51 * facing, 21, 51, 2);
            xPos = x;
            xStart = x;
            yPos = y;
            yStart = y;
            //guardSprite.IsAnimating = false;
            stationary = stationaryGuard;
            if (stationary)
            {
                Stand(true);
            }
        }

        /**
         * Draw
         * 
         * Draws the Guard to the screen
         * 
         */
        public void Draw(SpriteBatch sb)
        {
            guardSprite.Draw(sb, xPos, yPos);
        }

        /**
        * DrawMenu
        * 
        * Draws the guard's sprite in the desired position
        * for the state menu
        * 
        * @param sb - The SpriteBatch to be used for drawing
        * 
        */
        public void DrawMenu(SpriteBatch sb)
        {
            guardSprite.Draw(sb, 870, 190);
            guardSprite.CurrFrame = 3;
            guardSprite.spriteSheetYOffset = 102;
        }

        /**
         * Update
         * 
         * Updates the Guard's sprite based on its current facing
         * and how it is moving
         * 
         * @param gametime - The current elapsed game time
         * 
         */
        public void Update(GameTime gametime)
        {
            guardSprite.spriteSheetYOffset = 51 * facing;

            if (!move)
            {
                waitCount += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (waitCount > waitTime)
                {
                    waitCount = 0;
                    move = true;
                    if (!stationary)
                    {
                        StartMovement();
                    }
                }
            }
            guardSprite.Update(gametime);
        }

        /**
         * Stand
         * 
         * Changes the guard's sprite to standing
         * 
         */
        public void Stand(bool reset)
        {
            if (guardSprite.IsAnimating)
            {
                move = false;
                guardSprite.numberOfFrames = 3;
                guardSprite.CurrFrame = 2;
                guardSprite.IsAnimating = false;

                if (reset)
                {
                    return;
                }

                //The guard should turn around when he stops
                if (facing > 1)
                {
                    facing -= 2;
                }
                else
                {
                    facing += 2;
                }
            }
        }

        /**
         * StartMovement
         * 
         * Changes the protagonist's sprite to its moving
         * version, and kicks off the movement animation
         * 
         */
        public void StartMovement()
        {
            move = true;
            if (!guardSprite.IsAnimating)
            {
                guardSprite.numberOfFrames = 2;
                guardSprite.CurrFrame = 0;
                guardSprite.IsAnimating = true;
            }
        }

        /**
         * Reset
         * 
         * Resets the guard's position in the maze, as well as his movement timers
         * 
         */
        public void Reset()
        {
            xPos = xStart;
            yPos = yStart;
            facing = startFacing;
            moveCount = 0.0f;
            waitCount = 0.0f;
            walkCount = 0.0f;
            move = false;
            Stand(true);
        }

        #endregion
    }
}
