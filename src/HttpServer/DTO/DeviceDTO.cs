public class DeviceDTO
{
    public int DeviceId { get; set; }
    public string DeviceGuid { get; set; }       // Unique device identifier
    public string DeviceName { get; set; }       // User-friendly name
    public bool IsThisDevice { get; set; }       // True for local device
    public DateTime LastSeenAt { get; set; }
    public bool IsConnected { get; set; }

    // Changed from field to property
    public List<string> DeviceIps { get; set; }

    public DeviceDTO()
    {
        // Initialize the IP list
        this.DeviceIps = new List<string>();

        // Optional: default values
        this.LastSeenAt = DateTime.UtcNow;
        this.IsConnected = false;
        this.IsThisDevice = false;
    }
}
