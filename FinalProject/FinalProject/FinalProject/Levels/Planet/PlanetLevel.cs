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
    public class PlanetLevel : Level
    {
        public Camera camera;

        Terrain terrain;
        Skybox skybox;
        
        LaserGun laserGun;
        LaserBeam laserBeam;

        // Randomly generate the number of aliens
        const int MIN_ALIEN_COUNT = 10;
        const int MAX_ALIEN_COUNT = 15;

        // DEBUG:
        //const int MIN_ALIEN_COUNT = 1;
        //const int MAX_ALIEN_COUNT = 1;

        List<Alien> aliens;
        

        Effect explosionEffect;

        CollisionBillboard collisionBillboard;
        PlayerHealth playerHealth = new PlayerHealth();
        FauxAstroid fauxAstroid;
        public Vector3[] fauxPosition = new Vector3[30];

        public float milliseconds;
        Bombard bombard;
        
        public PlanetLevel(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            InitializeCamera();
            spreadFaux();
            // The gun model needs to be render after everything so it appears 'on top'
            this.DrawOrder = 3;
            base.Initialize();

        }

        private void InitializeCamera()
        {
            Terrain terrain = InitializeTerrain();

            // Start in the center of the world
            float midX = (float)(terrain.MinX + terrain.MaxX) / 2.0f;
            float midZ = (float)(terrain.MinZ + terrain.MaxZ) / 2.0f;
            Vector3 position = new Vector3(midX, 0, midZ);

            camera = new PlanetCamera(Game, Vector3.Forward, Vector3.Up, position, terrain);
            camera.DrawOrder = 2;
            
            Game.Components.Add(camera);
            Game.Services.AddService(typeof(Camera), camera);
        }

        private Terrain InitializeTerrain()
        {
            terrain = new Terrain(Game, 1.0f, 100.0f);
            terrain.DrawOrder = 1;
            Game.Components.Add(terrain);
            playerHealth.setMaxHealth();
            return terrain;
        }

        protected override void LoadContent()
        {
            LoadContentForBombardment();
            LoadLaserGun();
            LoadSkybox();
            LoadAliens();
            //camera.showRadar = true;
            base.LoadContent();
        }

        private void LoadContentForBombardment()
        {
            Texture2D bombTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\shader");
            Texture2D smokeTexture = Game.Content.Load<Texture2D>(@"Textures\smoke");

            Model BombardmentModel = Game.Content.Load<Model>(@"Models\ammo");
            bombard = new Bombard(BombardmentModel, terrain, bombTexture, camera, milliseconds, explosionEffect, smokeTexture);

            collisionBillboard = new CollisionBillboard(GraphicsDevice, Game.Content,
                       Game.Content.Load<Texture2D>(@"Textures\smoke"), new Vector2(50), bombard.collisionPosition);

            fauxAstroid = new FauxAstroid(GraphicsDevice, Game.Content,
                      Game.Content.Load<Texture2D>(@"Textures\fauxAstroid"), new Vector2(10), fauxPosition);
        }


        private void LoadLaserGun()
        {
            Model laserGunModel = Game.Content.Load<Model>(@"Models\Weapons\lasergun");
            laserGun = new LaserGun(laserGunModel);
        }

        private void LoadSkybox()
        {
            skybox = new Skybox(Game, @"Backgrounds\Sunset", 500f);
            skybox.DrawOrder = 0;
            Game.Components.Add(skybox);
        }

        private void LoadAliens()
        {
            Model alienModel = Game.Content.Load<Model>(@"Models\alien");

            Random randomNumberGenerator = (Random)Game.Services.GetService(typeof(Random));
            int alienCount = randomNumberGenerator.Next(MIN_ALIEN_COUNT, MAX_ALIEN_COUNT);

            aliens = new List<Alien>();

            for (int i = 0; i < alienCount; i++)
            {
                // Place each alien at a random point on the terrain
                Vector3 position = GetUniqueRandomPointInWorld(randomNumberGenerator);
                //camera.alienRadarCount = alienCount;
                //Vector3 position = camera.Position - new Vector3(0, 0, 50.0f);
                position.Y = terrain.GetHeight(position.X, position.Z);

                Alien alien = new Alien(alienModel, position, Vector3.UnitZ);
                aliens.Add(alien);

                Console.WriteLine("Alien[" + i + "]: " + alien.Position);
            }
        }

        private Vector3 GetUniqueRandomPointInWorld(Random randomNumberGenerator)
        {
            // Be optimistic that a unique position will be found after one try
            bool unique = true;

            // Arbitrary distance between objects in the world to avoid multiple objects overlapping
            float minimumDistanceAllowed = 5.0f;

            Vector3 randomPosition;

            do
            {
                // Generate a random point and see if anything else is there
                randomPosition = terrain.GetRandomPoint();

                // Check for overlap with camera
                if (Vector3.Distance(randomPosition, camera.Position) < minimumDistanceAllowed)
                    unique = false;

                // Check for overlap with existent aliens
                foreach (Alien a in aliens)
                {
                    if (Vector3.Distance(randomPosition, a.Position) < minimumDistanceAllowed)
                        unique = false;
                }
            } while (!unique);

            return randomPosition;
        }

        protected override void UnloadResources()
        {
            Game.Components.Remove(camera);
            Game.Components.Remove(terrain);
            Game.Components.Remove(skybox);
            Game.Services.RemoveService(typeof(Camera));
            Game.Services.RemoveService(typeof(Terrain));
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateBombard(gameTime);
            UpdateAliens();
            UpdateLaserBeam(gameTime);
            CheckCollisions();
            base.Update(gameTime);
        }

        private void UpdateBombard(GameTime gameTime)
        {
            bombard.Update(gameTime);
            collisionBillboard.pos = bombard.collisionPosition[0];
            collisionBillboard = new CollisionBillboard(GraphicsDevice, Game.Content,
                      Game.Content.Load<Texture2D>(@"Textures\smoke"), new Vector2(50), bombard.collisionPosition);
        }

        private void UpdateLaserBeam(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                bool isLaserBeamNull = laserBeam == null;

                if (!isLaserBeamNull && !laserBeam.IsAlive)
                    RemoveLaserBeam();

                // Allow only one beam in the world at a time
                if (isLaserBeamNull || (!isLaserBeamNull && !laserBeam.IsAlive))
                {
                    CreateLaserBeam();
                }
            }
            else
            {
                if (laserBeam != null)
                    laserBeam.Update(gameTime);
            }
        }

        private void CreateLaserBeam()
        {
            Model laserBeamModel = Game.Content.Load<Model>(@"Models\Weapons\laserbeam");
            laserBeam = new LaserBeam(laserBeamModel, camera);
        }

        private void UpdateAliens()
        {
            for (int i = 0; i < aliens.Count; i++)
            {
                Alien alien = aliens[i];
                alien.Update(camera, terrain);
                //camera.alienPosition[i] = alien.Position;
            }
        }

        private void CheckCollisions()
        {
            CheckCameraCollisions();
            CheckLaserBeamCollisions();
        }

        private void CheckCameraCollisions()
        {
            for (int i = 0; i < aliens.Count; i++)
            {
                Alien alien = aliens[i];
                if (alien.Collides(camera.Position))
                {
                    // Push the camera back some if it hits an alien
                    camera.Position = camera.Position - 5.0f * camera.Direction;
                }
            }
        }

        private void CheckLaserBeamCollisions()
        {
            if (laserBeam == null)
                return;

            // See if laser beam collides with an enemy
            for (int i = 0; i < aliens.Count && laserBeam.IsAlive; i++)
            {
                Alien alien = aliens[i];

                if (laserBeam.Collides(alien))
                {
                    aliens.RemoveAt(i);
                    RemoveLaserBeam();
                }
            }
        }

        private void RemoveLaserBeam()
        {
            Console.WriteLine("Removing laser beam...");
            laserBeam.IsAlive = false;
        }

        public override void Draw(GameTime gameTime)
        {
            PrepareGraphicsDeviceForDrawing3D();
            laserGun.Draw(camera);

            DrawBombard();
            DrawAliens();

            if (laserBeam != null)
                laserBeam.Draw(camera);

            base.Draw(gameTime);
        }

        private void DrawBombard()
        {
            bombard.Draw();
            collisionBillboard.Draw(camera.View, camera.Projection, camera.Up, camera.Side);
            fauxAstroid.Draw(camera.View, camera.Projection, camera.Up, camera.Side);
        }

        private void DrawAliens()
        {
            foreach (Alien alien in aliens)
                alien.Draw(camera);
        }
        protected override bool LevelOver()
        {
            bool allAliensGone = aliens.Count == 0;
            return allAliensGone || IsPlayerOffWorld();
        }

        private bool IsPlayerOffWorld()
        {
            // Get the range of coordinates allowed in the world
            float maxX = terrain.MaxX;
            float minX = terrain.MinX;

            float maxZ = terrain.MaxZ;
            float minZ = terrain.MinZ;

            // Make sure the move won't place the camera outside the world
            bool beyondX = (camera.Position.X < minX || camera.Position.X > maxX);
            bool beyondZ = (camera.Position.Z < minZ || camera.Position.Z > maxZ);
            return beyondX || beyondZ;
        }

        private void spreadFaux()
        {
            Random random = (Random)Game.Services.GetService(typeof(Random));

            for (int i = 0; i < fauxPosition.Length; i++)
                fauxPosition[i] = new Vector3(
                    20 + (float)random.NextDouble() * 1000,
                    (float)random.NextDouble() * 300 + 100,
                   20 +(float)random.NextDouble()* 1000
                );
        }
    }
}
