namespace Project.DbModels
{
    public class Changes
    {
        public int IdChanges { get; set; }
        public int IdCartridgeFk { get; set; }
        public int IdPrinterFk { get; set; }
        public string CommentStatus { get; set; }
    }
}
