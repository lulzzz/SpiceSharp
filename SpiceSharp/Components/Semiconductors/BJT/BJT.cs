﻿using SpiceSharp.Circuits;
using SpiceSharp.Diagnostics;
using SpiceSharp.Parameters;
using SpiceSharp.Sparse;
using SpiceSharp.Behaviors;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A bipolar junction transistor (BJT)
    /// </summary>
    [SpicePins("Collector", "Base", "Emitter", "Substrate")]
    public class BJT : CircuitComponent
    {
        /// <summary>
        /// Register default behaviors
        /// </summary>
        public override void RegisterBehaviors()
        {
            this.RegisterBehavior(typeof(ComponentBehaviors.BJTTemperatureBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.BJTLoadBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.BJTAcBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.BJTNoiseBehavior));
            this.RegisterBehavior(typeof(ComponentBehaviors.BJTTruncateBehavior));
        }

        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("area"), SpiceInfo("Area factor")]
        public Parameter BJTarea { get; } = new Parameter(1);
        [SpiceName("temp"), SpiceInfo("Instance temperature")]
        public double BJT_TEMP
        {
            get => BJTtemp - Circuit.CONSTCtoK;
            set => BJTtemp.Set(value + Circuit.CONSTCtoK);
        }
        public Parameter BJTtemp { get; } = new Parameter(300.15);
        [SpiceName("off"), SpiceInfo("Device initially off")]
        public bool BJToff { get; set; }
        [SpiceName("icvbe"), SpiceInfo("Initial B-E voltage")]
        public Parameter BJTicVBE { get; } = new Parameter();
        [SpiceName("icvce"), SpiceInfo("Initial C-E voltage")]
        public Parameter BJTicVCE { get; } = new Parameter();
        [SpiceName("sens_area"), SpiceInfo("flag to request sensitivity WRT area")]
        public bool BJTsenParmNo { get; set; }
        [SpiceName("colnode"), SpiceInfo("Number of collector node")]
        public int BJTcolNode { get; private set; }
        [SpiceName("basenode"), SpiceInfo("Number of base node")]
        public int BJTbaseNode { get; private set; }
        [SpiceName("emitnode"), SpiceInfo("Number of emitter node")]
        public int BJTemitNode { get; private set; }
        [SpiceName("substnode"), SpiceInfo("Number of substrate node")]
        public int BJTsubstNode { get; private set; }
        [SpiceName("colprimenode"), SpiceInfo("Internal collector node")]
        public int BJTcolPrimeNode { get; private set; }
        [SpiceName("baseprimenode"), SpiceInfo("Internal base node")]
        public int BJTbasePrimeNode { get; private set; }
        [SpiceName("emitprimenode"), SpiceInfo("Internal emitter node")]
        public int BJTemitPrimeNode { get; private set; }
        [SpiceName("cpi"), SpiceInfo("Internal base to emitter capactance")]
        public double BJTcapbe { get; internal set; }
        [SpiceName("cmu"), SpiceInfo("Internal base to collector capactiance")]
        public double BJTcapbc { get; internal set; }
        [SpiceName("cbx"), SpiceInfo("Base to collector capacitance")]
        public double BJTcapbx { get; internal set; }
        [SpiceName("ccs"), SpiceInfo("Collector to substrate capacitance")]
        public double BJTcapcs { get; internal set; }

        /// <summary>
        /// Methods
        /// </summary>
        [SpiceName("ic"), SpiceInfo("Initial condition vector")]
        public void SetIC(double[] value)
        {
            switch (value.Length)
            {
                case 2: BJTicVCE.Set(value[1]); goto case 1;
                case 1: BJTicVBE.Set(value[0]); break;
                default:
                    throw new CircuitException("Bad parameter");
            }
        }
        [SpiceName("vbe"), SpiceInfo("B-E voltage")]
        public double GetVBE(Circuit ckt) => ckt.State.States[0][BJTstate + BJTvbe];
        [SpiceName("vbc"), SpiceInfo("B-C voltage")]
        public double GetVBC(Circuit ckt) => ckt.State.States[0][BJTstate + BJTvbc];
        [SpiceName("cc"), SpiceInfo("Current at collector node")]
        public double GetCC(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcc];
        [SpiceName("cb"), SpiceInfo("Current at base node")]
        public double GetCB(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcb];
        [SpiceName("gpi"), SpiceInfo("Small signal input conductance - pi")]
        public double GetGPI(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgpi];
        [SpiceName("gmu"), SpiceInfo("Small signal conductance - mu")]
        public double GetGMU(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgmu];
        [SpiceName("gm"), SpiceInfo("Small signal transconductance")]
        public double GetGM(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgm];
        [SpiceName("go"), SpiceInfo("Small signal output conductance")]
        public double GetGO(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgo];
        [SpiceName("qbe"), SpiceInfo("Charge storage B-E junction")]
        public double GetQBE(Circuit ckt) => ckt.State.States[0][BJTstate + BJTqbe];
        [SpiceName("cqbe"), SpiceInfo("Cap. due to charge storage in B-E jct.")]
        public double GetCQBE(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcqbe];
        [SpiceName("qbc"), SpiceInfo("Charge storage B-C junction")]
        public double GetQBC(Circuit ckt) => ckt.State.States[0][BJTstate + BJTqbc];
        [SpiceName("cqbc"), SpiceInfo("Cap. due to charge storage in B-C jct.")]
        public double GetCQBC(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcqbc];
        [SpiceName("qcs"), SpiceInfo("Charge storage C-S junction")]
        public double GetQCS(Circuit ckt) => ckt.State.States[0][BJTstate + BJTqcs];
        [SpiceName("cqcs"), SpiceInfo("Cap. due to charge storage in C-S jct.")]
        public double GetCQCS(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcqcs];
        [SpiceName("qbx"), SpiceInfo("Charge storage B-X junction")]
        public double GetQBX(Circuit ckt) => ckt.State.States[0][BJTstate + BJTqbx];
        [SpiceName("cqbx"), SpiceInfo("Cap. due to charge storage in B-X jct.")]
        public double GetCQBX(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcqbx];
        [SpiceName("gx"), SpiceInfo("Conductance from base to internal base")]
        public double GetGX(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgx];
        [SpiceName("cexbc"), SpiceInfo("Total Capacitance in B-X junction")]
        public double GetCEXBC(Circuit ckt) => ckt.State.States[0][BJTstate + BJTcexbc];
        [SpiceName("geqcb"), SpiceInfo("d(Ibe)/d(Vbc)")]
        public double GetGEQCB(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgeqcb];
        [SpiceName("gccs"), SpiceInfo("Internal C-S cap. equiv. cond.")]
        public double GetGCCS(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgccs];
        [SpiceName("geqbx"), SpiceInfo("Internal C-B-base cap. equiv. cond.")]
        public double GetGEQBX(Circuit ckt) => ckt.State.States[0][BJTstate + BJTgeqbx];
        [SpiceName("cs"), SpiceInfo("Substrate current")]
        public double GetCS(Circuit ckt)
        {
            if (ckt.State.UseDC)
                return 0.0;
            else
                return -ckt.State.States[0][BJTstate + BJTcqcs];
        }
        [SpiceName("ce"), SpiceInfo("Emitter current")]
        public double GetCE(Circuit ckt)
        {
            double value = -ckt.State.States[0][BJTstate + BJTcc];
            value -= ckt.State.States[0][BJTstate + BJTcb];
            if (ckt.Method != null && !(ckt.State.Domain == CircuitState.DomainTypes.Time && ckt.State.UseDC))
                value += ckt.State.States[0][BJTstate + BJTcqcs];
            return value;
        }
        [SpiceName("p"), SpiceInfo("Power dissipation")]
        public double GetPOWER(Circuit ckt)
        {
            double value = ckt.State.States[0][BJTstate + BJTcc] * ckt.State.Solution[BJTcolNode];
            value += ckt.State.States[0][BJTstate + BJTcb] * ckt.State.Solution[BJTbaseNode];
            if (ckt.Method != null && !(ckt.State.Domain == CircuitState.DomainTypes.Time && ckt.State.UseDC))
                value -= ckt.State.States[0][BJTstate + BJTcqcs] * ckt.State.Solution[BJTsubstNode];
            
            double tmp = -ckt.State.States[0][BJTstate + BJTcc];
            tmp -= ckt.State.States[0][BJTstate + BJTcb];
            if (ckt.Method != null && !(ckt.State.Domain == CircuitState.DomainTypes.Time && ckt.State.UseDC))
                tmp += ckt.State.States[0][BJTstate + BJTcqcs];
            value += tmp * ckt.State.Solution[BJTemitNode];
            return value;
        }

        /// <summary>
        /// Extra variables
        /// </summary>
        public double BJTtSatCur { get; internal set; }
        public double BJTtBetaF { get; internal set; }
        public double BJTtBetaR { get; internal set; }
        public double BJTtBEleakCur { get; internal set; }
        public double BJTtBCleakCur { get; internal set; }
        public double BJTtBEcap { get; internal set; }
        public double BJTtBEpot { get; internal set; }
        public double BJTtBCcap { get; internal set; }
        public double BJTtBCpot { get; internal set; }
        public double BJTtDepCap { get; internal set; }
        public double BJTtf1 { get; internal set; }
        public double BJTtf4 { get; internal set; }
        public double BJTtf5 { get; internal set; }
        public double BJTtVcrit { get; internal set; }
        public int BJTstate { get; internal set; }

        /// <summary>
        /// Matrix elements
        /// </summary>
        internal MatrixElement BJTcolColPrimePtr { get; private set; }
        internal MatrixElement BJTbaseBasePrimePtr { get; private set; }
        internal MatrixElement BJTemitEmitPrimePtr { get; private set; }
        internal MatrixElement BJTcolPrimeColPtr { get; private set; }
        internal MatrixElement BJTcolPrimeBasePrimePtr { get; private set; }
        internal MatrixElement BJTcolPrimeEmitPrimePtr { get; private set; }
        internal MatrixElement BJTbasePrimeBasePtr { get; private set; }
        internal MatrixElement BJTbasePrimeColPrimePtr { get; private set; }
        internal MatrixElement BJTbasePrimeEmitPrimePtr { get; private set; }
        internal MatrixElement BJTemitPrimeEmitPtr { get; private set; }
        internal MatrixElement BJTemitPrimeColPrimePtr { get; private set; }
        internal MatrixElement BJTemitPrimeBasePrimePtr { get; private set; }
        internal MatrixElement BJTcolColPtr { get; private set; }
        internal MatrixElement BJTbaseBasePtr { get; private set; }
        internal MatrixElement BJTemitEmitPtr { get; private set; }
        internal MatrixElement BJTcolPrimeColPrimePtr { get; private set; }
        internal MatrixElement BJTbasePrimeBasePrimePtr { get; private set; }
        internal MatrixElement BJTemitPrimeEmitPrimePtr { get; private set; }
        internal MatrixElement BJTsubstSubstPtr { get; private set; }
        internal MatrixElement BJTcolPrimeSubstPtr { get; private set; }
        internal MatrixElement BJTsubstColPrimePtr { get; private set; }
        internal MatrixElement BJTbaseColPrimePtr { get; private set; }
        internal MatrixElement BJTcolPrimeBasePtr { get; private set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int BJTvbe = 0;
        public const int BJTvbc = 1;
        public const int BJTcc = 2;
        public const int BJTcb = 3;
        public const int BJTgpi = 4;
        public const int BJTgmu = 5;
        public const int BJTgm = 6;
        public const int BJTgo = 7;
        public const int BJTqbe = 8;
        public const int BJTcqbe = 9;
        public const int BJTqbc = 10;
        public const int BJTcqbc = 11;
        public const int BJTqcs = 12;
        public const int BJTcqcs = 13;
        public const int BJTqbx = 14;
        public const int BJTcqbx = 15;
        public const int BJTgx = 16;
        public const int BJTcexbc = 17;
        public const int BJTgeqcb = 18;
        public const int BJTgccs = 19;
        public const int BJTgeqbx = 20;
        public const int BJTpinCount = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the device</param>
        public BJT(CircuitIdentifier name) : base(name, BJTpinCount)
        {
        }

        /// <summary>
        /// Setup the device
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            BJTModel model = Model as BJTModel;

            // Allocate nodes
            var nodes = BindNodes(ckt);
            BJTcolNode = nodes[0].Index;
            BJTbaseNode = nodes[1].Index;
            BJTemitNode = nodes[2].Index;
            BJTsubstNode = nodes[3].Index;

            // Add a series collector node if necessary
            if (model.BJTcollectorResist.Value == 0)
                BJTcolPrimeNode = BJTcolNode;
            else if (BJTcolPrimeNode == 0)
                BJTcolPrimeNode = CreateNode(ckt, Name.Grow("#col")).Index;

            // Add a series base node if necessary
            if (model.BJTbaseResist.Value == 0)
                BJTbasePrimeNode = BJTbaseNode;
            else if (BJTbasePrimeNode == 0)
                BJTbasePrimeNode = CreateNode(ckt, Name.Grow("#base")).Index;

            // Add a series emitter node if necessary
            if (model.BJTemitterResist.Value == 0)
                BJTemitPrimeNode = BJTemitNode;
            else if (BJTemitPrimeNode == 0)
                BJTemitPrimeNode = CreateNode(ckt, Name.Grow("#emit")).Index;

            // Get matrix elements
            var matrix = ckt.State.Matrix;
            BJTcolColPrimePtr = matrix.GetElement(BJTcolNode, BJTcolPrimeNode);
            BJTbaseBasePrimePtr = matrix.GetElement(BJTbaseNode, BJTbasePrimeNode);
            BJTemitEmitPrimePtr = matrix.GetElement(BJTemitNode, BJTemitPrimeNode);
            BJTcolPrimeColPtr = matrix.GetElement(BJTcolPrimeNode, BJTcolNode);
            BJTcolPrimeBasePrimePtr = matrix.GetElement(BJTcolPrimeNode, BJTbasePrimeNode);
            BJTcolPrimeEmitPrimePtr = matrix.GetElement(BJTcolPrimeNode, BJTemitPrimeNode);
            BJTbasePrimeBasePtr = matrix.GetElement(BJTbasePrimeNode, BJTbaseNode);
            BJTbasePrimeColPrimePtr = matrix.GetElement(BJTbasePrimeNode, BJTcolPrimeNode);
            BJTbasePrimeEmitPrimePtr = matrix.GetElement(BJTbasePrimeNode, BJTemitPrimeNode);
            BJTemitPrimeEmitPtr = matrix.GetElement(BJTemitPrimeNode, BJTemitNode);
            BJTemitPrimeColPrimePtr = matrix.GetElement(BJTemitPrimeNode, BJTcolPrimeNode);
            BJTemitPrimeBasePrimePtr = matrix.GetElement(BJTemitPrimeNode, BJTbasePrimeNode);
            BJTcolColPtr = matrix.GetElement(BJTcolNode, BJTcolNode);
            BJTbaseBasePtr = matrix.GetElement(BJTbaseNode, BJTbaseNode);
            BJTemitEmitPtr = matrix.GetElement(BJTemitNode, BJTemitNode);
            BJTcolPrimeColPrimePtr = matrix.GetElement(BJTcolPrimeNode, BJTcolPrimeNode);
            BJTbasePrimeBasePrimePtr = matrix.GetElement(BJTbasePrimeNode, BJTbasePrimeNode);
            BJTemitPrimeEmitPrimePtr = matrix.GetElement(BJTemitPrimeNode, BJTemitPrimeNode);
            BJTsubstSubstPtr = matrix.GetElement(BJTsubstNode, BJTsubstNode);
            BJTcolPrimeSubstPtr = matrix.GetElement(BJTcolPrimeNode, BJTsubstNode);
            BJTsubstColPrimePtr = matrix.GetElement(BJTsubstNode, BJTcolPrimeNode);
            BJTbaseColPrimePtr = matrix.GetElement(BJTbaseNode, BJTcolPrimeNode);
            BJTcolPrimeBasePtr = matrix.GetElement(BJTcolPrimeNode, BJTbaseNode);

            // Allocate states
            BJTstate = ckt.State.GetState(21);
        }

        /// <summary>
        /// Unsetup
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Unsetup(Circuit ckt)
        {
            // Remove references
            BJTcolColPrimePtr = null;
            BJTbaseBasePrimePtr = null;
            BJTemitEmitPrimePtr = null;
            BJTcolPrimeColPtr = null;
            BJTcolPrimeBasePrimePtr = null;
            BJTcolPrimeEmitPrimePtr = null;
            BJTbasePrimeBasePtr = null;
            BJTbasePrimeColPrimePtr = null;
            BJTbasePrimeEmitPrimePtr = null;
            BJTemitPrimeEmitPtr = null;
            BJTemitPrimeColPrimePtr = null;
            BJTemitPrimeBasePrimePtr = null;
            BJTcolColPtr = null;
            BJTbaseBasePtr = null;
            BJTemitEmitPtr = null;
            BJTcolPrimeColPrimePtr = null;
            BJTbasePrimeBasePrimePtr = null;
            BJTemitPrimeEmitPrimePtr = null;
            BJTsubstSubstPtr = null;
            BJTcolPrimeSubstPtr = null;
            BJTsubstColPrimePtr = null;
            BJTbaseColPrimePtr = null;
            BJTcolPrimeBasePtr = null;
        }
    }
}
