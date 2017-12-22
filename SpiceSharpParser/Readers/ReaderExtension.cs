﻿using System;
using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharp.Circuits;
using SpiceSharp.Parser.Subcircuits;
using static SpiceSharp.Parser.SpiceSharpParserConstants;
using System.Linq;
using SpiceSharp.Attributes;

namespace SpiceSharp.Parser.Readers
{
    /// <summary>
    /// Provides static methods for reading statements.
    /// </summary>
    public static class ReaderExtension
    {
        /// <summary>
        /// This dictionary will keep the parameters
        /// </summary>
        private static Dictionary<Type, Dictionary<string, SpiceParameter>> Parameters = new Dictionary<Type, Dictionary<string, SpiceParameter>>();

        /// <summary>
        /// Register a type in the setter dictionary
        /// </summary>
        /// <param name="t"></param>
        public static Dictionary<string, SpiceParameter> GetParameters(Type t)
        {
            // Create a new dictionary if necessary
            Dictionary<string, SpiceParameter> dict;
            if (!Parameters.TryGetValue(t, out dict))
            {
                dict = new Dictionary<string, SpiceParameter>();
                Parameters.Add(t, dict);

                // Add the setters
                var parameters = SpiceParameters.List(t);
                foreach (var parameter in parameters)
                {
                    foreach (var name in parameter.Names)
                        dict.Add(name.Name.ToLower(), parameter);
                }
            }
            return dict;
        }

        /// <summary>
        /// Read named parameters for a Parameterized object
        /// </summary>
        /// <param name="obj">The parameterized object</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="start">The starting index</param>
        public static void ReadParameters(this Netlist netlist, object obj, List<Token> parameters, int start = 0)
        {
            var compontentParameters = GetParameters(obj.GetType());
            for (int i = start; i < parameters.Count; i++)
            {              
                if (parameters[i].kind == TokenConstants.ASSIGNMENT)
                {
                    AssignmentToken at = parameters[i] as AssignmentToken;
                    string pname;
                    if (at.Name.kind == WORD)
                        pname = at.Name.image.ToLower();
                    else
                        throw new ParseException(parameters[i], "Invalid assignment");

                    SpiceParameter p;
                    if (compontentParameters.TryGetValue(pname, out p) == false)
                    {
                        continue; //TODO: improve ...
                    }
                    
                    switch (at.Value.kind)
                    {
                        case VALUE:
                            if (compontentParameters.TryGetValue(pname, out p))
                                p.Set(obj, netlist.Readers.ParseDouble(at.Value.image.ToLower()));
                            else
                                Diagnostics.CircuitWarning.Warning(obj, $"Unrecognized parameter \"{pname}\"");
                            break;
                        case EXPRESSION:
                            if (compontentParameters.TryGetValue(pname, out p))
                                p.Set(obj, netlist.Readers.ParseDouble(at.Value.image.Substring(1, at.Value.image.Length - 2).ToLower()));
                            else
                                Diagnostics.CircuitWarning.Warning(obj, $"Unrecognized parameter \"{pname}\"");
                            break;
                        case WORD:
                        case STRING:
                            if (compontentParameters.TryGetValue(pname, out p))
                                p.Set(obj, netlist.ParseString(at.Value));
                            else
                                Diagnostics.CircuitWarning.Warning(obj, $"Unrecognized parameter \"{pname}\"");
                            break;
                        default:
                            throw new ParseException(parameters[i], "Invalid assignment");
                    }
                }
            }

            if (obj is Entity ent) {
                var behaviorParameters = ent.GetParameters();

                for (int i = start; i < parameters.Count; i++)
                {
                    if (parameters[i].kind == TokenConstants.ASSIGNMENT)
                    {
                        AssignmentToken at = parameters[i] as AssignmentToken;
                        string pname;
                        if (at.Name.kind == WORD)
                            pname = at.Name.image.ToLower();
                        else
                            throw new ParseException(parameters[i], "Invalid assignment");

                        if (behaviorParameters.Contains(pname) == false)
                        {
                            continue;
                        }

                        switch (at.Value.kind)
                        {
                            case VALUE:
                                ent.Set(pname, netlist.Readers.ParseDouble(at.Value.image.ToLower()));
                                break;
                            case EXPRESSION:
                                 ent.Set(pname, netlist.Readers.ParseDouble(at.Value.image.Substring(1, at.Value.image.Length - 2).ToLower()));
                                break;
                            case WORD:
                            default:
                                throw new ParseException(parameters[i], "Invalid assignment");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read nodes for a component
        /// </summary>
        /// <param name="c">The circuit component</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="index">The index where to start reading, defaults to 0</param>
        public static void ReadNodes(this Component c, SubcircuitPath path, List<Token> parameters, int index = 0)
        {
            int count = c.PinCount;

            // Check that there are enough parameters left to read
            if (parameters.Count < index + count)
            {
                if (parameters.Count > 0)
                    throw new ParseException(parameters[parameters.Count - 1], $"{count} nodes expected");
                throw new ParseException($"Unexpected end of line, {count} nodes expected");
            }

            // Extract the nodes
            Identifier[] nodes = new Identifier[count];
            for (int i = index; i < index + count; i++)
            {
                if (IsNode(parameters[i]))
                {
                    // Map to a new node if necessary, else make the node local to the current path
                    Identifier node = new Identifier(parameters[i].image);
                    if (path.NodeMap.TryGetValue(node, out Identifier mapped))
                        node = mapped;
                    else if (path.InstancePath != null)
                        node = path.InstancePath.Grow(node);

                    // Store the node
                    nodes[i] = node;
                }
                else
                    throw new ParseException(parameters[i], "Node expected");
            }

            // Succeeded
            c.Connect(nodes);
        }

        /// <summary>
        /// Check if the token can represent a node
        /// </summary>
        /// <param name="t">Token</param>
        /// <returns></returns>
        public static bool IsNode(Token t)
        {
            if (t.kind == WORD)
                return true;
            if (t.kind == IDENTIFIER)
                return true;
            if (IsNumber(t.image))
                return true;
            return false;
        }

        /// <summary>
        /// Check if the token can represent a (component) name
        /// </summary>
        /// <param name="t">Token</param>
        /// <returns></returns>
        public static bool IsName(Token t)
        {
            if (t.kind == WORD && char.IsLetter(t.image[0]))
                return true;
            if (t.kind == IDENTIFIER && char.IsLetter(t.image[0]))
                return true;
            return false;
        }

        /// <summary>
        /// Check if the token can represent a value
        /// </summary>
        /// <param name="t">Token</param>
        /// <returns></returns>
        public static bool IsValue(Token t)
        {
            if (t.kind == VALUE)
                return true;
            if (t.kind == EXPRESSION)
                return true;
            return false;
        }

        /// <summary>
        /// Parse a token to a double.
        /// Both literal values and expressions {...} are accepted.
        /// </summary>
        /// <param name="netlist">The netlist</param>
        /// <param name="t">The token</param>
        /// <returns></returns>
        public static double ParseDouble(this Netlist netlist, Token t)
        {
            switch (t.kind)
            {
                case VALUE:
                    return netlist.Readers.ParseDouble(t.image);
                case EXPRESSION:
                    return netlist.Readers.ParseDouble(t.image.Substring(1, t.image.Length - 2));
                default:
                    throw new ParseException(t, "Value or expression expected");
            }
        }

        /// <summary>
        /// Parse a vector of double values
        /// </summary>
        /// <param name="netlist">The netlist</param>
        /// <param name="t">The token</param>
        /// <returns></returns>
        public static double[] ParseDoubleVector(this Netlist netlist, Token t)
        {
            if (t.kind == TokenConstants.VECTOR)
            {
                var vt = t as VectorToken;
                double[] values = new double[vt.Tokens.Length];
                for (int i = 0; i < values.Length; i++)
                    values[i] = netlist.ParseDouble(vt.Tokens[i]);
                return values;
            }
            else if (t.kind == VALUE || t.kind == EXPRESSION)
                return new double[] { netlist.ParseDouble(t) };
            else
                throw new ParseException(t, "Vector expected");
        }

        /// <summary>
        /// Parse a token to a string
        /// Both literal and quoted "..." are accepted.
        /// </summary>
        /// <param name="netlist">The netlist</param>
        /// <param name="t">The token</param>
        /// <returns></returns>
        public static string ParseString(this Netlist netlist, Token t)
        {
            switch (t.kind)
            {
                case WORD:
                    return t.image;
                case STRING:
                    return t.image.Substring(1, t.image.Length - 2);
                default:
                    throw new ParseException(t, "String expected");
            }
        }

        /// <summary>
        /// Find a model
        /// </summary>
        /// <typeparam name="T">The model class</typeparam>
        /// <param name="netlist">Netlist</param>
        /// <param name="t">Token</param>
        /// <returns></returns>
        public static T FindModel<T>(this Netlist netlist, Token t) where T : Entity
        {
            switch (t.kind)
            {
                case WORD:
                case IDENTIFIER:
                    return netlist.Path.FindModel<T>(netlist.Circuit.Objects, new Identifier(t.image));
                default:
                    throw new ParseException(t, "Invalid model name");
            }
        }

        /// <summary>
        /// Check for digit-only strings
        /// </summary>
        /// <param name="value">The string</param>
        /// <returns></returns>
        private static bool IsNumber(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsNumber(value[i]))
                    return false;
            }
            return true;
        }
    }
}
