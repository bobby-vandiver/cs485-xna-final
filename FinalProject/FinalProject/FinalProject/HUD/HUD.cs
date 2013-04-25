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
    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        Texture2D RadarText;
        Texture2D playerRadar;
        Texture2D alienRadar;
        public Vector3[] alienPosition = new Vector3[20];
        public int alienRadarCount;

        PlayerHealth playerHealth = new PlayerHealth();
        Texture2D powerBar;
        SpriteFont healthFont;
        public float intensity = 0;
        float scale = 1f;
        int scale_base = 1;
        bool displayHealthBar;

        public HUD(Game game, bool displayHealthBar,float scale, int scale_base)
            : base(game)
        {
            this.scale = scale;
            this.scale_base = scale_base;
            this.displayHealthBar = displayHealthBar;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            playerHealth.setMaxHealth();
            powerBar = Game.Content.Load<Texture2D>(@"Textures\pBar");
            healthFont = Game.Content.Load<SpriteFont>(@"Fonts\healthFont");
            RadarText = Game.Content.Load<Texture2D>(@"Textures\Radar");
            playerRadar = Game.Content.Load<Texture2D>(@"Textures\circle");
            alienRadar = Game.Content.Load<Texture2D>(@"Textures\acircle");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = (SpriteFont)Game.Services.GetService(typeof(SpriteFont));

            spriteBatch.Begin();

            DisplayHealthBar();
            DisplayRadar();

            spriteBatch.End();
            base.Draw(gameTime);
        }


        private void DisplayHealthBar()
        {
            if (displayHealthBar == true)
            {
                DrawRectangle(new Rectangle((Game.Window.ClientBounds.Width - 300), 30, playerHealth.playerHealth, 40), playerHealth.GetColor(), powerBar);
                spriteBatch.DrawString(healthFont, "Health: " + playerHealth.playerHealth, new Vector2(Game.Window.ClientBounds.Width - 500, 30), Color.Black);
            }
        }

        private void DisplayRadar()
        {
            Camera camera = (Camera)Game.Services.GetService(typeof(Camera));

            DrawRectangle(new Rectangle(-40, -45, 300, 300), Color.White, RadarText);
            if (camera != null&&playerRadar!=null)
            {
                DrawRectangle(new Rectangle((int)(camera.Position.X /scale + scale_base), (int)(camera.Position.Z /scale + scale_base), 20, 20), Color.Green, playerRadar);
            }
            for (int i = 0; i < alienRadarCount; i++)
            {
                DrawRectangle(new Rectangle((int)(alienPosition[i].X /scale + scale_base), (int)(alienPosition[i].Z /scale + scale_base), 20, 20), Color.Red, powerBar);

            }
        }

        public void DrawRectangle(Rectangle coords, Color color, Texture2D text)
        {
            var rect = new Texture2D(GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });
            this.spriteBatch.Draw(text, coords, color);
        }

        public void RedHealth()
        {
            playerHealth.decrementPlayerHealth((int)(intensity * 10));
        }

        public bool IsPlayerDead()
        {
            return playerHealth.IsPlayerDead();
        }

        public void DecrementPlayerHealth(int amount)
        {
            playerHealth.decrementPlayerHealth(amount);
        }
    }
}
