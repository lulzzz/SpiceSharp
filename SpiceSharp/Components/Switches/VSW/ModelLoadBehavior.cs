﻿using SpiceSharp.Circuits;
using SpiceSharp.Attributes;

namespace SpiceSharp.Behaviors.VSW
{
    /// <summary>
    /// Load behavior for a <see cref="Components.VoltageSwitchModel"/>
    /// </summary>
    public class ModelLoadBehavior : Behaviors.LoadBehavior
    {
        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("ron"), SpiceInfo("Resistance when closed")]
        public Parameter VSWon { get; } = new Parameter();
        [SpiceName("roff"), SpiceInfo("Resistance when off")]
        public Parameter VSWoff { get; } = new Parameter();
        [SpiceName("vt"), SpiceInfo("Threshold voltage")]
        public Parameter VSWthresh { get; } = new Parameter();
        [SpiceName("vh"), SpiceInfo("Hysteresis voltage")]
        public Parameter VSWhyst { get; } = new Parameter();
        [SpiceName("gon"), SpiceInfo("Conductance when closed")]
        public double VSWonConduct { get; private set; }
        [SpiceName("goff"), SpiceInfo("Conductance when closed")]
        public double VSWoffConduct { get; private set; }

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        /// <returns></returns>
        public override void Setup(Entity component, Circuit ckt)
        {
            if (!VSWon.Given)
            {
                VSWonConduct = 1.0;
                VSWon.Value = 1.0;
            }
            else
                VSWonConduct = 1.0 / VSWon.Value;

            if (!VSWoff.Given)
            {
                VSWoffConduct = ckt.State.Gmin;
                VSWoff.Value = 1.0 / VSWoffConduct;
            }
            else
                VSWoffConduct = 1.0 / VSWoff.Value;
        }

        /// <summary>
        /// Load behavior
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Load(Circuit ckt)
        {
            // Do nothing
        }
    }
}
