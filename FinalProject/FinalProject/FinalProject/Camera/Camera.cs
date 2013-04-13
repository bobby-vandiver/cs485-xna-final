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
        
        // Access these via their respective properties to ensure normalization
        Vector3 direction;
        Vector3 up;
        Vector3 position;

        #region Camera Axes and Matrices Properties
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

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
        #endregion

        float nearPlane = 0.001f;
        float farPlane = 1000f;

        const float DEFAULT_FIELD_OF_VIEW = MathHelper.PiOver4;
        float fieldOfView;

        float movementSpeed = 0.5f;

        const float DEFAULT_ROTATION_RATE = MathHelper.PiOver4 / 125;

        // Subclasses can specify different rotation rates for each axis if needed
        #region Rotation Rates
        protected float YawRotationRate;
        protected float PitchRotationRate;
        protected float RollRotationRate;
        #endregion

        float currentPitch = 0;
        float currentYaw = 0;
        float currentRoll = 0;

        // Subclasses can access the current yaw/pitch/roll via properties
        #region Current Pitch/Yaw/Roll properties
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
        #endregion

        public Camera(Game game, Vector3 direction, Vector3 up, Vector3 position, float fieldOfView = DEFAULT_FIELD_OF_VIEW)
            : base(game)
        {
            // Use the property setters to ensure vectors are unit length
            this.Direction = direction;
            this.Up = up;
            this.Position = position;

            this.fieldOfView = fieldOfView;

            // Use the default rotation rate
            this.YawRotationRate = DEFAULT_ROTATION_RATE;
            this.PitchRotationRate = DEFAULT_ROTATION_RATE;
            this.RollRotationRate = DEFAULT_ROTATION_RATE;
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

        private void UpdateDirectionFromKeyboard()
        {
            UpdateYaw();
            UpdatePitch();
            UpdateRoll();
        }

        protected virtual void UpdateYaw()
        {
            float previousYawAngle = currentYaw;
            Vector3 previousDirection = Direction;
            
            float yawAngle = CalculateYawRotationAngleFromKeyboard();
            ApplyYawRotation(yawAngle);

            RestrictYawRotation(previousDirection, previousYawAngle);
        }

        protected virtual void UpdatePitch()
        {
            float previousPitchAngle = currentPitch;

            Vector3 previousDirection = Direction;
            Vector3 previousUp = Up;

            float pitchAngle = CalculatePitchRotationAngleFromKeyboard();
            ApplyPitchRotation(pitchAngle);
            
            RestrictPitchRotation(previousDirection, previousUp, previousPitchAngle);
        }

        protected virtual void UpdateRoll()
        {
            Vector3 previousUp = Up;
            RestrictRollRotation(previousUp, 0);
        }

        private float CalculateYawRotationAngleFromKeyboard()
        {
            float yawAngle = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                yawAngle = YawRotationRate;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                yawAngle = -YawRotationRate;

            return yawAngle;
        }

        private float CalculatePitchRotationAngleFromKeyboard()
        {
            float pitchAngle = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                pitchAngle = -PitchRotationRate;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                pitchAngle = PitchRotationRate;

            return pitchAngle;
        }

        private void ApplyYawRotation(float yawAngle)
        {
            Matrix yawMatrix = Matrix.CreateFromAxisAngle(Up, yawAngle);
            Direction = Vector3.Transform(Direction, yawMatrix);
            currentYaw += yawAngle;
        }

        // Hook for subclasses to enforce yaw restrictions
        protected virtual void RestrictYawRotation(Vector3 previousDirection, float previousYawAngle)
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
        protected virtual void RestrictPitchRotation(Vector3 previousDirection, Vector3 previousUp, float previousPitchAngle)
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
        protected virtual void RestrictRollRotation(Vector3 previousUp, float previousRollAngle)
        {
            return;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = (SpriteFont)Game.Services.GetService(typeof(SpriteFont));

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
