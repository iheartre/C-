using iHeartCS.Data;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace iHeartCS
{
    internal class BLECommunicator
    {
        public class NewDataReceivedEventArgs : EventArgs
        {
            public OximeterRawData? Data { get; init; }
        }

        public class NewDeviceConncectedEventArgs : EventArgs
        {
            public string MacAddress { get; set; }
            public NewDeviceConncectedEventArgs(string macAddress)
            {
                MacAddress = macAddress;
            }
        }

        private readonly Thread thread;
        private bool terminationRequested = false;
        private readonly RingBuffer<byte> rxBuffer = new(100);

        public event EventHandler<NewDataReceivedEventArgs>? NewDataReceived;
        public event EventHandler<NewDeviceConncectedEventArgs>? NewDeviceConnected;

        public BLECommunicator()
        {
            thread = new Thread(threadWork);
        }

        public void Start()
        {
            thread.Start();
        }

        public async void Stop()
        {
            terminationRequested = true;
            await Task.Run(() => thread.Join());
        }

        private void threadWork()
        {
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            deviceWatcher.Start();

            while (!terminationRequested)
            {
                Thread.Sleep(100);
            }
        }


        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            string todo = "";
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            string todo = "";
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            string todo = "";
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            string todo = "";
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            string[] split = args.Id.Split('-');

            if (args.Name.Contains("iHeart"))
            {
                NewDeviceConnected?.Invoke(this, new NewDeviceConncectedEventArgs(MacAddresFromId(args.Id)));
                BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(args.Id);
                GattDeviceServicesResult servicesResult = await bluetoothLeDevice.GetGattServicesAsync();

                if (servicesResult.Status == GattCommunicationStatus.Success)
                {
                    var services = servicesResult.Services;

                    GattCharacteristicsResult characteristicsResult = await services[2].GetCharacteristicsAsync();

                    if (characteristicsResult.Status == GattCommunicationStatus.Success)
                    {
                        var characteristic = characteristicsResult.Characteristics[0];

                        GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);
                        if (status == GattCommunicationStatus.Success)
                        {
                            characteristic.ValueChanged += Characteristic_ValueChanged;
                        }
                    }
                }

            }
        }

        private string MacAddresFromId(string id)
        {
            string[] parts = id.Split(new char[] { '-' });
            if (parts.Length == 2)
                return (parts[1].Trim());

            return "−";
        }

        void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            DateTime dt = DateTime.Now;

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] buf = new byte[args.CharacteristicValue.Length];
            reader.ReadBytes(buf);
            rxBuffer.AddRange(buf);

            OximeterRawData? data = DataUtils.OximeterRawDataFromRingBuffer(rxBuffer);

            if (data != null)
                NewDataReceived?.Invoke(this, new NewDataReceivedEventArgs() { Data = data });
        }


    }
}
