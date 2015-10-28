﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Threading.Tasks;

using NUnit.Framework;

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Effects;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.EntityModel;
using SiliconStudio.Xenko.Extensions;

namespace SiliconStudio.Xenko.Graphics.Tests
{
    public class TestCubemapDisplay : TestGameBase
    {
        private Entity mainCamera;
        private float cameraDistance = 3;
        private float cameraHeight = 3;
        private Vector3 cameraUp = Vector3.UnitY;

        public TestCubemapDisplay()
        {
            GraphicsDeviceManager.PreferredGraphicsProfile = new[] { GraphicsProfile.Level_9_1 };
        }

        protected override async Task LoadContent()
        {
            await base.LoadContent();

            CreatePipeline();

            var material = Asset.Load<Material>("BasicMaterial");
            var textureCube = Asset.Load<Texture>("uv_cube");
            material.Parameters.Set(TexturingKeys.TextureCube0, textureCube);
            var mesh = new Mesh()
            {
                Draw = GeometricPrimitive.GeoSphere.New(GraphicsDevice).ToMeshDraw(),
                Material = material
            };
            mesh.Parameters.Set(RenderingParameters.RenderGroup, RenderGroups.Group1);

            var entity = new Entity()
            {
                new ModelComponent()
                {
                    Model = new Model() { mesh }
                },
                new TransformationComponent()
            };
            
            Entities.Add(entity);

            var mainCameraTargetEntity = new Entity(Vector3.Zero);
            Entities.Add(mainCameraTargetEntity);
            mainCamera = new Entity()
            {
                new CameraComponent
                {
                    AspectRatio = 8/4.8f,
                    FarPlane = 1000,
                    NearPlane = 1,
                    VerticalFieldOfView = 0.6f,
                    Target = mainCameraTargetEntity,
                    TargetUp = cameraUp,
                },
                new TransformationComponent
                {
                    Translation = cameraDistance * Vector3.UnitX
                }
            };
            Entities.Add(mainCamera);

            RenderSystem.Pipeline.SetCamera(mainCamera.Get<CameraComponent>());

            // Add a custom script
            Script.Add(GameScript1);
        }

        private void CreatePipeline()
        {
            RenderSystem.Pipeline.Renderers.Add(new CameraSetter(Services));
            RenderSystem.Pipeline.Renderers.Add(new RenderTargetSetter(Services) { ClearColor = Color.CornflowerBlue });
            RenderSystem.Pipeline.Renderers.Add(new ModelRenderer(Services, "CubemapDisplayEffect"));
        }

        private async Task GameScript1()
        {
            while (IsRunning)
            {
                // Wait next rendering frame
                await Script.NextFrame();

                var angle = Math.PI * UpdateTime.Total.TotalMilliseconds / 5000;
                mainCamera.Transform.Translation = new Vector3((float)(cameraDistance * Math.Cos(angle)), cameraHeight, (float)(cameraDistance * Math.Sin(angle)));
            }
        }

        public static void Main()
        {
            using (var game = new TestCubemapDisplay())
                game.Run();
        }

        /// <summary>
        /// Run the test
        /// </summary>
        [Test]
        public void RunCubemapRendering()
        {
            RunGameTest(new TestCubemapDisplay());
        }
    }
}