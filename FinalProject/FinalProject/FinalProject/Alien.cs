using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class Alien : BasicModel
    {
        // Necessary kludge to ensure placement on the terrain is believable
        const float POSITION_ABOVE_GROUND = 3.5f;
        
        public Vector3 Position;

        // Always access via the property to ensure unit length
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

        float movementSpeed = 0.1f;

        // The aliens will waddle across the screen
        // The model used has a single bone, thus making programmatic animation impossible
        const float MAX_ROLL_ANGLE = MathHelper.PiOver4 / 12;

        float rollRate = MathHelper.PiOver4 / 100;
        float rollAngle = 0.0f;

        // Offset the terrain boundaries to make "hitting the wall" more natural
        const float MODEL_OFFSET = 5.0f;

        public Alien(Model model, Vector3 position, Vector3 direction)
            : base(model)
        {
            this.Position = position;
            this.Position.Y += POSITION_ABOVE_GROUND;
            this.Direction = direction;
        }

        public void Update(Camera camera, Terrain terrain)
        {
            UpdateRollAngle();
            UpdatePosition(camera, terrain);
        }

        private void UpdateRollAngle()
        {
            rollAngle += rollRate;

            // Do the pendulum dance!
            if (Math.Abs(rollAngle) > MAX_ROLL_ANGLE)
                rollRate *= -1;
        }

        private void UpdatePosition(Camera camera, Terrain terrain)
        {
            // Move in a straight line along the direction the alien is facing
            Position += Direction * movementSpeed;

            // Keep the alien moving on the terrain
            RestrictPositionToTerrainBoundaries(terrain);

            // Ensure the alien still appears on the terrain
            Position.Y = terrain.GetHeight(Position.X, Position.Z) + POSITION_ABOVE_GROUND;
        }

        private void RestrictPositionToTerrainBoundaries(Terrain terrain)
        {
            float maxX = terrain.MaxX - MODEL_OFFSET;
            float minX = terrain.MinX + MODEL_OFFSET;

            float maxZ = terrain.MaxZ - MODEL_OFFSET;
            float minZ = terrain.MinZ + MODEL_OFFSET;

            // Change direction once we hit the edge of the map
            if (Position.X < minX || Position.X > maxX)
                direction.X *= -1;

            if (Position.Z < minZ || Position.Z > maxZ)
                direction.Z *= -1;

        }

        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.1f);
            Matrix rotation = Matrix.CreateRotationZ(rollAngle);
            Matrix translation = Matrix.CreateTranslation(Position);
            return meshTransform * scale * rotation * translation;
        }
    }
}
