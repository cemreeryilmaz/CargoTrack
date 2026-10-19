/* ============================================================
   CargoTrack - 250'lik Ornek Veri Seti (MSSQL)
   - Tum ana tablolar 250 satira TAMAMLANIR (mevcut veriler korunur).
   - GUID PK'ler, soft-delete (IsDeleted=0) ve audit alanlari doldurulur.
   - Referans butunlugu: cocuk tablolar, ebeveyn tablolardan deterministik
     (ROW_NUMBER + modulo) secimle iliskilendirilir -> FK hatasi olmaz.
   - Yeni kullanicilar admin ile ayni parola hash'ini kullanir (parola: Admin123!).
   - Tekrar calistirilabilir: her tablo 250'ye ulasana kadar ekler (idempotent).
   ============================================================ */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @target  INT = 250;
DECLARE @now     datetime2 = SYSUTCDATETIME();

/* ---- Parola hash'i (admin'den al; yoksa durdur) ---- */
DECLARE @pwd nvarchar(max) =
    (SELECT TOP 1 PasswordHash FROM AspNetUsers
     WHERE PasswordHash IS NOT NULL ORDER BY (CASE WHEN UserName = 'admin' THEN 0 ELSE 1 END), UserName);
IF @pwd IS NULL
BEGIN
    RAISERROR('PasswordHash bulunamadi. Once uygulamayi calistirip admin kullanicisinin olusmasini saglayin.', 16, 1);
    ROLLBACK TRANSACTION; RETURN;
END

DECLARE @userRole uniqueidentifier = (SELECT TOP 1 Id FROM AspNetRoles WHERE NormalizedName = N'USER');

/* ============================================================
   0) Yardimci liste tablolari (gercekci Turkce veriler)
   ============================================================ */
SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #fn
FROM (VALUES
 (N'Ahmet'),(N'Mehmet'),(N'Mustafa'),(N'Ali'),(N'Huseyin'),(N'Hasan'),(N'Ibrahim'),(N'Osman'),(N'Yusuf'),(N'Murat'),
 (N'Omer'),(N'Ramazan'),(N'Kemal'),(N'Kadir'),(N'Emre'),(N'Burak'),(N'Cem'),(N'Can'),(N'Deniz'),(N'Oguz'),
 (N'Serkan'),(N'Tolga'),(N'Volkan'),(N'Selim'),(N'Baris'),(N'Eren'),(N'Kaan'),(N'Onur'),(N'Ugur'),(N'Yavuz'),
 (N'Ayse'),(N'Fatma'),(N'Emine'),(N'Hatice'),(N'Zeynep'),(N'Elif'),(N'Meryem'),(N'Serife'),(N'Zehra'),(N'Sultan'),
 (N'Hanife'),(N'Merve'),(N'Busra'),(N'Esra'),(N'Derya'),(N'Gamze'),(N'Seda'),(N'Pinar'),(N'Ebru'),(N'Melike')
) v(n);
DECLARE @fnc INT = (SELECT COUNT(*) FROM #fn);

SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #ln
FROM (VALUES
 (N'Yilmaz'),(N'Kaya'),(N'Demir'),(N'Sahin'),(N'Celik'),(N'Yildiz'),(N'Yildirim'),(N'Ozturk'),(N'Aydin'),(N'Ozdemir'),
 (N'Arslan'),(N'Dogan'),(N'Kilic'),(N'Aslan'),(N'Cetin'),(N'Kara'),(N'Koc'),(N'Kurt'),(N'Ozkan'),(N'Simsek'),
 (N'Polat'),(N'Korkmaz'),(N'Cakir'),(N'Erdogan'),(N'Yucel'),(N'Gunes'),(N'Aksoy'),(N'Bulut'),(N'Keskin'),(N'Avci'),
 (N'Tas'),(N'Ozer'),(N'Bozkurt'),(N'Turan'),(N'Acar'),(N'Guler'),(N'Kaplan'),(N'Cevik'),(N'Tekin'),(N'Duman'),
 (N'Unal'),(N'Sari'),(N'Yalcin'),(N'Bal'),(N'Cakmak'),(N'Aktas'),(N'Gungor'),(N'Topal'),(N'Ucar'),(N'Ozturkmen')
) v(n);
DECLARE @lnc INT = (SELECT COUNT(*) FROM #ln);

SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #prov
FROM (VALUES
 (N'Adana'),(N'Adiyaman'),(N'Afyonkarahisar'),(N'Agri'),(N'Amasya'),(N'Ankara'),(N'Antalya'),(N'Artvin'),(N'Aydin'),(N'Balikesir'),
 (N'Bilecik'),(N'Bingol'),(N'Bitlis'),(N'Bolu'),(N'Burdur'),(N'Bursa'),(N'Canakkale'),(N'Cankiri'),(N'Corum'),(N'Denizli'),
 (N'Diyarbakir'),(N'Edirne'),(N'Elazig'),(N'Erzincan'),(N'Erzurum'),(N'Eskisehir'),(N'Gaziantep'),(N'Giresun'),(N'Gumushane'),(N'Hakkari'),
 (N'Hatay'),(N'Isparta'),(N'Mersin'),(N'Istanbul'),(N'Izmir'),(N'Kars'),(N'Kastamonu'),(N'Kayseri'),(N'Kirklareli'),(N'Kirsehir'),
 (N'Kocaeli'),(N'Konya'),(N'Kutahya'),(N'Malatya'),(N'Manisa'),(N'Kahramanmaras'),(N'Mardin'),(N'Mugla'),(N'Mus'),(N'Nevsehir'),
 (N'Nigde'),(N'Ordu'),(N'Rize'),(N'Sakarya'),(N'Samsun'),(N'Siirt'),(N'Sinop'),(N'Sivas'),(N'Tekirdag'),(N'Tokat'),
 (N'Trabzon'),(N'Tunceli'),(N'Sanliurfa'),(N'Usak'),(N'Van'),(N'Yozgat'),(N'Zonguldak'),(N'Aksaray'),(N'Bayburt'),(N'Karaman'),
 (N'Kirikkale'),(N'Batman'),(N'Sirnak'),(N'Bartin'),(N'Ardahan'),(N'Igdir'),(N'Yalova'),(N'Karabuk'),(N'Kilis'),(N'Osmaniye'),(N'Duzce')
) v(n);
DECLARE @provc INT = (SELECT COUNT(*) FROM #prov);

SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #dist
FROM (VALUES
 (N'Merkez'),(N'Cankaya'),(N'Kadikoy'),(N'Besiktas'),(N'Uskudar'),(N'Bornova'),(N'Konak'),(N'Nilufer'),(N'Osmangazi'),(N'Selcuklu'),
 (N'Seyhan'),(N'Yildirim'),(N'Kecioren'),(N'Mamak'),(N'Esenler'),(N'Pendik'),(N'Maltepe'),(N'Kartal'),(N'Bagcilar'),(N'Umraniye'),
 (N'Karsiyaka'),(N'Cigli'),(N'Muratpasa'),(N'Kepez'),(N'Odunpazari'),(N'Tepebasi'),(N'Ilkadim'),(N'Melikgazi'),(N'Sahinbey'),(N'Battalgazi')
) v(n);
DECLARE @distc INT = (SELECT COUNT(*) FROM #dist);

SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #title
FROM (VALUES
 (N'Kurye'),(N'Sube Gorevlisi'),(N'Dagitim Personeli'),(N'Operasyon Uzmani'),
 (N'Sube Muduru'),(N'Transfer Operatoru'),(N'Musteri Temsilcisi'),(N'Depo Sorumlusu')
) v(n);
DECLARE @titlec INT = (SELECT COUNT(*) FROM #title);

SELECT n, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #street
FROM (VALUES
 (N'Ataturk Cad.'),(N'Istiklal Cad.'),(N'Cumhuriyet Mah.'),(N'Gazi Bulvari'),(N'Fevzi Cakmak Sok.'),(N'Barbaros Mah.'),
 (N'Mevlana Cad.'),(N'Inonu Cad.'),(N'Hurriyet Mah.'),(N'Yavuz Selim Sok.'),(N'Fatih Cad.'),(N'Mimar Sinan Mah.'),
 (N'Kazim Karabekir Sok.'),(N'Gul Sok.'),(N'Lale Cad.'),(N'Papatya Sok.'),(N'Cicek Mah.'),(N'Zafer Bulvari'),(N'Menekse Sok.'),(N'Bahcelievler Mah.')
) v(n);
DECLARE @streetc INT = (SELECT COUNT(*) FROM #street);

/* Tutarli Il - Ilce ciftleri (adresler icin) */
SELECT il, ilce, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #pair
FROM (VALUES
 (N'Istanbul',N'Avcilar'),(N'Istanbul',N'Kadikoy'),(N'Istanbul',N'Besiktas'),(N'Istanbul',N'Uskudar'),(N'Istanbul',N'Bakirkoy'),(N'Istanbul',N'Pendik'),(N'Istanbul',N'Maltepe'),(N'Istanbul',N'Kartal'),(N'Istanbul',N'Beylikduzu'),(N'Istanbul',N'Sisli'),(N'Istanbul',N'Fatih'),(N'Istanbul',N'Umraniye'),(N'Istanbul',N'Bagcilar'),(N'Istanbul',N'Esenyurt'),
 (N'Ankara',N'Cankaya'),(N'Ankara',N'Kecioren'),(N'Ankara',N'Mamak'),(N'Ankara',N'Yenimahalle'),(N'Ankara',N'Etimesgut'),(N'Ankara',N'Sincan'),(N'Ankara',N'Altindag'),(N'Ankara',N'Pursaklar'),
 (N'Izmir',N'Karsiyaka'),(N'Izmir',N'Bornova'),(N'Izmir',N'Konak'),(N'Izmir',N'Buca'),(N'Izmir',N'Cigli'),(N'Izmir',N'Bayrakli'),(N'Izmir',N'Karabaglar'),(N'Izmir',N'Gaziemir'),(N'Izmir',N'Balcova'),
 (N'Bursa',N'Osmangazi'),(N'Bursa',N'Nilufer'),(N'Bursa',N'Yildirim'),(N'Bursa',N'Gemlik'),(N'Bursa',N'Inegol'),(N'Bursa',N'Mudanya'),
 (N'Antalya',N'Muratpasa'),(N'Antalya',N'Kepez'),(N'Antalya',N'Konyaalti'),(N'Antalya',N'Alanya'),(N'Antalya',N'Manavgat'),(N'Antalya',N'Serik'),
 (N'Adana',N'Seyhan'),(N'Adana',N'Cukurova'),(N'Adana',N'Yuregir'),(N'Adana',N'Saricam'),
 (N'Konya',N'Selcuklu'),(N'Konya',N'Meram'),(N'Konya',N'Karatay'),(N'Konya',N'Eregli'),
 (N'Gaziantep',N'Sahinbey'),(N'Gaziantep',N'Sehitkamil'),(N'Gaziantep',N'Nizip'),
 (N'Kocaeli',N'Izmit'),(N'Kocaeli',N'Gebze'),(N'Kocaeli',N'Darica'),(N'Kocaeli',N'Korfez'),
 (N'Mersin',N'Yenisehir'),(N'Mersin',N'Toroslar'),(N'Mersin',N'Akdeniz'),(N'Mersin',N'Mezitli'),(N'Mersin',N'Tarsus'),
 (N'Diyarbakir',N'Baglar'),(N'Diyarbakir',N'Kayapinar'),(N'Diyarbakir',N'Yenisehir'),(N'Diyarbakir',N'Sur'),
 (N'Kayseri',N'Melikgazi'),(N'Kayseri',N'Kocasinan'),(N'Kayseri',N'Talas'),
 (N'Samsun',N'Ilkadim'),(N'Samsun',N'Atakum'),(N'Samsun',N'Canik'),
 (N'Eskisehir',N'Odunpazari'),(N'Eskisehir',N'Tepebasi'),
 (N'Denizli',N'Pamukkale'),(N'Denizli',N'Merkezefendi'),
 (N'Sanliurfa',N'Haliliye'),(N'Sanliurfa',N'Eyyubiye'),(N'Sanliurfa',N'Karakopru'),
 (N'Malatya',N'Battalgazi'),(N'Malatya',N'Yesilyurt'),
 (N'Trabzon',N'Ortahisar'),(N'Trabzon',N'Akcaabat'),
 (N'Erzurum',N'Yakutiye'),(N'Erzurum',N'Palandoken'),(N'Erzurum',N'Aziziye'),
 (N'Van',N'Ipekyolu'),(N'Van',N'Tusba'),(N'Van',N'Edremit'),
 (N'Sakarya',N'Adapazari'),(N'Sakarya',N'Serdivan'),(N'Sakarya',N'Erenler'),
 (N'Balikesir',N'Altieylul'),(N'Balikesir',N'Karesi'),(N'Balikesir',N'Edremit'),
 (N'Manisa',N'Yunusemre'),(N'Manisa',N'Sehzadeler'),(N'Manisa',N'Akhisar'),
 (N'Hatay',N'Antakya'),(N'Hatay',N'Iskenderun'),(N'Hatay',N'Defne'),
 (N'Aydin',N'Efeler'),(N'Aydin',N'Nazilli'),(N'Aydin',N'Kusadasi'),
 (N'Mugla',N'Mentese'),(N'Mugla',N'Bodrum'),(N'Mugla',N'Fethiye'),(N'Mugla',N'Marmaris')
) v(il, ilce);
DECLARE @pairc INT = (SELECT COUNT(*) FROM #pair);

/* Sayac tablosu (1..250) */
SELECT TOP (@target) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) rn
INTO #nums
FROM sys.all_objects;

/* ============================================================
   1) Cities -> 250
   ============================================================ */
DECLARE @needCities INT = @target - (SELECT COUNT(*) FROM Cities WHERE IsDeleted = 0);
INSERT INTO Cities (Id, Name, CreatedDate, IsDeleted)
SELECT NEWID(),
       p.n + CASE WHEN n.rn > @provc THEN N' ' + d.n ELSE N'' END,
       DATEADD(DAY, -(n.rn % 120), @now), 0
FROM #nums n
JOIN #prov p ON p.rn = ((n.rn - 1) % @provc) + 1
JOIN #dist d ON d.rn = ((n.rn - 1) % @distc) + 1
WHERE n.rn <= @needCities;

SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #City2 FROM Cities WHERE IsDeleted = 0;
DECLARE @cityc INT = (SELECT COUNT(*) FROM #City2);

/* ============================================================
   2) AspNetUsers -> 250 (yeni musteriler + User rolu)
   ============================================================ */
DECLARE @needUsers INT = @target - (SELECT COUNT(*) FROM AspNetUsers);
INSERT INTO AspNetUsers
 (Id, FirstName, LastName, UserName, NormalizedUserName, Email, NormalizedEmail,
  EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber,
  PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, BranchId)
SELECT NEWID(), f.n, l.n,
       N'musteri' + RIGHT('000' + CAST(n.rn AS varchar(10)), 3),
       N'MUSTERI' + RIGHT('000' + CAST(n.rn AS varchar(10)), 3),
       N'musteri' + RIGHT('000' + CAST(n.rn AS varchar(10)), 3) + N'@cargotrack.com',
       N'MUSTERI' + RIGHT('000' + CAST(n.rn AS varchar(10)), 3) + N'@CARGOTRACK.COM',
       1, @pwd, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()),
       N'05' + RIGHT('000000000' + CAST(500000000 + n.rn AS varchar(10)), 9),
       0, 0, 1, 0, NULL
FROM #nums n
JOIN #fn f ON f.rn = ((n.rn - 1) % @fnc) + 1
JOIN #ln l ON l.rn = ((n.rn * 13 - 1) % @lnc) + 1
WHERE n.rn <= @needUsers;

IF @userRole IS NOT NULL
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT u.Id, @userRole
    FROM AspNetUsers u
    WHERE u.UserName LIKE 'musteri%'
      AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles ur WHERE ur.UserId = u.Id AND ur.RoleId = @userRole);

SELECT Id, UserName, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #User2 FROM AspNetUsers;
DECLARE @userc INT = (SELECT COUNT(*) FROM #User2);

/* ============================================================
   3) Branches -> 250 (City'e bagli)
   ============================================================ */
DECLARE @needBranches INT = @target - (SELECT COUNT(*) FROM Branches WHERE IsDeleted = 0);
INSERT INTO Branches (Id, Name, CityId, CreatedDate, IsDeleted)
SELECT NEWID(), d.n + N' Subesi', c.Id, DATEADD(DAY, -(n.rn % 100), @now), 0
FROM #nums n
JOIN #dist d ON d.rn = ((n.rn - 1) % @distc) + 1
JOIN #City2 c ON c.rn = ((n.rn - 1) % @cityc) + 1
WHERE n.rn <= @needBranches;

SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #Branch2 FROM Branches WHERE IsDeleted = 0;
DECLARE @branchc INT = (SELECT COUNT(*) FROM #Branch2);

/* ============================================================
   4) TransferCenters -> 250 (City'e bagli)
   ============================================================ */
DECLARE @needTc INT = @target - (SELECT COUNT(*) FROM TransferCenters WHERE IsDeleted = 0);
INSERT INTO TransferCenters (Id, Name, CityId, CreatedDate, IsDeleted)
SELECT NEWID(), p.n + N' Transfer Merkezi', c.Id, DATEADD(DAY, -(n.rn % 100), @now), 0
FROM #nums n
JOIN #prov p ON p.rn = ((n.rn - 1) % @provc) + 1
JOIN #City2 c ON c.rn = ((n.rn - 1) % @cityc) + 1
WHERE n.rn <= @needTc;

SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #Tc2 FROM TransferCenters WHERE IsDeleted = 0;
DECLARE @tcc INT = (SELECT COUNT(*) FROM #Tc2);

/* ============================================================
   5) Employees -> 250 (Branch'e bagli)
   ============================================================ */
DECLARE @needEmp INT = @target - (SELECT COUNT(*) FROM Employees WHERE IsDeleted = 0);
INSERT INTO Employees (Id, FirstName, LastName, Title, IsActive, BranchId, AppUserId, CreatedDate, IsDeleted)
SELECT NEWID(), f.n, l.n, t.n, 1, b.Id, NULL, DATEADD(DAY, -(n.rn % 90), @now), 0
FROM #nums n
JOIN #fn f ON f.rn = ((n.rn * 3 - 1) % @fnc) + 1
JOIN #ln l ON l.rn = ((n.rn * 17 - 1) % @lnc) + 1
JOIN #title t ON t.rn = ((n.rn - 1) % @titlec) + 1
JOIN #Branch2 b ON b.rn = ((n.rn - 1) % @branchc) + 1
WHERE n.rn <= @needEmp;

SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #Emp2 FROM Employees WHERE IsDeleted = 0;
DECLARE @empc INT = (SELECT COUNT(*) FROM #Emp2);

/* ============================================================
   6) CargoPrices -> 250
   ============================================================ */
DECLARE @needPrice INT = @target - (SELECT COUNT(*) FROM CargoPrices WHERE IsDeleted = 0);
INSERT INTO CargoPrices (Id, Name, DeliveryType, BasePrice, PricePerKg, PricePerDesi, DifferentCityFee, IsActive, CreatedDate, IsDeleted)
SELECT NEWID(),
       CASE ((n.rn - 1) % 3) WHEN 0 THEN N'Standart Tarife ' WHEN 1 THEN N'Ekspres Tarife ' ELSE N'Ayni Gun Tarife ' END
         + CAST(n.rn AS varchar(10)),
       ((n.rn - 1) % 3) + 1,
       CAST(40 + (n.rn % 60) AS decimal(18,2)),
       CAST(4 + (n.rn % 10) AS decimal(18,2)),
       CAST(6 + (n.rn % 12) AS decimal(18,2)),
       CAST(15 + (n.rn % 35) AS decimal(18,2)),
       CASE WHEN n.rn % 7 = 0 THEN 0 ELSE 1 END,
       DATEADD(DAY, -(n.rn % 100), @now), 0
FROM #nums n
WHERE n.rn <= @needPrice;

/* ============================================================
   7) Abouts -> 250
   ============================================================ */
DECLARE @needAbout INT = @target - (SELECT COUNT(*) FROM Abouts WHERE IsDeleted = 0);
INSERT INTO Abouts (Id, Title, Description, ImageUrl, CreatedDate, IsDeleted)
SELECT NEWID(),
       N'Hakkimizda Baslik ' + CAST(n.rn AS varchar(10)),
       N'CargoTrack olarak musteri memnuniyetini on planda tutan, guvenilir ve hizli kargo cozumleri sunuyoruz. (Kayit ' + CAST(n.rn AS varchar(10)) + N')',
       N'https://ui-avatars.com/api/?name=CargoTrack+' + CAST(n.rn AS varchar(10)) + N'&background=00040c&color=fff&size=256',
       DATEADD(DAY, -(n.rn % 150), @now), 0
FROM #nums n
WHERE n.rn <= @needAbout;

/* ============================================================
   8) ContactInfos -> 250
   ============================================================ */
DECLARE @needContact INT = @target - (SELECT COUNT(*) FROM ContactInfos WHERE IsDeleted = 0);
INSERT INTO ContactInfos (Id, Address, Email, PhoneNumber, CreatedDate, IsDeleted)
SELECT NEWID(),
       s.n + N' No:' + CAST((n.rn % 200) + 1 AS varchar(10)) + N', ' + p.n,
       N'info' + CAST(n.rn AS varchar(10)) + N'@cargotrack.com',
       N'0850' + RIGHT('0000000' + CAST(2000000 + n.rn AS varchar(10)), 7),
       DATEADD(DAY, -(n.rn % 150), @now), 0
FROM #nums n
JOIN #street s ON s.rn = ((n.rn - 1) % @streetc) + 1
JOIN #prov p ON p.rn = ((n.rn - 1) % @provc) + 1
WHERE n.rn <= @needContact;

/* ============================================================
   9) Addresses -> 250 (User'a bagli, 1:1)
   ============================================================ */
DECLARE @needAddr INT = @target - (SELECT COUNT(*) FROM Addresses WHERE IsDeleted = 0);
INSERT INTO Addresses (Id, Title, City, District, FullAddress, UserId, CreatedDate, IsDeleted)
SELECT NEWID(),
       CASE ((n.rn - 1) % 4) WHEN 0 THEN N'Ev' WHEN 1 THEN N'Is' WHEN 2 THEN N'Ofis' ELSE N'Depo' END,
       pr.il, pr.ilce,
       s.n + N' No:' + CAST((n.rn % 150) + 1 AS varchar(10)) + N' Daire:' + CAST((n.rn % 20) + 1 AS varchar(10)),
       u.Id, DATEADD(DAY, -(n.rn % 120), @now), 0
FROM #nums n
JOIN #User2 u ON u.rn = ((n.rn - 1) % @userc) + 1
JOIN #pair pr ON pr.rn = ((n.rn - 1) % @pairc) + 1
JOIN #street s ON s.rn = ((n.rn - 1) % @streetc) + 1
WHERE n.rn <= @needAddr;

/* ============================================================
   10) Cargos -> 250 (User + Branch'e bagli)
   CargoStatus: 1..9, CargoType: 1..4, DeliveryType: 1..3
   ============================================================ */
DECLARE @needCargo INT = @target - (SELECT COUNT(*) FROM Cargos WHERE IsDeleted = 0);
;WITH g AS (
    SELECT n.rn,
           ((n.rn - 1) % 9) + 1 AS st,
           CAST(0.5 + (n.rn % 50) AS float) AS wgt,
           CAST(10 + (n.rn % 40) AS float) AS wdt,
           CAST(10 + (n.rn * 3 % 50) AS float) AS hgt,
           CAST(10 + (n.rn * 7 % 60) AS float) AS lng,
           su.Id AS SenderId, re.Id AS ReceiverId,
           ob.Id AS OriginBranchId, db.Id AS DestBranchId
    FROM #nums n
    JOIN #User2 su ON su.rn = ((n.rn - 1) % @userc) + 1
    JOIN #User2 re ON re.rn = ((n.rn * 7 + 3 - 1) % @userc) + 1
    JOIN #Branch2 ob ON ob.rn = ((n.rn - 1) % @branchc) + 1
    JOIN #Branch2 db ON db.rn = ((n.rn * 11 + 5 - 1) % @branchc) + 1
    WHERE n.rn <= @needCargo
)
INSERT INTO Cargos
 (Id, TrackCode, ShipmentDate, EstimatedArrivalDate, Weight, CargoType, CargoStatus,
  DestinationBranchId, OriginBranchId, ReceiverId, SenderId, CreatedDate,
  CurrentBranchId, CurrentTransferCenterId, DeliveredDate, DeliveredToName, DeliveryCode,
  DeliveryType, Desi, FailedAttemptCount, Height, IsDeleted, Length, Price, Width)
SELECT NEWID(),
       N'MYC-2026-' + RIGHT('000000' + CAST(100000 + g.rn AS varchar(10)), 6),
       DATEADD(DAY, -(g.rn % 60), @now),
       DATEADD(DAY, (g.rn % 5) + 1, DATEADD(DAY, -(g.rn % 60), @now)),
       g.wgt, ((g.rn - 1) % 4) + 1, g.st,
       g.DestBranchId, g.OriginBranchId, g.ReceiverId, g.SenderId, DATEADD(DAY, -(g.rn % 60), @now),
       CASE WHEN g.st IN (4,5,6) THEN g.DestBranchId WHEN g.st = 2 THEN g.OriginBranchId ELSE NULL END,
       NULL,
       CASE WHEN g.st = 6 THEN DATEADD(DAY, -(g.rn % 10), @now) ELSE NULL END,
       CASE WHEN g.st = 6 THEN N'Teslim Alan Kisi ' + CAST(g.rn AS varchar(10)) ELSE NULL END,
       CASE WHEN g.st >= 5 THEN RIGHT('000000' + CAST((g.rn * 37) % 1000000 AS varchar(10)), 6) ELSE NULL END,
       ((g.rn - 1) % 3) + 1,
       CAST(g.wdt * g.hgt * g.lng / 3000.0 AS float),
       CASE WHEN g.st IN (7,8,9) THEN ((g.rn - 1) % 3) + 1 ELSE 0 END,
       g.hgt, 0, g.lng,
       CAST(50 + g.wgt * 5 + (g.wdt * g.hgt * g.lng / 3000.0) * 8 AS decimal(18,2)),
       g.wdt
FROM g;

SELECT Id, CargoStatus, ROW_NUMBER() OVER (ORDER BY Id) rn INTO #Cargo2 FROM Cargos WHERE IsDeleted = 0;
DECLARE @cargoc INT = (SELECT COUNT(*) FROM #Cargo2);

/* ============================================================
   11) CargoMovements -> 250 (Cargo 1:1, Branch/TC/Employee'e bagli)
   ============================================================ */
DECLARE @needMove INT = @target - (SELECT COUNT(*) FROM CargoMovements WHERE IsDeleted = 0);
INSERT INTO CargoMovements
 (Id, CargoId, OldStatus, NewStatus, MovementDate, Description, BranchId, TransferCenterId, EmployeeId, CreatedDate, IsDeleted)
SELECT NEWID(), c.Id,
       CASE WHEN c.CargoStatus > 1 THEN c.CargoStatus - 1 ELSE 1 END,
       c.CargoStatus,
       DATEADD(HOUR, -(n.rn % 240), @now),
       N'Durum guncellemesi: hareket kaydi #' + CAST(n.rn AS varchar(10)),
       b.Id,
       CASE WHEN c.CargoStatus = 3 THEN tc.Id ELSE NULL END,
       e.Id, DATEADD(HOUR, -(n.rn % 240), @now), 0
FROM #nums n
JOIN #Cargo2 c ON c.rn = n.rn
JOIN #Branch2 b ON b.rn = ((n.rn - 1) % @branchc) + 1
JOIN #Tc2 tc ON tc.rn = ((n.rn - 1) % @tcc) + 1
JOIN #Emp2 e ON e.rn = ((n.rn - 1) % @empc) + 1
WHERE n.rn <= @needMove AND c.rn <= @needMove;

/* ============================================================
   12) Deliveries -> 250 (Cargo 1:1, Employee'e bagli)
   ============================================================ */
DECLARE @needDel INT = @target - (SELECT COUNT(*) FROM Deliveries WHERE IsDeleted = 0);
INSERT INTO Deliveries
 (Id, CargoId, DeliveryCode, IsConfirmed, DeliveredDate, ReceivedByName, EmployeeId, CreatedDate, IsDeleted)
SELECT NEWID(), c.Id,
       RIGHT('000000' + CAST((n.rn * 53) % 1000000 AS varchar(10)), 6),
       CASE WHEN c.CargoStatus = 6 THEN 1 ELSE 0 END,
       CASE WHEN c.CargoStatus = 6 THEN DATEADD(DAY, -(n.rn % 10), @now) ELSE NULL END,
       CASE WHEN c.CargoStatus = 6 THEN f.n + N' ' + l.n ELSE N'Beklemede' END,
       e.Id, DATEADD(DAY, -(n.rn % 15), @now), 0
FROM #nums n
JOIN #Cargo2 c ON c.rn = n.rn
JOIN #Emp2 e ON e.rn = ((n.rn - 1) % @empc) + 1
JOIN #fn f ON f.rn = ((n.rn * 5 - 1) % @fnc) + 1
JOIN #ln l ON l.rn = ((n.rn * 11 - 1) % @lnc) + 1
WHERE n.rn <= @needDel AND c.rn <= @needDel;

/* ============================================================
   13) DeliveryExceptions -> 250 (Cargo 1:1, Employee'e bagli)
   ============================================================ */
DECLARE @needExc INT = @target - (SELECT COUNT(*) FROM DeliveryExceptions WHERE IsDeleted = 0);
INSERT INTO DeliveryExceptions
 (Id, CargoId, Reason, Description, ExceptionDate, AttemptNumber, EmployeeId, CreatedDate, IsDeleted)
SELECT NEWID(), c.Id,
       ((n.rn - 1) % 5) + 1,
       CASE ((n.rn - 1) % 5)
            WHEN 0 THEN N'Adres hatali, alici bulunamadi.'
            WHEN 1 THEN N'Alici adreste degil.'
            WHEN 2 THEN N'Kargo hasarli tespit edildi.'
            WHEN 3 THEN N'Alici teslim almayi reddetti.'
            ELSE N'Diger teslimat sorunu.' END,
       DATEADD(DAY, -(n.rn % 20), @now),
       ((n.rn - 1) % 3) + 1,
       e.Id, DATEADD(DAY, -(n.rn % 20), @now), 0
FROM #nums n
JOIN #Cargo2 c ON c.rn = n.rn
JOIN #Emp2 e ON e.rn = ((n.rn * 3 - 1) % @empc) + 1
WHERE n.rn <= @needExc AND c.rn <= @needExc;

/* ============================================================
   14) AuditLogs -> 250 (User + Cargo referanslari)
   ============================================================ */
DECLARE @needAudit INT = @target - (SELECT COUNT(*) FROM AuditLogs WHERE IsDeleted = 0);
INSERT INTO AuditLogs
 (Id, UserId, UserName, ActionType, EntityName, EntityId, ActionDate, Description, OldValue, NewValue, CreatedDate, IsDeleted)
SELECT NEWID(), u.Id, u.UserName,
       ((n.rn - 1) % 8) + 1,
       CASE ((n.rn - 1) % 6)
            WHEN 0 THEN N'Cargo' WHEN 1 THEN N'Branch' WHEN 2 THEN N'Employee'
            WHEN 3 THEN N'Delivery' WHEN 4 THEN N'CargoPrice' ELSE N'TransferCenter' END,
       c.Id,
       DATEADD(HOUR, -(n.rn % 500), @now),
       N'Otomatik denetim kaydi #' + CAST(n.rn AS varchar(10)),
       CASE WHEN ((n.rn - 1) % 8) + 1 = 4 THEN N'Eski Durum: ' + CAST(((n.rn - 1) % 9) + 1 AS varchar(10)) ELSE N'' END,
       CASE WHEN ((n.rn - 1) % 8) + 1 = 4 THEN N'Yeni Durum: ' + CAST((n.rn % 9) + 1 AS varchar(10)) ELSE N'' END,
       DATEADD(HOUR, -(n.rn % 500), @now), 0
FROM #nums n
JOIN #User2 u ON u.rn = ((n.rn - 1) % @userc) + 1
JOIN #Cargo2 c ON c.rn = ((n.rn - 1) % @cargoc) + 1
WHERE n.rn <= @needAudit;

COMMIT TRANSACTION;

/* ============================================================
   Ozet: tablo satir sayilari
   ============================================================ */
SELECT 'Cities' AS TableName, COUNT(*) AS [RowCount] FROM Cities WHERE IsDeleted = 0
UNION ALL SELECT 'Branches', COUNT(*) FROM Branches WHERE IsDeleted = 0
UNION ALL SELECT 'TransferCenters', COUNT(*) FROM TransferCenters WHERE IsDeleted = 0
UNION ALL SELECT 'AspNetUsers', COUNT(*) FROM AspNetUsers
UNION ALL SELECT 'Employees', COUNT(*) FROM Employees WHERE IsDeleted = 0
UNION ALL SELECT 'CargoPrices', COUNT(*) FROM CargoPrices WHERE IsDeleted = 0
UNION ALL SELECT 'Abouts', COUNT(*) FROM Abouts WHERE IsDeleted = 0
UNION ALL SELECT 'ContactInfos', COUNT(*) FROM ContactInfos WHERE IsDeleted = 0
UNION ALL SELECT 'Addresses', COUNT(*) FROM Addresses WHERE IsDeleted = 0
UNION ALL SELECT 'Cargos', COUNT(*) FROM Cargos WHERE IsDeleted = 0
UNION ALL SELECT 'CargoMovements', COUNT(*) FROM CargoMovements WHERE IsDeleted = 0
UNION ALL SELECT 'Deliveries', COUNT(*) FROM Deliveries WHERE IsDeleted = 0
UNION ALL SELECT 'DeliveryExceptions', COUNT(*) FROM DeliveryExceptions WHERE IsDeleted = 0
UNION ALL SELECT 'AuditLogs', COUNT(*) FROM AuditLogs WHERE IsDeleted = 0
ORDER BY TableName;

/* Temp temizlik */
DROP TABLE #fn, #ln, #prov, #dist, #title, #street, #pair, #nums, #City2, #User2, #Branch2, #Tc2, #Emp2, #Cargo2;
