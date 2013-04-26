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
        bool[] shake = new bool[7];
        int shakeIndex = 1;
        Vector3 tempDirection;
        Vector3 tempPosition;
        Vector3 tempUp;
        bool test = false;

        Camera camera;
        #endregion
        public Bombard(Model model, Terrain terrain, 
            Texture2D tex, Camera camera, float milliseconds, Effect effect, Texture2D smokeTexture)
        {
            this.texture = tex;
            this.model = model;
            this.camera = camera;
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
             
                shake[0] = true;
                shake[1] = true;
                if (!test)
                {
                    tempPosition = camera.position;
                    tempDirection = camera.direction;
                    tempUp = camera.Up;
                    test = true;
                }
                ///
                if (intensity > .6f)
                {
                    camera.RedHealth();
                    ShakeCamera(camera);
                }
             
            }     
        }
        
        //Shake Camera 
        private void ShakeCamera(Camera camera)
        {
            if (shake[0])
            {
                if ((milliseconds > (previousMilliseconds + 50)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(.5f);
                    shake[shakeIndex] = false;
                    shakeIndex = 2;
                    shake[shakeIndex] = true;
                }
                if ((milliseconds > (previousMilliseconds + 75)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(-.5f); 
                    shake[shakeIndex] = false;
                    shakeIndex = 3;
                    shake[shakeIndex] = true;
                }
                if ((milliseconds > (previousMilliseconds + 100)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(-.15f); 
                    shake[shakeIndex] = false;
                    shakeIndex = 4;
                    shake[shakeIndex] = true;
                }
                if ((milliseconds > (previousMilliseconds + 125)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(.15f); 
                    shake[shakeIndex] = false;
                    shakeIndex = 5;
                    shake[shakeIndex] = true;
                }
                if ((milliseconds > (previousMilliseconds + 150)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(.2f); 
                    shake[shakeIndex] = false;
                    shakeIndex = 6;
                    shake[shakeIndex] = true;
                }
                if ((milliseconds > (previousMilliseconds + 175)) && shake[shakeIndex])
                {
                    camera.shakeUp = true;
                    camera.CameraShake(-.2f); 
                    shake[shakeIndex] = false;
                    shakeIndex = 0;
                    test = true;
                }
                if ((milliseconds > (previousMilliseconds + 200) && test))
                {
                    camera.direction = tempDirection;
                    camera.position = tempPosition;
                    camera.up = tempUp;
                    test = false;
                    shake[0] = false;
                    shake[1] = false;
                }

            }

        }

        public void Update(GameTime gameTime)
        {

            this.milliseconds += gameTime.ElapsedGameTime.Milliseconds;
            NewAsteroid(this.camera);
            ShakeCamera(this.camera);
            intensity = (float)(1 - (Math.Sqrt(Math.Pow(((double)camera.position.X - (double)collisionPosition[0].X), 2) + Math.Pow(((double)camera.position.Z - (double)collisionPosition[0].Z), 2)))/600);
            Console.WriteLine(intensity);
            camera.intensity = intensity;
            
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
