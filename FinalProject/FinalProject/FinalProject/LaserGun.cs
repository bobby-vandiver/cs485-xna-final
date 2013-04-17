using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    class LaserGun : BasicModel
    {
        public LaserGun(Model model)
            : base(model)
        {
        }

        // Returns a matrix so the gun stays with the camera
        protected override Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Matrix scale = Matrix.CreateScale(0.2f);
            Matrix rotation = Matrix.CreateRotationY(MathHelper.Pi);
            Matrix translation = Matrix.CreateTranslation(new Vector3(0.7f, -0.7f, -1.8f));

            return meshTransform * scale * rotation * translation * Matrix.Invert(camera.View);
        }
    }
}
