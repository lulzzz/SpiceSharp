using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

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
        /// <param name="ckt">Circuit</param>
        /// <param name="timeStep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timeStep)
        {
            var method = ckt.Method;
            method.Terr(bsim1.B1states + BSIM1.B1qb, ckt, ref timeStep);
            method.Terr(bsim1.B1states + BSIM1.B1qg, ckt, ref timeStep);
            method.Terr(bsim1.B1states + BSIM1.B1qd, ckt, ref timeStep);
        }
    }
}
