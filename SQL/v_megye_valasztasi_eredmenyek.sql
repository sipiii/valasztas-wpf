USE valasztas;

CREATE OR REPLACE VIEW v_megye_valasztasi_eredmenyek AS
SELECT
    e.Ev AS Ev,
    m.Nev AS MegyeNev,
    p.Nev AS PartNev,
    p.Rovidites AS Rovidites,
    e.Szavazatok AS Szavazatok,
    e.Mandatumok AS Mandatumok,
    ROUND(
        100.0 * e.Szavazatok / NULLIF(t.OsszSzavazat, 0),
        2
    ) AS SzavazataranySzazalek
FROM eredmeny e
JOIN megye m ON m.MegyeId = e.MegyeId
JOIN part p  ON p.PartId = e.PartId
JOIN (
    SELECT Ev, MegyeId, SUM(Szavazatok) AS OsszSzavazat
    FROM eredmeny
    GROUP BY Ev, MegyeId
) t ON t.Ev = e.Ev AND t.MegyeId = e.MegyeId;
