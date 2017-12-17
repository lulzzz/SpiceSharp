﻿using System;
using SpiceSharp.Diagnostics;
using SpiceSharp.Circuits;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A class that represents a circuit component/device.
    /// It can be connected in a circuit and it also has parameters.
    /// </summary>
    public abstract class Component : Entity
    {
        /// <summary>
        /// Private variables
        /// </summary>
        private Identifier[] connections = null;
        private int[] indices = null;

        /// <summary>
        /// Get the number of nodes
        /// </summary>
        public virtual int PinCount => connections.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the component</param>
        public Component(Identifier name, int nodecount)
            : base(name)
        {
            // Initialize
            if (nodecount > 0)
            {
                connections = new Identifier[nodecount];
                indices = new int[nodecount];
            }
            else
            {
                connections = null;
                indices = null;
            }
        }

        /// <summary>
        /// Get the model of the circuit component (if any)
        /// </summary>
        public Entity Model { get; protected set; } = null;

        /// <summary>
        /// Connect the component in the circuit
        /// </summary>
        /// <param name="nodes"></param>
        public virtual void Connect(params Identifier[] nodes)
        {
            if (nodes.Length != connections.Length)
                throw new CircuitException($"{Name}: Node count mismatch. {nodes.Length} given, {connections.Length} expected.");
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null)
                    throw new ArgumentNullException("node " + (i + 1));
                connections[i] = nodes[i];
            }
        }

        /// <summary>
        /// Get the connection of the component
        /// </summary>
        /// <param name="i">The index</param>
        /// <returns></returns>
        public virtual Identifier GetNode(int i)
        {
            if (i < 0 || i >= connections.Length)
                throw new IndexOutOfRangeException();
            return connections[i];
        }

        /// <summary>
        /// Get the node index of the component
        /// </summary>
        /// <param name="i">The index</param>
        /// <returns></returns>
        public virtual int GetNodeIndex(int i)
        {
            if (i < 0 || i >= connections.Length)
                throw new IndexOutOfRangeException();
            return indices[i];
        }

        /// <summary>
        /// Helper function for binding nodes to the circuit
        /// </summary>
        /// <param name="ckt"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        protected Node[] BindNodes(Circuit ckt)
        {
            // Map connected nodes
            Node[] nodes = new Node[connections.Length];
            for (int i = 0; i < connections.Length; i++)
            {
                nodes[i] = ckt.Nodes.Map(connections[i]);
                indices[i] = nodes[i].Index;
            }

            // Return all nodes
            return nodes;
        }

        /// <summary>
        /// Helper function for binding an extra equation in a circuit
        /// </summary>
        /// <param name="ckt">The circuit</param>
        /// <param name="type">The type</param>
        /// <returns></returns>
        protected Node CreateNode(Circuit ckt, Identifier name, Node.NodeType type = Node.NodeType.Voltage)
        {
            // TODO: remove this after finishing refactor of BSIM project

            // Map the extra equations
            return ckt.Nodes.Create(name, type);
        }
    }
}
