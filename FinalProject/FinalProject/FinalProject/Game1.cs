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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Random randomNumberGenerator;

        public enum GameState { Start, Play, End }
        GameState currentGameState;

        // Each level will use this to communicate its state so the Game object can manage transitions
        public enum LevelState { Start, Play, End }
        public LevelState CurrentLevelState;

        const int LEVEL_COUNT = 3;
        int currentLevel = 0;
        Level level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            currentGameState = GameState.Start;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            font = Content.Load<SpriteFont>(@"Fonts\GameFont");
            randomNumberGenerator = new Random();

            // The font and the random number generator need to be accessible from every where
            Services.AddService(typeof(SpriteFont), font);
            Services.AddService(typeof(Random), randomNumberGenerator);
        }

        protected override void UnloadContent()
        {
            Services.RemoveService(typeof(SpriteFont));
            Services.RemoveService(typeof(Random));
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateGameState();

            base.Update(gameTime);
        }

        private void UpdateGameState()
        {
            switch (currentGameState)
            {
                case GameState.Start:
                    // Wait until the player presses "Enter" to start the first level
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Play;
                        currentLevel = 0;
                        CurrentLevelState = LevelState.Start;
                    }
                    break;

                case GameState.Play:
                    UpdateLevelState();
                    break;
                
                case GameState.End:
                    break;
                
                default:
                    throw new NotImplementedException("Invalid game state!");
            }
        }

        private void UpdateLevelState()
        {
            switch (CurrentLevelState)
            {
                case LevelState.Start:
                    // Load the level component
                    LoadLevel();
                    CurrentLevelState = LevelState.Play;
                    break;
                
                case LevelState.Play:
                    //TODO: Add pause support -- maybe
                    break;
                
                case LevelState.End:
                    // Remove the level component
                    Components.Remove(level);

                    // Progress to the next level
                    currentLevel++;
                    
                    // See if we've reached the end of the game
                    if (currentLevel >= LEVEL_COUNT)
                        currentGameState = GameState.End;
                    
                    break;

                default:
                    throw new NotImplementedException("Invalid level state!");
            }
        }

        private void LoadLevel()
        {
            // Not the best way to do this, but for now it works...
            switch (currentLevel)
            {
                case 0:
                    level = new PlanetLevel(this);
                    break;
                default:
                    throw new NotImplementedException("Requested level: [" + currentLevel + "] does not exist!");
            }

            Components.Add(level);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            string message;
            Vector2 position;

            switch (currentGameState)
            {
                case GameState.Start:
                    message = "Press enter to start.";
                    position = CalculateTextCenterPosition(message);
                    DrawString(message, position);
                    break;

                case GameState.Play:
                    break;

                case GameState.End:
                    message = "You have reached the end.";
                    position = CalculateTextCenterPosition(message);
                    DrawString(message, position);
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawString(string text, Vector2 position)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, Color.White);
            spriteBatch.End();
        }

        private Vector2 CalculateTextCenterPosition(string text)
        {
            Vector2 textDimensions = font.MeasureString(text);

            int windowHeight = Window.ClientBounds.Height;
            int windowWidth = Window.ClientBounds.Width;

            // Center the message
            Vector2 position = new Vector2(
                (windowWidth / 2) - (textDimensions.X / 2),
                (windowHeight / 2) - (textDimensions.Y / 2));

            return position;
        }
    }
}
