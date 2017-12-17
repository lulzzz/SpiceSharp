using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

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
        /// <param name="ckt">Circuit</param>
        /// <param name="timeStep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timeStep)
        {
            var method = ckt.Method;
            method.Terr(bsim2.B2states + BSIM2.B2qb, ckt, ref timeStep);
            method.Terr(bsim2.B2states + BSIM2.B2qg, ckt, ref timeStep);
            method.Terr(bsim2.B2states + BSIM2.B2qd, ckt, ref timeStep);
        }
    }
}
