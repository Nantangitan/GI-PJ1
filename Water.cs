﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Water : ColoredGameObject
    {
        //Local Variable declarations
        private Project1Game gameaccess;
        public Water(Project1Game game){

            int max = (int)Math.Pow(2,game.scale)+1;

            Vector3 surfacenormal = new Vector3(0, 1, 0);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                {
                    new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), surfacenormal, Color.Blue), // Front FBLN
                    new VertexPositionNormalColor(new Vector3(0f, 0f, max), surfacenormal, Color.Blue), //FTLN
                    new VertexPositionNormalColor(new Vector3(max, 0f, max), surfacenormal, Color.Blue), //FTRN
                    new VertexPositionNormalColor(new Vector3(max, 0f, max), surfacenormal, Color.Blue), //FBLN
                    new VertexPositionNormalColor(new Vector3(max, 0f, 0f), surfacenormal, Color.Blue), //FTRN
                    new VertexPositionNormalColor(new Vector3(0f, 0f, 0f), surfacenormal, Color.Blue), //FBRN)
                });

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

        //We use a custom update method
        public override void Update(GameTime gametime)
        {
            throw new NotImplementedException();
        }

        //Update including light
        public void Update(GameTime gameTime, Vector3 light)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            basicEffect.AmbientLightColor = gameaccess.ambient();


            basicEffect.Alpha = 0.75f;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = gameaccess.diffuse();
            basicEffect.DirectionalLight0.Direction = light;
            basicEffect.DirectionalLight0.SpecularColor = gameaccess.specular();
        }

        //DRAW!
        public override void Draw(GameTime gametime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);

            // Apply the basic effect technique and draw the water
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
