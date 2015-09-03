﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject{

        int size;
        int polycount;
        int degree;
        float[,] coords;
        Random rngesus;
        public Landscape(Game game, int degree){
            this.degree = degree;
            this.size = (int)Math.Pow(2,this.degree)+1;
            this.polycount = (int)Math.Pow(this.size - 1, 2) * 2;
            this.rngesus = new Random();
            this.coords = new float[size, size];

            Generate(0,this.size,0,this.size,100,65);
            vertices = Buffer.Vertex.New(game.GraphicsDevice, TerrainModel(this.coords));

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
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

        private float fouravg(float a, float b, float c, float d){
            return (a + b + c + d) / (float)4.0;
        }

        private VertexPositionNormalColor[] TerrainModel(float[,] map){
            VertexPositionNormalColor[] VList = new VertexPositionNormalColor[this.polycount*3];
            int index=0;
            
            //Upper Triangles in Mesh
            for (int i = 0; i < this.size-1; i++){
                for (int j = 0; j < this.size - 1;j++ ){
                    VList[index] = new VertexPositionNormalColor(new Vector3(i,map[i,j],j),new Vector3(0,0,1),getColor(map[i,j]));
                    VList[index + 1] = new VertexPositionNormalColor(new Vector3(i, map[i, j + 1], j + 1), new Vector3(0, 0, 1), getColor(map[i,j+1]));
                    VList[index + 2] = new VertexPositionNormalColor(new Vector3(i + 1, map[i + 1, j + 1], j + 1), new Vector3(0, 0, 1), getColor(map[i+1,j+1]));
                    index += 3;
                }
            }

            //Lower Triangles in Mesh
            for (int i = 1; i < this.size; i++){
                for (int j = 1; j < this.size; j++){
                    VList[index] = new VertexPositionNormalColor(new Vector3(i, map[i, j], j), new Vector3(0, 0, 1), getColor(map[i,j]));
                    VList[index + 1] = new VertexPositionNormalColor(new Vector3(i, map[i, j - 1], j - 1), new Vector3(0, 0, 1), getColor(map[i,j-1]));
                    VList[index+2] = new VertexPositionNormalColor(new Vector3(i-1, map[i-1, j-1], j-1), new Vector3(0, 0, 1), getColor(map[i-1,j-1]));
                    index += 3;
                }
            }
            return VList;
        }

        private Color getColor(float vert){
            if (vert >= 0 && vert < 10){
                return Color.Green;
            }
            if (vert >= 10){
                return Color.Gray;
            }
            if (vert < -5){
                return Color.Blue;
            }
            else{
                return Color.SandyBrown;
            }
        }

        /*public Landscape(Game game){
            Vector3 frontBottomLeft = new Vector3(-1.0f, -1.0f, -1.0f);
            Vector3 frontTopLeft = new Vector3(-1.0f, 1.0f, -1.0f);
            Vector3 frontTopRight = new Vector3(1.0f, 1.0f, -1.0f);
            Vector3 frontBottomRight = new Vector3(1.0f, -1.0f, -1.0f);
            Vector3 backBottomLeft = new Vector3(-1.0f, -1.0f, 1.0f);
            Vector3 backBottomRight = new Vector3(1.0f, -1.0f, 1.0f);
            Vector3 backTopLeft = new Vector3(-1.0f, 1.0f, 1.0f);
            Vector3 backTopRight = new Vector3(1.0f, 1.0f, 1.0f);

            Vector3 frontBottomLeftNormal = new Vector3(-0.333f, -0.333f, -0.333f);
            Vector3 frontTopLeftNormal = new Vector3(-0.333f, 0.333f, -0.333f);
            Vector3 frontTopRightNormal = new Vector3(0.333f, 0.333f, -0.333f);
            Vector3 frontBottomRightNormal = new Vector3(0.333f, -0.333f, -0.333f);
            Vector3 backBottomLeftNormal = new Vector3(-0.333f, -0.333f, 0.333f);
            Vector3 backBottomRightNormal = new Vector3(0.333f, -0.333f, 0.333f);
            Vector3 backTopLeftNormal = new Vector3(-0.333f, 0.333f, 0.333f);
            Vector3 backTopRightNormal = new Vector3(0.333f, 0.333f, 0.333f);

            vertices = Buffer.Vertex.New(
                            game.GraphicsDevice,
                            new[]
                    {
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Orange), // Front
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange), // BACK
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.Orange),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.Orange),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.Orange),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed), // Top
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed), // Bottom
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange), // Left
                    new VertexPositionNormalColor(backBottomLeft, backBottomLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomLeft, frontBottomLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopLeft, backTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontTopLeft, frontTopLeftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange), // Right
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backBottomRight, backBottomRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontBottomRight, frontBottomRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(frontTopRight, frontTopRightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(backTopRight, backTopRightNormal, Color.DarkOrange),
                });

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        } */

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.World = Matrix.Translation(-10, -5, 8);// *Matrix.RotationX(1.57f);
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
