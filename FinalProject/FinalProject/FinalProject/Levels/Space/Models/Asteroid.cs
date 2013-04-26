using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class Asteroid : BasicModel
    {
        public Vector3 position;
        Vector3 initialPosition;

        Vector3 direction;

        float rotationRate;
        float rotationAngle;
        
        public BoundingSphere bs;

        float time = 0.0f;
        float asteroidSpeed = .03f;

        public Boolean IsAlive;
        
        public Asteroid(Model model, Vector3 randomPoint, Camera camera)
            : base(model)
        {
            this.IsAlive = true;

            this.initialPosition = randomPoint;
            this.position = this.initialPosition;

            Random r = new Random();
            float randomZ = position.Y * asteroidSpeed * (float)r.NextDouble();
            this.direction = new Vector3(0, 0, randomZ);

            this.rotationRate = (float)(position.Y * .001);
            this.rotationAngle = 0.0f;

            this.bs = new BoundingSphere(initialPosition, 1.5f);
        }

        public override void Update(GameTime gameTime)
        {
            time += 5;

            position += direction;
            rotationAngle += rotationRate;

            bs = new BoundingSphere(position, 1.5f);

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
            Matrix scale = Matrix.CreateScale(2f);
            return meshTransform * scale * rotation * translation;
        }

        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(position, 3f);
        }

        public bool CollidesWith(BoundingSphere bs)
        {
            // Loop through each ModelMesh in both objects and compare
            // all bounding spheres for collisions
            if (this.bs.Intersects(bs))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
