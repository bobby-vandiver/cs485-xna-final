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
    public class Camera : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        Vector3 direction;
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            protected set
            {
                direction = value;
                direction.Normalize();
            }
        }

        Vector3 up;
        public Vector3 Up
        {
            get
            {
                return up;
            }
            protected set
            {
                up = value;
                up.Normalize();
            }
        }

        Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            protected set
            {
                position = value;
            }
        }

        public Vector3 Target
        {
            get
            {
                return Position + Direction;
            }
        }

        public Vector3 Side
        {
            get
            {
                Vector3 side = Vector3.Cross(direction, up);
                side.Normalize();
                return side;
            }
        }

        float nearPlane = 0.001f;
        float farPlane = 1000f;

        float fieldOfView = MathHelper.PiOver4;

        float movementSpeed = 0.5f;

        const float ROTATION_RATE = MathHelper.PiOver4 / 125;

        
        //const float POSITION_ABOVE_GROUND = 8.5f;

        float currentPitch = 0;
        float currentYaw = 0;
        float currentRoll = 0;

        // Sub classes can access the current yaw/pitch/roll via properties
        protected float Pitch
        {
            get { return currentPitch; }
            set { currentPitch = value; }
        }

        protected float Yaw
        {
            get { return currentYaw; }
            set { currentYaw = value; }
        }

        protected float Roll
        {
            get { return currentRoll; }
            set { currentRoll = value; }
        }
        
        public Camera(Game game, Vector3 direction, Vector3 up, Vector3 position)
            : base(game)
        {
            // Use the property setters to ensure vectors are unit length
            this.Direction = direction;
            this.Up = up;
            this.Position = position;
        }

        public override void Initialize()
        {
            UpdateView();
            CreateProjection(fieldOfView);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Need this for displaying debug messages
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePositionFromKeyboard();
            UpdateDirectionFromKeyboard();
            UpdateView();

            base.Update(gameTime);
        }

        private void UpdatePositionFromKeyboard()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // In case the movement cannot be performed, we can roll back
            Vector3 previousPosition = Position;

            if (keyboardState.IsKeyDown(Keys.W))
                Position += (Direction * movementSpeed);
            if (keyboardState.IsKeyDown(Keys.S))
                Position -= (Direction * movementSpeed);
            if (keyboardState.IsKeyDown(Keys.A))
                Position -= (Side * movementSpeed);
            if (keyboardState.IsKeyDown(Keys.D))
                Position += (Side * movementSpeed);

            RestrictPosition(previousPosition);
        }

        // This provides sub classes a hook to enforce restrictions on the camera's position
        protected virtual void RestrictPosition(Vector3 previousPosition)
        {
            return;
        }

        //private void RestrictPositionToTerrain(Vector3 previousPosition)
        //{
        //    Terrain terrain = ((Game1)Game).Terrain;

        //    float maxX = terrain.LimitX;
        //    float minX = -maxX;

        //    float maxZ = terrain.LimitZ;
        //    float minZ = -maxZ;

        //    // Make sure the move won't place the camera outside the world
        //    if (position.X < minX || position.X > maxX)
        //        position.X = previousPosition.X;

        //    if (position.Z < minZ || position.Z > maxZ)
        //        position.Z = previousPosition.Z;

        //    // Ensure the camera moves along the ground
        //    float height = terrain.GetHeight(position.X, position.Z) + POSITION_ABOVE_GROUND;
        //    position.Y = height;
        //}

        private void UpdateDirectionFromKeyboard()
        {
            UpdateYaw();
            UpdatePitch();
            UpdateRoll();
        }

        protected virtual void UpdateYaw()
        {
            float yawAngle = CalculateYawRotationAngleFromKeyboard();
            ApplyYawRotation(yawAngle);
        }

        protected virtual void UpdatePitch()
        {
            float pitchAngle = CalculatePitchRotationAngleFromKeyboard();
            ApplyPitchRotation(pitchAngle);
        }

        protected virtual void UpdateRoll()
        {
            return;
        }

        private float CalculateYawRotationAngleFromKeyboard()
        {
            float yawAngle = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                yawAngle = ROTATION_RATE;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                yawAngle = -ROTATION_RATE;

            return yawAngle;
        }

        private float CalculatePitchRotationAngleFromKeyboard()
        {
            float pitchAngle = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                pitchAngle = -ROTATION_RATE;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                pitchAngle = ROTATION_RATE;

            return pitchAngle;
        }

        private void ApplyYawRotation(float yawAngle)
        {
            Matrix yawMatrix = Matrix.CreateFromAxisAngle(Up, yawAngle);
            Direction = Vector3.Transform(Direction, yawMatrix);
            currentYaw += yawAngle;
        }

        // Hook for subclasses to enforce yaw restrictions
        protected virtual void RestrictYawRotation(Vector3 previousDirection)
        {
            return;
        }

        private void ApplyPitchRotation(float pitchAngle)
        {
            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(Side, pitchAngle);
            Direction = Vector3.Transform(Direction, pitchMatrix);
            Up = Vector3.Transform(Up, pitchMatrix);            
            currentPitch += pitchAngle;
        }

        // Hook for subclasses to enforce pitch restrictions
        protected virtual void RestrictPitchRotation(Vector3 previousDirection, Vector3 previousUp)
        {
            return;
        }

        private void ApplyRollRotation(float rollAngle)
        {
            Matrix rollMatrix = Matrix.CreateFromAxisAngle(Direction, rollAngle);
            Up = Vector3.Transform(Up, rollMatrix);
            currentRoll += rollAngle;
        }

        // Hook for subclasses to enforce roll restrictions
        protected virtual void RestrictRollRotation(Vector3 previousUp)
        {
            return;
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteFont font = ((Game1)Game).Font;

            spriteBatch.Begin();

            var message = GenerateDebugMessage();
            spriteBatch.DrawString(font, message, new Vector2(10, 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private string GenerateDebugMessage()
        {
            string positionMessage = "Position: " + Position;
            string directionMessage = "Direction: " + Direction;
            string targetMessage = "Target: " + Target;
            string upMessage = "Up: " + Up;
            string sideMessage = "Side: " + Side;

            return positionMessage + "\n" + directionMessage + "\n" + targetMessage + "\n" + upMessage + "\n" + sideMessage;
        }

        private void CreateProjection(float fieldOfView)
        {
            float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }

        private void UpdateView()
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
        }
    }
}
