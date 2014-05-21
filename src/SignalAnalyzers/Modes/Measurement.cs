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
    public class Measurement
    {
        protected XsaCore Xsa;
        public string ScpiName { get; set; }

        public Measurement(string name, XsaCore inst)
        {
            ScpiName = name;
            Xsa = inst;
        }

        public virtual void Configure()
        {
            Xsa.ScpiCommand("CONF:" + ScpiName);
        }

        public virtual void Initiate()
        {
            Xsa.ScpiCommand("INITiate:" + ScpiName);
        }
    }
}
