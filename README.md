# Választási Adatok WPF Alkalmazás

WPF .NET 8 alkalmazás magyarországi választási eredmények kezelésére MySQL adatbázissal.

## Projekt struktúra

```
valasztas-wpf/
├── SQL/
│   ├── CreateDatabase.sql                  # Adatbázis és táblák létrehozása
│   ├── ValasztasAdatok.sql                 # Kezdeti mintaadatok
│   └── v_megye_valasztasi_eredmenyek.sql   # Nézet definíciója
├── ValasztasWPF/
│   ├── Models/
│   │   ├── ValasztasiEredmeny.cs           # Eredmény modell
│   │   └── ImportData.cs                   # Import modellek
│   ├── App.xaml / App.xaml.cs
│   ├── MainWindow.xaml / MainWindow.xaml.cs
│   ├── ConnectionStringProvider.cs         # Kapcsolati string olvasása
│   ├── ImportParser.cs                     # TXT fájl elemző
│   ├── ValasztasRepository.cs              # Adatbázis műveletek
│   └── ValasztasWPF.csproj
├── import_somogy_2026 1.txt                # Minta importfájl
└── README.md
```

## Adatbázis beállítás

1. **Adatbázis és táblák létrehozása:**
   ```sql
   source SQL/CreateDatabase.sql
   ```

2. **Mintaadatok betöltése:**
   ```sql
   source SQL/ValasztasAdatok.sql
   ```

3. **Nézet létrehozása:**
   ```sql
   source SQL/v_megye_valasztasi_eredmenyek.sql
   ```

## Kapcsolati string beállítása

Az alkalmazás a futtatható állomány melletti `Preferences/MySQLConnection.txt` fájlból olvassa a kapcsolati stringet.

Hozd létre a fájlt az alábbi tartalommal (módosítsd az adataid szerint):

```
Server=localhost;Port=3306;Database=valasztas;Uid=root;Pwd=jelszo;CharSet=utf8mb4;
```

A fájl elérési útja futásidőben:
```
<alkalmazás könyvtár>/Preferences/MySQLConnection.txt
```

## Importfájl formátuma

Az importfájl első sora fejléc, amely tartalmazza az évet, a megyét és az esetleges új párt adatait:

```
Ev=2026;Megye=Somogy;Part=Delta Koalíció;Rovidites=DELTA
```

A többi sor párteredményeket tartalmaz `Rövidítés;Szavazatok;Mandátumok` formában:

```
FIDESZ;48000;3
DK;31000;2
MSZP;15000;1
JOBBIK;22000;1
LMP;8500;0
DELTA;41000;2
```

- Ha a párt rövidítése már szerepel az adatbázisban, az alkalmazás a meglévő rekordot használja.
- Ha nem szerepel, automatikusan létrehozza (a fejléc `Part` és `Rovidites` mezőivel, vagy a rövidítéssel mint névvel).
- Meglévő eredmény esetén az `ON DUPLICATE KEY UPDATE` frissíti az adatokat.

## Alkalmazás futtatása

**Előfeltétel:** .NET 8 SDK, Windows operációs rendszer (WPF)

```bash
cd ValasztasWPF
dotnet restore
dotnet run
```

Vagy Visual Studio 2022-ben nyisd meg a `ValasztasWPF.csproj` fájlt és futtasd (F5).

## Funkciók

- **Adatok megjelenítése:** Az alkalmazás indításkor betölti az összes választási eredményt a `v_megye_valasztasi_eredmenyek` nézetből.
- **Adatok importálása:** A *Fájl → Megnyitás* menüponttal TXT fájlból importálhatók új eredmények.
- **Automatikus frissítés:** Import után az adatrács automatikusan frissül.
- **Szavazatarány:** Az adatbázis nézet automatikusan kiszámolja a százalékos szavazatarányt megyénként.

## Függőségek

- [MySqlConnector](https://mysqlconnector.net/) 2.3.7 – aszinkron MySQL driver
