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
    // Tweaked version of the Skybox class from: http://rbwhitaker.wikidot.com/skyboxes-1
    public class Skybox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // The model to render the texture on
        Model cube;
        
        // The skybox texture to use
        string pathToTexture;
        TextureCube texture;

        // The effect for rendering
        Effect effect;

        // Allow the cube to be resized
        float cubeSize;
        const float DEFAULT_SIZE = 50f;

        public Skybox(Game game, string pathToTexture, float cubeSize = DEFAULT_SIZE)
            : base(game)
        {
            this.pathToTexture = pathToTexture;
            this.cubeSize = cubeSize;
        }

        protected override void LoadContent()
        {
            cube = Game.Content.Load<Model>(@"Models\cube");
            texture = Game.Content.Load<TextureCube>(pathToTexture);
            effect = Game.Content.Load<Effect>(@"Effects\Skybox");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = (Camera)Game.Services.GetService(typeof(Camera));

            Matrix view = camera.View;
            Matrix projection = camera.Projection;
            Vector3 position = camera.Position;

            GraphicsDevice.Clear(Color.Black);

            RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in cube.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = effect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(cubeSize) * Matrix.CreateTranslation(position));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(texture);
                        part.Effect.Parameters["CameraPosition"].SetValue(position);
                    }

                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }
            
            GraphicsDevice.RasterizerState = originalRasterizerState;

            base.Draw(gameTime);
        }
    }
}
