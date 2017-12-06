﻿using SpiceSharp.Circuits;
using SpiceSharp.Parameters;
using SpiceSharp.Sparse;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A diode
    /// </summary>
    [SpicePins("D+", "D-")]
    public class Diode : CircuitComponent
    {
        /// <summary>
        /// Register default behaviors
        /// </summary>
        public override void RegisterBehaviors()
        {
            RegisterBehavior(typeof(ComponentBehaviors.DiodeTemperatureBehavior));
            RegisterBehavior(typeof(ComponentBehaviors.DiodeLoadBehavior));
            RegisterBehavior(typeof(ComponentBehaviors.DiodeAcBehavior));
            RegisterBehavior(typeof(ComponentBehaviors.DiodeNoiseBehavior));
            RegisterBehavior(typeof(ComponentBehaviors.DiodeTruncateBehavior));
        }

        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("area"), SpiceInfo("Area factor")]
        public Parameter DIOarea { get; } = new Parameter(1);
        [SpiceName("temp"), SpiceInfo("Instance temperature")]
        public double DIO_TEMP
        {
            get => DIOtemp - Circuit.CONSTCtoK;
            set => DIOtemp.Set(value + Circuit.CONSTCtoK);
        }
        public Parameter DIOtemp { get; } = new Parameter();
        [SpiceName("off"), SpiceInfo("Initially off")]
        public bool DIOoff { get; set; }
        [SpiceName("ic"), SpiceInfo("Initial device voltage")]
        public double DIOinitCond { get; set; }
        [SpiceName("sens_area"), SpiceInfo("flag to request sensitivity WRT area")]
        public bool DIOsenParmNo { get; set; }
        [SpiceName("cd"), SpiceInfo("Diode capacitance")]
        public double DIOcap { get; set; }

        /// <summary>
        /// Methods
        /// </summary>
        [SpiceName("vd"), SpiceInfo("Diode voltage")]
        public double GetDIO_VOLTAGE(Circuit ckt) => ckt.State.States[0][DIOstate + DIOvoltage];
        [SpiceName("id"), SpiceName("c"), SpiceInfo("Diode current")]
        public double GetDIO_CURRENT(Circuit ckt) => ckt.State.States[0][DIOstate + DIOcurrent];
        [SpiceName("charge"), SpiceInfo("Diode capacitor charge")]
        public double GetDIO_CHARGE(Circuit ckt) => ckt.State.States[0][DIOstate + DIOcapCharge];
        [SpiceName("capcur"), SpiceInfo("Diode capacitor current")]
        public double GetDIO_CAPCUR(Circuit ckt) => ckt.State.States[0][DIOstate + DIOcapCurrent];
        [SpiceName("gd"), SpiceInfo("Diode conductance")]
        public double GetDIO_CONDUCT(Circuit ckt) => ckt.State.States[0][DIOstate + DIOconduct];
        [SpiceName("p"), SpiceInfo("Diode power")]

        /// <summary>
        /// Extra variables
        /// </summary>
        public double DIOtJctCap { get; internal set; }
        public double DIOtJctPot { get; internal set; }
        public double DIOtSatCur { get; internal set; }
        public double DIOtF1 { get; internal set; }
        public double DIOtDepCap { get; internal set; }
        public double DIOtVcrit { get; internal set; }
        public double DIOtBrkdwnV { get; internal set; }
        public int DIOposNode { get; internal set; }
        public int DIOposPrimeNode { get; internal set; }
        public int DIOnegNode { get; internal set; }
        public int DIOstate { get; private set; }

        /// <summary>
        /// Matrix element pointers
        /// </summary>
        internal MatrixElement DIOposPosPrimePtr;
        internal MatrixElement DIOnegPosPrimePtr;
        internal MatrixElement DIOposPrimePosPtr;
        internal MatrixElement DIOposPrimeNegPtr;
        internal MatrixElement DIOposPosPtr;
        internal MatrixElement DIOnegNegPtr;
        internal MatrixElement DIOposPrimePosPrimePtr;

        /// <summary>
        /// Constants
        /// </summary>
        public const int DIOvoltage = 0;
        public const int DIOcurrent = 1;
        public const int DIOconduct = 2;
        public const int DIOcapCharge = 3;
        public const int DIOcapCurrent = 4;
        public const int DIOpinCount = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the device</param>
        public Diode(CircuitIdentifier name) : base(name, DIOpinCount)
        {
        }

        /// <summary>
        /// Setup the device
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Setup(Circuit ckt)
        {
            var model = Model as DiodeModel;

            // Allocate nodes
            var nodes = BindNodes(ckt);
            DIOposNode = nodes[0].Index;
            DIOnegNode = nodes[1].Index;

            // Allocate states
            DIOstate = ckt.State.GetState(5);

            if (model.DIOresist.Value == 0)
                DIOposPrimeNode = DIOposNode;
            else
                DIOposPrimeNode = CreateNode(ckt, Name.Grow("#pos")).Index;

            var matrix = ckt.State.Matrix;
            DIOposPosPrimePtr = matrix.GetElement(DIOposNode, DIOposPrimeNode);
            DIOnegPosPrimePtr = matrix.GetElement(DIOnegNode, DIOposPrimeNode);
            DIOposPrimePosPtr = matrix.GetElement(DIOposPrimeNode, DIOposNode);
            DIOposPrimeNegPtr = matrix.GetElement(DIOposPrimeNode, DIOnegNode);
            DIOposPosPtr = matrix.GetElement(DIOposNode, DIOposNode);
            DIOnegNegPtr = matrix.GetElement(DIOnegNode, DIOnegNode);
            DIOposPrimePosPrimePtr = matrix.GetElement(DIOposPrimeNode, DIOposPrimeNode);
        }

        /// <summary>
        /// Unsetup the device
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Unsetup(Circuit ckt)
        {
            DIOposPosPrimePtr = null;
            DIOnegPosPrimePtr = null;
            DIOposPrimePosPtr = null;
            DIOposPrimeNegPtr = null;
            DIOposPosPtr = null;
            DIOnegNegPtr = null;
            DIOposPrimePosPrimePtr = null;
        }
    }
}
