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


namespace FinalProject
{
    public abstract class Level : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Level(Game game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Update the game state when the end of level is reached
            if (LevelOver())
                ((Game1)Game).CurrentLevelState = Game1.LevelState.End;

            base.Update(gameTime);
        }

        // Override this to provide logic necessary to determine end of level
        protected abstract bool LevelOver();

        protected void PrepareGraphicsDeviceForDrawing3D()
        {
            // Reset state in case SpriteBatch is used somewhere
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
