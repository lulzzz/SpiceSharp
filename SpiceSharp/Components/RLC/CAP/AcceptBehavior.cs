﻿using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

namespace SpiceSharp.Components.CAP
{
    /// <summary>
    /// Accept behavior for capacitances
    /// </summary>
    public class AcceptBehavior : CircuitObjectBehaviorAccept
    {
        /// <summary>
        /// Necessary behaviors
        /// </summary>
        private LoadBehavior load = null;

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        public override void Setup(CircuitObject component, Circuit ckt)
        {
            // Get behaviors
            load = GetBehavior<LoadBehavior>(component);
        }

        /// <summary>
        /// Accept the current timepoint
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Accept(Circuit ckt)
        {
            // Copy DC states when accepting the first timepoint
            if (ckt.State.Init == CircuitState.InitFlags.InitTransient)
                ckt.State.CopyDC(load.CAPstate + LoadBehavior.CAPqcap);
        }
    }
}