﻿using System;
using SpiceSharp.Behaviors;

namespace SpiceSharp.Components.ComponentBehaviors
{
    /// <summary>
    /// AC behaviour for a <see cref="CurrentSwitch"/>
    /// </summary>
    public class CurrentSwitchAcBehavior : CircuitObjectBehaviorAcLoad
    {
        /// <summary>
        /// Execute behaviour
        /// </summary>
        /// <param name="ckt"></param>
        public override void Execute(Circuit ckt)
        {
            var csw = ComponentTyped<CurrentSwitch>();
            CurrentSwitchModel model = csw.Model as CurrentSwitchModel;
            double current_state;
            double g_now;
            var state = ckt.State;
            var cstate = state.Complex;

            // Get the current state
            current_state = state.States[0][csw.CSWstate];
            g_now = current_state > 0.0 ? model.CSWonConduct : model.CSWoffConduct;

            // Load the Y-matrix
            cstate.Matrix[csw.CSWposNode, csw.CSWposNode] += g_now;
            cstate.Matrix[csw.CSWposNode, csw.CSWnegNode] -= g_now;
            cstate.Matrix[csw.CSWnegNode, csw.CSWposNode] -= g_now;
            cstate.Matrix[csw.CSWnegNode, csw.CSWnegNode] += g_now;
        }
    }
}