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

        public PlanetCamera(Game game, Vector3 direction, Vector3 up, Vector3 position)
            : base(game, direction, up, position)
        {
            // TODO: Construct any child components here
        }

        // This provides sub classes a hook to enforce restrictions on the camera's position
        protected override void RestrictPosition(Vector3 previousPosition, Terrain terrain)
        {
            //Terrain terrain = ((Game1)Game).Terrain;

            float maxX = terrain.LimitX;
            float minX = -maxX;

            float maxZ = terrain.LimitZ;
            float minZ = -maxZ;

            float newX = Position.X;
            float newY = Position.Y;
            float newZ = Position.Z;

            // Make sure the move won't place the camera outside the world
            if (Position.X < minX || Position.X > maxX)
                newX = previousPosition.X;

            if (Position.Z < minZ || Position.Z > maxZ)
                newZ = previousPosition.Z;

            // Ensure the camera moves along the ground
            float height = terrain.GetHeight(newX, newZ) + POSITION_ABOVE_GROUND;
            newY = height;

            Position = new Vector3(newX, newY, newZ);
        }

    }
}
