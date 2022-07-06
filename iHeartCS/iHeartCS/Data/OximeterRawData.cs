using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHeartCS.Data
{
    internal class OximeterRawData
    {
        public DateTime Timestamp { get; init; }
        public byte SpO2 { get; init; }
        public byte PulseRate { get; init; }
        public UInt32 IR1 { get; init; }
        public byte IRIndex1 { get; init; }
        public UInt32 IR2 { get; init; }
        public byte IRIndex2 { get; init; }
        public byte SpO2Status { get; init; }
        public UInt16 BatteryLevel { get; init; }
        public byte DeviceId { get; init; }
        public byte MacVersionIndex { get; init; }
        public byte MacVersion { get; init; }
    }
}
