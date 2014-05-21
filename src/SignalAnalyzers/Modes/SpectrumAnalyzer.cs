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
    internal class SpectrumAnalyzer : Mode
    {
        public SpectrumAnalyzer(XsaCore xsa) : base("SA", xsa)
        {
            trigger = new TriggerMenu(Xsa);

            // Add SAN
            Measurements.Add(typeof(SAN), new SAN(Xsa));
        }

        public SAN San
        {
            get { return GetMeasurement<SAN>(); }
        }

        public class SAN : Measurement
        {
            public SAN(XsaCore inst) : base("SAN", inst) { }

            public override void Initiate()
            {
                Xsa.ScpiCommand("INITiate:SAN");
            }

            public string TriggerSource
            {
                set { Xsa.ScpiCommand("TRIG:SAN:SOUR " + value); }
            }
        }

        private TriggerMenu trigger;
        public TriggerMenu Trigger
        {
            get { return trigger; }
        }
        public class TriggerMenu
        {
            private XsaCore Xsa;

            public TriggerMenu(XsaCore parent)
            {
                Xsa = parent;
            }

            private string source;
            public string Source
            {
                set
                {
                    if (source == null || value != source)
                    {
                        source = value;
                        Xsa.ScpiCommand("TRIG:SOUR " + value);
                    }
                }
            }

            private double level = double.NaN;
            public double RFBurstLevel
            {
                set
                {
                    if (value != level)
                    {
                        Xsa.ScpiCommand("TRIG:RFB:LEV:ABS " + value);
                        level = value;
                    }
                }
                get
                {
                    level = double.Parse(Xsa.ScpiQuery("TRIG:RFB:LEV:ABS?"));
                    return level;
                }
            }

            public double RFBurstDelay
            {
                set
                {
                    Xsa.ScpiCommand("TRIG:RFB:DEL " + value);
                    Xsa.ScpiCommand("TRIG:RFB:DEL:STAT " + (value > 0 ? "1" : "0"));
                }
            }

            public bool RFBurstDelayState
            {
                set { Xsa.ScpiCommand("TRIG:RFB:DEL:STAT " + (value ? "1" : "0")); }
            }
        }
    }
}
