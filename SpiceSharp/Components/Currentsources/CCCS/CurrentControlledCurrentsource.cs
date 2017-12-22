﻿using SpiceSharp.Circuits;
using SpiceSharp.Attributes;
using SpiceSharp.Diagnostics;
using SpiceSharp.Behaviors.CCCS;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A current-controlled current source
    /// </summary>
    [SpicePins("F+", "F-"), ConnectedPins()]
    public class CurrentControlledCurrentsource : Component
    {
        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("control"), SpiceInfo("Name of the controlling source")]
        public Identifier CCCScontName { get; set; }

        /// <summary>
        /// Nodes
        /// </summary>
        [SpiceName("pos_node"), SpiceInfo("Positive node of the source")]
        public int CCCSposNode { get; private set; }
        [SpiceName("neg_node"), SpiceInfo("Negative node of the source")]
        public int CCCSnegNode { get; private set; }
        public Voltagesource CCCScontSource { get; protected set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int CCCSpinCount = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the current controlled current source</param>
        public CurrentControlledCurrentsource(Identifier name) : base(name, CCCSpinCount)
        {
            // Make sure the current controlled current source happens after voltage sources
            Priority = -1;
            RegisterBehavior(new LoadBehavior());
            RegisterBehavior(new AcBehavior());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the current controlled current source</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="vsource">The name of the voltage source</param>
        /// <param name="gain">The current gain</param>
        public CurrentControlledCurrentsource(Identifier name, Identifier pos, Identifier neg, Identifier vsource, double gain)
            : this(name)
        {
            // Register behaviors
            Priority = -1;
            RegisterBehavior(new LoadBehavior(gain));
            RegisterBehavior(new AcBehavior());

            // Connect
            Connect(pos, neg);
            CCCScontName = vsource;
        }

        /// <summary>
        /// Setup the current controlled current source
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            var nodes = BindNodes(ckt);
            CCCSposNode = nodes[0].Index;
            CCCSnegNode = nodes[1].Index;

            // Find the voltage source for which the current is being measured
            if (ckt.Objects[CCCScontName] is Voltagesource vsrc)
                CCCScontSource = vsrc;
            else
                throw new CircuitException($"{Name}: Could not find voltage source '{CCCScontName}'");
        }
    }
}
