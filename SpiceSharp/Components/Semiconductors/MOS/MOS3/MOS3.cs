﻿using SpiceSharp.Circuits;
using SpiceSharp.Diagnostics;
using SpiceSharp.Parameters;
using SpiceSharp.Sparse;
using SpiceSharp.Behaviors;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A MOS3 Mosfet
    /// Level 3, a semi-empirical model(see reference for level 3).
    /// </summary>
    [SpicePins("Drain", "Gate", "Source", "Bulk"), ConnectedPins(0, 2, 3)]
    public class MOS3 : CircuitComponent
    {
        /// <summary>
        /// Register default behaviors
        /// </summary>
        public override void RegisterBehaviors()
        {
            this.RegisterBehavior(typeof(ComponentBehaviors.MOS3TemperatureBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.MOS3LoadBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.MOS3AcBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.MOS3NoiseBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.MOS3TruncateBehavior));
        }

        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("w"), SpiceInfo("Width")]
        public Parameter MOS3w { get; } = new Parameter(1e-4);
        [SpiceName("l"), SpiceInfo("Length")]
        public Parameter MOS3l { get; } = new Parameter(1e-4);
        [SpiceName("as"), SpiceInfo("Source area")]
        public Parameter MOS3sourceArea { get; } = new Parameter();
        [SpiceName("ad"), SpiceInfo("Drain area")]
        public Parameter MOS3drainArea { get; } = new Parameter();
        [SpiceName("ps"), SpiceInfo("Source perimeter")]
        public Parameter MOS3sourcePerimiter { get; } = new Parameter();
        [SpiceName("pd"), SpiceInfo("Drain perimeter")]
        public Parameter MOS3drainPerimiter { get; } = new Parameter();
        [SpiceName("nrs"), SpiceInfo("Source squares")]
        public Parameter MOS3sourceSquares { get; } = new Parameter(1);
        [SpiceName("nrd"), SpiceInfo("Drain squares")]
        public Parameter MOS3drainSquares { get; } = new Parameter(1);
        [SpiceName("off"), SpiceInfo("Device initially off")]
        public bool MOS3off { get; set; }
        [SpiceName("icvbs"), SpiceInfo("Initial B-S voltage")]
        public Parameter MOS3icVBS { get; } = new Parameter();
        [SpiceName("icvds"), SpiceInfo("Initial D-S voltage")]
        public Parameter MOS3icVDS { get; } = new Parameter();
        [SpiceName("icvgs"), SpiceInfo("Initial G-S voltage")]
        public Parameter MOS3icVGS { get; } = new Parameter();
        [SpiceName("temp"), SpiceInfo("Instance operating temperature")]
        public double MOS3_TEMP
        {
            get => MOS3temp - Circuit.CONSTCtoK;
            set => MOS3temp.Set(value + Circuit.CONSTCtoK);
        }
        public Parameter MOS3temp { get; } = new Parameter();
        [SpiceName("dnode"), SpiceInfo("Number of drain node")]
        public int MOS3dNode { get; internal set; }
        [SpiceName("gnode"), SpiceInfo("Number of gate node")]
        public int MOS3gNode { get; internal set; }
        [SpiceName("snode"), SpiceInfo("Number of source node")]
        public int MOS3sNode { get; internal set; }
        [SpiceName("bnode"), SpiceInfo("Number of bulk node")]
        public int MOS3bNode { get; internal set; }
        [SpiceName("dnodeprime"), SpiceInfo("Number of internal drain node")]
        public int MOS3dNodePrime { get; internal set; }
        [SpiceName("snodeprime"), SpiceInfo("Number of internal source node")]
        public int MOS3sNodePrime { get; internal set; }
        [SpiceName("sourceconductance"), SpiceInfo("Source conductance")]
        public double MOS3sourceConductance { get; internal set; }
        [SpiceName("drainconductance"), SpiceInfo("Drain conductance")]
        public double MOS3drainConductance { get; internal set; }
        [SpiceName("von"), SpiceInfo("Turn-on voltage")]
        public double MOS3von { get; internal set; }
        [SpiceName("vdsat"), SpiceInfo("Saturation drain voltage")]
        public double MOS3vdsat { get; internal set; }
        [SpiceName("sourcevcrit"), SpiceInfo("Critical source voltage")]
        public double MOS3sourceVcrit { get; internal set; }
        [SpiceName("drainvcrit"), SpiceInfo("Critical drain voltage")]
        public double MOS3drainVcrit { get; internal set; }
        [SpiceName("id"), SpiceName("cd"), SpiceInfo("Drain current")]
        public double MOS3cd { get; internal set; }
        [SpiceName("ibs"), SpiceInfo("B-S junction current")]
        public double MOS3cbs { get; internal set; }
        [SpiceName("ibd"), SpiceInfo("B-D junction current")]
        public double MOS3cbd { get; internal set; }
        [SpiceName("gmb"), SpiceName("gmbs"), SpiceInfo("Bulk-Source transconductance")]
        public double MOS3gmbs { get; internal set; }
        [SpiceName("gm"), SpiceInfo("Transconductance")]
        public double MOS3gm { get; internal set; }
        [SpiceName("gds"), SpiceInfo("Drain-Source conductance")]
        public double MOS3gds { get; internal set; }
        [SpiceName("gbd"), SpiceInfo("Bulk-Drain conductance")]
        public double MOS3gbd { get; internal set; }
        [SpiceName("gbs"), SpiceInfo("Bulk-Source conductance")]
        public double MOS3gbs { get; internal set; }
        [SpiceName("cbd"), SpiceInfo("Bulk-Drain capacitance")]
        public double MOS3capbd { get; internal set; }
        [SpiceName("cbs"), SpiceInfo("Bulk-Source capacitance")]
        public double MOS3capbs { get; internal set; }
        [SpiceName("cbd0"), SpiceInfo("Zero-Bias B-D junction capacitance")]
        public double MOS3Cbd { get; internal set; }
        [SpiceName("cbdsw0"), SpiceInfo("Zero-Bias B-D sidewall capacitance")]
        public double MOS3Cbdsw { get; internal set; }
        [SpiceName("cbs0"), SpiceInfo("Zero-Bias B-S junction capacitance")]
        public double MOS3Cbs { get; internal set; }
        [SpiceName("cbssw0"), SpiceInfo("Zero-Bias B-S sidewall capacitance")]
        public double MOS3Cbssw { get; internal set; }

        /// <summary>
        /// Methods
        /// </summary>
        [SpiceName("ic"), SpiceInfo("Vector of D-S, G-S, B-S voltages")]
        public void SetIC(double[] value)
        {
            switch (value.Length)
            {
                case 3: MOS3icVBS.Set(value[2]); goto case 2;
                case 2: MOS3icVGS.Set(value[1]); goto case 1;
                case 1: MOS3icVDS.Set(value[0]); break;
                default:
                    throw new CircuitException("Bad parameter");
            }
        }
        [SpiceName("rs"), SpiceInfo("Source resistance")]
        public double GetSOURCERESIST(Circuit ckt)
        {
            if (MOS3sNodePrime != MOS3sNode)
                return 1.0 / MOS3sourceConductance;
            else
                return 0.0;
        }
        [SpiceName("rd"), SpiceInfo("Drain resistance")]
        public double GetDRAINRESIST(Circuit ckt)
        {
            if (MOS3dNodePrime != MOS3dNode)
                return 1.0 / MOS3drainConductance;
            else
                return 0.0;
        }
        [SpiceName("vbd"), SpiceInfo("Bulk-Drain voltage")]
        public double GetVBD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3vbd];
        [SpiceName("vbs"), SpiceInfo("Bulk-Source voltage")]
        public double GetVBS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3vbs];
        [SpiceName("vgs"), SpiceInfo("Gate-Source voltage")]
        public double GetVGS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3vgs];
        [SpiceName("vds"), SpiceInfo("Drain-Source voltage")]
        public double GetVDS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3vds];
        [SpiceName("cgs"), SpiceInfo("Gate-Source capacitance")]
        public double GetCAPGS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3capgs];
        [SpiceName("qgs"), SpiceInfo("Gate-Source charge storage")]
        public double GetQGS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3qgs];
        [SpiceName("cqgs"), SpiceInfo("Capacitance due to gate-source charge storage")]
        public double GetCQGS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3cqgs];
        [SpiceName("cgd"), SpiceInfo("Gate-Drain capacitance")]
        public double GetCAPGD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3capgd];
        [SpiceName("qgd"), SpiceInfo("Gate-Drain charge storage")]
        public double GetQGD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3qgd];
        [SpiceName("cqgd"), SpiceInfo("Capacitance due to gate-drain charge storage")]
        public double GetCQGD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3cqgd];
        [SpiceName("cgb"), SpiceInfo("Gate-Bulk capacitance")]
        public double GetCAPGB(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3capgb];
        [SpiceName("qgb"), SpiceInfo("Gate-Bulk charge storage")]
        public double GetQGB(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3qgb];
        [SpiceName("cqgb"), SpiceInfo("Capacitance due to gate-bulk charge storage")]
        public double GetCQGB(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3cqgb];
        [SpiceName("qbd"), SpiceInfo("Bulk-Drain charge storage")]
        public double GetQBD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3qbd];
        [SpiceName("cqbd"), SpiceInfo("Capacitance due to bulk-drain charge storage")]
        public double GetCQBD(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3cqbd];
        [SpiceName("qbs"), SpiceInfo("Bulk-Source charge storage")]
        public double GetQBS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3qbs];
        [SpiceName("cqbs"), SpiceInfo("Capacitance due to bulk-source charge storage")]
        public double GetCQBS(Circuit ckt) => ckt.State.States[0][MOS3states + MOS3cqbs];
        [SpiceName("ib"), SpiceInfo("Bulk current")]
        public double GetCB(Circuit ckt) => MOS3cbd + MOS3cbs - ckt.State.States[0][MOS3states + MOS3cqgb];
        [SpiceName("ig"), SpiceInfo("Gate current")]
        public double GetCG(Circuit ckt) => ckt.State.UseDC ? 0.0 : ckt.State.States[0][MOS3states + MOS3cqgb] + ckt.State.States[0][MOS3states + MOS3cqgd] +
            ckt.State.States[0][MOS3states + MOS3cqgs];
        [SpiceName("is"), SpiceInfo("Source current")]
        public double GetCS(Circuit ckt)
        {
            double value = -MOS3cd;
            value -= MOS3cbd + MOS3cbs - ckt.State.States[0][MOS3states + MOS3cqgb];
            if (ckt.State.Domain == CircuitState.DomainTypes.Time && !ckt.State.UseDC)
            {
                value -= ckt.State.States[0][MOS3states + MOS3cqgb] + ckt.State.States[0][MOS3states + MOS3cqgd] +
                    ckt.State.States[0][MOS3states + MOS3cqgs];
            }
            return value;
        }
        [SpiceName("p"), SpiceInfo("Instantaneous power")]
        public double GetPOWER(Circuit ckt)
        {
            double temp;

            double value = MOS3cd * ckt.State.Solution[MOS3dNode];
            value += (MOS3cbd + MOS3cbs - ckt.State.States[0][MOS3states + MOS3cqgb]) * ckt.State.Solution[MOS3bNode];
            if (ckt.State.Domain == CircuitState.DomainTypes.Time && !ckt.State.UseDC)
            {
                value += (ckt.State.States[0][MOS3states + MOS3cqgb] + ckt.State.States[0][MOS3states + MOS3cqgd] +
                    ckt.State.States[0][MOS3states + MOS3cqgs]) * ckt.State.Solution[MOS3gNode];
            }
            temp = -MOS3cd;
            temp -= MOS3cbd + MOS3cbs;
            if (ckt.State.Domain == CircuitState.DomainTypes.Time && !ckt.State.UseDC)
            {
                temp -= ckt.State.States[0][MOS3states + MOS3cqgb] + ckt.State.States[0][MOS3states + MOS3cqgd] +
                    ckt.State.States[0][MOS3states + MOS3cqgs];
            }
            value += temp * ckt.State.Solution[MOS3sNode];
            return value;
        }

        /// <summary>
        /// Extra variables
        /// </summary>
        public double MOS3mode { get; internal set; }
        public double MOS3tTransconductance { get; internal set; }
        public double MOS3tSurfMob { get; internal set; }
        public double MOS3tPhi { get; internal set; }
        public double MOS3tVbi { get; internal set; }
        public double MOS3tVto { get; internal set; }
        public double MOS3tSatCur { get; internal set; }
        public double MOS3tSatCurDens { get; internal set; }
        public double MOS3tCbd { get; internal set; }
        public double MOS3tCbs { get; internal set; }
        public double MOS3tCj { get; internal set; }
        public double MOS3tCjsw { get; internal set; }
        public double MOS3tBulkPot { get; internal set; }
        public double MOS3tDepCap { get; internal set; }
        public double MOS3f2d { get; internal set; }
        public double MOS3f3d { get; internal set; }
        public double MOS3f4d { get; internal set; }
        public double MOS3f2s { get; internal set; }
        public double MOS3f3s { get; internal set; }
        public double MOS3f4s { get; internal set; }
        public double MOS3cgs { get; internal set; }
        public double MOS3cgd { get; internal set; }
        public double MOS3cgb { get; internal set; }
        public int MOS3states { get; internal set; }

        /// <summary>
        /// Matrix elements
        /// </summary>
        internal MatrixElement MOS3DdPtr { get; private set; }
        internal MatrixElement MOS3GgPtr { get; private set; }
        internal MatrixElement MOS3SsPtr { get; private set; }
        internal MatrixElement MOS3BbPtr { get; private set; }
        internal MatrixElement MOS3DPdpPtr { get; private set; }
        internal MatrixElement MOS3SPspPtr { get; private set; }
        internal MatrixElement MOS3DdpPtr { get; private set; }
        internal MatrixElement MOS3GbPtr { get; private set; }
        internal MatrixElement MOS3GdpPtr { get; private set; }
        internal MatrixElement MOS3GspPtr { get; private set; }
        internal MatrixElement MOS3SspPtr { get; private set; }
        internal MatrixElement MOS3BdpPtr { get; private set; }
        internal MatrixElement MOS3BspPtr { get; private set; }
        internal MatrixElement MOS3DPspPtr { get; private set; }
        internal MatrixElement MOS3DPdPtr { get; private set; }
        internal MatrixElement MOS3BgPtr { get; private set; }
        internal MatrixElement MOS3DPgPtr { get; private set; }
        internal MatrixElement MOS3SPgPtr { get; private set; }
        internal MatrixElement MOS3SPsPtr { get; private set; }
        internal MatrixElement MOS3DPbPtr { get; private set; }
        internal MatrixElement MOS3SPbPtr { get; private set; }
        internal MatrixElement MOS3SPdpPtr { get; private set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int MOS3vbd = 0;
        public const int MOS3vbs = 1;
        public const int MOS3vgs = 2;
        public const int MOS3vds = 3;
        public const int MOS3capgs = 4;
        public const int MOS3qgs = 5;
        public const int MOS3cqgs = 6;
        public const int MOS3capgd = 7;
        public const int MOS3qgd = 8;
        public const int MOS3cqgd = 9;
        public const int MOS3capgb = 10;
        public const int MOS3qgb = 11;
        public const int MOS3cqgb = 12;
        public const int MOS3qbd = 13;
        public const int MOS3cqbd = 14;
        public const int MOS3qbs = 15;
        public const int MOS3cqbs = 16;
        public const int MOS3pinCount = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the device</param>
        public MOS3(CircuitIdentifier name) : base(name, MOS3pinCount)
        {
        }

        /// <summary>
        /// Setup the device
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            var model = Model as MOS3Model;

            // Allocate nodes
            var nodes = BindNodes(ckt);
            MOS3dNode = nodes[0].Index;
            MOS3gNode = nodes[1].Index;
            MOS3sNode = nodes[2].Index;
            MOS3bNode = nodes[3].Index;

            // Add a series drain node if necessary
            if (model.MOS3drainResistance != 0 || (model.MOS3sheetResistance != 0 && MOS3drainSquares != 0))
                MOS3dNodePrime = CreateNode(ckt, Name.Grow("#drain")).Index;
            else
                MOS3dNodePrime = MOS3dNode;

            // Add a series source node if necessary
            if (model.MOS3sourceResistance != 0 || (model.MOS3sheetResistance != 0 && MOS3sourceSquares != 0))
                MOS3sNodePrime = CreateNode(ckt, Name.Grow("#source")).Index;
            else
                MOS3sNodePrime = MOS3sNode;

            // Get matrix elements
            var matrix = ckt.State.Matrix;
            MOS3DdPtr = matrix.GetElement(MOS3dNode, MOS3dNode);
            MOS3GgPtr = matrix.GetElement(MOS3gNode, MOS3gNode);
            MOS3SsPtr = matrix.GetElement(MOS3sNode, MOS3sNode);
            MOS3BbPtr = matrix.GetElement(MOS3bNode, MOS3bNode);
            MOS3DPdpPtr = matrix.GetElement(MOS3dNodePrime, MOS3dNodePrime);
            MOS3SPspPtr = matrix.GetElement(MOS3sNodePrime, MOS3sNodePrime);
            MOS3DdpPtr = matrix.GetElement(MOS3dNode, MOS3dNodePrime);
            MOS3GbPtr = matrix.GetElement(MOS3gNode, MOS3bNode);
            MOS3GdpPtr = matrix.GetElement(MOS3gNode, MOS3dNodePrime);
            MOS3GspPtr = matrix.GetElement(MOS3gNode, MOS3sNodePrime);
            MOS3SspPtr = matrix.GetElement(MOS3sNode, MOS3sNodePrime);
            MOS3BdpPtr = matrix.GetElement(MOS3bNode, MOS3dNodePrime);
            MOS3BspPtr = matrix.GetElement(MOS3bNode, MOS3sNodePrime);
            MOS3DPspPtr = matrix.GetElement(MOS3dNodePrime, MOS3sNodePrime);
            MOS3DPdPtr = matrix.GetElement(MOS3dNodePrime, MOS3dNode);
            MOS3BgPtr = matrix.GetElement(MOS3bNode, MOS3gNode);
            MOS3DPgPtr = matrix.GetElement(MOS3dNodePrime, MOS3gNode);
            MOS3SPgPtr = matrix.GetElement(MOS3sNodePrime, MOS3gNode);
            MOS3SPsPtr = matrix.GetElement(MOS3sNodePrime, MOS3sNode);
            MOS3DPbPtr = matrix.GetElement(MOS3dNodePrime, MOS3bNode);
            MOS3SPbPtr = matrix.GetElement(MOS3sNodePrime, MOS3bNode);
            MOS3SPdpPtr = matrix.GetElement(MOS3sNodePrime, MOS3dNodePrime);

            // Allocate states
            MOS3states = ckt.State.GetState(17);
        }

        /// <summary>
        /// Unsetup
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Unsetup(Circuit ckt)
        {
            // Remove references
            MOS3DdPtr = null;
            MOS3GgPtr = null;
            MOS3SsPtr = null;
            MOS3BbPtr = null;
            MOS3DPdpPtr = null;
            MOS3SPspPtr = null;
            MOS3DdpPtr = null;
            MOS3GbPtr = null;
            MOS3GdpPtr = null;
            MOS3GspPtr = null;
            MOS3SspPtr = null;
            MOS3BdpPtr = null;
            MOS3BspPtr = null;
            MOS3DPspPtr = null;
            MOS3DPdPtr = null;
            MOS3BgPtr = null;
            MOS3DPgPtr = null;
            MOS3SPgPtr = null;
            MOS3SPsPtr = null;
            MOS3DPbPtr = null;
            MOS3SPbPtr = null;
            MOS3SPdpPtr = null;
        }
    }
}
