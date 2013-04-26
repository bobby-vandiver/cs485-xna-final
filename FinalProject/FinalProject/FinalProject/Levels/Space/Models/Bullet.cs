using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class Bullet : BasicModel
    {
        Vector3 position;
        Vector3 dir;
        Camera cam;
        float rot;
        Vector3 firstPosition;
        public BoundingSphere bs;
        float time = 0.0f;
        float asteroidSpeed = .01f;
        public Boolean alive;
        public Matrix worldHolder = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix world = Matrix.Identity;
        Random r;
        Model model;
        public Bullet(Model model, Vector3 currentPoint, Camera camera)
            : base(model)
        {
            alive = true;
            this.model = model;
            position = currentPoint;
            this.cam = camera;
            // Direction will always be (0, 0, Z)
            Vector3 direction = cam.Direction;
            dir = direction;
            firstPosition = position;
            world = Matrix.CreateTranslation(position);
        }
        public override void Update(GameTime gameTime)
        {
            position += dir;
            rotation *= Matrix.CreateFromYawPitchRoll(0,0,rot);
            world *= Matrix.CreateTranslation(dir);
            base.Update(gameTime);
        }
        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(position, 1f);
        }
        public override void Draw(Camera camera)
        {
            if (alive)
            {
                Matrix[] transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect basicEffect in mesh.Effects)
                    {
                        basicEffect.EnableDefaultLighting();
                        basicEffect.EmissiveColor = new Vector3(-1, 0, 0);
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
            Matrix scale = Matrix.CreateScale(.05f);
            Matrix translation = Matrix.CreateTranslation(new Vector3(0f, -0.25f, -1.0f));
            worldHolder = meshTransform * scale * rotation * world * translation;
            return worldHolder;
        }
        public bool CollidesWith(BoundingSphere bs)
        {
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
