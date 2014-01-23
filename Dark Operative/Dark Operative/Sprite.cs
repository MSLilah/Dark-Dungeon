using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Dark_Operative
{
    class Sprite
    {
        #region Declarations
        //The actual sprite itself
        Texture2D spriteTexture;

        //Controls the framerate for the sprite's animation
        float frameRate = 0.02f;
        float elapsedTime = 0.0f;

        //Information on the spritesheet
        int spriteSheetXOffset= 0;
        int spriteSheetYOffset = 0;
        int spriteWidth = 32;
        int spriteHeight = 32;

        //Animation information
        int numberOfFrames = 1;
        int currentFrame = 0;

        //Position information
        int xPosition = 0;
        int yPosition = 0;

        bool animating = true;

        Color tinting = Color.White;
        #endregion

        public int X
        {
            set { xPosition = value; }
            get { return xPosition; }
        }

        public int Y
        {
            get { return yPosition; }
            set { yPosition = value; }
        }

        public int CurrFrame {
            get { return currentFrame }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, numberOfFrames); }
        }

        public float FrameLength
        {
            get { return frameRate; }
            set { frameRate = (float)Math.Max(value, 0f); }
        }

        public bool IsAnimating
        {
            get { return animating; }
            set { animating = value; }
        }

        public Color Tint
        {
            get { return tinting; }
            set { tinting = value; }
        }

        public Sprite(Texture2D spriteToSet,
                        int xOffset,
                        int yOffset,
                        int width,
                        int height,
                        int numFrames)
        {
            spriteTexture = spriteToSet;
            spriteSheetXOffset = xOffset;
            spriteSheetYOffset = yOffset;
            spriteWidth = width;
            spriteHeight = height;
            int numberOfFrames = numFrames;
        }


    }
}
