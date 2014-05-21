XsaLib
======

.NET Library for SCPI remote control of Agilent's X-Series Signal Analyzers (XSA).

**Requirements**
* [Agilent IO Libraries Suite v16.3](http://www.home.agilent.com/en/pd-1985909/io-libraries-suite-162?&cc=US&lc=eng) - Collection of libraries and utility programs. The IO libraries (SICL, VISA, and VISA COM) enable instrument communication for a variety of development environments (Agilent VEE Pro, Microsoft Visual Studio, etc.) that are compatible with GPIB, USB, LAN, RS-232, PXI, AXIe, and VXI test instruments from a variety of manufacturers. Several utility programs help you quickly and easily connect your instruments to your PC.

**Instruments Supported**
* PXA (N9030A)
* MXA (N9020A)
* EXA (N9010A)
* CXA (N9000A) *untested*

**Current XSA Apps (Modes) Supported**
Supported apps is work-in-progress.
* Spectrum Analyzer (SAN)
* WCDMA

Examples
-----
**Establish connection & query IDN header**
``` C#
MXA Mxa = new MXA();
Mxa.Connect("tcpip0::IpAddress::instr"); // Replace with your instrument's VISA address

// Populate the Mxa.IDN fields by downloading info from instrument
Mxa.DownloadHeader();

Console.WriteLine(Mxa.IDN.Company);
Console.WriteLine(Mxa.IDN.Firmware);
Console.WriteLine(Mxa.IDN.Model);
Console.WriteLine(Mxa.IDN.Serial);
```

**Basic W-CDMA adjacent channel power measurement**
``` C#
MXA Mxa = new MXA();
Mxa.Connect("tcpip0::IpAddress::instr"); // Replace with your instrument's VISA address

Mxa.Reset();
Mxa.SingleSweep = true;

Mxa.GetMode<WCDMA>().ACP.Configure();

Mxa.GetMode<WCDMA>().ACP.AveragingCount = 1;
Mxa.ScpiCommand("RAD:STAN W3GPP");
Mxa.CenterFrequency = 1e9;

Mxa.GetMode<WCDMA>().ACP.Initiate();

Console.WriteLine(Mxa.GetMode<WCDMA>().ACP.Fetch());

```
