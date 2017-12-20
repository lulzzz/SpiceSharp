using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;
using SpiceSharp.Simulations;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Truncate behavior for a <see cref="BSIM4v80"/>
    /// </summary>
    public class BSIM4v80TruncateBehavior : TruncateBehavior
    {
        private BSIM4v80 bsim4;

        public override void Setup(Entity component, Circuit ckt)
        {
            //TODO: improve it after finish of the refactor
            base.Setup(component, ckt);
            bsim4 = (BSIM4v80)component;
        }

        /// <summary>
        /// Truncate the timestep
        /// </summary>
        /// <param name="sim">Simulation</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(TimeSimulation sim, ref double timestep)
        {
            var method = sim.Method;
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qb, sim, ref timestep);
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qg, sim, ref timestep);
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qd, sim, ref timestep);
            if (bsim4.BSIM4trnqsMod != 0)
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qcdump, sim, ref timestep);
            if (bsim4.BSIM4rbodyMod != 0)
            {
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qbs, sim, ref timestep);
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qbd, sim, ref timestep);
            }
            if (bsim4.BSIM4rgateMod == 3)
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qgmid, sim, ref timestep);
        }
    }
}
