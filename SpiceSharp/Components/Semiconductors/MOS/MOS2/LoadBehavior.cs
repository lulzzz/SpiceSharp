﻿using System;
using SpiceSharp.Circuits;
using SpiceSharp.Attributes;
using SpiceSharp.Diagnostics;
using SpiceSharp.Sparse;
using SpiceSharp.Components.Transistors;

namespace SpiceSharp.Behaviors.MOS2
{
    /// <summary>
    /// General behavior of a <see cref="Components.MOS2"/>
    /// </summary>
    public class LoadBehavior : Behaviors.LoadBehavior
    {
        /// <summary>
        /// Necessary behaviors
        /// </summary>
        private TemperatureBehavior temp;
        private ModelTemperatureBehavior modeltemp;

        private static double[] sig1 = { 1.0, -1.0, 1.0, -1.0 };
        private static double[] sig2 = { 1.0, 1.0, -1.0, -1.0 };

        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("off"), SpiceInfo("Device initially off")]
        public bool MOS2off { get; set; }
        [SpiceName("icvbs"), SpiceInfo("Initial B-S voltage")]
        public Parameter MOS2icVBS { get; } = new Parameter();
        [SpiceName("icvds"), SpiceInfo("Initial D-S voltage")]
        public Parameter MOS2icVDS { get; } = new Parameter();
        [SpiceName("icvgs"), SpiceInfo("Initial G-S voltage")]
        public Parameter MOS2icVGS { get; } = new Parameter();
        [SpiceName("dnodeprime"), SpiceInfo("Number of internal drain node")]
        public int MOS2dNodePrime { get; internal set; }
        [SpiceName("snodeprime"), SpiceInfo("Number of internal source node")]
        public int MOS2sNodePrime { get; internal set; }
        [SpiceName("von"), SpiceInfo(" ")]
        public double MOS2von { get; internal set; }
        [SpiceName("vdsat"), SpiceInfo("Saturation drain voltage")]
        public double MOS2vdsat { get; internal set; }
        [SpiceName("id"), SpiceName("cd"), SpiceInfo("Drain current")]
        public double MOS2cd { get; internal set; }
        [SpiceName("ibs"), SpiceInfo("B-S junction current")]
        public double MOS2cbs { get; internal set; }
        [SpiceName("ibd"), SpiceInfo("B-D junction current")]
        public double MOS2cbd { get; internal set; }
        [SpiceName("gmb"), SpiceName("gmbs"), SpiceInfo("Bulk-Source transconductance")]
        public double MOS2gmbs { get; internal set; }
        [SpiceName("gm"), SpiceInfo("Transconductance")]
        public double MOS2gm { get; internal set; }
        [SpiceName("gds"), SpiceInfo("Drain-Source conductance")]
        public double MOS2gds { get; internal set; }
        [SpiceName("gbd"), SpiceInfo("Bulk-Drain conductance")]
        public double MOS2gbd { get; internal set; }
        [SpiceName("gbs"), SpiceInfo("Bulk-Source conductance")]
        public double MOS2gbs { get; internal set; }
        [SpiceName("cbd"), SpiceInfo("Bulk-Drain capacitance")]
        public double MOS2capbd { get; internal set; }
        [SpiceName("cbs"), SpiceInfo("Bulk-Source capacitance")]
        public double MOS2capbs { get; internal set; }

        /// <summary>
        /// Methods
        /// </summary>
        [SpiceName("ic"), SpiceInfo("Vector of D-S, G-S, B-S voltages")]
        public void SetIC(double[] value)
        {
            switch (value.Length)
            {
                case 3: MOS2icVBS.Set(value[2]); goto case 2;
                case 2: MOS2icVGS.Set(value[1]); goto case 1;
                case 1: MOS2icVDS.Set(value[0]); break;
                default:
                    throw new BadParameterException("ic");
            }
        }
        [SpiceName("vbd"), SpiceInfo("Bulk-Drain voltage")]
        public double GetVBD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2vbd];
        [SpiceName("vbs"), SpiceInfo("Bulk-Source voltage")]
        public double GetVBS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2vbs];
        [SpiceName("vgs"), SpiceInfo("Gate-Source voltage")]
        public double GetVGS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2vgs];
        [SpiceName("vds"), SpiceInfo("Drain-Source voltage")]
        public double GetVDS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2vds];
        [SpiceName("cgs"), SpiceInfo("Gate-Source capacitance")]
        public double GetCAPGS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2capgs];
        [SpiceName("qgs"), SpiceInfo("Gate-Source charge storage")]
        public double GetQGS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2qgs];
        [SpiceName("cqgs"), SpiceInfo("Capacitance due to gate-source charge storage")]
        public double GetCQGS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2cqgs];
        [SpiceName("cgd"), SpiceInfo("Gate-Drain capacitance")]
        public double GetCAPGD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2capgd];
        [SpiceName("qgd"), SpiceInfo("Gate-Drain charge storage")]
        public double GetQGD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2qgd];
        [SpiceName("cqgd"), SpiceInfo("Capacitance due to gate-drain charge storage")]
        public double GetCQGD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2cqgd];
        [SpiceName("cgb"), SpiceInfo("Gate-Bulk capacitance")]
        public double GetCAPGB(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2capgb];
        [SpiceName("qgb"), SpiceInfo("Gate-Bulk charge storage")]
        public double GetQGB(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2qgb];
        [SpiceName("cqgb"), SpiceInfo("Capacitance due to gate-bulk charge storage")]
        public double GetCQGB(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2cqgb];
        [SpiceName("qbd"), SpiceInfo("Bulk-Drain charge storage")]
        public double GetQBD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2qbd];
        [SpiceName("cqbd"), SpiceInfo("Capacitance due to bulk-drain charge storage")]
        public double GetCQBD(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2cqbd];
        [SpiceName("qbs"), SpiceInfo("Bulk-Source charge storage")]
        public double GetQBS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2qbs];
        [SpiceName("cqbs"), SpiceInfo("Capacitance due to bulk-source charge storage")]
        public double GetCQBS(Circuit ckt) => ckt.State.States[0][MOS2states + MOS2cqbs];
        [SpiceName("ib"), SpiceInfo("Bulk current ")]
        public double GetCB(Circuit ckt) => MOS2cbd + MOS2cbs - ckt.State.States[0][MOS2states + MOS2cqgb];
        [SpiceName("ig"), SpiceInfo("Gate current ")]
        public double GetCG(Circuit ckt) => ckt.State.UseDC ? 0.0 : ckt.State.States[0][MOS2states + MOS2cqgb] + ckt.State.States[0][MOS2states + MOS2cqgd] +
            ckt.State.States[0][MOS2states + MOS2cqgs];
        [SpiceName("is"), SpiceInfo("Source current ")]
        public double GetCS(Circuit ckt)
        {
            double value = -MOS2cd;
            value -= MOS2cbd + MOS2cbs - ckt.State.States[0][MOS2states + MOS2cqgb];
            if (ckt.State.Domain == State.DomainTypes.Time && !ckt.State.UseDC)
            {
                value -= ckt.State.States[0][MOS2states + MOS2cqgb] + ckt.State.States[0][MOS2states + MOS2cqgd] +
                    ckt.State.States[0][MOS2states + MOS2cqgs];
            }
            return value;
        }
        [SpiceName("p"), SpiceInfo("Instantaneous power ")]
        public double GetPOWER(Circuit ckt)
        {
            double temp;
            double value = MOS2cd * ckt.State.Solution[MOS2dNode];
            value += (MOS2cbd + MOS2cbs - ckt.State.States[0][MOS2states + MOS2cqgb]) * ckt.State.Solution[MOS2bNode];
            if (ckt.State.Domain == State.DomainTypes.Time && !ckt.State.UseDC)
            {
                value += (ckt.State.States[0][MOS2states + MOS2cqgb] + ckt.State.States[0][MOS2states + MOS2cqgd] +
                    ckt.State.States[0][MOS2states + MOS2cqgs]) * ckt.State.Solution[MOS2gNode];
            }
            temp = -MOS2cd;
            temp -= MOS2cbd + MOS2cbs;
            if (ckt.State.Domain == State.DomainTypes.Time && !ckt.State.UseDC)
            {
                temp -= ckt.State.States[0][MOS2states + MOS2cqgb] + ckt.State.States[0][MOS2states + MOS2cqgd] +
                    ckt.State.States[0][MOS2states + MOS2cqgs];
            }
            value += temp * ckt.State.Solution[MOS2sNode];
            return value;
        }

        /// <summary>
        /// Extra variables
        /// </summary>
        public double MOS2mode { get; internal set; }
        public int MOS2states { get; internal set; }

        /// <summary>
        /// Nodes
        /// </summary>
        protected int MOS2dNode, MOS2gNode, MOS2sNode, MOS2bNode;

        /// <summary>
        /// Matrix elements
        /// </summary>
        protected MatrixElement MOS2DdPtr { get; private set; }
        protected MatrixElement MOS2GgPtr { get; private set; }
        protected MatrixElement MOS2SsPtr { get; private set; }
        protected MatrixElement MOS2BbPtr { get; private set; }
        protected MatrixElement MOS2DPdpPtr { get; private set; }
        protected MatrixElement MOS2SPspPtr { get; private set; }
        protected MatrixElement MOS2DdpPtr { get; private set; }
        protected MatrixElement MOS2GbPtr { get; private set; }
        protected MatrixElement MOS2GdpPtr { get; private set; }
        protected MatrixElement MOS2GspPtr { get; private set; }
        protected MatrixElement MOS2SspPtr { get; private set; }
        protected MatrixElement MOS2BdpPtr { get; private set; }
        protected MatrixElement MOS2BspPtr { get; private set; }
        protected MatrixElement MOS2DPspPtr { get; private set; }
        protected MatrixElement MOS2DPdPtr { get; private set; }
        protected MatrixElement MOS2BgPtr { get; private set; }
        protected MatrixElement MOS2DPgPtr { get; private set; }
        protected MatrixElement MOS2SPgPtr { get; private set; }
        protected MatrixElement MOS2SPsPtr { get; private set; }
        protected MatrixElement MOS2DPbPtr { get; private set; }
        protected MatrixElement MOS2SPbPtr { get; private set; }
        protected MatrixElement MOS2SPdpPtr { get; private set; }

        /// <summary>
        /// Constants
        /// </summary>
        public const int MOS2vbd = 0;
        public const int MOS2vbs = 1;
        public const int MOS2vgs = 2;
        public const int MOS2vds = 3;
        public const int MOS2capgs = 4;
        public const int MOS2qgs = 5;
        public const int MOS2cqgs = 6;
        public const int MOS2capgd = 7;
        public const int MOS2qgd = 8;
        public const int MOS2cqgd = 9;
        public const int MOS2capgb = 10;
        public const int MOS2qgb = 11;
        public const int MOS2cqgb = 12;
        public const int MOS2qbd = 13;
        public const int MOS2cqbd = 14;
        public const int MOS2qbs = 15;
        public const int MOS2cqbs = 16;

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component"></param>
        /// <param name="ckt"></param>
        public override void Setup(Entity component, Circuit ckt)
        {
            var mos2 = component as Components.MOS2;

            // Get behaviors
            temp = GetBehavior<TemperatureBehavior>(component);
            modeltemp = GetBehavior<ModelTemperatureBehavior>(mos2.Model);

            // Nodes
            MOS2dNode = mos2.MOS2dNode;
            MOS2gNode = mos2.MOS2gNode;
            MOS2sNode = mos2.MOS2sNode;
            MOS2bNode = mos2.MOS2bNode;

            // Add a series drain node if necessary
            if (modeltemp.MOS2drainResistance != 0 || (temp.MOS2drainSquares != 0 && modeltemp.MOS2sheetResistance != 0))
                MOS2dNodePrime = CreateNode(ckt, component.Name.Grow("#drain")).Index;
            else
                MOS2dNodePrime = MOS2dNode;

            // Add a series source node if necessary
            if (modeltemp.MOS2sourceResistance != 0 || (temp.MOS2sourceSquares != 0 && modeltemp.MOS2sheetResistance != 0))
                MOS2sNodePrime = CreateNode(ckt, component.Name.Grow("#source")).Index;
            else
                MOS2sNodePrime = MOS2sNode;

            // Get matrix elements
            var matrix = ckt.State.Matrix;
            MOS2DdPtr = matrix.GetElement(MOS2dNode, MOS2dNode);
            MOS2GgPtr = matrix.GetElement(MOS2gNode, MOS2gNode);
            MOS2SsPtr = matrix.GetElement(MOS2sNode, MOS2sNode);
            MOS2BbPtr = matrix.GetElement(MOS2bNode, MOS2bNode);
            MOS2DPdpPtr = matrix.GetElement(MOS2dNodePrime, MOS2dNodePrime);
            MOS2SPspPtr = matrix.GetElement(MOS2sNodePrime, MOS2sNodePrime);
            MOS2DdpPtr = matrix.GetElement(MOS2dNode, MOS2dNodePrime);
            MOS2GbPtr = matrix.GetElement(MOS2gNode, MOS2bNode);
            MOS2GdpPtr = matrix.GetElement(MOS2gNode, MOS2dNodePrime);
            MOS2GspPtr = matrix.GetElement(MOS2gNode, MOS2sNodePrime);
            MOS2SspPtr = matrix.GetElement(MOS2sNode, MOS2sNodePrime);
            MOS2BdpPtr = matrix.GetElement(MOS2bNode, MOS2dNodePrime);
            MOS2BspPtr = matrix.GetElement(MOS2bNode, MOS2sNodePrime);
            MOS2DPspPtr = matrix.GetElement(MOS2dNodePrime, MOS2sNodePrime);
            MOS2DPdPtr = matrix.GetElement(MOS2dNodePrime, MOS2dNode);
            MOS2BgPtr = matrix.GetElement(MOS2bNode, MOS2gNode);
            MOS2DPgPtr = matrix.GetElement(MOS2dNodePrime, MOS2gNode);
            MOS2SPgPtr = matrix.GetElement(MOS2sNodePrime, MOS2gNode);
            MOS2SPsPtr = matrix.GetElement(MOS2sNodePrime, MOS2sNode);
            MOS2DPbPtr = matrix.GetElement(MOS2dNodePrime, MOS2bNode);
            MOS2SPbPtr = matrix.GetElement(MOS2sNodePrime, MOS2bNode);
            MOS2SPdpPtr = matrix.GetElement(MOS2sNodePrime, MOS2dNodePrime);

            // Allocate states
            MOS2states = ckt.State.GetState(17);

            MOS2vdsat = 0.0;
            MOS2von = 0.0;
        }

        /// <summary>
        /// Unsetup the behavior
        /// </summary>
        public override void Unsetup()
        {
            // Remove references
            MOS2DdPtr = null;
            MOS2GgPtr = null;
            MOS2SsPtr = null;
            MOS2BbPtr = null;
            MOS2DPdpPtr = null;
            MOS2SPspPtr = null;
            MOS2DdpPtr = null;
            MOS2GbPtr = null;
            MOS2GdpPtr = null;
            MOS2GspPtr = null;
            MOS2SspPtr = null;
            MOS2BdpPtr = null;
            MOS2BspPtr = null;
            MOS2DPspPtr = null;
            MOS2DPdPtr = null;
            MOS2BgPtr = null;
            MOS2DPgPtr = null;
            MOS2SPgPtr = null;
            MOS2SPsPtr = null;
            MOS2DPbPtr = null;
            MOS2SPbPtr = null;
            MOS2SPdpPtr = null;
        }

        /// <summary>
        /// Execute behavior
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Load(Circuit ckt)
        {
            var state = ckt.State;
            var rstate = state;
            var method = ckt.Method;
            double vt, EffectiveLength, DrainSatCur, SourceSatCur, GateSourceOverlapCap, GateDrainOverlapCap, GateBulkOverlapCap, Beta,
                OxideCap, vgs, vds, vbs, vbd, vgb, vgd, vgdo, delvbs, delvbd, delvgs, delvds, delvgd, cdhat, cbhat, von, evbs, evbd,
                vdsat, cdrain = 0.0, sargsw, vgs1, vgd1, vgb1, capgs = 0.0, capgd = 0.0, capgb = 0.0, gcgs, ceqgs, gcgd, ceqgd, gcgb, ceqgb, ceqbs,
                ceqbd, cdreq;
            int Check, xnrm, xrev;

            vt = Circuit.CONSTKoverQ * temp.MOS2temp;
            Check = 1;

            EffectiveLength = temp.MOS2l - 2 * modeltemp.MOS2latDiff;
            if ((temp.MOS2tSatCurDens == 0) || (temp.MOS2drainArea.Value == 0) || (temp.MOS2sourceArea.Value == 0))
            {
                DrainSatCur = temp.MOS2tSatCur;
                SourceSatCur = temp.MOS2tSatCur;
            }
            else
            {
                DrainSatCur = temp.MOS2tSatCurDens * temp.MOS2drainArea;
                SourceSatCur = temp.MOS2tSatCurDens * temp.MOS2sourceArea;
            }
            GateSourceOverlapCap = modeltemp.MOS2gateSourceOverlapCapFactor * temp.MOS2w;
            GateDrainOverlapCap = modeltemp.MOS2gateDrainOverlapCapFactor * temp.MOS2w;
            GateBulkOverlapCap = modeltemp.MOS2gateBulkOverlapCapFactor * EffectiveLength;
            Beta = temp.MOS2tTransconductance * temp.MOS2w / EffectiveLength;
            OxideCap = modeltemp.MOS2oxideCapFactor * EffectiveLength * temp.MOS2w;

            if ((state.Init == State.InitFlags.InitFloat || state.UseSmallSignal || (state.Init == State.InitFlags.InitTransient)) ||
                ((state.Init == State.InitFlags.InitFix) && (!MOS2off)))
            {
                // general iteration
                vbs = modeltemp.MOS2type * (rstate.Solution[MOS2bNode] - rstate.Solution[MOS2sNodePrime]);
                vgs = modeltemp.MOS2type * (rstate.Solution[MOS2gNode] - rstate.Solution[MOS2sNodePrime]);
                vds = modeltemp.MOS2type * (rstate.Solution[MOS2dNodePrime] - rstate.Solution[MOS2sNodePrime]);

                /* now some common crunching for some more useful quantities */

                vbd = vbs - vds;
                vgd = vgs - vds;
                vgdo = state.States[0][MOS2states + MOS2vgs] - state.States[0][MOS2states + MOS2vds];
                delvbs = vbs - state.States[0][MOS2states + MOS2vbs];
                delvbd = vbd - state.States[0][MOS2states + MOS2vbd];
                delvgs = vgs - state.States[0][MOS2states + MOS2vgs];
                delvds = vds - state.States[0][MOS2states + MOS2vds];
                delvgd = vgd - vgdo;

                /* these are needed for convergence testing */
                if (MOS2mode >= 0)
                {
                    cdhat = MOS2cd - MOS2gbd * delvbd + MOS2gmbs * delvbs + MOS2gm * delvgs + MOS2gds * delvds;
                }
                else
                {
                    cdhat = MOS2cd + (MOS2gmbs - MOS2gbd) * delvbd - MOS2gm * delvgd + MOS2gds * delvds;
                }
                cbhat = MOS2cbs + MOS2cbd + MOS2gbd * delvbd + MOS2gbs * delvbs;

                /* now lets see if we can bypass (ugh) */
                /* the following massive if should all be one
				* single compound if statement, but most compilers
				* can't handle it in one piece, so it is broken up
				* into several stages here
				*/
                /* NOBYPASS */
                /* ok - bypass is out, do it the hard way */

                von = modeltemp.MOS2type * MOS2von;
                /* 
				* limiting
				* We want to keep device voltages from changing
				* so fast that the exponentials churn out overflows 
				* and similar rudeness
				*/
                if (state.States[0][MOS2states + MOS2vds] >= 0)
                {
                    vgs = Transistor.DEVfetlim(vgs, state.States[0][MOS2states + MOS2vgs], von);
                    vds = vgs - vgd;
                    vds = Transistor.DEVlimvds(vds, state.States[0][MOS2states + MOS2vds]);
                    vgd = vgs - vds;
                }
                else
                {
                    vgd = Transistor.DEVfetlim(vgd, vgdo, von);
                    vds = vgs - vgd;
                    vds = -Transistor.DEVlimvds(-vds, -(state.States[0][MOS2states + MOS2vds]));
                    vgs = vgd + vds;
                }
                if (vds >= 0)
                {
                    vbs = Transistor.DEVpnjlim(vbs, state.States[0][MOS2states + MOS2vbs], vt, temp.MOS2sourceVcrit, ref Check);
                    vbd = vbs - vds;
                }
                else
                {
                    vbd = Transistor.DEVpnjlim(vbd, state.States[0][MOS2states + MOS2vbd], vt, temp.MOS2drainVcrit, ref Check);
                    vbs = vbd + vds;
                }
            }
            else
            {
                /* ok - not one of the simple cases, so we have to 
				* look at other possibilities 
				*/

                if ((state.Init == State.InitFlags.InitJct) && !MOS2off)
                {
                    vds = modeltemp.MOS2type * MOS2icVDS;
                    vgs = modeltemp.MOS2type * MOS2icVGS;
                    vbs = modeltemp.MOS2type * MOS2icVBS;
                    if ((vds == 0) && (vgs == 0) && (vbs == 0) && ((method != null || state.UseDC ||
                        state.Domain == State.DomainTypes.None) || (!state.UseIC)))
                    {
                        vbs = -1;
                        vgs = modeltemp.MOS2type * temp.MOS2tVto;
                        vds = 0;
                    }
                }
                else
                {
                    vbs = vgs = vds = 0;
                }
            }

            /* now all the preliminaries are over - we can start doing the
			* real work 
			*/

            vbd = vbs - vds;
            vgd = vgs - vds;
            vgb = vgs - vbs;

            /* bulk - source and bulk - drain doides
			* here we just evaluate the ideal diode current and the
			* correspoinding derivative (conductance).
			*/

            if (vbs <= 0)
            {
                MOS2gbs = SourceSatCur / vt;
                MOS2cbs = MOS2gbs * vbs;
                MOS2gbs += state.Gmin;
            }
            else
            {
                evbs = Math.Exp(vbs / vt);
                MOS2gbs = SourceSatCur * evbs / vt + state.Gmin;
                MOS2cbs = SourceSatCur * (evbs - 1);
            }
            if (vbd <= 0)
            {
                MOS2gbd = DrainSatCur / vt;
                MOS2cbd = MOS2gbd * vbd;
                MOS2gbd += state.Gmin;
            }
            else
            {
                evbd = Math.Exp(vbd / vt);
                MOS2gbd = DrainSatCur * evbd / vt + state.Gmin;
                MOS2cbd = DrainSatCur * (evbd - 1);
            }
            if (vds >= 0)
            {
                /* normal mode */
                MOS2mode = 1;
            }
            else
            {
                /* inverse mode */
                MOS2mode = -1;
            }
            {
                /* moseq2(vds, vbs, vgs, gm, gds, gmbs, qg, qc, qb, 
				* cggb, cgdb, cgsb, cbgb, cbdb, cbsb)
				*/
                /* note:  cgdb, cgsb, cbdb, cbsb never used */

                /* 
				* this routine evaluates the drain current, its derivatives and
				* the charges associated with the gate, channel and bulk
				* for mosfets
				* 
				*/

                double arg;
                double sarg;
                double[] a4 = new double[4], b4 = new double[4], x4 = new double[8], poly4 = new double[8];
                double beta1, dsrgdb, d2sdb2;
                double sphi = 0.0; /* square root of phi */
                double sphi3 = 0.0; /* square root of phi cubed */
                double barg, d2bdb2, factor, dbrgdb, eta, vbin, argd = 0.0, args = 0.0, argss, argsd, argxs = 0.0, argxd = 0.0, daddb2, dasdb2, dbargd, dbargs, dbxwd, dbxws,
                    dgddb2, dgddvb, dgdvds, gamasd, xwd, xws, ddxwd, gammad, vth, cfs, cdonco, xn = 0.0, argg = 0.0, vgst, sarg3, sbiarg, dgdvbs, body, gdbdv,
                    dodvbs, dodvds = 0.0, dxndvd = 0.0, dxndvb = 0.0, udenom, dudvgs, dudvds, dudvbs, gammd2, argv, vgsx, ufact, ueff, dsdvgs, dsdvbs, a1, a3, a, b1,
                    b3, b, c1, c, d1, fi, p0, p2, p3, p4, p, r3, r, ro, s2, s, v1, v2, xv, y3, delta4, xvalid = 0.0, bsarg = 0.0, dbsrdb = 0.0, bodys = 0.0, gdbdvs = 0.0, sargv,
                    xlfact, dldsat, xdv, xlv, vqchan, dqdsat, vl, dfundg, dfunds, dfundb, xls, dldvgs = 0.0, dldvds = 0.0, dldvbs = 0.0, dfact, clfact, xleff, deltal,
                    xwb, vdson, cdson, didvds, gdson, gmw, gbson, expg, xld;
                double xlamda = modeltemp.MOS2lambda;
                /* 'local' variables - these switch d & s around appropriately
				 * so that we don't have to worry about vds < 0
				 */
                double lvbs = MOS2mode == 1 ? vbs : vbd;
                double lvds = MOS2mode * vds;
                double lvgs = MOS2mode == 1 ? vgs : vgd;
                double phiMinVbs = temp.MOS2tPhi - lvbs;
                double tmp; /* a temporary variable, not used for more than */
                            /* about 10 lines at a time */
                int iknt, jknt, i, j;

                /* 
				* compute some useful quantities
				*/

                if (lvbs <= 0.0)
                {
                    sarg = Math.Sqrt(phiMinVbs);
                    dsrgdb = -0.5 / sarg;
                    d2sdb2 = 0.5 * dsrgdb / phiMinVbs;
                }
                else
                {
                    sphi = Math.Sqrt(temp.MOS2tPhi);
                    sphi3 = temp.MOS2tPhi * sphi;
                    sarg = sphi / (1.0 + 0.5 * lvbs / temp.MOS2tPhi);
                    tmp = sarg / sphi3;
                    dsrgdb = -0.5 * sarg * tmp;
                    d2sdb2 = -dsrgdb * tmp;
                }
                if ((lvds - lvbs) >= 0)
                {
                    barg = Math.Sqrt(phiMinVbs + lvds);
                    dbrgdb = -0.5 / barg;
                    d2bdb2 = 0.5 * dbrgdb / (phiMinVbs + lvds);
                }
                else
                {
                    barg = sphi / (1.0 + 0.5 * (lvbs - lvds) / temp.MOS2tPhi);
                    tmp = barg / sphi3;
                    dbrgdb = -0.5 * barg * tmp;
                    d2bdb2 = -dbrgdb * tmp;
                }
                /* 
				* calculate threshold voltage (von)
				* narrow - channel effect
				*/

                /* XXX constant per device */
                factor = 0.125 * modeltemp.MOS2narrowFactor * 2.0 * Circuit.CONSTPI * Transistor.EPSSIL / OxideCap * EffectiveLength;
                /* XXX constant per device */
                eta = 1.0 + factor;
                vbin = temp.MOS2tVbi * modeltemp.MOS2type + factor * phiMinVbs;
                if ((modeltemp.MOS2gamma > 0.0) || (modeltemp.MOS2substrateDoping > 0.0))
                {
                    xwd = modeltemp.MOS2xd * barg;
                    xws = modeltemp.MOS2xd * sarg;

                    /* 
					* short - channel effect with vds .ne. 0.0
					*/

                    argss = 0.0;
                    argsd = 0.0;
                    dbargs = 0.0;
                    dbargd = 0.0;
                    dgdvds = 0.0;
                    dgddb2 = 0.0;
                    if (modeltemp.MOS2junctionDepth > 0)
                    {
                        tmp = 2.0 / modeltemp.MOS2junctionDepth;
                        argxs = 1.0 + xws * tmp;
                        argxd = 1.0 + xwd * tmp;
                        args = Math.Sqrt(argxs);
                        argd = Math.Sqrt(argxd);
                        tmp = .5 * modeltemp.MOS2junctionDepth / EffectiveLength;
                        argss = tmp * (args - 1.0);
                        argsd = tmp * (argd - 1.0);
                    }
                    gamasd = modeltemp.MOS2gamma * (1.0 - argss - argsd);
                    dbxwd = modeltemp.MOS2xd * dbrgdb;
                    dbxws = modeltemp.MOS2xd * dsrgdb;
                    if (modeltemp.MOS2junctionDepth > 0)
                    {
                        tmp = 0.5 / EffectiveLength;
                        dbargs = tmp * dbxws / args;
                        dbargd = tmp * dbxwd / argd;
                        dasdb2 = -modeltemp.MOS2xd * (d2sdb2 + dsrgdb * dsrgdb * modeltemp.MOS2xd / (modeltemp.MOS2junctionDepth * argxs)) / (EffectiveLength *
                            args);
                        daddb2 = -modeltemp.MOS2xd * (d2bdb2 + dbrgdb * dbrgdb * modeltemp.MOS2xd / (modeltemp.MOS2junctionDepth * argxd)) / (EffectiveLength *
                            argd);
                        dgddb2 = -0.5 * modeltemp.MOS2gamma * (dasdb2 + daddb2);
                    }
                    dgddvb = -modeltemp.MOS2gamma * (dbargs + dbargd);
                    if (modeltemp.MOS2junctionDepth > 0)
                    {
                        ddxwd = -dbxwd;
                        dgdvds = -modeltemp.MOS2gamma * 0.5 * ddxwd / (EffectiveLength * argd);
                    }
                }
                else
                {
                    gamasd = modeltemp.MOS2gamma;
                    gammad = modeltemp.MOS2gamma;
                    dgddvb = 0.0;
                    dgdvds = 0.0;
                    dgddb2 = 0.0;
                }
                von = vbin + gamasd * sarg;
                vth = von;
                vdsat = 0.0;
                if (modeltemp.MOS2fastSurfaceStateDensity != 0.0 && OxideCap != 0.0)
                {
                    /* XXX constant per model */
                    cfs = Circuit.CHARGE * modeltemp.MOS2fastSurfaceStateDensity * 1e4 /* (cm *  * 2 / m *  * 2) */ ;
                    cdonco = -(gamasd * dsrgdb + dgddvb * sarg) + factor;
                    xn = 1.0 + cfs / OxideCap * temp.MOS2w * EffectiveLength + cdonco;
                    tmp = vt * xn;
                    von = von + tmp;
                    argg = 1.0 / tmp;
                    vgst = lvgs - von;
                }
                else
                {
                    vgst = lvgs - von;
                    if (lvgs <= von)
                    {
                        /* 
						* cutoff region
						*/
                        MOS2gds = 0.0;
                        goto line1050;
                    }
                }

                /* 
				* compute some more useful quantities
				*/

                sarg3 = sarg * sarg * sarg;
                /* XXX constant per model */
                sbiarg = Math.Sqrt(temp.MOS2tBulkPot);
                gammad = gamasd;
                dgdvbs = dgddvb;
                body = barg * barg * barg - sarg3;
                gdbdv = 2.0 * gammad * (barg * barg * dbrgdb - sarg * sarg * dsrgdb);
                dodvbs = -factor + dgdvbs * sarg + gammad * dsrgdb;
                if (modeltemp.MOS2fastSurfaceStateDensity.Value == 0.0)
                    goto line400;
                if (OxideCap == 0.0)
                    goto line410;
                dxndvb = 2.0 * dgdvbs * dsrgdb + gammad * d2sdb2 + dgddb2 * sarg;
                dodvbs = dodvbs + vt * dxndvb;
                dxndvd = dgdvds * dsrgdb;
                dodvds = dgdvds * sarg + vt * dxndvd;
                /* 
				* evaluate effective mobility and its derivatives
				*/
                line400:
                if (OxideCap <= 0.0) goto line410;
                udenom = vgst;
                tmp = modeltemp.MOS2critField * 100 /* cm / m */  * Transistor.EPSSIL / modeltemp.MOS2oxideCapFactor;
                if (udenom <= tmp) goto line410;
                ufact = Math.Exp(modeltemp.MOS2critFieldExp * Math.Log(tmp / udenom));
                ueff = modeltemp.MOS2surfaceMobility * 1e-4 /* (m *  * 2 / cm *  * 2) */  * ufact;
                dudvgs = -ufact * modeltemp.MOS2critFieldExp / udenom;
                dudvds = 0.0;
                dudvbs = modeltemp.MOS2critFieldExp * ufact * dodvbs / vgst;
                goto line500;
                line410:
                ufact = 1.0;
                ueff = modeltemp.MOS2surfaceMobility * 1e-4 /* (m *  * 2 / cm *  * 2) */ ;
                dudvgs = 0.0;
                dudvds = 0.0;
                dudvbs = 0.0;
                /* 
				* evaluate saturation voltage and its derivatives according to
				* grove - frohman equation
				*/
                line500:
                vgsx = lvgs;
                gammad = gamasd / eta;
                dgdvbs = dgddvb;
                if (modeltemp.MOS2fastSurfaceStateDensity != 0 && OxideCap != 0)
                {
                    vgsx = Math.Max(lvgs, von);
                }
                if (gammad > 0)
                {
                    gammd2 = gammad * gammad;
                    argv = (vgsx - vbin) / eta + phiMinVbs;
                    if (argv <= 0.0)
                    {
                        vdsat = 0.0;
                        dsdvgs = 0.0;
                        dsdvbs = 0.0;
                    }
                    else
                    {
                        arg = Math.Sqrt(1.0 + 4.0 * argv / gammd2);
                        vdsat = (vgsx - vbin) / eta + gammd2 * (1.0 - arg) / 2.0;
                        vdsat = Math.Max(vdsat, 0.0);
                        dsdvgs = (1.0 - 1.0 / arg) / eta;
                        dsdvbs = (gammad * (1.0 - arg) + 2.0 * argv / (gammad * arg)) / eta * dgdvbs + 1.0 / arg + factor * dsdvgs;
                    }
                }
                else
                {
                    vdsat = (vgsx - vbin) / eta;
                    vdsat = Math.Max(vdsat, 0.0);
                    dsdvgs = 1.0;
                    dsdvbs = 0.0;
                }
                if (modeltemp.MOS2maxDriftVel > 0)
                {
                    /* 
					* evaluate saturation voltage and its derivatives 
					* according to baum's theory of scattering velocity 
					* saturation
					*/
                    gammd2 = gammad * gammad;
                    v1 = (vgsx - vbin) / eta + phiMinVbs;
                    v2 = phiMinVbs;
                    xv = modeltemp.MOS2maxDriftVel * EffectiveLength / ueff;
                    a1 = gammad / 0.75;
                    b1 = -2.0 * (v1 + xv);
                    c1 = -2.0 * gammad * xv;
                    d1 = 2.0 * v1 * (v2 + xv) - v2 * v2 - 4.0 / 3.0 * gammad * sarg3;
                    a = -b1;
                    b = a1 * c1 - 4.0 * d1;
                    c = -d1 * (a1 * a1 - 4.0 * b1) - c1 * c1;
                    r = -a * a / 3.0 + b;
                    s = 2.0 * a * a * a / 27.0 - a * b / 3.0 + c;
                    r3 = r * r * r;
                    s2 = s * s;
                    p = s2 / 4.0 + r3 / 27.0;
                    p0 = Math.Abs(p);
                    p2 = Math.Sqrt(p0);
                    if (p < 0)
                    {
                        ro = Math.Sqrt(s2 / 4.0 + p0);
                        ro = Math.Log(ro) / 3.0;
                        ro = Math.Exp(ro);
                        fi = Math.Atan(-2.0 * p2 / s);
                        y3 = 2.0 * ro * Math.Cos(fi / 3.0) - a / 3.0;
                    }
                    else
                    {
                        p3 = (-s / 2.0 + p2);
                        p3 = Math.Exp(Math.Log(Math.Abs(p3)) / 3.0);
                        p4 = (-s / 2.0 - p2);
                        p4 = Math.Exp(Math.Log(Math.Abs(p4)) / 3.0);
                        y3 = p3 + p4 - a / 3.0;
                    }
                    iknt = 0;
                    a3 = Math.Sqrt(a1 * a1 / 4.0 - b1 + y3);
                    b3 = Math.Sqrt(y3 * y3 / 4.0 - d1);
                    for (i = 1; i <= 4; i++)
                    {
                        a4[i - 1] = a1 / 2.0 + sig1[i - 1] * a3;
                        b4[i - 1] = y3 / 2.0 + sig2[i - 1] * b3;
                        delta4 = a4[i - 1] * a4[i - 1] / 4.0 - b4[i - 1];
                        if (delta4 < 0)
                            continue;
                        iknt = iknt + 1;
                        tmp = Math.Sqrt(delta4);
                        x4[iknt - 1] = -a4[i - 1] / 2.0 + tmp;
                        iknt = iknt + 1;
                        x4[iknt - 1] = -a4[i - 1] / 2.0 - tmp;
                    }
                    jknt = 0;
                    for (j = 1; j <= iknt; j++)
                    {
                        if (x4[j - 1] <= 0) continue;
                        /* XXX implement this sanely */
                        poly4[j - 1] = x4[j - 1] * x4[j - 1] * x4[j - 1] * x4[j - 1] + a1 * x4[j - 1] * x4[j - 1] * x4[j - 1];
                        poly4[j - 1] = poly4[j - 1] + b1 * x4[j - 1] * x4[j - 1] + c1 * x4[j - 1] + d1;
                        if (Math.Abs(poly4[j - 1]) > 1.0e-6)
                            continue;
                        jknt = jknt + 1;
                        if (jknt <= 1)
                        {
                            xvalid = x4[j - 1];
                        }
                        if (x4[j - 1] > xvalid)
                            continue;
                        xvalid = x4[j - 1];
                    }
                    if (jknt > 0)
                    {
                        vdsat = xvalid * xvalid - phiMinVbs;
                    }
                }
                /* 
				* evaluate effective channel length and its derivatives
				*/
                if (lvds != 0.0)
                {
                    gammad = gamasd;
                    if ((lvbs - vdsat) <= 0)
                    {
                        bsarg = Math.Sqrt(vdsat + phiMinVbs);
                        dbsrdb = -0.5 / bsarg;
                    }
                    else
                    {
                        bsarg = sphi / (1.0 + 0.5 * (lvbs - vdsat) / temp.MOS2tPhi);
                        dbsrdb = -0.5 * bsarg * bsarg / sphi3;
                    }
                    bodys = bsarg * bsarg * bsarg - sarg3;
                    gdbdvs = 2.0 * gammad * (bsarg * bsarg * dbsrdb - sarg * sarg * dsrgdb);
                    if (modeltemp.MOS2maxDriftVel <= 0)
                    {
                        if (modeltemp.MOS2substrateDoping.Value == 0.0)
                            goto line610;
                        if (xlamda > 0.0)
                            goto line610;
                        argv = (lvds - vdsat) / 4.0;
                        sargv = Math.Sqrt(1.0 + argv * argv);
                        arg = Math.Sqrt(argv + sargv);
                        xlfact = modeltemp.MOS2xd / (EffectiveLength * lvds);
                        xlamda = xlfact * arg;
                        dldsat = lvds * xlamda / (8.0 * sargv);
                    }
                    else
                    {
                        argv = (vgsx - vbin) / eta - vdsat;
                        xdv = modeltemp.MOS2xd / Math.Sqrt(modeltemp.MOS2channelCharge);
                        xlv = modeltemp.MOS2maxDriftVel * xdv / (2.0 * ueff);
                        vqchan = argv - gammad * bsarg;
                        dqdsat = -1.0 + gammad * dbsrdb;
                        vl = modeltemp.MOS2maxDriftVel * EffectiveLength;
                        dfunds = vl * dqdsat - ueff * vqchan;
                        dfundg = (vl - ueff * vdsat) / eta;
                        dfundb = -vl * (1.0 + dqdsat - factor / eta) + ueff * (gdbdvs - dgdvbs * bodys / 1.5) / eta;
                        dsdvgs = -dfundg / dfunds;
                        dsdvbs = -dfundb / dfunds;
                        if (modeltemp.MOS2substrateDoping.Value == 0.0)
                            goto line610;
                        if (xlamda > 0.0)
                            goto line610;
                        argv = lvds - vdsat;
                        argv = Math.Max(argv, 0.0);
                        xls = Math.Sqrt(xlv * xlv + argv);
                        dldsat = xdv / (2.0 * xls);
                        xlfact = xdv / (EffectiveLength * lvds);
                        xlamda = xlfact * (xls - xlv);
                        dldsat = dldsat / EffectiveLength;
                    }
                    dldvgs = dldsat * dsdvgs;
                    dldvds = -xlamda + dldsat;
                    dldvbs = dldsat * dsdvbs;
                }

                // Edited to work
                goto line610_finish;
                line610:
                dldvgs = 0.0;
                dldvds = 0.0;
                dldvbs = 0.0;
                line610_finish:

                /* 
				* limit channel shortening at punch - through
				*/
                xwb = modeltemp.MOS2xd * sbiarg;
                xld = EffectiveLength - xwb;
                clfact = 1.0 - xlamda * lvds;
                dldvds = -xlamda - dldvds;
                xleff = EffectiveLength * clfact;
                deltal = xlamda * lvds * EffectiveLength;
                if (modeltemp.MOS2substrateDoping.Value == 0.0)
                    xwb = 0.25e-6;
                if (xleff < xwb)
                {
                    xleff = xwb / (1.0 + (deltal - xld) / xwb);
                    clfact = xleff / EffectiveLength;
                    dfact = xleff * xleff / (xwb * xwb);
                    dldvgs = dfact * dldvgs;
                    dldvds = dfact * dldvds;
                    dldvbs = dfact * dldvbs;
                }
                /* 
				* evaluate effective beta (effective kp)
				*/
                beta1 = Beta * ufact / clfact;
                /* 
				* test for mode of operation and branch appropriately
				*/
                gammad = gamasd;
                dgdvbs = dgddvb;
                if (lvds <= 1.0e-10)
                {
                    if (lvgs <= von)
                    {
                        if ((modeltemp.MOS2fastSurfaceStateDensity.Value == 0.0) || (OxideCap == 0.0))
                        {
                            MOS2gds = 0.0;
                            goto line1050;
                        }

                        MOS2gds = beta1 * (von - vbin - gammad * sarg) * Math.Exp(argg * (lvgs - von));
                        goto line1050;
                    }

                    MOS2gds = beta1 * (lvgs - vbin - gammad * sarg);
                    goto line1050;
                }

                if (lvgs > von)
                    goto line900;
                /* 
				* subthreshold region
				*/
                if (vdsat <= 0)
                {
                    MOS2gds = 0.0;
                    if (lvgs > vth)
                        goto doneval;
                    goto line1050;
                }
                vdson = Math.Min(vdsat, lvds);
                if (lvds > vdsat)
                {
                    barg = bsarg;
                    dbrgdb = dbsrdb;
                    body = bodys;
                    gdbdv = gdbdvs;
                }
                cdson = beta1 * ((von - vbin - eta * vdson * 0.5) * vdson - gammad * body / 1.5);
                didvds = beta1 * (von - vbin - eta * vdson - gammad * barg);
                gdson = -cdson * dldvds / clfact - beta1 * dgdvds * body / 1.5;
                if (lvds < vdsat)
                    gdson = gdson + didvds;
                gbson = -cdson * dldvbs / clfact + beta1 * (dodvbs * vdson + factor * vdson - dgdvbs * body / 1.5 - gdbdv);
                if (lvds > vdsat)
                    gbson = gbson + didvds * dsdvbs;
                expg = Math.Exp(argg * (lvgs - von));
                cdrain = cdson * expg;
                gmw = cdrain * argg;
                MOS2gm = gmw;
                if (lvds > vdsat)
                    MOS2gm = gmw + didvds * dsdvgs * expg;
                tmp = gmw * (lvgs - von) / xn;
                MOS2gds = gdson * expg - MOS2gm * dodvds - tmp * dxndvd;
                MOS2gmbs = gbson * expg - MOS2gm * dodvbs - tmp * dxndvb;
                goto doneval;

                line900:
                if (lvds <= vdsat)
                {
                    /* 
					* linear region
					*/
                    cdrain = beta1 * ((lvgs - vbin - eta * lvds / 2.0) * lvds - gammad * body / 1.5);
                    arg = cdrain * (dudvgs / ufact - dldvgs / clfact);
                    MOS2gm = arg + beta1 * lvds;
                    arg = cdrain * (dudvds / ufact - dldvds / clfact);
                    MOS2gds = arg + beta1 * (lvgs - vbin - eta * lvds - gammad * barg - dgdvds * body / 1.5);
                    arg = cdrain * (dudvbs / ufact - dldvbs / clfact);
                    MOS2gmbs = arg - beta1 * (gdbdv + dgdvbs * body / 1.5 - factor * lvds);
                }
                else
                {
                    /* 
					* saturation region
					*/
                    cdrain = beta1 * ((lvgs - vbin - eta * vdsat / 2.0) * vdsat - gammad * bodys / 1.5);
                    arg = cdrain * (dudvgs / ufact - dldvgs / clfact);
                    MOS2gm = arg + beta1 * vdsat + beta1 * (lvgs - vbin - eta * vdsat - gammad * bsarg) * dsdvgs;
                    MOS2gds = -cdrain * dldvds / clfact - beta1 * dgdvds * bodys / 1.5;
                    arg = cdrain * (dudvbs / ufact - dldvbs / clfact);
                    MOS2gmbs = arg - beta1 * (gdbdvs + dgdvbs * bodys / 1.5 - factor * vdsat) + beta1 * (lvgs - vbin - eta * vdsat - gammad *
                        bsarg) * dsdvbs;
                }
                /* 
				* compute charges for "on" region
				*/
                goto doneval;
                /* 
				* finish special cases
				*/
                line1050:
                cdrain = 0.0;
                MOS2gm = 0.0;
                MOS2gmbs = 0.0;
                /* 
				* finished
				*/

            }
            doneval:
            MOS2von = modeltemp.MOS2type * von;
            MOS2vdsat = modeltemp.MOS2type * vdsat;
            /* 
			* COMPUTE EQUIVALENT DRAIN CURRENT SOURCE
			*/
            MOS2cd = MOS2mode * cdrain - MOS2cbd;

            if (state.Domain == State.DomainTypes.Time || state.UseSmallSignal)
            {
                /* 
				* now we do the hard part of the bulk - drain and bulk - source
				* diode - we evaluate the non - linear capacitance and
				* charge
				* 
				* the basic equations are not hard, but the implementation
				* is somewhat long in an attempt to avoid log / exponential
				* evaluations
				*/
                /* 
				* charge storage elements
				* 
				* .. bulk - drain and bulk - source depletion capacitances
				*/
                /* CAPBYPASS */
                {
                    /* can't bypass the diode capacitance calculations */
                    /* CAPZEROBYPASS */
                    if (vbs < temp.MOS2tDepCap)
                    {
                        double arg = 1 - vbs / temp.MOS2tBulkPot, sarg;
                        /* 
						* the following block looks somewhat long and messy, 
						* but since most users use the default grading
						* coefficients of .5, and sqrt is MUCH faster than an
						* Math.Exp(Math.Log()) we use this special case code to buy time.
						* (as much as 10% of total job time!)
						*/
                        if (modeltemp.MOS2bulkJctBotGradingCoeff.Value == modeltemp.MOS2bulkJctSideGradingCoeff)
                        {
                            if (modeltemp.MOS2bulkJctBotGradingCoeff.Value == .5)
                            {
                                sarg = sargsw = 1 / Math.Sqrt(arg);
                            }
                            else
                            {
                                sarg = sargsw = Math.Exp(-modeltemp.MOS2bulkJctBotGradingCoeff * Math.Log(arg));
                            }
                        }
                        else
                        {
                            if (modeltemp.MOS2bulkJctBotGradingCoeff.Value == .5)
                            {
                                sarg = 1 / Math.Sqrt(arg);
                            }
                            else
                            {
                                /* NOSQRT */
                                sarg = Math.Exp(-modeltemp.MOS2bulkJctBotGradingCoeff * Math.Log(arg));
                            }
                            if (modeltemp.MOS2bulkJctSideGradingCoeff.Value == .5)
                            {
                                sargsw = 1 / Math.Sqrt(arg);
                            }
                            else
                            {
                                /* NOSQRT */
                                sargsw = Math.Exp(-modeltemp.MOS2bulkJctSideGradingCoeff * Math.Log(arg));
                            }
                        }
                        /* NOSQRT */
                        state.States[0][MOS2states + MOS2qbs] = temp.MOS2tBulkPot * (temp.MOS2Cbs * (1 - arg * sarg) / (1 - modeltemp.MOS2bulkJctBotGradingCoeff) +
                            temp.MOS2Cbssw * (1 - arg * sargsw) / (1 - modeltemp.MOS2bulkJctSideGradingCoeff));
                        MOS2capbs = temp.MOS2Cbs * sarg + temp.MOS2Cbssw * sargsw;
                    }
                    else
                    {
                        state.States[0][MOS2states + MOS2qbs] = temp.MOS2f4s + vbs * (temp.MOS2f2s + vbs * (temp.MOS2f3s / 2));
                        MOS2capbs = temp.MOS2f2s + temp.MOS2f3s * vbs;
                    }
                    /* CAPZEROBYPASS */
                }
                /* CAPBYPASS */
                /* can't bypass the diode capacitance calculations */
                {
                    /* CAPZEROBYPASS */
                    if (vbd < temp.MOS2tDepCap)
                    {
                        double arg = 1 - vbd / temp.MOS2tBulkPot, sarg;
                        /* 
						* the following block looks somewhat long and messy, 
						* but since most users use the default grading
						* coefficients of .5, and sqrt is MUCH faster than an
						* Math.Exp(Math.Log()) we use this special case code to buy time.
						* (as much as 10% of total job time!)
						*/
                        if (modeltemp.MOS2bulkJctBotGradingCoeff.Value == .5 && modeltemp.MOS2bulkJctSideGradingCoeff.Value == .5)
                        {
                            sarg = sargsw = 1 / Math.Sqrt(arg);
                        }
                        else
                        {
                            if (modeltemp.MOS2bulkJctBotGradingCoeff.Value == .5)
                            {
                                sarg = 1 / Math.Sqrt(arg);
                            }
                            else
                            {
                                /* NOSQRT */
                                sarg = Math.Exp(-modeltemp.MOS2bulkJctBotGradingCoeff * Math.Log(arg));
                            }
                            if (modeltemp.MOS2bulkJctSideGradingCoeff.Value == .5)
                            {
                                sargsw = 1 / Math.Sqrt(arg);
                            }
                            else
                            {
                                /* NOSQRT */
                                sargsw = Math.Exp(-modeltemp.MOS2bulkJctSideGradingCoeff * Math.Log(arg));
                            }
                        }
                        /* NOSQRT */
                        state.States[0][MOS2states + MOS2qbd] = temp.MOS2tBulkPot * (temp.MOS2Cbd * (1 - arg * sarg) / (1 - modeltemp.MOS2bulkJctBotGradingCoeff) +
                            temp.MOS2Cbdsw * (1 - arg * sargsw) / (1 - modeltemp.MOS2bulkJctSideGradingCoeff));
                        MOS2capbd = temp.MOS2Cbd * sarg + temp.MOS2Cbdsw * sargsw;
                    }
                    else
                    {
                        state.States[0][MOS2states + MOS2qbd] = temp.MOS2f4d + vbd * (temp.MOS2f2d + vbd * temp.MOS2f3d / 2);
                        MOS2capbd = temp.MOS2f2d + vbd * temp.MOS2f3d;
                    }
                    /* CAPZEROBYPASS */
                }

                if (method != null)
                {
                    /* (above only excludes tranop, since we're only at this
					* point if tran or tranop)
					*/

                    /* 
					* calculate equivalent conductances and currents for
					* depletion capacitors
					*/

                    /* integrate the capacitors and save results */
                    var result = method.Integrate(state, MOS2states + MOS2qbd, MOS2capbd);
                    MOS2gbd += result.Geq;
                    MOS2cbd += state.States[0][MOS2states + MOS2cqbd];
                    MOS2cd -= state.States[0][MOS2states + MOS2cqbd];
                    result = method.Integrate(state, MOS2states + MOS2qbs, MOS2capbs);
                    MOS2gbs += result.Geq;
                    MOS2cbs += state.States[0][MOS2states + MOS2cqbs];
                }
            }

            /* 
			 * check convergence
			 */
            if (!MOS2off || (!(state.Init == State.InitFlags.InitFix || state.UseSmallSignal)))
            {
                if (Check == 1)
                    state.IsCon = false;
            }
            state.States[0][MOS2states + MOS2vbs] = vbs;
            state.States[0][MOS2states + MOS2vbd] = vbd;
            state.States[0][MOS2states + MOS2vgs] = vgs;
            state.States[0][MOS2states + MOS2vds] = vds;

            /* 
			* meyer's capacitor model
			*/
            if (state.Domain == State.DomainTypes.Time || state.UseSmallSignal)
            {
                /* 
				 * calculate meyer's capacitors
				 */
                double icapgs, icapgd, icapgb;
                if (MOS2mode > 0)
                {
                    Transistor.DEVqmeyer(vgs, vgd, vgb, von, vdsat,
                        out icapgs, out icapgd, out icapgb, temp.MOS2tPhi, OxideCap);
                }
                else
                {
                    Transistor.DEVqmeyer(vgd, vgs, vgb, von, vdsat,
                        out icapgd, out icapgs, out icapgb, temp.MOS2tPhi, OxideCap);
                }
                state.States[0][MOS2states + MOS2capgs] = icapgs;
                state.States[0][MOS2states + MOS2capgd] = icapgd;
                state.States[0][MOS2states + MOS2capgb] = icapgb;

                vgs1 = state.States[1][MOS2states + MOS2vgs];
                vgd1 = vgs1 - state.States[1][MOS2states + MOS2vds];
                vgb1 = vgs1 - state.States[1][MOS2states + MOS2vbs];
                if (state.Domain == State.DomainTypes.Time && state.UseDC)
                {
                    capgs = 2 * state.States[0][MOS2states + MOS2capgs] + GateSourceOverlapCap;
                    capgd = 2 * state.States[0][MOS2states + MOS2capgd] + GateDrainOverlapCap;
                    capgb = 2 * state.States[0][MOS2states + MOS2capgb] + GateBulkOverlapCap;
                }
                else
                {
                    capgs = state.States[0][MOS2states + MOS2capgs] + state.States[1][MOS2states + MOS2capgs] + GateSourceOverlapCap;
                    capgd = state.States[0][MOS2states + MOS2capgd] + state.States[1][MOS2states + MOS2capgd] + GateDrainOverlapCap;
                    capgb = state.States[0][MOS2states + MOS2capgb] + state.States[1][MOS2states + MOS2capgb] + GateBulkOverlapCap;
                }

                /* 
				* store small - signal parameters (for meyer's model)
				* all parameters already stored, so done...
				*/

                /* PREDICTOR */
                if (method != null)
                {
                    state.States[0][MOS2states + MOS2qgs] = (vgs - vgs1) * capgs + state.States[1][MOS2states + MOS2qgs];
                    state.States[0][MOS2states + MOS2qgd] = (vgd - vgd1) * capgd + state.States[1][MOS2states + MOS2qgd];
                    state.States[0][MOS2states + MOS2qgb] = (vgb - vgb1) * capgb + state.States[1][MOS2states + MOS2qgb];
                }
                else
                {
                    /* TRANOP */
                    state.States[0][MOS2states + MOS2qgs] = capgs * vgs;
                    state.States[0][MOS2states + MOS2qgd] = capgd * vgd;
                    state.States[0][MOS2states + MOS2qgb] = capgb * vgb;
                }
                /* PREDICTOR */
            }
            /* NOBYPASS */
            if ((state.Init == State.InitFlags.InitTransient) || method == null)
            {
                /* initialize to zero charge conductances and current */

                gcgs = 0;
                ceqgs = 0;
                gcgd = 0;
                ceqgd = 0;
                gcgb = 0;
                ceqgb = 0;
            }
            else
            {
                if (capgs == 0)
                    state.States[0][MOS2states + MOS2cqgs] = 0;
                if (capgd == 0)
                    state.States[0][MOS2states + MOS2cqgd] = 0;
                if (capgb == 0)
                    state.States[0][MOS2states + MOS2cqgb] = 0;
                /* 
				 * calculate equivalent conductances and currents for
				 * meyer"s capacitors
				 */
                method.Integrate(state, out gcgs, out ceqgs, MOS2states + MOS2qgs, capgs);
                method.Integrate(state, out gcgd, out ceqgd, MOS2states + MOS2qgd, capgd);
                method.Integrate(state, out gcgb, out ceqgb, MOS2states + MOS2qgb, capgb);
                ceqgs = ceqgs - gcgs * vgs + method.Slope * state.States[0][MOS2states + MOS2qgs];
                ceqgd = ceqgd - gcgd * vgd + method.Slope * state.States[0][MOS2states + MOS2qgd];
                ceqgb = ceqgb - gcgb * vgb + method.Slope * state.States[0][MOS2states + MOS2qgb];
            }
            /* 
			* store charge storage info for meyer's cap in lx table
			*/

            /* 
			* load current vector
			*/
            ceqbs = modeltemp.MOS2type * (MOS2cbs - (MOS2gbs - state.Gmin) * vbs);
            ceqbd = modeltemp.MOS2type * (MOS2cbd - (MOS2gbd - state.Gmin) * vbd);
            if (MOS2mode >= 0)
            {
                xnrm = 1;
                xrev = 0;
                cdreq = modeltemp.MOS2type * (cdrain - MOS2gds * vds - MOS2gm * vgs - MOS2gmbs * vbs);
            }
            else
            {
                xnrm = 0;
                xrev = 1;
                cdreq = -(modeltemp.MOS2type) * (cdrain - MOS2gds * (-vds) - MOS2gm * vgd - MOS2gmbs * vbd);
            }
            rstate.Rhs[MOS2gNode] -= (modeltemp.MOS2type * (ceqgs + ceqgb + ceqgd));
            rstate.Rhs[MOS2bNode] -= (ceqbs + ceqbd - modeltemp.MOS2type * ceqgb);
            rstate.Rhs[MOS2dNodePrime] += (ceqbd - cdreq + modeltemp.MOS2type * ceqgd);
            rstate.Rhs[MOS2sNodePrime] += cdreq + ceqbs + modeltemp.MOS2type * ceqgs;

            /* 
			 * load y matrix
			 */
            MOS2DdPtr.Add(temp.MOS2drainConductance);
            MOS2GgPtr.Add(gcgd + gcgs + gcgb);
            MOS2SsPtr.Add(temp.MOS2sourceConductance);
            MOS2BbPtr.Add(MOS2gbd + MOS2gbs + gcgb);
            MOS2DPdpPtr.Add(temp.MOS2drainConductance + MOS2gds + MOS2gbd + xrev * (MOS2gm + MOS2gmbs) + gcgd);
            MOS2SPspPtr.Add(temp.MOS2sourceConductance + MOS2gds + MOS2gbs + xnrm * (MOS2gm + MOS2gmbs) + gcgs);
            MOS2DdpPtr.Add(-temp.MOS2drainConductance);
            MOS2GbPtr.Sub(gcgb);
            MOS2GdpPtr.Sub(gcgd);
            MOS2GspPtr.Sub(gcgs);
            MOS2SspPtr.Add(-temp.MOS2sourceConductance);
            MOS2BgPtr.Sub(gcgb);
            MOS2BdpPtr.Sub(MOS2gbd);
            MOS2BspPtr.Sub(MOS2gbs);
            MOS2DPdPtr.Add(-temp.MOS2drainConductance);
            MOS2DPgPtr.Add((xnrm - xrev) * MOS2gm - gcgd);
            MOS2DPbPtr.Add(-MOS2gbd + (xnrm - xrev) * MOS2gmbs);
            MOS2DPspPtr.Add(-MOS2gds - xnrm * (MOS2gm + MOS2gmbs));
            MOS2SPgPtr.Add(-(xnrm - xrev) * MOS2gm - gcgs);
            MOS2SPsPtr.Add(-temp.MOS2sourceConductance);
            MOS2SPbPtr.Add(-MOS2gbs - (xnrm - xrev) * MOS2gmbs);
            MOS2SPdpPtr.Add(-MOS2gds - xrev * (MOS2gm + MOS2gmbs));
        }

        /// <summary>
        /// Check convergence
        /// </summary>
        /// <param name="ckt">Circuit</param>
        /// <returns></returns>
        public override bool IsConvergent(Circuit ckt)
        {
            var config = ckt.Simulation.CurrentConfig;
            var state = ckt.State;

            double vbs, vgs, vds, vbd, vgd, vgdo, delvbs, delvbd, delvgs, delvds, delvgd, cdhat, cbhat;

            vbs = modeltemp.MOS2type * (state.Solution[MOS2bNode] - state.Solution[MOS2sNodePrime]);
            vgs = modeltemp.MOS2type * (state.Solution[MOS2gNode] - state.Solution[MOS2sNodePrime]);
            vds = modeltemp.MOS2type * (state.Solution[MOS2dNodePrime] - state.Solution[MOS2sNodePrime]);
            vbd = vbs - vds;
            vgd = vgs - vds;
            vgdo = state.States[0][MOS2states + MOS2vgs] - state.States[0][MOS2states + MOS2vds];
            delvbs = vbs - state.States[0][MOS2states + MOS2vbs];
            delvbd = vbd - state.States[0][MOS2states + MOS2vbd];
            delvgs = vgs - state.States[0][MOS2states + MOS2vgs];
            delvds = vds - state.States[0][MOS2states + MOS2vds];
            delvgd = vgd - vgdo;

            /* these are needed for convergence testing */

            if (MOS2mode >= 0)
            {
                cdhat = MOS2cd - MOS2gbd * delvbd + MOS2gmbs * delvbs +
                    MOS2gm * delvgs + MOS2gds * delvds;
            }
            else
            {
                cdhat = MOS2cd - (MOS2gbd - MOS2gmbs) * delvbd - 
                    MOS2gm * delvgd + MOS2gds * delvds;
            }
            cbhat = MOS2cbs + MOS2cbd + MOS2gbd * delvbd + MOS2gbs * delvbs;

            /*
             *  check convergence
             */
            double tol = config.RelTol * Math.Max(Math.Abs(cdhat), Math.Abs(MOS2cd)) + config.AbsTol;
            if (Math.Abs(cdhat - MOS2cd) >= tol)
            {
                state.IsCon = false;
                return false;
            }
            else
            {
                tol = config.RelTol * Math.Max(Math.Abs(cbhat), Math.Abs(MOS2cbs + MOS2cbd)) + config.AbsTol;
                if (Math.Abs(cbhat - (MOS2cbs + MOS2cbd)) > tol)
                {
                    state.IsCon = false;
                    return false;
                }
            }
            return true;
        }
    }
}
