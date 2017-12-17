using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// Truncate behavior for a <see cref="BSIM3v24"/>
    /// </summary>
    public class BSIM3v24TruncateBehavior : TruncateBehavior
    {
        private BSIM3v24 bsim3;

        public override void Setup(Entity component, Circuit ckt)
        {
            //TODO: improve it after finish of the refactor
            base.Setup(component, ckt);
            bsim3 = (BSIM3v24)component;
        }

        /// <summary>
        /// Truncate
        /// </summary>
        /// <param name="ckt">Circuit</param>
        /// <param name="timeStep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timeStep)
        {
            var method = ckt.Method;
            method.Terr(bsim3.BSIM3states + BSIM3v24.BSIM3qb, ckt, ref timeStep);
            method.Terr(bsim3.BSIM3states + BSIM3v24.BSIM3qg, ckt, ref timeStep);
            method.Terr(bsim3.BSIM3states + BSIM3v24.BSIM3qd, ckt, ref timeStep);
        }
    }
}
