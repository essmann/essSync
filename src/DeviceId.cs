using System.Buffers.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static class DeviceId
{
    public static void Initialize()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "essSyncKeys");
        string privateKeyPath = Path.Combine(folderPath, "privateKey.pem");
        string publicKeyPath = Path.Combine(folderPath, "cert.pem");

        Directory.CreateDirectory(folderPath);

        RSA rsa = RSA.Create(2048);

        CertificateRequest req = new CertificateRequest(
            "CN=essSync Device",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10));
        string deviceId = generateDeviceId(cert.RawData);
        try
        {
            if (File.Exists(privateKeyPath) && File.Exists(publicKeyPath))
            {
                string privateKeyPem = File.ReadAllText(privateKeyPath);
                rsa.ImportFromPem(privateKeyPem);
                Console.WriteLine("Loaded existing device ID keys.");

            }
            else
            {
                string privateKeyPem = rsa.ExportPkcs8PrivateKeyPem();
                File.WriteAllText(privateKeyPath, privateKeyPem);
                File.WriteAllText(publicKeyPath, cert.ExportCertificatePem());

                Console.WriteLine("Generated new device ID keys.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error initializing device keys: " + ex.Message);
        }
    }

    private static string generateDeviceId(byte[] derBytes)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(derBytes);
        return Convert.ToBase64String(hashBytes);

    }
    public static string getDeviceId()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "essSyncKeys");
        string publicKeyPath = Path.Combine(folderPath, "cert.pem");

        if (!File.Exists(publicKeyPath))
        {
            throw new FileNotFoundException("Public key file not found. Device ID not initialized.");
        }

        string publicKeyPem = File.ReadAllText(publicKeyPath);
        var cert = X509Certificate2.CreateFromPem(publicKeyPem);
        return generateDeviceId(cert.RawData);
    }

}