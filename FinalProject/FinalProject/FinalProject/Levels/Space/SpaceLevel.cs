﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FinalProject
{
    public class SpaceLevel : Level
    {
        Camera camera;
        int time;
        Spaceship Spaceship;
        SpriteBatch spriteBatch;
        AsteroidField field;
        Texture2D crosshair;
        SpriteFont font;
        int score;
        HUD hud;
        public bool levelOverForVideo;


        public SpaceLevel(Game game)
            : base(game)
        {
        }
        private void InitializeHud()
        {
            hud = new HUD(Game, false);
            hud.DrawOrder = 4;
            Game.Components.Add(hud);
        }
        public override void Initialize()
        {
            InitializeHud();
            InitializeCamera();
            score = 0;
            this.DrawOrder = 3;
            base.Initialize();
        }
        private AsteroidField InitializeField()
        {
            field = new AsteroidField(Game, camera, 100.0f,hud);
            field.DrawOrder = 1;
            Game.Components.Add(field);
            return field;
        }
        private void InitializeCamera()
        {
            
            Vector3 position = new Vector3(0, 40, 5);
            Vector3 direction = new Vector3(0, 0, -1);
            camera = new SpaceCamera(Game, direction, Vector3.Up, position);
            camera.DrawOrder = 2;

            Game.Components.Add(camera);
            Game.Services.AddService(typeof(Camera), camera);
            AsteroidField field = InitializeField();
        }

        protected override void LoadContent()
        {
        
            spriteBatch = new SpriteBatch(GraphicsDevice);
            crosshair = Game.Content.Load<Texture2D>(@"Textures\Crosshair");
            font = Game.Content.Load<SpriteFont>(@"Fonts\GameFont");
            base.LoadContent();
        }

        protected override void UnloadResources()
        {
            Game.Components.Remove(field);
            Game.Components.Remove(camera);
            Game.Components.Remove(hud);
            Game.Services.RemoveService(typeof(HUD));
            Game.Services.RemoveService(typeof(Camera));
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
                hud = field.GetHud(); 
                base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            PrepareGraphicsDeviceForDrawing3D();
            base.Draw(gameTime);
            spriteBatch.Begin();

            spriteBatch.Draw(crosshair,
                new Vector2((Game.Window.ClientBounds.Width / 2)
                    - (crosshair.Width / 2),
                    (Game.Window.ClientBounds.Height / 2)
                    - (crosshair.Height / 2)),
                    Color.Red);
            
            spriteBatch.End();
        }

        protected override bool LevelOver()
        {

            
            return field.game_over;

        }

    }
}
