﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * Map.cs
 * 
 * @author Hailee Kenney
 * @author Lilah Ingvaldsen
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

        //The image that represents the player's goal
        Texture2D goalImage;

        // The x length of a section of wall
        int wallXDim = 30;

        // The y length of a section of wall
        int wallYDim = 30;

        // Dimensions of the goal
        int goalXDim = 30;
        int goalYDim = 30;

        Vector3 protagStart;
        ArrayList guardCoords = new ArrayList();
        ArrayList monsterCoords = new ArrayList();

        //Constants representing the various things in each
        //map slot
        public const int WALL = 1;
        public const int GOAL = 2;
        public const int PROTAGONIST = 3;
        public const int GUARD_UP = 4;
        public const int GUARD_RIGHT = 5;
        public const int GUARD_DOWN = 6;
        public const int GUARD_LEFT = 7;
        public const int MONSTER_UP = 8;
        public const int MONSTER_RIGHT = 9;
        public const int MONSTER_DOWN = 10;
        public const int MONSTER_LEFT = 11;
        public const int GUARD_STATIONARY_UP = 12;
        public const int GUARD_STATIONARY_RIGHT = 13;
        public const int GUARD_STATIONARY_DOWN = 14;
        public const int GUARD_STATIONARY_LEFT = 15;
        #endregion

        #region Properties

        public int[,] Layout
        {
            get { return levelLayout; }
        }

        public Vector3 ProtagStartCoords
        {
            get { return protagStart; }
        }

        public ArrayList MonsterCoords
        {
            get { return monsterCoords; }
        }

        public ArrayList GuardCoords
        {
            get { return guardCoords; }
        }
        #endregion

        #region Methods

        /**
         * Map
         * 
         * Constructor for the Map class
         */
        public Map(int[,] mapLayout, Texture2D wall, Texture2D goal)
        {
            levelLayout = mapLayout;
            wallImage = wall;
            goalImage = goal;

            //Count the guards and monsters so we can return their start coordinates as an array
            for (int i = 0; i < levelLayout.GetLength(0); i++)
            {
                for (int j = 0; j < levelLayout.GetLength(1); j++)
                {
                    if (levelLayout[i, j] >= GUARD_UP && levelLayout[i, j] <= GUARD_LEFT)
                    {
                        guardCoords.Add(MapStartCoords(i, j, levelLayout[i, j] - 4, false));
                    }
                    else if (levelLayout[i, j] >= MONSTER_UP && levelLayout[i, j] <= MONSTER_LEFT)
                    {
                        monsterCoords.Add(MapStartCoords(i, j, levelLayout[i, j] - 8, false));
                    }
                    else if (levelLayout[i, j] == PROTAGONIST)
                    {
                        protagStart = MapStartCoords(i, j, 0, false);
                    }
                    else if (levelLayout[i, j] >= GUARD_STATIONARY_UP && levelLayout[i, j] <= GUARD_STATIONARY_LEFT)
                    {
                        guardCoords.Add(MapStartCoords(i, j, levelLayout[i, j] - 12, true));
                    }
                }
            }

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

                    if (levelLayout[i, j] == WALL)
                    {
                        sb.Draw(wallImage, new Rectangle(i * wallXDim, j * wallYDim, wallXDim, wallYDim), Color.White);
                    }
                    else if (levelLayout[i, j] == GOAL)
                    {
                        sb.Draw(goalImage, new Rectangle(i * goalXDim, j * goalYDim, goalXDim, goalYDim), Color.White);
                    }
                }
            }
        }

        /**
         * MapStartCoords
         * 
         * Returns the starting coordinates of a given entity using its map slot and its facing
         * 
         * @return A Vector2 containing the position of the entity's upper left corner
         * 
         */
        protected Vector3 MapStartCoords(int i, int j, int facing, bool stationary)
        {
            int x = i * 30;
            int y = j * 30;

            //Modify spacing based on the direction the entity is facing so that
            //the entity is in the middle of a corridor
            if (facing == 0)
            {
                x += 20;
                y -= 5;
            }
            else if (facing == 1)
            {
                y += 3;
                x += 5;
            }
            else if (facing == 2)
            {
                x += 20;
                y += 5;
            }
            else if (facing == 3)
            {
                x -= 5;
                y += 3;
            }

            //Give a higher facing value if the entity is stationary
            //so that Game1 will know not to move that entity
            if (stationary)
            {
                return new Vector3(x, y, facing + 4);
            }
            return new Vector3(x, y, facing);
        }

        /**
         * CollideWithElement
         * 
         * Calculates if movement in a given direction causes a given
         * rectangle to collide with a wall
         * 
         * @param hitBox The Rectangle that will move
         * @param facing An integer representing the direction
         *                of movement
         * @param moveRate the number of pixels the Rectangle will move
         * @param type What to check for collision with the player
         * 
         * @return True if there will be a collision with a wall, else false
         */
        public bool CollideWithElement(Rectangle hitBox, int facing, int moveRate, int type)
        {
            int xDim;
            int yDim;

            if (type == WALL)
            {
                xDim = wallXDim;
                yDim = wallYDim;
            }
            else if (type == GOAL)
            {
                xDim = goalXDim;
                yDim = goalYDim;
            }
            else
            {
                return true;
            }

            int top = hitBox.Top / yDim;
            int bottom = hitBox.Bottom / yDim;
            int left = hitBox.Left / xDim;
            int right = hitBox.Right / xDim;

            //Check if the entity enters the same square as an element
            //of the given type if they were to move at the given moveRate
            //in the given direction
            #region Facing Up
            if (facing == 0)
            {
                if (((hitBox.Top - moveRate) / yDim) != top && top != 0) 
                {
                    for (int i = left; i <= right; i++)
                    {
                        if (levelLayout[i,top-1] == type)
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
                if (((hitBox.Right + moveRate) / xDim) != right && right != 39) 
                {
                    for (int i = top; i <= bottom; i++)
                    {
                        if (levelLayout[right+1, i] == type)
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
                if (((hitBox.Bottom + moveRate) / yDim) != bottom && bottom != 22)
                {
                    for (int i = left; i <= right; i++)
                    {
                        if (levelLayout[i, bottom+1] == type)
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
                if (((hitBox.Left - moveRate) / xDim) != left && left != 0)
                {
                    for (int i = top; i <= bottom; i++)
                    {
                        if (levelLayout[left-1, i] == type)
                        {
                            return true;
                        }
                    }
                }
            }

            #endregion

            return false;
        }

        /**
         * WallBetween
         * 
         * Takes two hitboxes and checks if there are any walls between them
         * 
         * @param guard The hitbox of a guard
         * @param protag The hitbox of the protagonist
         * @param facing The direction the guard is facing, used to determine the
         *                 direction to check
         *                 
         * @return True if there is a wall between the protagonist and the guard, false otherwise
         * 
         */
        public bool WallBetween(Rectangle guard, Rectangle protag, int facing)
        {
            int guardTop = guard.Top / wallYDim;
            int guardBottom = guard.Bottom / wallYDim;
            int guardLeft = guard.Left / wallXDim;
            int guardRight = guard.Right / wallXDim;

            int protagTop = protag.Top / wallYDim;
            int protagBottom = protag.Bottom / wallYDim;
            int protagLeft = protag.Left / wallXDim;
            int protagRight = protag.Right / wallXDim;

            //This method will only be called if a guard would have LOS without any
            //walls in the way. Check if there is a wall in the row/column shared
            //by the protagonist and the guard between their two locations
            #region Facing Up
            if (facing == 0)
            {
                for (int i = guardLeft; i <= guardRight; i++)
                {
                    for (int j = guardTop; j > protagBottom; j--)
                    {
                        if (levelLayout[i, j] == WALL)
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
                for (int j = guardTop; j <= guardBottom - 1; j++)
                {
                    for (int i = guardRight; i < protagLeft; i++)
                    {
                        if (levelLayout[i, j] == WALL)
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
                for (int i = guardLeft; i <= guardRight; i++)
                {
                    for (int j = guardBottom; j < protagTop; j++)
                    {
                        if (levelLayout[i, j] == WALL)
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
                for (int j = guardTop; j <= guardBottom - 1; j++)
                {
                    for (int i = guardLeft; i > protagRight; i--)
                    {
                        if (levelLayout[i, j] == WALL)
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
