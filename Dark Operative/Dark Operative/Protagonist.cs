using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/**
 * Protagonist.cs
 * 
 * A class representing the Protagonist of our game. 
 * 
 * @author Lilah Ingvaldsen
 * 
 */

namespace Dark_Operative
{
    class Protagonist
    {
        #region Declarations
        Sprite protagSprite;
        int xPos = 0;
        int yPos = 0;

        //Tells which direction she is facing. 0 is up, 1 is right, 2 is down, 3 is left
        int facing = 0;

        //Used to control the protagonist's movement
        int moveRate = 3;
        float moveCount = 0.0f;
        float moveDelay = 0.01f;
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
            set { facing = value; }
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

        public Rectangle BoundingBox
        {
            get { return new Rectangle(xPos, yPos, 21, 51); }
        }

        public bool Animating
        {
            set { protagSprite.IsAnimating = value; }
        }
        #endregion

        #region Methods

        /**
         * Protagonist
         * 
         * Constructor for the Protagonist class
         * 
         * @param texture - The texture to be used for the
         *                  protagonist's sprite
         *  
         */
        public Protagonist(Texture2D texture)
        {
            protagSprite = new Sprite(texture, 0, 0, 21, 51, 2);
            //protagSprite.IsAnimating = false;
        }

        /**
         * Draw
         * 
         * Draws the protagonist's sprite
         * 
         * @param sb - The SpriteBatch to be used for the drawing
         * 
         */
        public void Draw(SpriteBatch sb)
        {
            protagSprite.Draw(sb, xPos, yPos);
        }


        /**
         * Update
         * 
         * Updates the Protagonist's sprite based on its current facing
         * and how it is moving
         * 
         * @param gametime - The current elapsed game time
         * 
         */
        public void Update(GameTime gametime)
        {
            protagSprite.Y = 51 * facing;
            protagSprite.Update(gametime);
        }

        /**
         * Reset
         * 
         * Resets the Protagonist upon death.
         * 
         */
        public void Reset()
        {
            //TODO: This will probably just reset the player's position in the maze
        }

        #endregion
    }
}
