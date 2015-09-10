﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Sun : ColoredGameObject
    {
        int worldsize;
        Vector3 ambientcolour;
        Vector3 directionalcolour;
        Vector3 specularcolour;
        Vector3 lightdirection;
        Vector3 diffusecolour;

        public Sun(Project1Game game)
        {

            worldsize = (int)Math.Pow(2, game.scale) + 1;

            ambientcolour = new Vector3(0.2f, 0.2f, 0.2f);
            directionalcolour = new Vector3(0, 0, 0);
            specularcolour = new Vector3(0,0,0);
            lightdirection = new Vector3(0, 0, 0);
            diffusecolour = new Vector3(0, 0, 0);


            Vector3 frontNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                {
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f), frontNormal, Color.Yellow), // Front FBLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, -4.0f), frontNormal, Color.Yellow), //FTLN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, -4.0f), frontNormal, Color.Yellow), //FTRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f), frontNormal, Color.Yellow), //FBLN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, -4.0f), frontNormal, Color.Yellow), //FTRN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, -4.0f), frontNormal, Color.Yellow), //FBRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, 4.0f), backNormal, Color.Yellow), // BACK BBLN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), backNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, 4.0f), backNormal, Color.Yellow), //BTLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, 4.0f), backNormal, Color.Yellow), //BBLN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, 4.0f), backNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), backNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, -4.0f), topNormal, Color.Yellow), // Top FTLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, 4.0f), topNormal, Color.Yellow), //BTLN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), topNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, -4.0f), topNormal, Color.Yellow), //FTLN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), topNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, -4.0f), topNormal, Color.Yellow), //FTRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f), bottomNormal, Color.Yellow), // Bottom FBLN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, 4.0f), bottomNormal, Color.Yellow), //BBRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, 4.0f), bottomNormal, Color.Yellow), //BBLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f),bottomNormal, Color.Yellow), //FBLN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, -4.0f), bottomNormal, Color.Yellow), //FBRN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, 4.0f), bottomNormal, Color.Yellow), //BBRN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f), leftNormal, Color.Yellow), // Left FBLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, 4.0f), leftNormal, Color.Yellow), //BBLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, 4.0f), leftNormal, Color.Yellow), //BTLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, -4.0f, -4.0f), leftNormal, Color.Yellow), //FBLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, 4.0f), leftNormal, Color.Yellow), //BTLN
                    new VertexPositionNormalColor(new Vector3(-4.0f, 4.0f, -4.0f), leftNormal, Color.Yellow), //FTLN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, -4.0f), rightNormal, Color.Yellow), // Right FBRN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), rightNormal, Color.Yellow), //BTRN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, 4.0f), rightNormal, Color.Yellow), //BBRN
                    new VertexPositionNormalColor(new Vector3(4.0f, -4.0f, -4.0f), rightNormal, Color.Yellow), //FBRN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, -4.0f), rightNormal, Color.Yellow), //FTRN
                    new VertexPositionNormalColor(new Vector3(4.0f, 4.0f, 4.0f), rightNormal, Color.Yellow), //BTRN
                });

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;

        }

        public Vector3 getLightDirection(){
            return this.lightdirection;
        }

        public Vector3 getSpecular()
        {
            return this.specularcolour;
        }

        public Vector3 getAmbient()
        {
            return this.ambientcolour;
        }

        public Vector3 getDiffuse()
        {
            return this.diffusecolour;
        }

        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            basicEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            float sunxpos = worldsize/2 - (1.1f*worldsize/2 *(float)Math.Cos(time));
            float sunypos = -worldsize/2 * (float)Math.Sin(time);
            basicEffect.World = Matrix.Translation(sunxpos, sunypos, worldsize / 2);

            //Change global lighting values
            ambientcolour = new Vector3(0.1f, 0.1f, 0.1f);
            specularcolour = new Vector3(0.1f, 0.1f, 0.166f);
            diffusecolour = new Vector3(0.6f, 0.6f, 0.6f);
            lightdirection.X = (float)Math.Cos(time);
            lightdirection.Y = (float)Math.Sin(time);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the sun
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
