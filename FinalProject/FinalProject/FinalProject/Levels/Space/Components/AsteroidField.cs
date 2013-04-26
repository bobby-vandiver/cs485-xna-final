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
        Audio audio;
        SpriteFont font;

        int asteroids_killed = 0;
        int times_hit = 0;
        int loop_timer = 0;
        int score = 0;
        string status="";

        public Boolean game_over = false;
        
        const int ASTEROID_COUNT = 50;
        const int FRAGMENT_COUNT = 10;
        
        public Asteroid[] asteroids;
        public Fragment[] explosions;
        
        Boolean Explode = false;
        Bullet bullet;

        Model smallAsteroidModel;
        Model largeAsteroidModel;

        Texture2D backgroundTexture;
        float maxHeight;
        
        Camera camera;

        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        Spaceship ship;
        HUD hud;

        public AsteroidField(Game game, Camera camera, float maxHeight, HUD hud)
            : base(game)
        {
            this.hud = hud;
            this.camera = camera;
            this.maxHeight = maxHeight;
        }
        public override void Initialize()
        {
            audio = (Audio)Game.Services.GetService(typeof(Audio));
            audio.PlayBackgroundMusic("ObstacleCourse");
            base.Initialize();
        }


        protected override void LoadContent()
        {
            font = (SpriteFont)Game.Services.GetService(typeof(SpriteFont));

            Model spaceshipmodel = Game.Content.Load<Model>(@"Models\spaceship");
            ship = new Spaceship(spaceshipmodel, camera);

            // Load textures
            backgroundTexture = Game.Content.Load<Texture2D>(@"Backgrounds\stars");
            
            //load asteroids one for large, one for small
            smallAsteroidModel = Game.Content.Load<Model>(@"Models\asteroid1");
            largeAsteroidModel = Game.Content.Load<Model>(@"Models\asteroid");
            
            GenerateAsteroidField();
            
            basicEffect = new BasicEffect(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            loop_timer++;

            UpdateBullet(gameTime);
            UpdateAsteroids(gameTime);
            UpdateExplosions(gameTime);
            UpdateHud();

            CheckCollisions();

            // Check for end of level condition
            if (score > 10000 || ship.damage_count > 20)
                game_over = true;

            base.Update(gameTime);
        }

        private void UpdateHud()
        {
            UpdateRadar();
            UpdateScoreAndStatus();
        }

        private void UpdateScoreAndStatus()
        {
            if (loop_timer % 30 == 0)
            {
                ship.col = Vector3.Zero;
                status = "Warning! " + (20 - times_hit) + " hit before crash land";
                score = (times_hit * -10) + (asteroids_killed * 30);
            }
        }

        private void UpdateRadar()
        {
            for (int i = 0; i < 10; i++)
                hud.alienPosition[i] = asteroids[i].position;
            hud.alienRadarCount = 10;
        }

        private void UpdateAsteroids(GameTime gameTime)
        {
            for (int i = 0; i < ASTEROID_COUNT; i++)
            {
                asteroids[i].Update(gameTime);
                if (asteroids[i].world.Translation.Z > (camera.Position.Z + 100))
                {
                    asteroids[i].alive = false;
                    GenerateAsteroid(i);
                }
            }
        }

        private void CheckCollisions()
        {
            CheckShipCollisions();
            CheckBulletCollisions();
        }

        private void CheckShipCollisions()
        {
            // Check for collisions with the ship
            for (int i = 0; i < ASTEROID_COUNT; i++)
            {
                if (asteroids[i].Collides(ship))
                {
                    audio.PlayCue("flashbang");
                    ship.col = new Vector3(1, 0, 0);
                    status = "HIT!";
                    Explode = true;
                    ship.damage_count++;
                    times_hit++;
                }
            }

        }

        private void CheckBulletCollisions()
        {
            // Check for collisions with bullet
            for (int i = 0; i < ASTEROID_COUNT && bullet != null; i++)
            {
                if (asteroids[i].Collides(bullet))
                {
                    audio.PlayCue("explosion");
                    asteroids_killed++;
                    GenerateAsteroid(i);
                    bullet.IsAlive = false;
                    status = "ASTEROID HIT!";

                    //GenerateExplosionField(asteroids[i].position);
                    //Explode = true;
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            if (explosions == null)
                return;

            for (int i = 0; i < FRAGMENT_COUNT; i++)
            {
                //GenerateExplosionField(new Vector3(0, 0, 100));
                if (Explode == true && explosions[i] != null)
                {
                    explosions[i].Update(gameTime);
                }
            }
        }

        private void UpdateBullet(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                audio.PlayCue("laser");
                CreateBullet();
            }

            if (bullet != null)
                bullet.Update(gameTime);
        }

        private void CreateBullet()
        {
            if(bullet == null || !bullet.IsAlive)
                bullet = new Bullet(largeAsteroidModel, camera.Position, camera);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            DrawBackground();
            DrawScoreAndStatus();

            PrepareGraphicsDeviceForDrawing3D();
            PrepareBasicEffectForDrawing3D();

            DrawBullet();
            DrawAsteroidField();
            //DrawExplosionField();
            
            ship.Draw(camera);

            base.Draw(gameTime);
        }

        private void DrawBullet()
        {
            if (bullet != null)
                bullet.Draw(camera);
        }

        private void DrawBackground()
        {
            int windowWidth = Game.Window.ClientBounds.Width;
            int windowHeight = Game.Window.ClientBounds.Height;
            var destination = new Rectangle(0, 0, windowWidth, windowHeight);
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, destination, Color.White);
            spriteBatch.End();
        }

        private void DrawScoreAndStatus()
        {
            string scoreText = "Score: " + score;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, scoreText, new Vector2(200, 10), Color.White);
            spriteBatch.DrawString(font, status, new Vector2(400, 20), Color.Red);
            spriteBatch.End();
        }
        
        private void DrawAsteroidField()
        {
            for(int i = 0; i<ASTEROID_COUNT;i++)
            {
                    asteroids[i].Draw(camera);
            }
        }

        private void DrawExplosionField()
        {
            for (int i = 0; i < FRAGMENT_COUNT && explosions != null; i++)
            {
                explosions[i].Draw(camera);
            }
        }

        private void GenerateAsteroidField()
        {
            asteroids = new Asteroid[ASTEROID_COUNT+1];
            for (int i = 0; i < ASTEROID_COUNT; i++)
            {
                Vector3 placement = GetRandomPoint();
                asteroids[i] = new Asteroid(largeAsteroidModel, placement, camera);   
            }
        }

        private void GenerateExplosionField(Vector3 startPos)
        {
            explosions = new Fragment[FRAGMENT_COUNT + 1];
            for (int i = 0; i < FRAGMENT_COUNT; i++)
            {
                Vector3 placement = startPos;
                explosions[i] = new Fragment(largeAsteroidModel, placement, camera);
            }
        }

        private void GenerateAsteroid(int index)
        {
            Vector3 placement = GetRandomPoint();
            asteroids[index] = new Asteroid(largeAsteroidModel, placement, camera);
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
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
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
            float z = (float)randomNumberGenerator.NextDouble()*-200 + (-15);
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
