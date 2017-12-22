﻿using SpiceSharp.Circuits;
using SpiceSharp.Attributes;

namespace SpiceSharp.Behaviors.BJT
{
    /// <summary>
    /// Noise behavior for <see cref="Components.BJTModel"/>
    /// </summary>
    public class ModelNoiseBehavior : Behaviors.NoiseBehavior
    {
        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("kf"), SpiceInfo("Flicker Noise Coefficient")]
        public Parameter BJTfNcoef { get; } = new Parameter();
        [SpiceName("af"), SpiceInfo("Flicker Noise Exponent")]
        public Parameter BJTfNexp { get; } = new Parameter(1);

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        /// <returns></returns>
        public override void Setup(Entity component, Circuit ckt)
        {
            DataOnly = true;
        }

        /// <summary>
        /// Noise behavior
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Noise(Circuit ckt)
        {
            // Do nothing
        }
    }
}
