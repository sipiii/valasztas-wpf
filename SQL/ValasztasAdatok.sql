USE valasztas;

-- Megyék
INSERT IGNORE INTO megye (Nev) VALUES
    ('Baranya'),
    ('Bács-Kiskun'),
    ('Békés'),
    ('Somogy');

-- Pártok
INSERT IGNORE INTO part (Nev, Rovidites) VALUES
    ('Fidesz-KDNP', 'FIDESZ'),
    ('Demokratikus Koalíció', 'DK'),
    ('Magyar Szocialista Párt', 'MSZP'),
    ('Jobbik', 'JOBBIK'),
    ('Lehet Más a Politika', 'LMP');

-- Eredmények 2022 - Baranya megye
INSERT IGNORE INTO eredmeny (Ev, MegyeId, PartId, Szavazatok, Mandatumok)
SELECT 2022, m.MegyeId, p.PartId, v.Szavazatok, v.Mandatumok
FROM (
    SELECT 'FIDESZ' AS Rov, 52341 AS Szavazatok, 4 AS Mandatumok UNION ALL
    SELECT 'DK',     28910, 2 UNION ALL
    SELECT 'MSZP',   14230, 1 UNION ALL
    SELECT 'JOBBIK', 19870, 1 UNION ALL
    SELECT 'LMP',     7650, 0
) v
JOIN megye m ON m.Nev = 'Baranya'
JOIN part  p ON p.Rovidites = v.Rov;
