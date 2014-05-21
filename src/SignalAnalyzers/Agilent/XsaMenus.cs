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
    public abstract partial class XsaCore : Instrument
    {
        public class MarkerMenu : Menu
        {
            public MarkerMenu(XsaCore xsa) : base(xsa) { }

            public void EnableMarker(int markerNumber)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":STAT 1");
            }
            public void DisableMarker(int markerNumber)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":STAT 0");
            }
            public void SetMode(int markerNumber, string mode)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":MODE " + mode);
            }
            public void SetFrequency(int markerNumber, double frequency)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":X " + frequency);
            }
            public void SetCounter(int markerNumber, bool state)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":FCO " + (state ? "1" : "0"));
            }
            public void GetPeak(int markerNumber)
            {
                XSA.ScpiCommand("CALC:MARK" + markerNumber + ":MAX");
            }
            public string GetY(int markerNumber)
            {
                return XSA.ScpiQuery("CALC:MARK" + markerNumber + ":Y?");
            }
            public string GetFcoX(int markerNumber)
            {
                return XSA.ScpiQuery("CALC:MARK" + markerNumber + ":FCO:X?");
            }
        }

        public class TriggerMenu : Menu
        {
            public TriggerMenu(XsaCore xsa) : base(xsa) { }

            public string TriggerSource
            {
                set { XSA.ScpiCommand("TRIG:SOUR " + value); }
            }
        }

        public abstract class Menu
        {
            private XsaCore xsa;
            public Menu(XsaCore xsa) { this.xsa = xsa; }
            public XsaCore XSA { get { return xsa; } }
        }
    }
}
