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
        Camera camera;

        Terrain terrain;
        Skybox skybox;

        LaserGun laserGun;
        LaserBeam laserBeam;

        // Randomly generate the number of aliens
        const int MIN_ALIEN_COUNT = 1;
        //const int MAX_ALIEN_COUNT = 6;
        const int MAX_ALIEN_COUNT = 1;

        List<Alien> aliens;

        public PlanetLevel(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            InitializeCamera();
            // The gun model needs to be rendered after everything so it appears 'on top'
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
            return terrain;
        }

        protected override void LoadContent()
        {
            LoadLaserGun();
            LoadSkybox();
            LoadAliens();
            base.LoadContent();
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
                //Vector3 position = GetUniqueRandomPointInWorld(randomNumberGenerator);

                Vector3 position = camera.Position - new Vector3(0, 0, 30.0f);
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
            } while(!unique);

            return randomPosition;
        }

        protected override void UnloadResources()
         {
            Game.Components.Remove(camera);
            Game.Components.Remove(terrain);
            Game.Components.Remove(skybox);
            Game.Services.RemoveService(typeof(Camera));
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateAliens();
            UpdateLaserBeam();
            CheckCollisions();
            base.Update(gameTime);
        }

        private void UpdateLaserBeam()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                bool isLaserBeamNull = laserBeam == null;
                bool isComponentRemoved = !Game.Components.Contains(laserBeam);

                // Allow only one beam in the world at a time
                if (isLaserBeamNull || (!isLaserBeamNull && isComponentRemoved))
                {
                    CreateLaserBeam();
                }
            }
        }

        private void CreateLaserBeam()
        {
            laserBeam = new LaserBeam(Game, camera);
            laserBeam.DrawOrder = 4;
            Game.Components.Add(laserBeam);
        }

        private void UpdateAliens()
        {
            for (int i = 0; i < aliens.Count; i++)
            {
                Alien alien = aliens[i];
                alien.Update(camera, terrain);
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
            // See if laser beam collides with an enemy
            for (int i = 0; i < aliens.Count && laserBeam != null; i++)
            {
                LaserBeamModel laserBeamModel = laserBeam.LaserBeamModel;
                Alien alien = aliens[i];

                if (laserBeamModel.Collides(alien.Model))
                {
                    aliens.RemoveAt(i);
                    Game.Components.Remove(laserBeam);
                    laserBeam = null;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            PrepareGraphicsDeviceForDrawing3D();
            laserGun.Draw(camera);

            foreach (Alien alien in aliens)
                alien.Draw(camera);

            base.Draw(gameTime);
        }
        protected override bool LevelOver()
        {
            return aliens.Count == 0;
        }

    }
}
