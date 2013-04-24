using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace FinalProject
{
    class Bombard
    {


      
        #region Declarations
        Model model;
    
        Effect effect;

        //Location for asteroid
        Vector3 location;
        
        //Collision Position 
        public Vector3[] collisionPosition = new Vector3[5];

        public float intensity;

        //Texture for asteroid ----- might just light and not apply a texture-- in progress
        Texture2D texture;
    
        //Start asteroid in random position
        Random randomNumber = new Random();

        //Fly asteroid into world with different speeds and angles
        float i = 195;

        float speed = 10;


        public float milliseconds;
        float previousMilliseconds;
        Terrain terrain;

        //Smoke Texture
        Texture2D smokeTexture;

        //Used for size of the asteroid
        float size = 0;

        //For shake function to shake camera and return it back to original position
        const int SHAKE_COUNT = 6;

        bool isShaking = false;
        bool[] shake = new bool[SHAKE_COUNT];
        int shakeIndex = 0;

        Vector3 tempDirection;
        Vector3 tempPosition;
        Vector3 tempUp;
        bool test = false;

        Camera camera;
        HUD hud;
        #endregion

        public Bombard(Model model, Terrain terrain, 
            Texture2D tex, Camera camera, HUD hud, float milliseconds, Effect effect, Texture2D smokeTexture)
        {
            this.texture = tex;
            this.model = model;
            this.camera = camera;
            this.hud = hud;
            this.terrain = terrain;
            this.milliseconds = milliseconds;
            this.effect = effect;
            this.smokeTexture = smokeTexture;
            Vector3 rand = RandomPosition();
        }

        //Random position for asteroid            
        protected Vector3 RandomPosition()
        {

            // Find a random point in the world
            float x = (float)randomNumber.NextDouble() * (terrain.MaxX - terrain.MinX) + terrain.MinX;
            float z = (float)randomNumber.NextDouble() * (terrain.MaxZ - terrain.MinZ) + terrain.MinZ;

            size = (float)randomNumber.NextDouble() * 20 + 10;
            previousMilliseconds = milliseconds;

            
            return new Vector3(x, 0, z);


        }
        
        protected void DrawSmoke()
        {

            
            
        }
        
        //Create new asteroid when hits the ground
        protected void NewAsteroid(Camera camera)
        {
            //Console.WriteLine(intensity);
            if (i < -5)
            {
                
                location = RandomPosition();
                i = 195;
             
                isShaking = true;
                shake[0] = true;
                if (!test)
                {
                    tempPosition = camera.Position;
                    tempDirection = camera.Direction;
                    tempUp = camera.Up;
                    test = true;
                }
                if (intensity > .6f)
                {
                    hud.RedHealth();
                    ShakeCamera(camera);
                }
             
            }     
        }
        
        //Shake Camera 
        private void ShakeCamera(Camera camera)
        {
            int[] millsecondOffsets = { 50, 75, 100, 125, 150, 175 };

            for (int i = 0; i < SHAKE_COUNT; i++)
            {
                int millisecondOffset = millsecondOffsets[i];
                UpdateShake(millisecondOffset);
            }

            if ((milliseconds > (previousMilliseconds + 200) && test))
            {
                camera.Direction = tempDirection;
                camera.Position = tempPosition;
                camera.Up = tempUp;
                test = false;
                isShaking = false;
                shake[0] = false;
            }
        }

        private void UpdateShake(int millisecondsOffset)
        {
            float[] shakeAngles = { 0.5f, -0.5f, 0.15f, -0.15f, 0.2f, -02f };

            if(!isShaking)
                return;

            if (milliseconds > (previousMilliseconds + millisecondsOffset) && shake[shakeIndex])
            {
                camera.shakeUp = true;

                float shakeAngle = shakeAngles[shakeIndex];
                camera.CameraShake(shakeAngle);

                shake[shakeIndex++] = false;
                if (shakeIndex >= SHAKE_COUNT)
                {
                    shakeIndex = 0;
                    test = true;
                }
                else
                {
                    shake[shakeIndex] = true;
                }
            }
        }

        public void Update(GameTime gameTime)
        {

            this.milliseconds += gameTime.ElapsedGameTime.Milliseconds;
            NewAsteroid(this.camera);
            ShakeCamera(this.camera);
            intensity = (float)(1 - (Math.Sqrt(Math.Pow(((double)camera.Position.X - (double)collisionPosition[0].X), 2) + Math.Pow(((double)camera.Position.Z - (double)collisionPosition[0].Z), 2)))/600);
            //Console.WriteLine(camera.intensity);
            camera.intensity = intensity;
            hud.intensity = intensity;
            
        }

        private void spreadCollision() 
        {
              for (int i = 1; i < collisionPosition.Length; i++)
                collisionPosition[i] =  new Vector3(collisionPosition[0].X +(float)randomNumber.NextDouble(),
                    (-20),
                   collisionPosition[0].X +(float)randomNumber.NextDouble()
                );

           
        }

        public void Draw()
        {
            float x = (speed - (i * .05f))/2;
           

            Matrix View = this.camera.View;
            Matrix Projection = this.camera.Projection;
            Vector3 Up = this.camera.Up;
            Vector3 cPosition = this.camera.Position;
            Matrix world = Matrix.CreateRotationZ(0) * Matrix.CreateScale(size - (i * .05f)) *
            Matrix.CreateTranslation(location.X - i, i-= x, location.Z - i);

            collisionPosition[0].X = location.X - 15;
            collisionPosition[0].Y = 25; 
            collisionPosition[0].Z = location.Z - 15;
            spreadCollision();
            //transforms camera position in to a Vector4 so the effect file can use
            Vector4 cameraPosition = new Vector4(cPosition, 0);

            // Set the effect parameters
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["Texture"].SetValue(texture);

        
            ////draw model 
            //foreach (ModelMesh modelMesh in model.Meshes)
            //{
            //    foreach (ModelMeshPart modelPart in modelMesh.MeshParts)
            //    {
            //        modelPart.Effect = effect; // apply your shader code
            //        modelMesh.Draw();
            //    }
            //}

            //if (i < 40)
            //{
            //    world = Matrix.CreateRotationZ(0) * Matrix.CreateScale(size - (i * .05f)) *
            //    Matrix.CreateTranslation(location.X - 15, 20-i, location.Z - 15);
            //    // Set the effect parameters
            //    effect.Parameters["World"].SetValue(world);
            //    effect.Parameters["View"].SetValue(View);
            //    effect.Parameters["Projection"].SetValue(Projection);
            //    effect.Parameters["Texture"].SetValue(smokeTexture);
            //}

            //draw model 
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (ModelMeshPart modelPart in modelMesh.MeshParts)
                {
                    modelPart.Effect = effect; // apply your shader code
                    modelMesh.Draw();
                }
            }

        }
    }
}
