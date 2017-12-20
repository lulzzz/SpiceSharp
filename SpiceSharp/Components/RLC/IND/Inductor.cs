﻿using SpiceSharp.Circuits;
using SpiceSharp.Behaviors.IND;

namespace SpiceSharp.Components
{
    /// <summary>
    /// An inductor
    /// </summary>
    [SpicePins("L+", "L-")]
    public class Inductor : Component
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public int INDposNode { get; internal set; }
        public int INDnegNode { get; internal set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int INDpinCount = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the inductor</param>
        public Inductor(Identifier name)
            : base(name, INDpinCount)
        {
            RegisterBehavior(new LoadBehavior());
            RegisterBehavior(new TransientBehavior());
            RegisterBehavior(new AcBehavior());
            RegisterBehavior(new AcceptBehavior());
            RegisterBehavior(new TruncateBehavior());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the inductor</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="ind">The inductance</param>
        public Inductor(Identifier name, Identifier pos, Identifier neg, double ind) 
            : base(name, INDpinCount)
        {
            // Register behaviors
            RegisterBehavior(new LoadBehavior());
            RegisterBehavior(new TransientBehavior(ind));
            RegisterBehavior(new AcBehavior());
            RegisterBehavior(new AcceptBehavior());
            RegisterBehavior(new TruncateBehavior());

            // Connect
            Connect(pos, neg);
        }

        /// <summary>
        /// Setup the inductor
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            var nodes = BindNodes(ckt);
            INDposNode = nodes[0].Index;
            INDnegNode = nodes[1].Index;
        }
    }
}
