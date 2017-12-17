﻿using System;
using System.Text;
using System.IO;
using SpiceSharp;
using SpiceSharp.Circuits;
using SpiceSharp.Parameters;
using SpiceSharp.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiceSharp.Diagnostics;
using SpiceSharp.Parser.Readers;
using System.Collections.Generic;

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
            var mosfets = r.Netlist.Readers[StatementType.Component].Find<MosfetReader>().Mosfets;
            BSIMParser.AddMosfetGenerators(mosfets);
            var levels = r.Netlist.Readers[StatementType.Model].Find<MosfetModelReader>().Levels;
            BSIMParser.AddMosfetModelGenerators(levels);
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
        protected T Test<T>(Netlist n, Identifier name, string[] names = null, double[] values = null, Identifier[] nodes = null) where T: Entity
        {
            var obj = n.Circuit.Objects[name];
            Assert.AreEqual(typeof(T), obj.GetType());

            // Test all parameters
            if (names != null)
                TestParameters(obj, names, values);

            // Test all nodes
            if (nodes != null)
                TestNodes((Component)obj, nodes);

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
        protected void TestParameters(object p, string[] names, double[] values)
        {
            if (names.Length != values.Length)
                throw new Exception("Unit test error: parameter name array does not match the value array");

            Dictionary<string, Func<double>> getter = new Dictionary<string, Func<double>>();
            var parameters = SpiceParameters.List(p.GetType());
            foreach (var parameter in parameters)
            {
                foreach (var n in parameter.Names)
                    getter.Add(n.Name.ToLower(), () => parameter.Get(p));
            }

            // Test all parameters
            for (int i = 0; i < names.Length; i++)
            {
                double expected = values[i];
                double actual = getter[names[i]]();
                double tol = Math.Min(Math.Abs(expected), Math.Abs(actual)) * 1e-6;
                Assert.AreEqual(expected, actual, tol);
            }
        }

        /// <summary>
        /// Test the nodes of a circuitcomponent
        /// </summary>
        /// <param name="c">The component</param>
        /// <param name="nodes">The expected node names</param>
        protected void TestNodes(Component c, Identifier[] nodes)
        {
            // Test all nodes
            for (int i = 0; i < nodes.Length; i++)
            {
                Identifier expected = nodes[i];
                Identifier actual = c.GetNode(i);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
