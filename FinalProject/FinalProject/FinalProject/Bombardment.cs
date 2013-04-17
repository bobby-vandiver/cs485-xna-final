using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FinalProject
{
    class Bombardment : Microsoft.Xna.Framework.Game
    {
        Random randomNumber = new Random();
        float i = -5;
        float ii = .01f;
        float iii = 0f;
        Vector3 location;
        float speed;
        public float milliseconds;
        float previousMilliseconds;
        Terrain terrain;
        Vector3 rand;
        float size = 0;
        bool[] shake = new bool[7];
        int shakeIndex = 1;
        Vector3 tempDirection;
        Vector3 tempPosition;
        Vector3 tempUp;
        Effect effect; 
        bool test = false;
        Model model;
        public Bombardment(Model model, float speed, Vector3 location, Terrain terrain, float milliseconds,Effect effect)
            : base(model)
        {
            this.location = location;
            this.speed = speed;
            this.terrain = terrain;
            this.milliseconds = milliseconds;
            this.rand = RandomPosition();
            this.effect = effect;
            this.model = model;

        }

        protected Vector3 RandomPosition()
        {
            
            // Find a random point in the world
            float x = (float)randomNumber.NextDouble() * (terrain.MaxX - terrain.MinX) + terrain.MinX;
            float z = (float)randomNumber.NextDouble() * (terrain.MaxZ - terrain.MinZ) + terrain.MinZ;
            
            
            size = (float)randomNumber.NextDouble() * 20 + 5;
            previousMilliseconds = milliseconds;

            return new Vector3(x, 0, z);


        }

        protected void NewAsteroid(Camera camera) 
        {
            if (i < -5)
            {
                location = RandomPosition();
                i = 300;
                ii = .01f;
                iii = 300;
                shake[0] = true;
                shake[1] = true;
                if (!test)
                {
                    tempPosition = camera.position;
                    tempDirection = camera.direction;
                    tempUp = camera.Up;
                    test = true;
                }        
            }

            //camera.ApplyYawRotation(-.30f);
            //if ((milliseconds > (previousMilliseconds + 150)))
            //    camera.ApplyPitchRotation(.30f);
            

        }

        private void StartShake()
        {
            for (int y = 0; y < 2; y++)
            {
                shake[y] = true;
            }
        }   
        

        private void ShakeCamera(Camera camera)
        {
            if (shake[0])
            {
                if ((milliseconds > (previousMilliseconds + 50)) && shake[shakeIndex])
                {
                    camera.ApplyYawRotation(.05f);
                    shake[shakeIndex] = false;
                    shakeIndex = 2;
                    shake[shakeIndex] = true;

                }
                if ((milliseconds > (previousMilliseconds + 75)) && shake[shakeIndex])
                {

                    camera.ApplyYawRotation(-.15f);
                    shake[shakeIndex] = false;
                    shakeIndex = 3;
                    shake[shakeIndex] = true;
                    

                }
                if ((milliseconds > (previousMilliseconds + 100)) && shake[shakeIndex])
                {

                    camera.ApplyRollRotation(.05f);
                    shake[shakeIndex] = false;
                    shakeIndex = 4;
                    shake[shakeIndex] = true;

                }
                if ((milliseconds > (previousMilliseconds + 125)) && shake[shakeIndex])
                {
                    camera.ApplyRollRotation(-.05f);
                    shake[shakeIndex] = false;
                    shakeIndex = 5;
                    shake[shakeIndex] = true;

                }
                if ((milliseconds > (previousMilliseconds + 150)) && shake[shakeIndex])
                {
                    camera.ApplyYawRotation(.05f);
                    shake[shakeIndex] = false;
                    shakeIndex = 6;
                    shake[shakeIndex] = true;

                } 
                if ((milliseconds > (previousMilliseconds + 175)) && shake[shakeIndex])
                {
                    camera.ApplyYawRotation(-.05f);
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

        // Returns a matrix to drop the asteroid. 
        public Matrix GetWorld(Matrix meshTransform, Camera camera)
        {
            Console.WriteLine("Previous Milli: " + previousMilliseconds);
            Console.WriteLine("Direction: " + camera.direction);
            Console.WriteLine("Previous Direction: " + tempDirection);
            Console.WriteLine("Milli: " + milliseconds);
            NewAsteroid(camera);
            ShakeCamera(camera);

            string line = " X: " + location.X + " Z: " + location.Z + " Size: " + size;
            Console.WriteLine(line);
            Matrix scale = Matrix.CreateScale(size);
            Matrix rotation = Matrix.CreateRotationY(MathHelper.Pi);
            Matrix translation = Matrix.CreateTranslation(location.X+ iii--, i-= ((ii += .01f)*speed), location.Z - iii);


            return meshTransform * scale * rotation * translation;

        }



       // protected override void drawModel(Camera camera)
       // {
                
       //     //draw model 
       //     foreach (ModelMesh mm in model.Meshes)
       //     {
       //         foreach (ModelMeshPart mp in mm.MeshParts)
       //         {

       //     Matrix[] transforms = new Matrix[model.Bones.Count];
       //     Model.CopyAbsoluteBoneTransformsTo(transforms);

       //     Matrix world = GetWorld(transforms[mm.ParentBone.Index], camera);
        
       //     // set global variables in the shader 
       //     effect.Parameters["WorldViewProjection"].SetValue(world);

       //             mp.Effect = effect; // apply your shader code
       //             mm.Draw();
       //         }
       //     }         
   
       //}


        





    }
}
