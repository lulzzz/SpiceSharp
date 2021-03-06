﻿using SpiceSharp.Circuits;
using MathNet.Numerics.LinearAlgebra;

namespace SpiceSharp.Simulations
{
    /// <summary>
    /// Provides static methods for basic simulations involving the circuit
    /// </summary>
    public static class SimulationCircuit
    {
        /// <summary>
        /// Load the circuit for simulation
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public static void Load(this Circuit ckt)
        {
            var state = ckt.State;
            var rstate = state.Real;
            var nodes = ckt.Nodes;

            // Start the stopwatch
            ckt.Statistics.LoadTime.Start();

            // Clear rhs and matrix
            rstate.Clear();

            // Load all devices
            // ckt.Load(this, state);
            foreach (var c in ckt.Objects)
                c.Load(ckt);

            // Check modes
            if (state.UseDC)
            {
                // Consider doing nodeset & ic assignments
                if ((state.Init & (CircuitState.InitFlags.InitJct | CircuitState.InitFlags.InitFix)) != 0)
                {
                    // Do nodesets
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        if (nodes.Nodeset.ContainsKey(node.Name))
                        {
                            double ns = nodes.Nodeset[node.Name];
                            if (ZeroNoncurRow(rstate.Matrix, nodes, node.Index))
                            {
                                rstate.Rhs[node.Index] = 1.0e10 * ns;
                                rstate.Matrix[node.Index, node.Index] = 1.0e10;
                            }
                            else
                            {
                                rstate.Rhs[node.Index] = ns;
                                rstate.Solution[node.Index] = ns;
                                rstate.Matrix[node.Index, node.Index] = 1.0;
                            }
                        }
                    }
                }

                if (state.Domain == CircuitState.DomainTypes.Time && !state.UseIC)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        if (nodes.IC.ContainsKey(node.Name))
                        {
                            double ic = nodes.IC[node.Name];
                            if (ZeroNoncurRow(rstate.Matrix, nodes, node.Index))
                            {
                                rstate.Rhs[node.Index] = 1.0e10 * ic;
                                rstate.Matrix[node.Index, node.Index] = 1.0e10;
                            }
                            else
                            {
                                rstate.Rhs[node.Index] = ic;
                                rstate.Solution[node.Index] = ic;
                                rstate.Matrix[node.Index, node.Index] = 1.0;
                            }
                        }
                    }
                }
            }

            // Keep statistics
            ckt.Statistics.LoadTime.Stop();
        }

        /// <summary>
        /// Set the initial conditions
        /// </summary>
        /// <param name="ckt"></param>
        public static void Ic(this Circuit ckt)
        {
            var state = ckt.State;
            var rstate = state.Real;
            var nodes = ckt.Nodes;

            // Clear the current solution
            rstate.Solution.Clear();

            // Go over all nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (nodes.Nodeset.ContainsKey(node.Name))
                {
                    state.HadNodeset = true;
                    rstate.Solution[node.Index] = nodes.Nodeset[node.Name];
                }
                if (nodes.IC.ContainsKey(node.Name))
                {
                    rstate.Solution[node.Index] = nodes.IC[node.Name];
                }
            }

            // Use initial conditions
            if (state.UseIC)
            {
                foreach (var c in ckt.Objects)
                    c.SetIc(ckt);
            }
        }

        /// <summary>
        /// Reset the row to 0.0 and return true if the row is a current equation
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <param name="nodes">The list of nodes</param>
        /// <param name="rownum">The row number</param>
        /// <returns></returns>
        private static bool ZeroNoncurRow(Matrix<double> matrix, CircuitNodes nodes, int rownum)
        {
            bool currents = false;
            for (int n = 0; n < nodes.Count; n++)
            {
                var node = nodes[n];
                double x = matrix[rownum, node.Index];
                if (x != 0.0)
                {
                    if (node.Type == CircuitNode.NodeType.Current)
                        currents = true;
                    else
                        matrix[rownum, node.Index] = 0.0;
                }
            }
            return currents;
        }
    }
}
