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
    public class AsteroidField : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Asteroid bigAsteroid;
        Asteroid Asteroid;
        SpriteFont font;
        int score = 0;
        Boolean firstDraw = false;
        const int A_COUNT = 50;
        const int B_COUNT = 10;
        Asteroid[] asteroids;
        Bullet[] bullets;
        Model a1;
        Model a2;

        // Background
        Texture2D backgroundTexture;

        // Height of each (x, z) point in the world
        float maxHeight;
        
        Camera cam;
        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        public AsteroidField(Game game,Camera camera, float maxHeight)
            : base(game)
        {
            cam = camera;
            this.maxHeight = maxHeight;
        }

        public override void Initialize()
        {
            
            base.Initialize();
        }


        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>(@"Fonts\GameFont");

            // Load textures
            backgroundTexture = Game.Content.Load<Texture2D>(@"Backgrounds\stars");
            //load asteroids one for large, one for small
            a1 = Game.Content.Load<Model>(@"Models\asteroid1");
            a2 = Game.Content.Load<Model>(@"Models\asteroid");
            GenAsteroidField();
            GenBullet();
            basicEffect = new BasicEffect(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < A_COUNT; i++)
            {
                asteroids[i].Update(gameTime);
                if (asteroids[i].world.Translation.Z >  (cam.Position.Z + 100))
                {
                    asteroids[i].alive = false;
                    GenAsteroid(i);
                }        
            }

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                bullets[0] = new Bullet(a2, cam.Position, cam);
                firstDraw = true;
            }
            bullets[0].Update(gameTime);
            for (int i = 0; i < A_COUNT; i++)
            {
                if(asteroids[i].CollidesWith(bullets[0].bs))
                {
                    score+= 10;
                    asteroids[i].alive = false;
                    bullets[0].alive = false;
                }
            }
            base.Update(gameTime);

        }

        private void GenBullet()
        {
            bullets = new Bullet[B_COUNT+1];
            bullets[0] = new Bullet(a2, cam.Position, cam);
        }
        private void DrawBullet()
        {
            if (firstDraw)
            {
                bullets[0].Draw(cam);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            DrawBackground();
            PrepareGraphicsDeviceForDrawing3D();
            PrepareBasicEffectForDrawing3D();
            DrawBullet();
            DrawAsteroidField();
            base.Draw(gameTime);
        }

        private void DrawBackground()
        {
            int windowWidth = Game.Window.ClientBounds.Width;
            int windowHeight = Game.Window.ClientBounds.Height;
            var destination = new Rectangle(0, 0, windowWidth, windowHeight);

            spriteBatch.Begin();
            string scoreText = "Score: " + score;
            spriteBatch.Draw(backgroundTexture, destination, Color.White);
            spriteBatch.DrawString(font, scoreText,
                new Vector2(10, 10), Color.Red);
            spriteBatch.End();
        }
        private void DrawAsteroidField()
        {
            for(int i = 0; i<A_COUNT;i++)
            {
                    asteroids[i].Draw(cam);
            }
        }
        private void GenAsteroidField()
        {
            Vector3 placement;
            asteroids = new Asteroid[A_COUNT+1];
            for (int i = 0; i < A_COUNT; i++)
            {
                placement = GetRandomPoint();
                asteroids[i] = new Asteroid(a2, placement, cam);   
            }
        }
        private void GenAsteroid(int index)
        {
            Vector3 placement;
            
                placement = GetRandomPoint();
                asteroids[index] = new Asteroid(a2, placement, cam);
            
        }


        private void PrepareGraphicsDeviceForDrawing3D()
        {
            // Reset state in case SpriteBatch is used somewhere
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void PrepareBasicEffectForDrawing3D()
        {
            Camera camera = (Camera)Game.Services.GetService(typeof(Camera));

            // Set matrices
            basicEffect.World = Matrix.Identity;
            basicEffect.View = cam.View;
            basicEffect.Projection = cam.Projection;
            // Enable lighting
            basicEffect.LightingEnabled = true;

            // Set up directional light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 1));
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
            basicEffect.DirectionalLight0.SpecularColor = Color.White.ToVector3();

            basicEffect.Techniques[0].Passes[0].Apply();

        }

        // Generates a random point in the world and on the terrain
        public Vector3 GetRandomPoint()
        {
            Random randomNumberGenerator = (Random)Game.Services.GetService(typeof(Random));
            // Find a random point in the world
            float x = (float)randomNumberGenerator.Next(-50,50)*(float)randomNumberGenerator.NextDouble();
            float z = (float)randomNumberGenerator.NextDouble()*-200;
            float y = (float)randomNumberGenerator.Next(-30, 30)*(float)randomNumberGenerator.NextDouble();
            return new Vector3(x, 40+y, -20+z);
        }
        public Vector3 GenerateRandomDirection()
        {
            Random randomNumberGenerator = (Random)Game.Services.GetService(typeof(Random));
            // Find a random point in the world
            float x = (float)randomNumberGenerator.NextDouble() * 100;
            float z = (float)randomNumberGenerator.NextDouble() * 100;
            float y = (float)randomNumberGenerator.NextDouble() * 100;
            return new Vector3(x, y, z);
        }


        // MathHelper doesn't provide an implementation for non-float
        private int Clamp(int value, int minValue, int maxValue)
        {
            if (value <= minValue)
                return minValue;
            else if (value >= maxValue)
                return maxValue;
            else
                return value;
        }

    }
}
