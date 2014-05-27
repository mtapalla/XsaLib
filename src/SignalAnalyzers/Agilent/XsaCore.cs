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
using System.Xml.Serialization;

namespace XsaLib
{
    #region Constants

    public static class MechAttenLimit
    {
        public const double MAX = 70.0;
        public const double MIN = 0.0;
    }

    #endregion Constants

    public abstract partial class XsaCore : Instrument
    {
        private Dictionary<Type, Mode> Modes = new Dictionary<Type, Mode>();

        public XsaCore()
            : base()
        {
            LoadMenus();
            LoadModes();
        }

        private void LoadMenus()
        {
            markerMenu = new MarkerMenu(this);
            triggerMenu = new TriggerMenu(this);
        }

        private void LoadModes()
        {
            // TODO: Auto-detect available modes and load accordingly
            Modes.Add(typeof(SpectrumAnalyzer), new SpectrumAnalyzer(this));
            Modes.Add(typeof(WCDMA), new WCDMA(this));
        }

        #region Accessors
        public double MechanicalAttenuation
        {
            set
            {
                if (value > MechAttenLimit.MAX)
                    ScpiCommand("POW:ATT " + MechAttenLimit.MAX);
                else if (value < MechAttenLimit.MIN)
                    ScpiCommand("POW:ATT " + MechAttenLimit.MIN);
                else
                    ScpiCommand("POW:ATT " + value);
            }
            get { return double.Parse(ScpiQuery("POW:ATT?")); }
        }

        public double CenterFrequency
        {
            set
            {
                if (value < -80e6 || value > 51e9)
                    throw new ArgumentOutOfRangeException("CenterFreq", value, "Must be between -80e6 and 50e9 Hz");

                ScpiCommand(String.Format("FREQ:CENT {0} GHz", value / 1e9));
            }
            get
            {
                ScpiCommand("FREQ:CENT?");
                return double.Parse(ReadString());
            }
        }

        public double Span
        {
            set { ScpiCommand(String.Format("FREQ:SPAN {0}", value)); }
            get { return double.Parse(ScpiQuery("FREQ:SPAN?")); }
        }

        public bool ContinuousSweep
        {
            set { ScpiCommand("INIT:CONT " + (value ? "ON" : "OFF")); }
            get { return ScpiQuery("INIT:CONT?") == "1" ? true : false; }
        }

        // Sets the analyzer for Continuous measurement operation. The single/continuous state is Meas Global so the setting will affect all measurements.
        public bool SingleSweep
        {
            set { ScpiCommand("INIT:CONT " + (value ? "OFF" : "ON")); }
            get { return ScpiQuery("INIT:CONT?") == "0" ? true : false; }
        }

        public bool PreselectorEnabled
        {
            set { ScpiCommand("POW:MW:PRES " + (value ? "1" : "0")); }
            get { return ScpiQuery("POW:MW:PRES?") == "1" ? true : false; }
        }

        public bool DisplayEnabled
        {
            set { ScpiCommand("DISP:ENAB " + (value ? "1" : "0")); }
            get { return (CleanScpiQuery(ScpiQuery("DISP:ENAB?")).Equals("1") ? true : false); }
        }

        public bool AutoCal
        {
            set { ScpiCommand("CAL:AUTO " + (value ? "ON" : "OFF")); }
            get { return (CleanScpiQuery(ScpiQuery("CAL:AUTO?")).Equals("ON") ? true : false); }
        }

        public bool AutoRBW
        {
            set { ScpiCommand("BAND:AUTO " + (value ? "1" : "0")); }
            get { return (CleanScpiQuery(ScpiQuery("BAND:AUTO?")).Equals("1") ? true : false); }
        }

        public bool AutoVBW
        {
            set { ScpiCommand("BAND:VID:AUTO " + (value ? "1" : "0")); }
            get { return (CleanScpiQuery(ScpiQuery("BAND:VID:AUTO?")).Equals("1") ? true : false); }
        }

        public bool AutoSweepTime
        {
            set { ScpiCommand("SWE:TIME:AUTO " + (value ? "1" : "0")); }
            get { return (CleanScpiQuery(ScpiQuery("SWE:TIME:AUTO?")).Equals("1") ? true : false); }
        }

        public double ResolutionBandwidth
        {
            set { ScpiCommand("BAND " + value); }
        }

        public double VideoBandwidth
        {
            set { ScpiCommand("BAND:VID " + value); }
        }

        public string[] Options
        {
            get
            {
                ScpiCommand("*OPT?");
                string s = ReadString();

                s = s.Trim(new char[] { '\"', ' ', '\n', '\r' });

                string[] options = s.Split(',');
                return options;
            }
        }

        #endregion

        #region Menus
        private MarkerMenu markerMenu;
        public MarkerMenu Markers
        {
            get { return markerMenu; }
        }

        private TriggerMenu triggerMenu;
        public TriggerMenu Trigger
        {
            get { return triggerMenu; }
        }
        #endregion

        public T GetMode<T>() where T : Mode
        {
            try
            {
                return (T)Modes[typeof(T)];
            }
            catch (Exception ex)
            {
                throw new Exception("Mode \"" + typeof(T).Name.ToString() + "\" is not available on this instrument!", ex);
            }
        }

        public void Reset()
        {
            ScpiCommand("*RST");
            ClearErrors();
        }

        public void ClearErrors()
        {
            ScpiCommand("*CLS");
        }

        public string GetError()
        {
            ScpiCommand("SYST:ERR?");
            string s = ReadString();
            return s.TrimEnd('\n');
        }

        public void InitImm()
        {
            ScpiCommand("INIT:IMM");
        }
    }
}
