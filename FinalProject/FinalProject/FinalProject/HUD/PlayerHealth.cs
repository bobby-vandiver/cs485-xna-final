using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace FinalProject
{
    class PlayerHealth
    {
        public int playerHealth = 0;
        int maxHealth = 200;
        int lowHelath = 20;
        SpriteBatch spriteBatch;
        Texture2D rectangle;
        GraphicsDeviceManager graphics;
        Game game;
        

        public PlayerHealth()
        {
        }

        public void Initialize()
        {
        }

        protected void LoadContent()
        {

        }
        public Color GetColor()
        {
            if (playerHealth > 60)
            {
                return Color.Green;
            }
            else
                return Color.Red;
        }

        public void setMaxHealth()
        {
            playerHealth = maxHealth;
        }

        public void decrementPlayerHealth(int amount)
        {
            playerHealth -= amount;
        }

        public void incrementPlayerHealth(int amount)
        {
            playerHealth += amount;

        }
     
        public void Draw()
        {
            spriteBatch.Begin();
            

            spriteBatch.End();


        }
    }
}
