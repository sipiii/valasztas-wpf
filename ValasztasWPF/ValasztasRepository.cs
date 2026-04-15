using MySqlConnector;
using ValasztasWPF.Models;

namespace ValasztasWPF
{
    public class ValasztasRepository
    {
        private readonly string _connectionString;

        public ValasztasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ValasztasiEredmeny>> GetAllAsync()
        {
            var list = new List<ValasztasiEredmeny>();
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = "SELECT Ev, MegyeNev, PartNev, Rovidites, Szavazatok, Mandatumok, SzavazataranySzazalek FROM v_megye_valasztasi_eredmenyek ORDER BY Ev, MegyeNev, Szavazatok DESC;";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ValasztasiEredmeny
                {
                    Ev = reader.GetInt32("Ev"),
                    MegyeNev = reader.GetString("MegyeNev"),
                    PartNev = reader.GetString("PartNev"),
                    Rovidites = reader.GetString("Rovidites"),
                    Szavazatok = reader.GetInt32("Szavazatok"),
                    Mandatumok = reader.GetInt32("Mandatumok"),
                    SzavazataranySzazalek = reader.IsDBNull(reader.GetOrdinal("SzavazataranySzazalek"))
                        ? 0m
                        : reader.GetDecimal("SzavazataranySzazalek")
                });
            }

            return list;
        }

        public async Task ImportAsync(ImportData data)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                int megyeId = await GetOrCreateMegyeAsync(conn, transaction, data.Header.Megye);

                if (!string.IsNullOrWhiteSpace(data.Header.PartRovidites))
                {
                    await GetOrCreatePartAsync(conn, transaction, data.Header.PartNev, data.Header.PartRovidites);
                }

                foreach (var row in data.Rows)
                {
                    int partId = await GetOrCreatePartAsync(conn, transaction, row.Rovidites, row.Rovidites);
                    await UpsertEredmenyAsync(conn, transaction, data.Header.Ev, megyeId, partId, row.Szavazatok, row.Mandatumok);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static async Task<int> GetOrCreateMegyeAsync(MySqlConnection conn, MySqlTransaction transaction, string nev)
        {
            var selectSql = "SELECT MegyeId FROM megye WHERE Nev = @nev";
            await using var selectCmd = new MySqlCommand(selectSql, conn, transaction);
            selectCmd.Parameters.AddWithValue("@nev", nev);
            var result = await selectCmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
                return Convert.ToInt32(result);

            var insertSql = "INSERT INTO megye (Nev) VALUES (@nev)";
            await using var insertCmd = new MySqlCommand(insertSql, conn, transaction);
            insertCmd.Parameters.AddWithValue("@nev", nev);
            await insertCmd.ExecuteNonQueryAsync();
            return (int)insertCmd.LastInsertedId;
        }

        private static async Task<int> GetOrCreatePartAsync(MySqlConnection conn, MySqlTransaction transaction, string nev, string rovidites)
        {
            var selectSql = "SELECT PartId FROM part WHERE Rovidites = @rov";
            await using var selectCmd = new MySqlCommand(selectSql, conn, transaction);
            selectCmd.Parameters.AddWithValue("@rov", rovidites);
            var result = await selectCmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
                return Convert.ToInt32(result);

            var partNev = string.IsNullOrWhiteSpace(nev) ? rovidites : nev;
            var insertSql = "INSERT INTO part (Nev, Rovidites) VALUES (@nev, @rov)";
            await using var insertCmd = new MySqlCommand(insertSql, conn, transaction);
            insertCmd.Parameters.AddWithValue("@nev", partNev);
            insertCmd.Parameters.AddWithValue("@rov", rovidites);
            await insertCmd.ExecuteNonQueryAsync();
            return (int)insertCmd.LastInsertedId;
        }

        private static async Task UpsertEredmenyAsync(MySqlConnection conn, MySqlTransaction transaction,
            int ev, int megyeId, int partId, int szavazatok, int mandatumok)
        {
            var sql = @"INSERT INTO eredmeny (Ev, MegyeId, PartId, Szavazatok, Mandatumok)
                        VALUES (@ev, @megyeId, @partId, @szav, @mand)
                        ON DUPLICATE KEY UPDATE
                            Szavazatok = VALUES(Szavazatok),
                            Mandatumok = VALUES(Mandatumok)";
            await using var cmd = new MySqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@ev", ev);
            cmd.Parameters.AddWithValue("@megyeId", megyeId);
            cmd.Parameters.AddWithValue("@partId", partId);
            cmd.Parameters.AddWithValue("@szav", szavazatok);
            cmd.Parameters.AddWithValue("@mand", mandatumok);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
