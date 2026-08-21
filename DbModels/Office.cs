namespace Project.DbModels
{
    public class Office
    {
        public int Id { get; set; }
        public int? Parent { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }
}
