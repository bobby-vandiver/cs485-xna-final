using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class Fragment : BasicModel
    {
        public Vector3 position;
        Vector3 initialPosition;

        Vector3 direction;

        float rotationRate;
        float rotationAngle;
        
        float time = 0.0f;
        const float TIME_TO_LIVE = 10.0f;

        float asteroidSpeed = .003f;
        
        public Boolean IsAlive;

        public Fragment(Model model, Vector3 starting, Camera camera)
            : base(model)
        {
            this.IsAlive = true;

            this.initialPosition = starting;
            this.position = this.initialPosition;

            Random r = new Random();
            this.direction = new Vector3(asteroidSpeed * (float)r.NextDouble(), 
                asteroidSpeed * (float)r.NextDouble(),
                asteroidSpeed*(float)r.NextDouble());
            
            this.rotationRate = (float)(position.Y*.001);
            this.rotationAngle = 0.0f;
        }
        
        public override void Update(GameTime gameTime)
        {
            time += 5;
            if (time > TIME_TO_LIVE)
                IsAlive = false;

            position+= direction;
            rotationAngle += rotationRate;

            base.Update(gameTime);
        }
        
        public override void Draw(Camera camera)
        {
            if (IsAlive)
            {
                Matrix[] transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw each mesh in the model
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect basicEffect in mesh.Effects)
                    {
                        basicEffect.EnableDefaultLighting();
                        basicEffect.EmissiveColor = new Vector3(-.8f,-.8f,-.8f);
                        basicEffect.Projection = camera.Projection;
                        basicEffect.View = camera.View;
                        Matrix world = GetWorld(transforms[mesh.ParentBone.Index], camera);
                        basicEffect.World = world;
                    }
                    mesh.Draw();
                }
            }
        }
        
        // Returns a matrix for the asteroids current position
        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix translation = Matrix.CreateTranslation(position);
            Matrix rotation = Matrix.CreateFromYawPitchRoll(rotationAngle, rotationAngle, rotationAngle);
            Matrix scale = Matrix.CreateScale(.5f);
            return meshTransform * scale * rotation * translation;
        }
    }
}
