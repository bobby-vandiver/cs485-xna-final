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

        #region Video Variables
        Video video;
        VideoPlayer videoPlayer;
        
        bool videoPlaying;
        const int VIDEO_LENGTH_SECS = 25;
        #endregion

        #region Services
        SpriteFont font;
        Random randomNumberGenerator;
        Audio audio;
        #endregion

        #region Game State Variables
        public enum GameState { Start, Play, End }
        GameState currentGameState;
        #endregion

        #region Level State Variables
        // Each level will use this to communicate its state so the Game object can manage transitions
        public enum LevelState { Instructions, Start, Play, End }
        public LevelState CurrentLevelState;
        
        const int LEVEL_COUNT = 2;
        int currentLevel = 0;
        Level level;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1600;
            //graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
            currentGameState = GameState.Start;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load resources to play the intro video
            LoadVideoResources();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            font = Content.Load<SpriteFont>(@"Fonts\GameFont");
            randomNumberGenerator = new Random();
            audio = new Audio(this);
            
            // The font, the random number generator and audio need to be accessible from every where
            Services.AddService(typeof(SpriteFont), font);
            Services.AddService(typeof(Random), randomNumberGenerator);
            Services.AddService(typeof(Audio), audio);
        }

        private void LoadVideoResources()
        {
            videoPlayer = new VideoPlayer();
            videoPlayer.Volume = 0;
            video = Content.Load<Video>(@"Videos\Intro");
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
                        if (!videoPlaying)
                        {
                            audio.PlayCue("stateTransition");
                            audio.PlayBackgroundMusic("DigitalStream");

                            videoPlayer.Play(video);
                            videoPlaying = true;
                        }
                    }

                    UpdateVideo();
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

        private void UpdateVideo()
        {
            // Pressing space bar will skip the movie
            bool spaceBarDown = Keyboard.GetState().IsKeyDown(Keys.Space);
            bool endOfVideoReached = videoPlayer.PlayPosition.Seconds == VIDEO_LENGTH_SECS;

            if ((videoPlaying && spaceBarDown) || (videoPlaying && endOfVideoReached))
                StopVideo();
        }

        private void StopVideo()
        {
            videoPlayer.Stop();
            videoPlaying = false;

            audio.StopBackgroundMusic();

            // Start the game when the video is over
            currentGameState = GameState.Play;
            currentLevel = 0;
            CurrentLevelState = LevelState.Instructions;
        }

        private void UpdateLevelState()
        {
            switch (CurrentLevelState)
            {
                case LevelState.Instructions:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        CurrentLevelState = LevelState.Start;
                    break;

                case LevelState.Start:
                    audio.PlayCue("stateTransition");

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
                    CurrentLevelState = LevelState.Instructions;
                    
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
                    level = new SpaceLevel(this);
                    break;
                case 1:
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
                    DrawLevelMessages();
                    break;

                case GameState.End:
                    message = "You have reached the end.";
                    position = CalculateTextCenterPosition(message);
                    DrawString(message, position);
                    break;
            }

            DrawVideoFrame();

            base.Draw(gameTime);
        }

        private void DrawLevelMessages()
        {
            string[] instructions =
                {
                    "Dodge or destroy the asteroids!",
                    "Kill all the aliens and dodge the falling asteroids!"
                };

            if (CurrentLevelState == LevelState.Instructions)
            {
                string message = instructions[currentLevel];
                Vector2 position = CalculateTextCenterPosition(message);
                DrawString(message, position);
            }
        }

        private void StartingMessage()
        {
                string text = "Mission Starting...";
                Vector2 position = CalculateTextCenterPosition(text);
                spriteBatch.Begin();
                spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y + 50), Color.White);
                spriteBatch.End();
        }

        private void DrawVideoFrame()
        {
            spriteBatch.Begin();

            if (videoPlayer.State != MediaState.Stopped)
            {
                Texture2D videoTexture = videoPlayer.GetTexture();
                Rectangle videoRectangle = GetScreenDimensionsForVideo();
                if (videoTexture != null)
                {
                    spriteBatch.Draw(videoTexture, videoRectangle, Color.White);
                }
            }
            
            spriteBatch.End();
        }

        private Rectangle GetScreenDimensionsForVideo()
        {
            return new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
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
