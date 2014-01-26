using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * Sprite.cs
 * 
 * This file has been adapted from the AnimatedSprite class of the Star Control tutorial
 * located at http://www.xnaresources.com
 * 
 * @author Preben Ingvaldsen
 * 
 **/


namespace Dark_Operative
{
    class Sprite
    {
        #region Declarations
        //The actual sprite itself
        Texture2D spriteTexture;

        //Controls the framerate for the sprite's animation
        float frameRate = 0.5f;
        float elapsedTime = 0.0f;

        //Information on the spritesheet
        int spriteSheetXOffset= 0;
        public int spriteSheetYOffset = 0;
        int spriteWidth = 32;
        int spriteHeight = 32;

        //Animation information
        public int numberOfFrames = 1;
        int currentFrame = 0;

        //Position information
        int xPosition = 0;
        int yPosition = 0;

        bool animating = true;

        Color tinting = Color.White;
        #endregion

        #region Properties
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
            get { return currentFrame; }
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
#endregion

        public Sprite(Texture2D spriteToSet, int xOffset, int yOffset, int width, int height, int numFrames)
        {
            spriteTexture = spriteToSet;
            spriteSheetXOffset = xOffset;
            spriteSheetYOffset = yOffset;
            spriteWidth = width;
            spriteHeight = height;
            numberOfFrames = numFrames;
        }

        public Rectangle getFrame()
        {
            return new Rectangle(
                spriteSheetXOffset + (spriteWidth * currentFrame),
                spriteSheetYOffset,
                spriteWidth,
                spriteHeight);
        }

        public void Update(GameTime gametime)
        {
            if (animating)
            {
                //Accumulate elapsed time...
                elapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

                //Until it passes our frame length
                if (elapsedTime > frameRate)
                {
                    //Increment the current frame, wrapping back to 0 at iFrameCount
                    currentFrame = (currentFrame + 1) % numberOfFrames;

                    //Reset the elapsed frame time
                    elapsedTime = 0.0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset)
        {
            spriteBatch.Draw(spriteTexture, new Rectangle(xPosition + XOffset, yPosition + YOffset, spriteWidth, spriteHeight),
                getFrame(), tinting);
        }
    }
}
