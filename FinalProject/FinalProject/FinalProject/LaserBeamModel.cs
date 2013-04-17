using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class LaserBeamModel : BasicModel
    {
        public Vector3 Position;

        public LaserBeamModel(Model model, Vector3 position)
            : base(model)
        {
            this.Position = position;
        }

        protected override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Position, 10.0f);
        }

        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.01f);
            Matrix translation = Matrix.CreateTranslation(Position + new Vector3(0, -.05f, 0));
            return meshTransform * scale * translation;

        }
    }
}
