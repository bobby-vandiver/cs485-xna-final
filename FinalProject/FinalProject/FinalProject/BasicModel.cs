using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class BasicModel
    {
        public Model Model { get; protected set; }

        public BasicModel(Model model)
        {
            this.Model = model;
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


        //public virtual void Draw(Camera camera)
        //{
        //    //    Matrix[] transforms = new Matrix[Model.Bones.Count];
        //    //    Model.CopyAbsoluteBoneTransformsTo(transforms);

        //    //draw model 
        //    foreach (ModelMesh mm in model.Meshes)
        //    {
        //        foreach (ModelMeshPart mp in mm.MeshParts)
        //        {

        //            Matrix[] transforms = new Matrix[model.Bones.Count];
        //            Model.CopyAbsoluteBoneTransformsTo(transforms);

        //            Matrix world = GetWorld(transforms[mm.ParentBone.Index], camera);

        //            // set global variables in the shader 
        //            effect.Parameters["WorldViewProjection"].SetValue(world);

        //            mp.Effect = effect; // apply your shader code
        //            mm.Draw();
        //        }
        //    }

        //}


        // Provide a way for subclasses to alter the world transformation of the model
        protected virtual Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            return Matrix.Identity * meshTransform;
        }




    }
}
