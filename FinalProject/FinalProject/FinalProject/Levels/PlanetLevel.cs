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
        LaserGun laserGun;
        Effect explosionEffect;

        CollisionBillboard collisionBillboard;
        PlayerHealth playerHealth = new PlayerHealth();
        FauxAstroid fauxAstroid;
        public Vector3[] fauxPosition = new Vector3[30];
        Random randomNumber = new Random();

        Skybox skybox;
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
            Model laserGunModel = Game.Content.Load<Model>(@"Models\Weapons\lasergun");
            laserGun = new LaserGun(laserGunModel);
                    
            Texture2D bombTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\shader");
            Texture2D smokeTexture = Game.Content.Load<Texture2D>(@"Textures\smoke");

            Model BombardmentModel = Game.Content.Load<Model>(@"Models\ammo");
            bombard = new Bombard(BombardmentModel, terrain, bombTexture, camera, milliseconds, explosionEffect,smokeTexture);

            collisionBillboard = new CollisionBillboard(GraphicsDevice, Game.Content,
                       Game.Content.Load<Texture2D>(@"Textures\smoke"), new Vector2(50), bombard.collisionPosition);

            fauxAstroid = new FauxAstroid(GraphicsDevice, Game.Content,
                      Game.Content.Load<Texture2D>(@"Textures\fauxAstroid"), new Vector2(10), fauxPosition);



            skybox = new Skybox(Game, @"Backgrounds\Sunset", 500f);
            skybox.DrawOrder = 0;
            Game.Components.Add(skybox);
            base.LoadContent();
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
            bombard.Update(gameTime);
            collisionBillboard.pos = bombard.collisionPosition[0];
            collisionBillboard = new CollisionBillboard(GraphicsDevice, Game.Content,
                      Game.Content.Load<Texture2D>(@"Textures\smoke"), new Vector2(50), bombard.collisionPosition);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            PrepareGraphicsDeviceForDrawing3D();
            laserGun.Draw(camera);
            bombard.Draw();
            collisionBillboard.Draw(camera.View,camera.Projection,camera.Up,camera.Side);
            fauxAstroid.Draw(camera.View, camera.Projection, camera.Up, camera.Side);

            base.Draw(gameTime);
        }
        protected override bool LevelOver()
        {
            return false;
        }

        private void spreadFaux()
        {
            for (int i = 0; i < fauxPosition.Length; i++)
                fauxPosition[i] = new Vector3(
                    50 + (float)randomNumber.NextDouble() * 1000,
                    (float)randomNumber.NextDouble() * 300 + 100,
                   50 +(float)randomNumber.NextDouble()* 1000
                );
        }
    }
}
