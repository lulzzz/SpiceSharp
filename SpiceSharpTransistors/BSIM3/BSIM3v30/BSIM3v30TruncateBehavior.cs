using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

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
        /// <param name="ckt">Circuit</param>
        /// <param name="timeStep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timeStep)
        {
            var method = ckt.Method;
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qb, ckt, ref timeStep);
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qg, ckt, ref timeStep);
            method.Terr(bsim3.BSIM3states + BSIM3v30.BSIM3qd, ckt, ref timeStep);
        }
    }
}
