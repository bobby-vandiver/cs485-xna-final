using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class BasicModel
    {
        public Model Model { get; protected set; }

        private BoundingSphere boundingSphere
        {
            get { return GetBoundingSphere(); }
        }

        public BasicModel(Model model)
        {
            this.Model = model;
        }

        protected virtual BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.Zero, 0);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw each mesh in the model
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.EnableDefaultLighting();

                    basicEffect.Projection = camera.Projection;
                    basicEffect.View = camera.View;

                    Matrix world = GetWorld(transforms[mesh.ParentBone.Index], camera);
                    basicEffect.World = world;
                }
                mesh.Draw();
            }
        }

        // Provide a way for subclasses to alter the world transformation of the model
        protected virtual Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            return Matrix.Identity * meshTransform;
        }

        // Provides rudimentary collision detection with points
        public virtual bool Collides(Vector3 point)
        {
            return boundingSphere.Contains(point) != ContainmentType.Disjoint;
        }

        // Provides rudimentary collision detection with models
        public virtual bool Collides(BasicModel otherModel)
        {
            bool intersects = boundingSphere.Intersects(otherModel.boundingSphere);
            bool contains = boundingSphere.Contains(otherModel.boundingSphere) != ContainmentType.Disjoint;
            return intersects || contains;
        }
    }
}
