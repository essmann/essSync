using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace essSync.src.Database
{
    public class SharedContext : DbContext
    {
        public DbSet<SharedFolder> SharedFolders { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<FolderDevice> FolderDevices { get; set; }

        public string DbPath { get; }

        public SharedContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "essSync.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary keys explicitly
            modelBuilder.Entity<SharedFolder>()
                .HasKey(f => f.SharedFolderId);

            modelBuilder.Entity<SharedFile>()
                .HasKey(f => f.SharedFileId);

            modelBuilder.Entity<Device>()
                .HasKey(d => d.DeviceId);

            // Composite key for FolderDevice (many-to-many)
            modelBuilder.Entity<FolderDevice>()
                .HasKey(fd => new { fd.FolderId, fd.DeviceId });

            // Configure relationships explicitly
            modelBuilder.Entity<FolderDevice>()
                .HasOne(fd => fd.SharedFolder)
                .WithMany(sf => sf.FolderDevices)
                .HasForeignKey(fd => fd.FolderId);

            modelBuilder.Entity<FolderDevice>()
                .HasOne(fd => fd.Device)
                .WithMany(d => d.FolderDevices)
                .HasForeignKey(fd => fd.DeviceId);

            modelBuilder.Entity<SharedFile>()
                .HasOne(sf => sf.SharedFolder)
                .WithMany(folder => folder.Files)
                .HasForeignKey(sf => sf.FolderId);

            // Unique constraints
            modelBuilder.Entity<SharedFolder>()
                .HasIndex(f => f.FolderGuid)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.DeviceGuid)
                .IsUnique();
        }
    }

    // Represents a shared folder
    public class SharedFolder
    {
        public int SharedFolderId { get; set; }
        public string FolderName { get; set; }      // Display name
        public string LocalPath { get; set; }        // Local and absolute path on this device
        public string FolderGuid { get; set; }       // Global unique identifier
        public bool IsPaused { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSyncedAt { get; set; }

        public List<SharedFile> Files { get; set; } = new();
        public List<FolderDevice> FolderDevices { get; set; } = new();
    }

    // Represents a file/directory in a shared folder
    public class SharedFile
    {
        public int SharedFileId { get; set; }
        public int FolderId { get; set; }
        public string RelativePath { get; set; }     // Path relative to folder root
        public bool IsDirectory { get; set; }
        public long FileSize { get; set; }
        public string FileHash { get; set; }         // SHA256 hash of content
        public DateTime ModifiedAt { get; set; }     // File modification time
        public DateTime LastCheckedAt { get; set; }  // When we last verified this file
        public bool IsDeleted { get; set; }          // Soft delete flag

        public SharedFolder SharedFolder { get; set; }
    }

    // Represents a device (computer/phone) in the sync network
    public class Device
    {
        public int DeviceId { get; set; }
        public string DeviceGuid { get; set; }       // Unique device identifier
        public string DeviceName { get; set; }       // User-friendly name
        public bool IsThisDevice { get; set; }       // True for local device
        public DateTime LastSeenAt { get; set; }
        public bool IsConnected { get; set; }

        public List<FolderDevice> FolderDevices { get; set; } = new();
    }

    // Junction table: which devices have access to which folders
    public class FolderDevice
    {
        public int FolderId { get; set; }
        public int DeviceId { get; set; }
        public bool CanWrite { get; set; }           // Read-only vs read-write
        public DateTime SharedAt { get; set; }

        public SharedFolder SharedFolder { get; set; }
        public Device Device { get; set; }
    }
}