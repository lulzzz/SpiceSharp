using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;
using SpiceSharp.Simulations;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Truncate behavior for a <see cref="BSIM1"/>
    /// </summary>
    public class BSIM1TruncateBehavior : TruncateBehavior
    {
        private BSIM1 bsim1;

        /// <summary>
        /// Setup the behaviour
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        public override void Setup(Entity component, Circuit ckt)
        {
            base.Setup(component, ckt);
            bsim1 = (BSIM1)component;
        }

        /// <summary>
        /// Truncate
        /// </summary>
        /// <param name="sim">Simulation</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(TimeSimulation sim, ref double timestep)
        {
            var method = sim.Method;
            method.Terr(bsim1.B1states + BSIM1.B1qb, sim, ref timestep);
            method.Terr(bsim1.B1states + BSIM1.B1qg, sim, ref timestep);
            method.Terr(bsim1.B1states + BSIM1.B1qd, sim, ref timestep);
        }
    }
}
