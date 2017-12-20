﻿using SpiceSharp.Circuits;
using SpiceSharp.Simulations;

namespace SpiceSharp.Behaviors.DIO
{
    /// <summary>
    /// Truncate behavior for a <see cref="Components.Diode"/>
    /// </summary>
    public class TruncateBehavior : Behaviors.TruncateBehavior
    {
        /// <summary>
        /// Necessary behaviors
        /// </summary>
        private LoadBehavior load;

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        /// <returns></returns>
        public override void Setup(Entity component, Circuit ckt)
        {
            // Get behaviors
            load = GetBehavior<LoadBehavior>(component);
        }

        /// <summary>
        /// Truncate the timestep
        /// </summary>
        /// <param name="sim">Simulation</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(TimeSimulation sim, ref double timestep)
        {
            sim.Circuit.Method.Terr(load.DIOstate + LoadBehavior.DIOcapCharge, sim, ref timestep);
        }
    }
}
