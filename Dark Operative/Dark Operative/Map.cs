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
 */


namespace Dark_Operative
{
    class Map
    {
        #region Declarations
        // An array which describes the game map
        // 1 = wall, 0 = empty space
        // Dimensions: 40x22 squares
        int[][] levelLayout;

        // The image that represents a 30x30 section of wall
        Texture2D wallImage;

        // The x length of a section of wall
        int wallXDim = 30;

        // The y length of a section of wall
        int wallYDim = 30;
        #endregion

        #region Properties

        public int[][] Layout
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
        public Map(int[][] mapLayout, Texture2D wall)
        {
            levelLayout = mapLayout;
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
            for (int i = 0; i < levelLayout.Length; i++)
            {
                for (int j = 0; j < levelLayout[i].Length; j++)
                {
                    if(levelLayout[i][j] == 1) {
                        sb.Draw(wallImage, new Rectangle(i * wallXDim, j * wallYDim, wallXDim, wallYDim), Color.White);
                    }
                }
            }
        }

        #endregion

    }
}
