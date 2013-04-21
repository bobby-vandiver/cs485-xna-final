using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FinalProject
{
    public class Terrain : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Vertex data
        VertexPositionNormalTexture[] vertices;

        // Vertex indices
        int totalIndices;
        int[] indices;

        // The number of vertices in the XZ plane of the world
        int totalVertices;

        // The number of vertices on the X-axis and Z-axis
        int vertexWidthCount;
        int vertexLengthCount;

        // Terrain constraints on object placement
        public float MinX { get { return 0; } }
        public float MaxX { get { return vertexWidthCount * cellSize; } }

        public float MinZ { get { return 0; } }
        public float MaxZ { get { return vertexLengthCount * cellSize; } }

        // Height map to generate the mesh from
        Texture2D heightMap;

        // Texture to apply to generated mesh
        Texture2D groundTexture;

        // Background
        Texture2D backgroundTexture;

        // Height of each (x, z) point in the world
        float maxHeight;
        float[,] heights;
        
        // Each cell is a square, with sides of length cellSize
        float cellSize;
        int totalCells;

        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        public Terrain(Game game, float cellSize, float maxHeight)
            : base(game)
        {
            this.cellSize = cellSize;
            this.maxHeight = maxHeight;
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            LoadHeightMap();

            // Load textures
            groundTexture = Game.Content.Load<Texture2D>(@"Textures\planet_Dagobah1200");
            backgroundTexture = Game.Content.Load<Texture2D>(@"Backgrounds\stars");

            basicEffect = new BasicEffect(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            PrepareGraphicsDeviceForDrawing3D();
            PrepareBasicEffectForDrawing3D();
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, totalVertices, indices, 0, totalCells * 2); 

            base.Draw(gameTime);
        }

        private void PrepareGraphicsDeviceForDrawing3D()
        {
            // Reset state in case SpriteBatch is used somewhere
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void PrepareBasicEffectForDrawing3D()
        {
            Camera camera = (Camera)Game.Services.GetService(typeof(Camera));

            // Set matrices
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;

            // Specify texture to apply
            basicEffect.Texture = groundTexture;
            basicEffect.TextureEnabled = true;

            // Enable lighting
            basicEffect.LightingEnabled = true;

            // Set up directional light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 1));
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
            basicEffect.DirectionalLight0.SpecularColor = Color.White.ToVector3();

            basicEffect.Techniques[0].Passes[0].Apply();

        }

        // Generates a point in the world and on the terrain
        public Vector3 GetRandomPoint()
        {
            Random randomNumberGenerator = (Random)Game.Services.GetService(typeof(Random));

            // Find a random point in the world
            float x = (float)randomNumberGenerator.NextDouble() * (MaxX - MinX) + MinX;
            float z = (float)randomNumberGenerator.NextDouble() * (MaxZ - MinZ) + MinZ;

            // Ensure the point is on the terrain
            float y = GetHeight(x, z);
            return new Vector3(x, y, z);
        }

        public float GetHeight(float x, float z)
        {
            // Restrict point to some where on the map
            x = MathHelper.Clamp(x, MinX, MaxX);
            z = MathHelper.Clamp(z, MinZ, MaxZ);

            // Get the coordinates of the cell boundary
            float up = (float)Math.Floor(z);
            float left = (float)Math.Floor(x);
            float right = left + cellSize;
            float down = up + cellSize;

            // Convert boundaries to array indices
            int upIndex = (int)(up / cellSize);
            int leftIndex = (int)(left / cellSize);
            int rightIndex = (int)(right / cellSize);
            int downIndex = (int)(down / cellSize);

            // Make sure the indices don't reference out of bounds elements
            int maxXIndex = vertexWidthCount - 1;
            int maxZIndex = vertexLengthCount -1 ;

            upIndex = Clamp(upIndex, 0, maxZIndex);
            leftIndex = Clamp(leftIndex, 0, maxXIndex);
            rightIndex = Clamp(rightIndex, 0, maxXIndex);
            downIndex = Clamp(downIndex, 0, maxZIndex);

            // Get height at each corner
            float upperLeftHeight = heights[leftIndex, upIndex];
            float bottomLeftHeight = heights[leftIndex, downIndex];

            float upperRightHeight = heights[rightIndex, upIndex];
            float bottomRightHeight = heights[rightIndex, downIndex];

            // Interpolate height at specified point
            float weightedBottomLeftHeight = bottomLeftHeight * (right - x) * (up - z);
            float weightedBottomRightHeight = bottomRightHeight * (x - left) * (up - z);

            float weightedUpperLeftHeight = upperLeftHeight * (right - x) * (z - down);
            float weightUpperRightHeight = upperRightHeight * (x - left) * (z - down);

            float height = weightedBottomLeftHeight + weightedBottomRightHeight + weightedUpperLeftHeight + weightUpperRightHeight;
            height /= ((right - left) * (up - down));

            return height;
        }

        // MathHelper doesn't provide an implementation for non-float
        private int Clamp(int value, int minValue, int maxValue)
        {
            if (value <= minValue)
                return minValue;
            else if (value >= maxValue)
                return maxValue;
            else
                return value;
        }

        private void LoadHeightMap()
        {
            heightMap = Game.Content.Load<Texture2D>(@"HeightMaps\heightmap1");

            // Each pixel in the height map will be mapped to a vertice in the world
            vertexWidthCount = heightMap.Width;
            vertexLengthCount = heightMap.Height;

            SetTotals();

            // Read height map, then create the mesh for the terrain
            ReadHeightMap();
            CreateVertices();
            CreateVertexIndices();
            CalculateTriangleNormals();
        }

        // Determines total number of vertices, cells and indices needed so
        // they can be reused in various places for different calculations
        private void SetTotals()
        {
            // Determing how many vertices there are
            totalVertices = vertexLengthCount * vertexWidthCount;

            // Each cell consists of two triangles, each requiring 3 vertices
            totalCells = (vertexWidthCount - 1) * (vertexLengthCount - 1);
            totalIndices = totalCells * 6;
        }

        private void ReadHeightMap()
        {
            // Extract pixel data
            Color[] heightMapData = new Color[vertexWidthCount * vertexLengthCount];
            heightMap.GetData<Color>(heightMapData);

            heights = new float[vertexWidthCount, vertexLengthCount];

            // 1 to 1 pixel to height mapping
            for (int z = 0; z < vertexLengthCount; z++)
            {
                for (int x = 0; x < vertexWidthCount; x++)
                {
                    int pixel = (z * vertexWidthCount + x);

                    // Get RGB value of pixel
                    float red = heightMapData[pixel].R;
                    float green = heightMapData[pixel].G;
                    float blue = heightMapData[pixel].B;

                    // Average the colors
                    float height = (red + green + blue) / 3;

                    // Scale to (0 - 1) range
                    height /= (255.0f * 3.0f);

                    // Scale again by the maxHeight to get the final height
                    heights[x, z] = height * maxHeight;
                }
            }
        }

        private void CreateVertices()
        {
            vertices = new VertexPositionNormalTexture[totalVertices];

            for (int z = 0; z < vertexLengthCount; z++)
            {
                for (int x = 0; x < vertexWidthCount; x++)
                {
                    // Calculate vertex position
                    Vector3 position = new Vector3(x * cellSize, heights[x, z], z * cellSize);

                    // Calculate texture mapping
                    float u = (float)x / vertexWidthCount;
                    float v = (float)z / vertexLengthCount;
                    Vector2 textureCoordinate = new Vector2(u, v);

                    // Create the vertex
                    int vertex = z * vertexWidthCount + x;
                    vertices[vertex] = new VertexPositionNormalTexture(position, Vector3.Zero, textureCoordinate);
                }
            }
        }

        private void CreateVertexIndices()
        {
            indices = new int[totalIndices];

            int i = 0;

            // Calculate the index for each vertex
            for (int z = 0; z < vertexLengthCount - 1; z++)
            {
                for (int x = 0; x < vertexWidthCount - 1; x++)
                {
                    // Calculate the indices for each corner of the cell
                    int upperLeft = z * vertexWidthCount + x;
                    int upperRight = upperLeft + 1;
                    int lowerLeft = upperLeft + vertexWidthCount;
                    int lowerRight = lowerLeft + 1;

                    // Upper triangle of the cell
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;

                    // Lower triangle of the cell
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                    indices[i++] = lowerLeft;
                }
            }
        }

        private void CalculateTriangleNormals()
        {
            // Calculate normal for each triangle
            for (int i = 0; i < totalIndices; i += 3)
            {
                // Get position of each corner
                Vector3 v1 = vertices[indices[i]].Position;
                Vector3 v2 = vertices[indices[i + 1]].Position;
                Vector3 v3 = vertices[indices[i + 2]].Position;

                // Calculate unit normal to the triangle
                Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
                normal.Normalize();

                // Let this be added to the normal for every vertex in the triangle
                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;
            }

            // Ensure all normals are unit length
            for (int i = 0; i < totalVertices; i++)
                vertices[i].Normal.Normalize();
        }
    }
}
