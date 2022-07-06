using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHeartCS.Data
{
    internal static class DataUtils
    {
        public static OximeterRawData? OximeterRawDataFromRingBuffer(RingBuffer<byte> buffer)
        {
            if (buffer == null)
                return null;

            if (buffer.Length < 20)
                return null;

            try
            {
                int pos = 0;
                int length = 0;
                byte[] b4 = new byte[4];
                byte[] b2 = new byte[2];

                DateTime DateTime = DateTime.Now;
                byte SpO2 = 0;
                byte PulseRate = 0;
                UInt32 IR1 = 0;
                byte IRIndex1 = 0;
                UInt32 IR2 = 0;
                byte IRIndex2 = 0;
                byte SpO2Status = 0;
                UInt16 BatteryLevel = 0;
                byte DeviceId = 0;
                byte MacVersionIndex = 0;
                byte MacVersion = 0;

                foreach (byte b in buffer)
                {
                    length++;
                    switch (pos)
                    {
                        case 0:
                            if (b == 0x55)
                                pos = 1;
                            break;
                        case 1:
                            if (b == 0xAA)
                            {
                                pos = 2;
                                break;
                            }
                            pos = 0;
                            break;
                        case 2:
                            PulseRate = b;
                            pos = 3;
                            break;
                        case 3:
                            SpO2 = b;
                            pos = 4;
                            break;
                        case 4:
                            b4[0] = b; ;
                            pos = 5;
                            break;
                        case 5:
                            b4[1] = b;
                            pos = 6;
                            break;
                        case 6:
                            b4[2] = b;
                            pos = 7;
                            break;
                        case 7:
                            b4[3] = b;
                            IR1 = BitConverter.ToUInt32(b4);
                            pos = 8;
                            break;
                        case 8:
                            IRIndex1 = b;
                            pos = 9;
                            break;
                        case 9:
                            b4[0] = b;
                            pos = 10;
                            break;
                        case 10:
                            b4[1] = b;
                            pos = 11;
                            break;
                        case 11:
                            b4[2] = b;
                            pos = 12;
                            break;
                        case 12:
                            b4[3] = b;
                            IR2 = BitConverter.ToUInt32(b4);
                            pos = 13;
                            break;
                        case 13:
                            IRIndex2 = b;
                            pos = 14;
                            break;
                        case 14:
                            SpO2Status = b;
                            pos = 15;
                            break;
                        case 15:
                            b2[0] = b;
                            pos = 16;
                            break;
                        case 16:
                            b2[1] = b;
                            BatteryLevel = (UInt16)(BitConverter.ToUInt16(b2, 0) * 6.6f / 4096.0f * 1000);
                            pos = 17;
                            break;
                        case 17:
                            DeviceId = b;
                            pos = 18;
                            break;
                        case 18:
                            MacVersionIndex = b;
                            pos = 19;
                            break;
                        case 19:
                            MacVersion = b;
                            buffer.RemoveFromBegining(length);
                            OximeterRawData data = new OximeterRawData()
                            {
                                Timestamp = DateTime.Now,
                                SpO2 = SpO2,
                                PulseRate = PulseRate,
                                IR1 = IR1,
                                IRIndex1 = IRIndex1,
                                IR2 = IR2,
                                IRIndex2 = IRIndex2,
                                SpO2Status = SpO2Status,
                                BatteryLevel = BatteryLevel,
                                DeviceId = DeviceId,
                                MacVersionIndex = MacVersionIndex,
                                MacVersion = MacVersion
                            };
                            return data;
                    }
                }
            }

            catch { }

            return null;
        }
    }
}
