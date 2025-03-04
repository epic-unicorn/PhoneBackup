namespace PhoneBackup.Models
{
    public class BackupEntry
    {
        public Phone Phone { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public bool InSync { get; set; }
        public bool IsAvailalble { get; set; }
    }
}
