﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject{

        //Declaration of internal variables
        public int size;                                //Map side length
        private int polycount;                          //Number of polygons in the map model
        private int degree;                             //Degree of map size (2^degree)
        private int maxheight;                          //Max height for objects in the terrain
        private float[,] coords;                        //Coordinate map used for DiamondSquare
        private VertexPositionNormalColor[] terrain;    //Terrain vertices
        private Random rngesus;                         //Random number generator
        private Project1Game gameaccess;                //Access to public game functions

        public Landscape(Project1Game game, int degree){
            this.degree = degree;
            this.size = (int)Math.Pow(2,this.degree)+1;
            this.maxheight = this.size/2;
            this.polycount = (int)Math.Pow(this.size - 1, 2) * 2;
            this.rngesus = new Random();
            this.coords = new float[size, size];

            //Generate the heightmap using DiamondSquare
            Generate(0,this.size,0,size,maxheight,size/2);
            //Generate the terrain model
            this.terrain = TerrainModel(this.coords);
            //Place terrain model into vertex buffer
            vertices = Buffer.Vertex.New(game.GraphicsDevice, TerrainModel(this.coords));

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.gameaccess = game;
            this.game = game;
            
        }

        //Recursive Diamond Square
        private void Generate(int xmin, int xmax, int ymin, int ymax, int maxnoise, int remaining){

            //Termination condition
            if(remaining <= 0){
                return;
            }

            //Local variable declarations, performed after termination check for efficiency
            float bl, br, tl, tr, c;
            float t, b, l, r;
            float noise;
            float avg;

            //Algorithm Steps
            //Diamond Step
            for(int i = xmin + remaining; i < xmax; i += remaining){
                for(int j = ymin + remaining; j < ymax; j += remaining){
                    bl = this.coords[i-remaining,j-remaining];
                    br = this.coords[i,j-remaining];
                    tl = this.coords[i-remaining,j];
                    tr = this.coords[i,j];
                    avg = fouravg(bl,br,tl,tr);
                    if(rngesus.Next(0,2) > 0){
                        noise = rngesus.Next(0,maxnoise);
                    }
                    else{
                        noise = -rngesus.Next(0,maxnoise);
                    }
                    this.coords[i - (remaining / 2), j - (remaining / 2)] = (avg + noise);
                }
            }

            //Square Step
            for (int i = xmin + 2 * remaining; i < xmax; i += remaining){
                for (int j = ymin + (2 * remaining); j < ymax; j += remaining){
                    b = this.coords[i - remaining, j - remaining];
                    br = this.coords[i, j - remaining];
                    tl = this.coords[i - remaining, j];
                    t = this.coords[i, j];
                    c = coords[i - remaining / 2, j - remaining / 2];

                    l = this.coords[i-(3*remaining/2),j-(remaining/2)];
                    r = this.coords[i-remaining/2,j-3*remaining/2];

                    if(rngesus.Next(0,2) > 0){
                        noise = rngesus.Next(0,maxnoise);
                    }
                    else{
                        noise = -rngesus.Next(0,maxnoise);
                    }

                    this.coords[i - remaining, j - remaining / 2] = fouravg(b, tl, c, l) + noise;
                    this.coords[i - remaining/2, j - remaining] = fouravg(b, tl, c, r) + noise;
                    
                }
            }

            Generate(xmin,xmax,ymin,ymax,maxnoise/2,remaining/2);
        }

        //Average 4 values
        private float fouravg(float a, float b, float c, float d){
            return (a + b + c + d) / (float)4.0;
        }

        //Generate a 3D model for the terrain
        private VertexPositionNormalColor[] TerrainModel(float[,] map){
            VertexPositionNormalColor[] VList = new VertexPositionNormalColor[this.polycount*3];
            int index=0;
            
            Vector3 p1,p2,p3;
            Vector3 normal;

            //Upper Triangles in Mesh
            for (int i = 0; i < this.size-1; i++){
                for (int j = 0; j < this.size - 1;j++ ){

                    p1 = new Vector3(i, map[i, j], j);
                    p2 = new Vector3(i, map[i, j + 1], j + 1);
                    p3 = new Vector3(i + 1, map[i + 1, j + 1], j + 1);

                    normal = genNormal(p1,p2,p3);

                    VList[index] = new VertexPositionNormalColor(p1,normal,getColor(map[i,j]));
                    VList[index + 1] = new VertexPositionNormalColor(p2, normal, getColor(map[i,j+1]));
                    VList[index + 2] = new VertexPositionNormalColor(p3, normal, getColor(map[i+1,j+1]));
                    index += 3;
                }
            }

            //Lower Triangles in Mesh
            for (int i = 1; i < this.size; i++){
                for (int j = 1; j < this.size; j++){

                    p1 = new Vector3(i, map[i, j], j);
                    p2 = new Vector3(i, map[i, j - 1], j - 1);
                    p3 = new Vector3(i - 1, map[i - 1, j - 1], j - 1);

                    normal = genNormal(p1, p2, p3);

                    VList[index] = new VertexPositionNormalColor(p1, normal, getColor(map[i,j]));
                    VList[index + 1] = new VertexPositionNormalColor(p2, normal, getColor(map[i,j-1]));
                    VList[index+2] = new VertexPositionNormalColor(p3, normal, getColor(map[i-1,j-1]));
                    index += 3;
                }
            }
            return VList;
        }

        //Set vertex colour based on height
        private Color getColor(float vert){
            if (vert >1 && vert <= 0.15*maxheight){
                return Color.Green;
            }
            if (vert > 0.15*maxheight && vert <= 0.4*maxheight){
                return Color.Gray;
            }
            if (vert > 0.4*maxheight)
            {
                return Color.White;
            }
            else{
                return Color.SandyBrown;
            }
        }

        //Generate normals for surfaces
        private Vector3 genNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a);
            normal = Vector3.Normalize(normal);
            return normal;
        }

        //Update the lighting on the landscape
        public void Update(GameTime gameTime, Vector3 light)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.AmbientLightColor = gameaccess.ambient();

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = gameaccess.diffuse();
            basicEffect.DirectionalLight0.Direction = light;
            basicEffect.DirectionalLight0.SpecularColor = gameaccess.specular();
        }

        //We're using a slightly modified update, see above
        public override void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the terrain.
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        //Test for collisions
        public float isColliding(Vector3 pos)
        {
            for(int i=0; i<polycount; i++)
            {
                if(PointInTriangle(pos, terrain[i * 3].Position, terrain[i * 3 + 1].Position, terrain[i * 3 + 2].Position))
                {
                    return calcZ(terrain[i * 3].Position, terrain[i * 3 + 1].Position, terrain[i * 3 + 2].Position, pos);
                }
            }
            //return here just to keep the thing happy
            return -100000.0f;
        }

        //With thanks for this algorithm to Erik Rufelt on http://www.gamedev.net/topic/597393-getting-the-height-of-a-point-on-a-triangle/
        private static float calcZ(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 pos)
        {
            float det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);

            float l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Z - p3.Z)) / det;
            float l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Z - p3.Z)) / det;
            float l3 = 1.0f - l1 - l2;

            return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
        }

        //With thanks for this algorithm to Glenn Slayden on http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        private static bool PointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float s = p0.Z * p2.X - p0.X * p2.Z + (p2.Z - p0.Z) * p.X + (p0.X - p2.X) * p.Z;
            float t = p0.X * p1.Z - p0.Z * p1.X + (p0.Z - p1.Z) * p.X + (p1.X - p0.X) * p.Z;

            if ((s < 0.0f) != (t < 0.0f))
            {
                return false;
            }

            float A = -p1.Z * p2.X + p0.Z * (p2.X - p1.X) + p0.X * (p1.Z - p2.Z) + p1.X * p2.Z;
            if (A < 0.0f)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0.0f && t > 0.0f && (s + t) < A;
        }

        public float heightAtPoint(int a, int b)
        {
            return coords[a, b];
        }
    }
}
