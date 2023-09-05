namespace API.DTOs.OtrosSistemas
{
    public class GeneralDTO
    {
        public Guid Userid { get; set; }
        public string userName { get; set; }
        public string firsName { get; set; }
        public string secondName { get; set; }
        public string lastName { get; set; }
        public string secondLastName { get; set; }
        public int origin { get; set; }
        public bool active { get; set; }
    }
}
