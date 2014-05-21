/*
 ****************************************************************************
 * Author: Marc Tapalla
 * Email: marc.tapalla@gmail.com
 *   
 * Library for SCPI remote control of Agilent's X-Series Signal Analyzers.
 * 
 * Instruments Supported:
 *  PXA (N9030A)
 *  MXA (N9020A)
 *  EXA (N9010A)
 *  CXA (N9000A) untested
 * 
 ****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XsaLib
{
    public abstract class Mode
    {
        protected XsaCore Xsa;
        internal Dictionary<Type, Measurement> Measurements;

        public Mode(string name, XsaCore xsa)
        {
            ScpiName = name;
            Xsa = xsa;

            Measurements = new Dictionary<Type, Measurement>();
        }

        public string ScpiName { get; set; }

        protected T GetMeasurement<T>() where T : Measurement
        {
            return (T)Measurements[typeof(T)];
        }

        public bool HasMeasurement<T>() where T : Measurement
        {
            return Measurements.ContainsKey(typeof(T));
        }

        public virtual void Load()
        {
            Xsa.ScpiCommand("INST:SEL " + ScpiName);
        }
    }
}
