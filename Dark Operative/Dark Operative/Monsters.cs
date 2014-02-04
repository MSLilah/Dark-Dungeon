using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/**
 * Monster.cs
 * 
 * A class representing the monsters
 * 
 * @author Justice Nichols
 * 
 */
namespace Dark_Operative
{
    class Monster
    {

        #region Declarations

        //The monster's sprite
        Sprite monsterSprite;

        //Movement control variables
        int moveRate = 1;
        float moveCount = 0.0f;
        float moveDelay = 0.01f;

        //Determines the amount of time for which a monster will wait
        //in a single location
        float waitCount = 0.0f;
        float waitTime = 1.0f;
        bool move = false;

        //Determines how long a monster will walk for
        //TODO: Monster should stop when it reaches the end of a corridor.
        //This should be removed later.
        float walkCount = 0.0f;
        float walkTime = 7.0f;

        //The direction in which the monster is facing
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

        public bool Move
        {
            get { return move; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(xPos, yPos, 21, 51); }
        }

        #endregion

        #region Methods

        /**
         * Monster
         * 
         * Constructor for the Monster class
         * 
         */
        public Monster(Texture2D spriteToSet, int x, int y, int directionToFace)
        {
            facing = directionToFace;
            startFacing = facing;
            monsterSprite = new Sprite(spriteToSet, 0, 51 * facing, 21, 51, 2);
            xPos = x;
            xStart = x;
            yPos = y;
            yStart = y;
            monsterSprite.IsAnimating = false;
        }

        /**
         * Draw
         * 
         * Draws the Monster to the screen
         * 
         */
        public void Draw(SpriteBatch sb)
        {
            monsterSprite.Draw(sb, xPos, yPos);
        }

        /**
         * Update
         * 
         * Updates the Monster's sprite based on its current facing
         * and how it is moving
         * 
         * @param gametime - The current elapsed game time
         * 
         */
        public void Update(GameTime gametime)
        {
            monsterSprite.spriteSheetYOffset = 51 * facing;

            if (!move)
            {
                waitCount += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (waitCount > waitTime)
                {
                    waitCount = 0;
                    move = true;
                    StartMovement();
                }
            }

            /*else
            {
                walkCount += (float)gametime.ElapsedGameTime.TotalSeconds;
                if (walkCount > walkTime)
                {
                    walkCount = 0;
                    move = false;
                    Stand(false);
                }
            }*/
            monsterSprite.Update(gametime);
        }

        /**
         * Stand
         * 
         * Changes the monster's sprite to standing
         * 
         */
        public void Stand(bool reset)
        {
            if (monsterSprite.IsAnimating)
            {
                move = false;
                monsterSprite.numberOfFrames = 3;
                monsterSprite.CurrFrame = 2;
                monsterSprite.IsAnimating = false;

                if (reset)
                {
                    return;
                }

                //The monster should turn around when he stops
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
            if (!monsterSprite.IsAnimating)
            {
                monsterSprite.numberOfFrames = 2;
                monsterSprite.CurrFrame = 0;
                monsterSprite.IsAnimating = true;
            }
        }

        /**
         * Reset
         * 
         * Resets the monster's position in the maze, as well as his movement timers
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


