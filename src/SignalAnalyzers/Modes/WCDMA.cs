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
    internal class WCDMA : Mode
    {
        public WCDMA(XsaCore xsa) : base("WCDMA", xsa)
        {
            trigger = new TriggerMenu(xsa);

            // Add LPST
            Measurements.Add(typeof(ListPowerStep), new ListPowerStep(xsa));
            Measurements.Add(typeof(CombinedWcdma), new CombinedWcdma(xsa));
            Measurements.Add(typeof(Acp), new Acp(xsa));
        }

        #region Properties
        public ListPowerStep ListPowerstep
        {
            get
            {
                return GetMeasurement<ListPowerStep>();
            }
        }

        public CombinedWcdma CombinedWCDMA
        {
            get
            {
                return GetMeasurement<CombinedWcdma>();
            }
        }

        public Acp ACP
        {
            get
            {
                return GetMeasurement<Acp>();
            }
        }
        private TriggerMenu trigger;
        public TriggerMenu Trigger
        {
            get { return trigger; }
        }

        #endregion

        #region Measurements
        public class ListPowerStep : Measurement
        {
            public ListPowerStep(XsaCore inst) : base("LPST", inst) { }

            public double Frequency
            {
                set { Xsa.ScpiCommand("LPST:LIST:FREQ " + value); }
            }

            public string CalcLpstList()
            {
                return Xsa.ScpiQuery("CALC:LPST:LIST?");
            }

            public string TriggerSource
            {
                set { Xsa.ScpiCommand("LPST:TRIG:SOUR " + value); }
            }

            public int AverageCount
            {
                set { Xsa.ScpiCommand("LPST:AVER:COUN " + value); }
            }

            public bool Averaging
            {
                set { Xsa.ScpiCommand("LPST:AVER " + (value ? "1" : "0")); }
            }

            public double ResolutionBandwidth
            {
                set { Xsa.ScpiCommand("LPST:BAND " + value); }
            }

            public double VideoBandwidth
            {
                set { Xsa.ScpiCommand("LPST:BAND:VID " + value); }
            }

            public int SweepPoints
            {
                set { Xsa.ScpiCommand("LPST:SWE:POIN " + value); }
            }

            public double SweepStepTime
            {
                set { Xsa.ScpiCommand("LPST:SWE:STEP:TIME " + value); }
            }

            public double SweepStepLength
            {
                set { Xsa.ScpiCommand("LPST:SWE:STEP:LENG " + value); }
            }

            public int StepsCount
            {
                set { Xsa.ScpiCommand("LPST:LIST:STEP " + value); }
            }

            public double SweepStepOffset
            {
                set { Xsa.ScpiCommand("LPST:LIST:SWE:STEP:OFFS " + value); }
            }

            public string ListTime
            {
                set { Xsa.ScpiCommand("LPST:LIST:TIME " + value); }
            }

            public bool ListState
            {
                set { Xsa.ScpiCommand("LPST:LIST:STAT " + (value ? "1" : "0")); }
            }

            public string Display
            {
                set { Xsa.ScpiCommand("DISP:LPST:VIEW " + value); }
            }
        }

        public class CombinedWcdma : Measurement
        {
            public CombinedWcdma(XsaCore inst)
                : base("CWCD", inst)
            {
            }

            public string Fetch1()
            {
                return Xsa.ScpiQuery("FETC:CWCD?");
            }

            public string Fetch2()
            {
                return Xsa.ScpiQuery("FETC:CWCD2?");
            }

            public int AveragingCount
            {
                set { Xsa.ScpiCommand("CWCD:AVER:COUN " + value); }
            }

            public bool Averaging
            {
                set { Xsa.ScpiCommand("CWCD:AVER " + (value ? "1" : "0")); }
            }

            public double[] FrequencyList
            {
                set
                {
                    String command = "CWCD:LIST:FREQ ";
                    foreach (double d in value)
                    {
                        command += d + "Hz,";
                    }
                    command = command.TrimEnd(',');

                    Xsa.ScpiCommand(command);
                }
            }

            public bool IncludeIQOffset
            {
                set { Xsa.ScpiCommand("CALC:CWCD:RHO:IQOF:INCL " + (value ? "1" : "0")); }
            }

            public string TriggerSource
            {
                set { Xsa.ScpiCommand("TRIG:CWCD:SOUR " + value); }
            }
        }

        public class Acp : Measurement
        {
            public Acp(XsaCore inst)
                : base("ACP", inst)
            {
            }

            public string Fetch()
            {
                return Xsa.ScpiQuery("FETC:ACP?");
            }

            public string TriggerSource
            {
                set { Xsa.ScpiCommand("TRIG:ACP:SOUR " + value); }
            }

            public int AveragingCount
            {
                set { Xsa.ScpiCommand("ACP:AVER:COUN " + value); }
            }

            public bool Averaging
            {
                set { Xsa.ScpiCommand("ACP:AVER " + (value ? "1" : "0")); }
            }
        }
        #endregion

        #region Menus
        public class TriggerMenu
        {
            private XsaCore Parent;

            public TriggerMenu(XsaCore parent)
            {
                Parent = parent;
            }

            //private double level = double.NaN;
            public double RFBurstLevel
            {
                set
                {
                    //if (value != level)
                    //{
                    Parent.ScpiCommand("TRIG:RFB:LEV:ABS " + value);
                    //    level = value;
                    //}
                }
                get
                {
                    //level = double.Parse(Parent.ScpiQuery("TRIG:RFB:LEV:ABS?"));
                    return double.Parse(Parent.ScpiQuery("TRIG:RFB:LEV:ABS?"));
                }
            }

            public double RFBurstDelay
            {
                set
                {
                    Parent.ScpiCommand("TRIG:RFB:DEL " + value);
                    Parent.ScpiCommand("TRIG:RFB:DEL:STAT " + (value > 0 ? "1" : "0"));
                }
            }

            public bool RFBurstDelayState
            {
                set { Parent.ScpiCommand("TRIG:RFB:DEL:STAT " + (value ? "1" : "0")); }
            }
        }
        #endregion
    }
}
