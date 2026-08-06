using System.IO;

namespace GuardianCommunication.Hardware.XRayDevice
{
    public class CustomFileSystemWatcher : FileSystemWatcher
    {
        private readonly DtoXRayDevice _xRayDevice;

        public DtoXRayDevice XRayDevice => _xRayDevice;

        public CustomFileSystemWatcher(string path, DtoXRayDevice xRayDevice) : base(path)
        {
            _xRayDevice = xRayDevice;
        }

        public override int GetHashCode()
        {
            return _xRayDevice.Id;
        }
    }
}
