namespace ValasztasWPF.Models
{
    public class ValasztasiEredmeny
    {
        public int Ev { get; set; }
        public string MegyeNev { get; set; } = string.Empty;
        public string PartNev { get; set; } = string.Empty;
        public string Rovidites { get; set; } = string.Empty;
        public int Szavazatok { get; set; }
        public int Mandatumok { get; set; }
        public decimal SzavazataranySzazalek { get; set; }
    }
}
