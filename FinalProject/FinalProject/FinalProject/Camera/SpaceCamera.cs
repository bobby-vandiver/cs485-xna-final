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
    public class SpaceCamera : Camera
    {
        const float POSITION_ABOVE_GROUND = 8.5f;
        float movementSpeed = 1f;
        public Boolean rollingLeft = false;
        public Boolean rollingRight = false;
        public SpaceCamera(Game game, Vector3 direction, Vector3 up, Vector3 position)
            : base(game, direction, up, position)
        {
 
        }
        protected override void UpdatePositionFromKeyboard()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // In case the movement cannot be performed, we can roll back
            Vector3 previousPosition = Position;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Position += (Direction * movementSpeed);
                RestrictPosition(previousPosition);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Position -= (Direction * movementSpeed);
                RestrictPosition(previousPosition);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Position -= (Side * movementSpeed);
               
                RestrictPosition(previousPosition);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Position += (Side * movementSpeed);
                
                RestrictPosition(previousPosition);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                
                ApplyRollRotation(-(movementSpeed * (.1f)));
                Position -= (Side * movementSpeed);
                rollingLeft = true;
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                ApplyRollRotation((movementSpeed * (.1f)));
                Position += (Side * movementSpeed);
                rollingRight = true;
            }
            
            
        }
        // This provides sub classes a hook to enforce restrictions on the camera's position
        protected override void RestrictPosition(Vector3 previousPosition)
        {
            float x = Position.X;
            float y = Position.Y;
            float z = Position.Z;

            // Make sure the ship cant leave the asteroid field (thats part of the game)
            if (Position.X < -15 || Position.X > 15)
                x = previousPosition.X;
            if (Position.Z < 0 || Position.Z > 15)
                z = previousPosition.Z;
            if (Position.Y < 35 || Position.Y > 45)
                y = previousPosition.Y;

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
