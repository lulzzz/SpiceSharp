﻿using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;

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
        /// <param name="ckt">Circuit</param>
        /// <param name="timestep">Timestep</param>
        public override void Truncate(Circuit ckt, ref double timestep)
        {
            var method = ckt.Method;
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qb, ckt, ref timestep);
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qg, ckt, ref timestep);
            method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qd, ckt, ref timestep);
            if (bsim4.BSIM4trnqsMod != 0)
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qcdump, ckt, ref timestep);
            if (bsim4.BSIM4rbodyMod != 0)
            {
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qbs, ckt, ref timestep);
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qbd, ckt, ref timestep);
            }
            if (bsim4.BSIM4rgateMod == 3)
                method.Terr(bsim4.BSIM4states + BSIM4v80.BSIM4qgmid, ckt, ref timestep);
        }
    }
}
