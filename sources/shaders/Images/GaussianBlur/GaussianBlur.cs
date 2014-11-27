﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core;

namespace SiliconStudio.Paradox.Effects.Images
{
    /// <summary>
    /// Provides a gaussian blur effect.
    /// </summary>
    /// <remarks>
    /// To improve performance of this gaussian blur is using:
    /// - a separable 1D horizontal and vertical blur
    /// - linear filtering to reduce the number of taps
    /// </remarks>
    public class GaussianBlur : ImageEffectBase
    {
        private readonly ImageEffect blurH;
        private readonly ImageEffect blurV;

        /// <summary>
        /// Shared parameters used by both by the Horizontal and Vertical pass.
        /// </summary>
        private readonly ParameterCollection sharedParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GaussianBlur(ImageEffectContext context)
            : base(context)
        {
            sharedParameters = new ParameterCollection();

            // Use shared Parameters for blurH and blurV
            blurH = new ImageEffect(context, "GaussianBlurEffect", sharedParameters).DisposeBy(this);
            // Setup specific Horizontal parameter for blurH
            blurH.Parameters.Set(GaussianBlurKeys.VerticalBlur, false);

            blurV = new ImageEffect(context, "GaussianBlurEffect", sharedParameters).DisposeBy(this);
            // Setup specific Vertical parameter for blurV
            blurV.Parameters.Set(GaussianBlurKeys.VerticalBlur, true);

            Radius = 4;
            SigmaRatio = 2.0f;
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public int Radius { get; set; }

        /// <summary>
        /// Gets or sets the sigma ratio. The sigma ratio is used to calculate the sigma based on the radius: The actual
        /// formula is <c>sigma = radius / SigmaRatio</c>. The default value is 2.0f.
        /// </summary>
        /// <value>The sigma ratio.</value>
        public float SigmaRatio { get; set; }

        protected override void DrawCore()
        {
            // Input texture
            var inputTexture = GetSafeInput(0);
            var outputHorizontal = Context.GetTemporaryRenderTarget2D(inputTexture.Description);
            // Update shared parameters
            sharedParameters.Set(GaussianBlurKeys.Radius, Radius);
            sharedParameters.Set(GaussianBlurKeys.SigmaRatio, SigmaRatio);

            // Horizontal pass
            blurH.SetInput(inputTexture);
            blurH.SetOutput(outputHorizontal);
            var size = Radius * 2 + 1;
            blurH.Draw("GaussianBlurH{0}x{0}", size);

            // Vertical pass
            blurV.SetInput(outputHorizontal);
            blurV.SetOutput(GetSafeOutput(0));
            blurV.Draw("GaussianBlurV{0}x{0}", size);

            Context.ReleaseTemporaryTexture(outputHorizontal);            
        }
    }
}