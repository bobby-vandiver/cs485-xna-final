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
    public class PlanetCamera : Camera
    {
        const float POSITION_ABOVE_GROUND = 8.5f;
        
        Terrain terrain;

        public PlanetCamera(Game game, Vector3 direction, Vector3 up, Vector3 position, Terrain terrain)
            : base(game, direction, up, position)
        {
            this.terrain = terrain;
        }

        // This provides sub classes a hook to enforce restrictions on the camera's position
        protected override void RestrictPosition(Vector3 previousPosition)
        {
            // Get the range of coordinates allowed in the world
            float maxX = terrain.MaxX;
            float minX = terrain.MinX;

            float maxZ = terrain.MaxZ;
            float minZ = terrain.MinZ;

            float x = Position.X;
            float y = Position.Y;
            float z = Position.Z;

            // Make sure the move won't place the camera outside the world
            if (Position.X < minX || Position.X > maxX)
                x = previousPosition.X;

            if (Position.Z < minZ || Position.Z > maxZ)
                z = previousPosition.Z;

            // Ensure the camera moves along the ground
            float height = terrain.GetHeight(x, z) + POSITION_ABOVE_GROUND;
            y = height;

            Position = new Vector3(x, y, z);
        }

        protected override void RestrictPitchRotation(Vector3 previousDirection, Vector3 previousUp, float previousPitchAngle)
        {
            float totalPitch = MathHelper.PiOver4 / 2;

            // We don't want the up vector to ever be altered
            Up = previousUp;

            // Enforce min/max pitch rotation
            if (Math.Abs(Pitch) >= totalPitch)
            {
                // Reset pitch and axes to previously known valid states
                Direction = previousDirection;
                Pitch = previousPitchAngle;
            }

            return;
        }
    }
}
