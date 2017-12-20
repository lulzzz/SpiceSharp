using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;
using SpiceSharp.Simulations;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Truncate behavior for a <see cref="MOS2"/>
    /// </summary>
    public class BSIM2TruncateBehavior : TruncateBehavior
    {
        private BSIM2 bsim2;

        public override void Setup(Entity component, Circuit ckt)
        {
            //TODO: improve it after finish of the refactor
            base.Setup(component, ckt);
            bsim2 = (BSIM2)component;
        }

        /// <summary>
        /// Truncate
        /// </summary>
        /// <param name="sim">Simulation</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(TimeSimulation sim, ref double timestep)
        {
            var method = sim.Method;
            method.Terr(bsim2.B2states + BSIM2.B2qb, sim, ref timestep);
            method.Terr(bsim2.B2states + BSIM2.B2qg, sim, ref timestep);
            method.Terr(bsim2.B2states + BSIM2.B2qd, sim, ref timestep);
        }
    }
}
