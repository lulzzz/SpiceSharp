﻿using SpiceSharp.Circuits;
using SpiceSharp.Parameters;
using SpiceSharp.Behaviors.MOS1;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A MOS1 Mosfet.
    /// Level 1, Shichman-Hodges.
    /// </summary>
    [SpicePins("Drain", "Gate", "Source", "Bulk"), ConnectedPins(0, 2, 3)]
    public class MOS1 : Component
    {
        /// <summary>
        /// Set the model for the MOS1 Mosfet
        /// </summary>
        public void SetModel(MOS1Model model) => Model = model;

        /// <summary>
        /// Nodes
        /// </summary>
        [SpiceName("dnode"), SpiceInfo("Number of the drain node ")]
        public int MOS1dNode { get; protected set; }
        [SpiceName("gnode"), SpiceInfo("Number of the gate node ")]
        public int MOS1gNode { get; protected set; }
        [SpiceName("snode"), SpiceInfo("Number of the source node ")]
        public int MOS1sNode { get; protected set; }
        [SpiceName("bnode"), SpiceInfo("Number of the node ")]
        public int MOS1bNode { get; protected set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int MOS1pinCount = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the device</param>
        public MOS1(Identifier name) : base(name, MOS1pinCount)
        {
            RegisterBehavior(new TemperatureBehavior());
            RegisterBehavior(new LoadBehavior());
            RegisterBehavior(new AcBehavior());
            RegisterBehavior(new NoiseBehavior());
            RegisterBehavior(new TruncateBehavior());
        }

        protected override void CollectNamedParameters()
        {
            base.CollectNamedParameters();

            foreach (var behavior in Model.Behaviors.Values)
            {
                base.CollectNamedParameters(behavior);
            }
        }

        /// <summary>
        /// Setup the device
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            // Allocate nodes
            var nodes = BindNodes(ckt);
            MOS1dNode = nodes[0].Index;
            MOS1gNode = nodes[1].Index;
            MOS1sNode = nodes[2].Index;
            MOS1bNode = nodes[3].Index;
        }
    }
}
