using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSettings
    {
        public DPSSymmetricKeySettings SymetricKeySettings { get; set; }
        public DPSX509Settings X509Settings { get; set; }
        public DPSTPMSettings TMPSettings { get; set; }
    }
}
