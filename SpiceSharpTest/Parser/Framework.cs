﻿using System;
using System.Text;
using System.IO;
using SpiceSharp.Parser;
using SpiceSharp.Parameters;
using SpiceSharp.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiceSharp.Diagnostics;

namespace SpiceSharpTest.Parser
{
    /// <summary>
    /// A nice framework
    /// </summary>
    public class Framework
    {
        /// <summary>
        /// Run a netlist using the standard parser
        /// </summary>
        /// <param name="lines">The netlist to parse</param>
        /// <returns></returns>
        protected Netlist Run(params string[] lines)
        {
            string netlist = string.Join(Environment.NewLine, lines);
            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(netlist));

            // Create the parser and run it
            NetlistReader r = new NetlistReader();

            // Add our BSIM transistor models
            var mosfets = r.Netlist.Readers[SpiceSharp.Parser.Readers.StatementType.Component].Find<SpiceSharp.Parser.Readers.MosfetReader>().Mosfets;
            mosfets.Add(typeof(BSIM1Model), BSIMParser.GenerateBSIM1);
            mosfets.Add(typeof(BSIM2Model), BSIMParser.GenerateBSIM2);
            mosfets.Add(typeof(BSIM3v30Model), BSIMParser.GenerateBSIM3);
            mosfets.Add(typeof(BSIM3v24Model), BSIMParser.GenerateBSIM3);
            var levels = r.Netlist.Readers[SpiceSharp.Parser.Readers.StatementType.Model].Find<SpiceSharp.Parser.Readers.MosfetModelReader>().Levels;
            levels.Add(4, BSIMParser.GenerateBSIM1Model);
            levels.Add(5, BSIMParser.GenerateBSIM2Model);
            levels.Add(49, BSIMParser.GenerateBSIM3Model);
            r.Parse(m);

            // Return the generated netlist
            return r.Netlist;
        }

        /// <summary>
        /// Perform the setup and temperature calculations
        /// This is often needed to calculate the default parameters
        /// </summary>
        /// <param name="n"></param>
        protected void Initialize(Netlist n)
        {
            n.Circuit.Setup();
            foreach (var c in n.Circuit.Objects)
                c.Temperature(n.Circuit);
        }

        /// <summary>
        /// Test a circuit object for all its parameters
        /// </summary>
        /// <param name="n">The netlist</param>
        /// <param name="name">The name of the object</param>
        /// <param name="names">The parameter names</param>
        /// <param name="values">The parameter values</param>
        /// <param name="nodes">The nodes (optional)</param>
        /// <returns></returns>
        protected T Test<T>(Netlist n, string name, string[] names = null, double[] values = null, string[] nodes = null)
        {
            ICircuitObject obj = n.Circuit.Objects[name.ToLower()];
            Assert.AreEqual(typeof(T), obj.GetType());

            // Test all parameters
            if (names != null)
                TestParameters((IParameterized)obj, names, values);

            // Test all nodes
            if (nodes != null)
                TestNodes((ICircuitComponent)obj, nodes);

            // Make sure there are no warnings
            if (CircuitWarning.Warnings.Count > 0)
                throw new Exception("Warning: " + CircuitWarning.Warnings[0]);
            return (T)obj;
        }

        /// <summary>
        /// Test a circuit object for all its parameters
        /// </summary>
        /// <param name="n">The netlist</param>
        /// <param name="name">The name of the object</param>
        /// <param name="names">The parameter names</param>
        /// <param name="values">The parameter values</param>
        /// <param name="nodes">The nodes (optional)</param>
        /// <returns></returns>
        protected T Test<T>(Netlist n, string[] name, string[] names = null, double[] values = null, string[] nodes = null)
        {
            for (int i = 0; i < name.Length; i++)
                name[i] = name[i].ToLower();
            ICircuitObject obj = n.Circuit.Objects[name];
            Assert.AreEqual(typeof(T), obj.GetType());

            // Test all parameters
            if (names != null)
                TestParameters((IParameterized)obj, names, values);

            // Test all nodes
            if (nodes != null)
                TestNodes((ICircuitComponent)obj, nodes);

            // Make sure there are no warnings
            if (CircuitWarning.Warnings.Count > 0)
                throw new Exception("Warning: " + CircuitWarning.Warnings[0]);
            return (T)obj;
        }

        /// <summary>
        /// Test the parameters of a parameterized object
        /// </summary>
        /// <param name="obj">The parameterized object</param>
        /// <param name="names">The parameter names</param>
        /// <param name="values">The expected parameter values</param>
        protected void TestParameters(IParameterized p, string[] names, double[] values)
        {
            if (names.Length != values.Length)
                throw new Exception("Unit test error: parameter name array does not match the value array");

            // Test all parameters
            for (int i = 0; i < names.Length; i++)
            {
                double expected = values[i];
                double actual = p.Ask(names[i]);
                double tol = Math.Min(Math.Abs(expected), Math.Abs(actual)) * 1e-6;
                Assert.AreEqual(expected, actual, tol);
            }
        }

        /// <summary>
        /// Test the nodes of a circuitcomponent
        /// </summary>
        /// <param name="c">The component</param>
        /// <param name="nodes">The expected node names</param>
        protected void TestNodes(ICircuitComponent c, string[] nodes)
        {
            // Test all nodes
            for (int i = 0; i < nodes.Length; i++)
            {
                string expected = nodes[i].ToLower();
                string actual = c.GetNode(i);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}