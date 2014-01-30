using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * Map.cs
 * 
 * @author Hailee Kenney
 * @author Preben Ingvaldsen
 */


namespace Dark_Operative
{
    class Map
    {
        #region Declarations
        // An array which describes the game map
        // 1 = wall, 0 = empty space
        // Dimensions: 40x22 squares
        int[,] levelLayout;

        // The image that represents a 30x30 section of wall
        Texture2D wallImage;

        // The x length of a section of wall
        int wallXDim = 30;

        // The y length of a section of wall
        int wallYDim = 30;

        //Constants representing the various things in each
        //map slot
        private int WALL = 1;
        #endregion

        #region Properties

        public int[,] Layout
        {
            get { return levelLayout; }
        }
        #endregion

        #region Methods

        /**
         * Map
         * 
         * Constructor for the Map class
         */
        public Map(int[,] mapLayout, Texture2D wall)
        {
            levelLayout = mapLayout;
            wallImage = wall;
        }

        /**
         * Update
         * 
         * A method which updates the map accordingly
         */
        public void Update(GameTime gametime)
        {
        }

        /**
         * Draw
         * 
         * Draws the walls of the map on the screen
         */
        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < levelLayout.GetLength(0); i++)
            {
                for (int j = 0; j < levelLayout.GetLength(1); j++)
                {

                    if (levelLayout[i, j] == 1)
                    {
                        sb.Draw(wallImage, new Rectangle(i * wallXDim, j * wallYDim, wallXDim, wallYDim), Color.White);
                    }
                }
            }
        }

        /**
         * CollideWithWall
         * 
         * Calculates if movement in a given direction causes a given
         * rectangle to collide with a wall
         * 
         * @param hitBox The Rectangle that will move
         * @param facing An integer representing the direction
         *                of movement
         * @param moveRate the number of pixels the Rectangle will move
         * 
         * @return True if there will be a collision with a wall, else false
         */
        public bool CollideWithWall(Rectangle hitBox, int facing, int moveRate)
        {
            int top = hitBox.Top / wallYDim;
            int bottom = hitBox.Bottom / wallYDim;
            int left = hitBox.Left / wallXDim;
            int right = hitBox.Right / wallXDim;

            #region Facing Up
            if (facing == 0)
            {
                if (((hitBox.Top - moveRate) / wallYDim) != top && top != 0) 
                {
                    for (int i = left; i <= right; i++)
                    {
                        if (levelLayout[i,top-1] == WALL)
                        {
                            return true;
                        }
                    }
                }
            }
            #endregion

            #region Facing Right

            else if (facing == 1)
            {
                if (((hitBox.Right + moveRate) / wallXDim) != right && right != 39) 
                {
                    for (int i = top; i <= bottom; i++)
                    {
                        if (levelLayout[right+1, i] == WALL)
                        {
                            return true;
                        }
                    }
                }
            }

            #endregion

            #region Facing Down

            else if (facing == 2)
            {
                if (((hitBox.Bottom + moveRate) / wallYDim) != bottom && bottom != 22)
                {
                    for (int i = left; i <= right; i++)
                    {
                        if (levelLayout[i, bottom+1] == WALL)
                        {
                            return true;
                        }
                    }
                }
            }
            #endregion

            #region Facing Left

            else if (facing == 3)
            {
                if (((hitBox.Left - moveRate) / wallXDim) != left && left != 0)
                {
                    for (int i = top; i <= bottom; i++)
                    {
                        if (levelLayout[left-1, i] == WALL)
                        {
                            return true;
                        }
                    }
                }
            }

            #endregion

            return false;
        }

        #endregion

    }
}
