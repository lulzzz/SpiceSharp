using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;
using SpiceSharp.Simulations;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Truncate behavior for a <see cref="BSIM3v30"/>
    /// </summary>
    public class BSIM3v30TruncateBehavior : TruncateBehavior
    {
        private BSIM3v30 bsim3;

        public override void Setup(Entity component, Circuit ckt)
        {
            //TODO: improve it after finish of the refactor
            base.Setup(component, ckt);
            bsim3 = (BSIM3v30)component;
        }

        /// <summary>
        /// Truncate the timestep
        /// </summary>
        /// <param name="sim">Simulation</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(TimeSimulation sim, ref double timestep)
        {
            var method = sim.Method;
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qb, sim, ref timestep);
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qg, sim, ref timestep);
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qd, sim, ref timestep);
        }
    }
}
