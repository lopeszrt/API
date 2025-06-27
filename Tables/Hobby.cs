namespace API.Tables
{
    public class Hobby
    {
        private readonly int _id;
        public string Name { get; set; }

        public readonly int ProfileId;

        public Hobby(int id, string name, int profileId)
        {
            _id = id;
            Name = name;
            ProfileId = profileId;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}