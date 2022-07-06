using System;
using System.Diagnostics;
using static iHeartCS.BLECommunicator;

namespace iHeartCS // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main()
        {
            BLECommunicator communicator = new();

            communicator.NewDeviceConnected += Communicator_NewDeviceConnected;
            communicator.NewDataReceived += Communicator_NewDataReceived;

            communicator.Start();

            Thread.Sleep(10000);

            communicator.Stop();
        }

        private static void Communicator_NewDataReceived(object? sender, NewDataReceivedEventArgs e)
        {
            if (e.Data != null)
                Debug.WriteLine($"{e.Data.IRIndex1:D3} / {e.Data.IR1}");
        }

        private static void Communicator_NewDeviceConnected(object? sender, NewDeviceConncectedEventArgs e)
        {
            Debug.WriteLine($"Oximeter connected: {e.MacAddress}");
        }
    }
}