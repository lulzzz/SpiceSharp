﻿using SpiceSharp.Circuits;

namespace SpiceSharp.Behaviors.DIO
{
    /// <summary>
    /// Truncate behavior for a <see cref="Diode"/>
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
        /// <param name="ckt">Circuit</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timestep)
        {
            ckt.Method.Terr(load.DIOstate + LoadBehavior.DIOcapCharge, ckt, ref timestep);
        }
    }
}