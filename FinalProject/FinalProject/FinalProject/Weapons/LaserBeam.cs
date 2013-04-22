using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class LaserBeam : BasicModel
    {
        // Position will refer to the "head" of the beam
        Vector3 Position;
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

        public bool IsAlive = false;

        // Use this to determine when to have the beam remove itself
        float maxDistance;

        // The laser beam will move at a constant, configurable speed
        float movementSpeed;

        // Default settings
        const float DEFAULT_MAX_DISTANCE = 15.0f;
        const float DEFAULT_MOVEMENT_SPEED = 0.1f;

        const float BOUNDING_SPHERE_RADIUS = 20.0f;

        public LaserBeam(Model model, Camera camera, float maxDistance = DEFAULT_MAX_DISTANCE,
            float movementSpeed = DEFAULT_MOVEMENT_SPEED)
            : base(model)
        {
            this.Direction = camera.Direction;
            this.Position = this.initialPosition = camera.Position + new Vector3(0, -.05f, 0) + 0.42f * Direction;
            this.maxDistance = maxDistance;
            this.movementSpeed = movementSpeed;
            this.IsAlive = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsAlive)
                UpdatePosition();

            base.Update(gameTime);
        }

        private void UpdatePosition()
        {
            Position += Direction * movementSpeed;
            CheckDistanceTraveled();

            // TODO:
            //CheckTerrainCollision();
        }
        private void CheckDistanceTraveled()
        {
            float distanceTraveled = Vector3.Distance(initialPosition, Position);
            if (distanceTraveled > maxDistance)
            {
                Console.WriteLine("Max distance traveled...");
                IsAlive = false;
            }
        }

        // TODO: Improve!!!
        //private void CheckTerrainCollision()
        //{
        //    Terrain terrain = (Terrain)Game.Services.GetService(typeof(Terrain));
        //    float minHeightAllowed = terrain.GetHeight(Position.X, Position.Z);
        //    if (Position.Y <= minHeightAllowed)
        //    {
        //        Console.WriteLine("Collided with the ground...");
        //        IsAlive = false;
        //    }
        //}

        public override void Draw(Camera camera)
        {
            if (IsAlive)
            {
                base.Draw(camera);
            }
        }

        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Position, BOUNDING_SPHERE_RADIUS);
        }

        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.01f);
            Matrix translation = Matrix.CreateTranslation(Position);
            return scale * translation;
        }
    }
}
