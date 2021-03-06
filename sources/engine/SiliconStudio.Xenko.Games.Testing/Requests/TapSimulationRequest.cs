// Copyright (c) 2014-2015 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;

namespace SiliconStudio.Xenko.Games.Testing.Requests
{
    [DataContract]
    internal class TapSimulationRequest : TestRequestBase
    {
        public PointerState State;
        public TimeSpan Delta;
        public Vector2 Coords;
        public Vector2 CoordsDelta;
    }
}