using essSync.src.Database;

public static class DeviceMapper
{
    public static Device ToEntity(DeviceDTO dto)
    {
        if (dto == null) return null;

        return new Device
        {
            DeviceId = dto.DeviceId,
            DeviceGuid = dto.DeviceGuid,
            DeviceName = dto.DeviceName,
            IsThisDevice = dto.IsThisDevice,
            LastSeenAt = dto.LastSeenAt,
            IsConnected = dto.IsConnected,
            DeviceIps = dto.DeviceIps?.Select(ip => new DeviceIp
            {
                DeviceGuid = dto.DeviceGuid,
                Ip = ip
            }).ToList() ?? new List<DeviceIp>()
        };
    }
}
