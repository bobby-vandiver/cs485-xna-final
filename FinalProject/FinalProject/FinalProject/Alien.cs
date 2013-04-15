using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class Alien : BasicModel
    {
        public Vector3 Position;
        const float POSITION_ABOVE_GROUND = 3.0f;

        public Alien(Model model, Vector3 position)
            : base(model)
        {
            this.Position = position;
            this.Position.Y += POSITION_ABOVE_GROUND;
        }

        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.1f);
            Matrix translation = Matrix.CreateTranslation(Position);
            return meshTransform * scale * translation;

        }
    }
}
