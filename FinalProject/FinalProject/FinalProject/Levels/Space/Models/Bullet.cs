using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class Bullet : BasicModel
    {
        Vector3 Position;
        Vector3 initialPosition;

        // Direction of motion
        Vector3 direction;
        Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                direction.Normalize();
            }
        }

        public bool IsAlive = false;
        public BoundingSphere bs;

        public Bullet(Model model, Vector3 currentPoint, Camera camera)
            : base(model)
        {
            this.Direction = camera.Direction;
            this.initialPosition = currentPoint;
            this.Position = this.initialPosition + new Vector3(0f, -0.25f, -1.0f);
            this.IsAlive = true;
        }
        public override void Update(GameTime gameTime)
        {
            Position += Direction;
            base.Update(gameTime);
        }
        
        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Position, 1f);
        }

        public override void Draw(Camera camera)
        {
            if (IsAlive)
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
            Matrix translation = Matrix.CreateTranslation(Position);
            return meshTransform * scale * translation;
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
