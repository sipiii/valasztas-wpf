namespace ValasztasWPF.Models
{
    public class ImportHeader
    {
        public int Ev { get; set; }
        public string Megye { get; set; } = string.Empty;
        public string PartNev { get; set; } = string.Empty;
        public string PartRovidites { get; set; } = string.Empty;
    }

    public class ImportRow
    {
        public string Rovidites { get; set; } = string.Empty;
        public int Szavazatok { get; set; }
        public int Mandatumok { get; set; }
    }

    public class ImportData
    {
        public ImportHeader Header { get; set; } = new();
        public List<ImportRow> Rows { get; set; } = new();
    }
}
