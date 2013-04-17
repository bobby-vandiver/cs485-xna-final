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

    public class LaserBeam : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Position will refer to the "head" of the beam
        public Vector3 Position;
        Vector3 initialPosition;

        // Direction of motion
        Vector3 direction;
        Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                direction.Normalize();
            }
        }

        // Use this to determine when to have the beam remove itself
        float maxDistance;

        // The laser beam will move at a constant, configurable speed
        float movementSpeed;

        // Default settings
        const float DEFAULT_MAX_DISTANCE = 15.0f;
        const float DEFAULT_MOVEMENT_SPEED = 0.1f;

        LaserBeamModel laserBeamModel;

        public LaserBeam(Game game, Camera camera, float maxDistance = DEFAULT_MAX_DISTANCE,
            float movementSpeed = DEFAULT_MOVEMENT_SPEED)
            : base(game)
        {
            this.Direction = camera.Direction;
            this.Position = this.initialPosition = camera.Position + 0.42f * Direction;
            this.maxDistance = maxDistance;
            this.movementSpeed = movementSpeed;
        }

        protected override void LoadContent()
        {
            Model model = Game.Content.Load<Model>(@"Models\Weapons\ammo");
            laserBeamModel = new LaserBeamModel(model, initialPosition);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePosition();
            base.Update(gameTime);
        }

        private void UpdatePosition()
        {
            Position += Direction * movementSpeed;
            laserBeamModel.Position = Position;

            float distanceTraveled = Vector3.Distance(initialPosition, Position);
            if (distanceTraveled > maxDistance)
            {
                Game.Components.Remove(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = (Camera)Game.Services.GetService(typeof(Camera));
            laserBeamModel.Draw(camera);
            base.Draw(gameTime);
        }
    }

    class LaserBeamModel : BasicModel
    {
        public Vector3 Position;

        public LaserBeamModel(Model model, Vector3 position)
            : base(model)
        {
            this.Position = position;
        }

        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.01f);
            Matrix translation = Matrix.CreateTranslation(Position + new Vector3(0, -.05f, 0));
            return meshTransform * scale * translation;

        }
    }
}
