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

        public PlanetLevel(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            InitializeCamera();
            base.Initialize();
        }

        private void InitializeCamera()
        {
            Terrain terrain = InitializeTerrain();

            Vector3 position = new Vector3(0, 40, 5);
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

        protected override void UnloadContent()
        {
            Game.Components.Remove(camera);
            Game.Components.Remove(terrain);
            Game.Services.RemoveService(typeof(Camera));
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        protected override bool LevelOver()
        {
            return false;
        }
    }
}
