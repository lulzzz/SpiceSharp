﻿using System;

namespace SpiceSharp.Components.Semiconductors
{
    /// <summary>
    /// A class with static methods for semiconductor devices
    /// </summary>
    public static class Semiconductor
    {
        /// <summary>
        /// Limit the per-iteration change of PN junction voltages
        /// Defined as DEVpnjlim in devsup.c
        /// </summary>
        /// <param name="vnew">The new voltage</param>
        /// <param name="vold">The old voltage</param>
        /// <param name="vt">Vt</param>
        /// <param name="vcrit">The critical voltage</param>
        /// <returns></returns>
        public static double DEVpnjlim(double vnew, double vold, double vt, double vcrit, ref bool limited)
        {
            double arg;
            if ((vnew > vcrit) && (Math.Abs(vnew - vold) > (vt + vt)))
            {
                if (vold > 0)
                {
                    arg = 1 + (vnew - vold) / vt;
                    if (arg > 0)
                        vnew = vold + vt * Math.Log(arg);
                    else
                        vnew = vcrit;
                }
                else
                    vnew = vt * Math.Log(vnew / vt);
                limited = true;
            }
            else
                limited = false;
            return vnew;
        }
    }
}
