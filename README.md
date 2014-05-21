XsaLib
======

.NET Library for SCPI remote control of Agilent's X-Series Signal Analyzers (XSA).

#### Instruments Supported
* PXA (N9030A)
* MXA (N9020A)
* EXA (N9010A)
* CXA (N9000A) *untested*

#### Current XSA Apps (Modes) Supported
Supported apps is work-in-progress.
* Spectrum Analyzer (SAN)
* WCDMA

Examples
-----
### Establish connection & query IDN header
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

### Basic W-CDMA adjacent channel power measurement
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
