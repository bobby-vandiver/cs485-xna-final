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
        Vector3 dir;
        Camera cam;
        float rot;
        Vector3 firstPosition;
        public BoundingSphere bs;
        float time = 0.0f;
        float asteroidSpeed = .03f;
        public Boolean alive;
        Model model;
        Matrix rotation = Matrix.Identity;
        public Matrix world = Matrix.Identity;
        
        public Asteroid(Model model, Vector3 randomPoint, Camera camera)
            : base(model)
        {
            alive = true;
            this.model = model;
            position = randomPoint;
            this.cam = camera;
            Random r = new Random();
            Vector3 direction = new Vector3(0, 0,
                position.Y*asteroidSpeed*(float)r.NextDouble());
            rot = (float)(position.Y*.001);
            dir = direction;
            firstPosition = position;
            world = Matrix.CreateTranslation(position);
            bs = new BoundingSphere(firstPosition, 1.5f);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            time += 5;
            position+= dir;
            rotation *= Matrix.CreateFromYawPitchRoll(rot,
                            rot, rot);
            // Move model
            bs = new BoundingSphere(position, 1.5f);
            world *= Matrix.CreateTranslation(dir);
            base.Update(gameTime);
        }
        public override void Draw(Camera camera)
        {
            if (alive)
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
            Matrix scale = Matrix.CreateScale(2f);
            return meshTransform * scale * rotation * world;
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
