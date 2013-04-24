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
        const float DEFAULT_MAX_DISTANCE = 45.0f;
        const float DEFAULT_MOVEMENT_SPEED = 1.0f;

        const float BOUNDING_SPHERE_RADIUS = 20.0f;

        public LaserBeam(Model model, Camera camera, float maxDistance = DEFAULT_MAX_DISTANCE,
            float movementSpeed = DEFAULT_MOVEMENT_SPEED)
            : base(model)
        {
            Matrix directionRotation = Matrix.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, camera.Roll);
            this.Direction = Vector3.Transform(-Vector3.UnitZ, directionRotation);

            this.initialPosition = camera.Position + new Vector3(0, -.05f, 0) + 0.45f * Direction;
            this.Position = this.initialPosition;

            this.maxDistance = maxDistance;
            this.movementSpeed = movementSpeed;
            
            this.IsAlive = true;
        }

        public void Update(GameTime gameTime, Terrain terrain)
        {
            if (IsAlive)
                UpdatePosition(terrain);

            base.Update(gameTime);
        }

        private void UpdatePosition(Terrain terrain)
        {
            Position += Direction * movementSpeed;
            CheckDistanceTraveled();
            CheckTerrainCollision(terrain);
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

        private void CheckTerrainCollision(Terrain terrain)
        {
            float minHeightAllowed = terrain.GetHeight(Position.X, Position.Z);
            if (Position.Y <= minHeightAllowed)
            {
                Console.WriteLine("Collided with the ground...");
                IsAlive = false;
            }
        }

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
            Matrix scale = Matrix.CreateScale(0.05f);
            Matrix translation = Matrix.CreateTranslation(Position);
            return scale * translation;
        }
    }
}
