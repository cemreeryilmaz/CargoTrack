/* ============================================================
   CargoTrack - Zengin Kargo Hareketleri + Teslimat Senaryolari
   ------------------------------------------------------------
   ONKOSUL: Once EF migration'i uygulayin (yeni kolonlar):
     - Cargos.DeliveryPreference (int, default 0)
     - Cargos.NeighborAddress    (nvarchar(250), null)
     - Cargos.AppointmentDate     (datetime2, null)
     - CargoMovements.LocationCity (nvarchar(100), null)

   Bu script:
     1) 50 kargoyu "Dagitimda" (OutForDelivery = 5) durumuna ceker.
     2) TUM kargolar icin cok duraklı (coklu sehir) hareket gecmisi uretir.
        Ornek: Istanbul -> Kocaeli -> Ankara -> ... -> Diyarbakir
        Ara duraklar, cikis/varis arasindaki boylamlara gore secilir.
     3) Her harekete LocationCity yazar (harita/rota icin).
   Tekrar calistirilabilir: hareketler silinip yeniden uretilir.
   ============================================================ */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @now datetime2 = SYSUTCDATETIME();

/* ---- 0) Il koordinatlari (ASCII il adlari - DB ile ayni) ---- */
IF OBJECT_ID('tempdb..#geo') IS NOT NULL DROP TABLE #geo;
SELECT name, lat, lng
INTO #geo
FROM (VALUES
 (N'Adana',37.00,35.32),(N'Adiyaman',37.76,38.28),(N'Afyonkarahisar',38.76,30.54),(N'Agri',39.72,43.05),
 (N'Amasya',40.65,35.83),(N'Ankara',39.93,32.85),(N'Antalya',36.90,30.70),(N'Artvin',41.18,41.82),
 (N'Aydin',37.84,27.84),(N'Balikesir',39.65,27.88),(N'Bilecik',40.14,29.98),(N'Bingol',38.88,40.50),
 (N'Bitlis',38.40,42.11),(N'Bolu',40.74,31.61),(N'Burdur',37.72,30.29),(N'Bursa',40.19,29.06),
 (N'Canakkale',40.15,26.41),(N'Cankiri',40.60,33.62),(N'Corum',40.55,34.95),(N'Denizli',37.78,29.09),
 (N'Diyarbakir',37.91,40.24),(N'Edirne',41.68,26.56),(N'Elazig',38.68,39.22),(N'Erzincan',39.75,39.49),
 (N'Erzurum',39.90,41.27),(N'Eskisehir',39.78,30.52),(N'Gaziantep',37.07,37.38),(N'Giresun',40.91,38.39),
 (N'Gumushane',40.46,39.48),(N'Hakkari',37.57,43.74),(N'Hatay',36.20,36.16),(N'Isparta',37.76,30.55),
 (N'Mersin',36.81,34.64),(N'Istanbul',41.01,28.98),(N'Izmir',38.42,27.14),(N'Kars',40.60,43.10),
 (N'Kastamonu',41.38,33.78),(N'Kayseri',38.73,35.49),(N'Kirklareli',41.73,27.22),(N'Kirsehir',39.14,34.17),
 (N'Kocaeli',40.77,29.92),(N'Konya',37.87,32.48),(N'Kutahya',39.42,29.98),(N'Malatya',38.35,38.31),
 (N'Manisa',38.61,27.43),(N'Kahramanmaras',37.58,36.93),(N'Mardin',37.31,40.74),(N'Mugla',37.22,28.36),
 (N'Mus',38.74,41.49),(N'Nevsehir',38.62,34.71),(N'Nigde',37.97,34.68),(N'Ordu',40.98,37.88),
 (N'Rize',41.02,40.52),(N'Sakarya',40.69,30.44),(N'Samsun',41.29,36.33),(N'Siirt',37.93,41.94),
 (N'Sinop',42.03,35.15),(N'Sivas',39.75,37.02),(N'Tekirdag',40.98,27.51),(N'Tokat',40.31,36.55),
 (N'Trabzon',41.00,39.72),(N'Tunceli',39.11,39.55),(N'Sanliurfa',37.17,38.79),(N'Usak',38.68,29.41),
 (N'Van',38.49,43.41),(N'Yozgat',39.82,34.81),(N'Zonguldak',41.45,31.79),(N'Aksaray',38.37,34.03),
 (N'Bayburt',40.26,40.22),(N'Karaman',37.18,33.22),(N'Kirikkale',39.85,33.52),(N'Batman',37.88,41.13),
 (N'Sirnak',37.52,42.46),(N'Bartin',41.63,32.34),(N'Ardahan',41.11,42.70),(N'Igdir',39.92,44.04),
 (N'Yalova',40.65,29.28),(N'Karabuk',41.20,32.63),(N'Kilis',36.72,37.12),(N'Osmaniye',37.07,36.25),
 (N'Duzce',40.84,31.16)
) v(name, lat, lng);

/* ---- 1) 50 kargoyu "Dagitimda" durumuna cek ---- */
IF OBJECT_ID('tempdb..#pick') IS NOT NULL DROP TABLE #pick;
SELECT TOP 50 Id INTO #pick FROM Cargos WHERE IsDeleted = 0 ORDER BY Id;

UPDATE c
   SET c.CargoStatus        = 5,              -- OutForDelivery
       c.DeliveryPreference = 0,              -- None (musteri henuz secmedi)
       c.NeighborAddress    = NULL,
       c.AppointmentDate    = NULL,
       c.DeliveredDate      = NULL,
       c.DeliveredToName    = NULL,
       c.CurrentBranchId    = c.DestinationBranchId,
       c.CurrentTransferCenterId = NULL,
       c.DeliveryCode       = RIGHT('000000' + CAST(ABS(CHECKSUM(c.Id)) % 1000000 AS varchar(10)), 6)
FROM Cargos c
JOIN #pick p ON p.Id = c.Id;

-- Bu kargolarin teslimat kayitlarini tutarli hale getir (henuz teslim edilmedi)
UPDATE d
   SET d.IsConfirmed   = 0,
       d.DeliveredDate = NULL,
       d.ReceivedByName = N'Beklemede'
FROM Deliveries d
JOIN #pick p ON p.Id = d.CargoId;

/* ---- 2) Her kargo icin cikis/varis sehirlerini ve koordinatlarini coz ---- */
IF OBJECT_ID('tempdb..#cg') IS NOT NULL DROP TABLE #cg;
SELECT c.Id AS CargoId, c.CargoStatus, c.ShipmentDate,
       c.OriginBranchId, c.DestinationBranchId,
       oc.Name AS OCity, dc.Name AS DCity,
       CAST(NULL AS nvarchar(100)) AS OProv, CAST(NULL AS float) AS OLat, CAST(NULL AS float) AS OLng,
       CAST(NULL AS nvarchar(100)) AS DProv, CAST(NULL AS float) AS DLat, CAST(NULL AS float) AS DLng
INTO #cg
FROM Cargos c
JOIN Branches ob ON ob.Id = c.OriginBranchId
JOIN Cities   oc ON oc.Id = ob.CityId
JOIN Branches db ON db.Id = c.DestinationBranchId
JOIN Cities   dc ON dc.Id = db.CityId
WHERE c.IsDeleted = 0;

-- Cikis ili koordinati (sehir adi il adiyla baslar: "Istanbul" ya da "Istanbul Kadikoy")
UPDATE cg
   SET OProv = g.name, OLat = g.lat, OLng = g.lng
FROM #cg cg
CROSS APPLY (
    SELECT TOP 1 g.* FROM #geo g
    WHERE cg.OCity COLLATE Latin1_General_CI_AI = g.name
       OR cg.OCity COLLATE Latin1_General_CI_AI LIKE g.name + N' %'
    ORDER BY LEN(g.name) DESC
) g;

-- Varis ili koordinati
UPDATE cg
   SET DProv = g.name, DLat = g.lat, DLng = g.lng
FROM #cg cg
CROSS APPLY (
    SELECT TOP 1 g.* FROM #geo g
    WHERE cg.DCity COLLATE Latin1_General_CI_AI = g.name
       OR cg.DCity COLLATE Latin1_General_CI_AI LIKE g.name + N' %'
    ORDER BY LEN(g.name) DESC
) g;

-- Eslesmeyen (beklenmedik) kayitlari ayikla
DELETE FROM #cg WHERE OLng IS NULL OR DLng IS NULL;

/* ---- 3) Duraklar: cikis(0) + ara duraklar(rn) + varis(99999) ---- */
IF OBJECT_ID('tempdb..#stops') IS NOT NULL DROP TABLE #stops;
CREATE TABLE #stops (CargoId uniqueidentifier, seq int, City nvarchar(100), Lat float, Lng float);

-- Cikis
INSERT INTO #stops (CargoId, seq, City, Lat, Lng)
SELECT CargoId, 0, OProv, OLat, OLng FROM #cg;

-- Ara duraklar: cikis-varis boylamlari arasinda kalan iller (en fazla 3 durak)
;WITH cand AS (
    SELECT cg.CargoId, g.name, g.lat, g.lng,
           ROW_NUMBER() OVER (
               PARTITION BY cg.CargoId
               ORDER BY CASE WHEN cg.DLng >= cg.OLng THEN g.lng ELSE -g.lng END
           ) AS rn,
           COUNT(*) OVER (PARTITION BY cg.CargoId) AS cnt
    FROM #cg cg
    JOIN #geo g
      ON g.lng > CASE WHEN cg.OLng < cg.DLng THEN cg.OLng ELSE cg.DLng END
     AND g.lng < CASE WHEN cg.OLng < cg.DLng THEN cg.DLng ELSE cg.OLng END
     AND g.name <> cg.OProv AND g.name <> cg.DProv
     AND g.lat BETWEEN (CASE WHEN cg.OLat < cg.DLat THEN cg.OLat ELSE cg.DLat END) - 2.0
                   AND (CASE WHEN cg.OLat < cg.DLat THEN cg.DLat ELSE cg.OLat END) + 2.0
)
INSERT INTO #stops (CargoId, seq, City, Lat, Lng)
SELECT CargoId, rn, name, lat, lng
FROM cand
WHERE (cnt <= 3)
   OR (cnt > 3 AND rn IN (
         CAST(ROUND(cnt * 0.25, 0) AS int),
         CAST(ROUND(cnt * 0.50, 0) AS int),
         CAST(ROUND(cnt * 0.75, 0) AS int)));

-- Varis
INSERT INTO #stops (CargoId, seq, City, Lat, Lng)
SELECT CargoId, 99999, DProv, DLat, DLng FROM #cg;

/* ---- 4) Siralama + durum atamasi ---- */
IF OBJECT_ID('tempdb..#ord') IS NOT NULL DROP TABLE #ord;
SELECT s.CargoId, s.City,
       ROW_NUMBER() OVER (PARTITION BY s.CargoId ORDER BY s.seq, s.City) AS idx,
       COUNT(*)    OVER (PARTITION BY s.CargoId) AS tot,
       cg.CargoStatus, cg.ShipmentDate,
       cg.OriginBranchId, cg.DestinationBranchId
INTO #ord
FROM #stops s
JOIN #cg cg ON cg.CargoId = s.CargoId;

IF OBJECT_ID('tempdb..#mv') IS NOT NULL DROP TABLE #mv;
SELECT o.CargoId, o.City, o.idx, o.tot, o.ShipmentDate,
       o.OriginBranchId, o.DestinationBranchId,
       CASE WHEN o.idx = o.tot THEN o.CargoStatus     -- son durak = kargonun guncel durumu
            WHEN o.idx = 1      THEN 1                 -- cikis = Created
            ELSE 3 END AS NewStatus,                  -- ara = InTransferCenter
       CAST(NULL AS int) AS OldStatus,
       DATEADD(HOUR, (o.idx - 1) * 8, o.ShipmentDate) AS MovementDate
INTO #mv
FROM #ord o;

-- OldStatus = bir onceki hareketin NewStatus'u
;WITH l AS (
    SELECT CargoId, idx, NewStatus,
           LAG(NewStatus, 1, 1) OVER (PARTITION BY CargoId ORDER BY idx) AS prev
    FROM #mv
)
UPDATE m
   SET OldStatus = l.prev
FROM #mv m
JOIN l ON l.CargoId = m.CargoId AND l.idx = m.idx;

/* ---- 5) Eski hareketleri sil, zengin hareketleri ekle ---- */
DELETE FROM CargoMovements;

INSERT INTO CargoMovements
 (Id, CargoId, OldStatus, NewStatus, MovementDate, Description, LocationCity,
  BranchId, TransferCenterId, EmployeeId, CreatedDate, IsDeleted)
SELECT NEWID(), m.CargoId, m.OldStatus, m.NewStatus, m.MovementDate,
       CASE
            WHEN m.idx = 1      THEN N'Kargo ' + m.City + N' subesinden kabul edildi.'
            WHEN m.idx = m.tot  THEN
                 CASE m.NewStatus
                      WHEN 5 THEN N'Kargo ' + m.City + N' icinde dagitima cikti.'
                      WHEN 6 THEN N'Kargo ' + m.City + N' adresinde teslim edildi.'
                      ELSE N'Kargo ' + m.City + N' varis subesine ulasti.'
                 END
            ELSE N'Kargo ' + m.City + N' transfer merkezinden gecti.'
       END,
       m.City,
       CASE WHEN m.idx = 1 THEN m.OriginBranchId
            WHEN m.idx = m.tot THEN m.DestinationBranchId
            ELSE NULL END,
       NULL, NULL, m.MovementDate, 0
FROM #mv m;

COMMIT TRANSACTION;

/* ---- Ozet ---- */
SELECT 'OutForDelivery (status=5) kargo' AS Metric, COUNT(*) AS Value
FROM Cargos WHERE IsDeleted = 0 AND CargoStatus = 5
UNION ALL
SELECT 'Toplam kargo hareketi', COUNT(*) FROM CargoMovements
UNION ALL
SELECT 'Hareketi 3+ olan kargo (cok duraklı)', COUNT(*) FROM (
    SELECT CargoId FROM CargoMovements GROUP BY CargoId HAVING COUNT(*) >= 3
) t
UNION ALL
SELECT 'LocationCity dolu hareket', COUNT(*) FROM CargoMovements WHERE LocationCity IS NOT NULL;

DROP TABLE #geo, #pick, #cg, #stops, #ord, #mv;
