using System.IO;
using ValasztasWPF.Models;

namespace ValasztasWPF
{
    public static class ImportParser
    {
        public static ImportData Parse(string filePath)
        {
            var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToArray();

            if (lines.Length == 0)
                throw new InvalidDataException("A fájl üres.");

            var result = new ImportData();
            result.Header = ParseHeader(lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                var row = ParseRow(lines[i]);
                result.Rows.Add(row);
            }

            return result;
        }

        private static ImportHeader ParseHeader(string line)
        {
            var parts = line.Split(';');
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in parts)
            {
                var idx = part.IndexOf('=');
                if (idx > 0)
                {
                    var key = part[..idx].Trim();
                    var value = part[(idx + 1)..].Trim();
                    dict[key] = value;
                }
            }

            if (!dict.TryGetValue("Ev", out var evStr) || !int.TryParse(evStr, out int ev))
                throw new InvalidDataException("Hiányzó vagy érvénytelen 'Ev' a fejlécből.");

            if (!dict.TryGetValue("Megye", out var megye) || string.IsNullOrWhiteSpace(megye))
                throw new InvalidDataException("Hiányzó 'Megye' a fejlécből.");

            dict.TryGetValue("Part", out var partNev);
            dict.TryGetValue("Rovidites", out var rovidites);

            return new ImportHeader
            {
                Ev = ev,
                Megye = megye,
                PartNev = partNev ?? string.Empty,
                PartRovidites = rovidites ?? string.Empty
            };
        }

        private static ImportRow ParseRow(string line)
        {
            var parts = line.Split(';');
            if (parts.Length < 3)
                throw new InvalidDataException($"Érvénytelen sor formátum: '{line}'");

            if (!int.TryParse(parts[1].Trim(), out int szavazatok))
                throw new InvalidDataException($"Érvénytelen szavazatszám: '{parts[1]}'");

            if (!int.TryParse(parts[2].Trim(), out int mandatumok))
                throw new InvalidDataException($"Érvénytelen mandátumszám: '{parts[2]}'");

            return new ImportRow
            {
                Rovidites = parts[0].Trim(),
                Szavazatok = szavazatok,
                Mandatumok = mandatumok
            };
        }
    }
}
