using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class Spaceship : BasicModel
    {
        public BoundingSphere bs;
        Camera cam;
        public Vector3 col;
        Boolean alive = true;
        public int damage_count = 0;
        const int DAMAGE_THRESH = 10;
        Matrix worldHolder = Matrix.Identity;
        public Spaceship(Model model,Camera camera)
            : base(model)
        {
            this.cam = camera;
            col = new Vector3(0, 0, 0);
            Vector3 position = new Vector3(0,40,6);
            bs = new BoundingSphere(position, .1f);
        }
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (col == new Vector3(1, 0, 0))
            {
                alive = !(alive);
            }
            if (damage_count == DAMAGE_THRESH)
            {
                alive = false;
            }
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
                        basicEffect.EmissiveColor = col;
                        basicEffect.Projection = camera.Projection;
                        basicEffect.View = camera.View;

                        Matrix world = GetWorld(transforms[mesh.ParentBone.Index], camera);
                        basicEffect.World = world;
                    }
                    mesh.Draw();
                }
            }
        }
        // Returns a matrix so the gun stays with the camera
        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.02f);
            Matrix rotation = Matrix.CreateRotationY(MathHelper.Pi);
            Matrix roll = Matrix.CreateRotationZ(cam.Roll);
            Matrix translation = Matrix.CreateTranslation(new Vector3(0f, -0.25f, -1.0f));
            worldHolder = meshTransform * scale * rotation * roll * translation * Matrix.Invert(camera.View);
            return worldHolder;
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
