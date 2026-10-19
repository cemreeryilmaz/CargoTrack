using System.ComponentModel.DataAnnotations;
using CargoTrack.Business.Services.AuditLogs;
using CargoTrack.Business.Services.Pricing;
using CargoTrack.DataAccess.Repositories.Branchs;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Deliveries;
using CargoTrack.DataAccess.Repositories.DeliveryExceptions;
using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.CargoMovementDtos;
using CargoTrack.DTO.DTOs.DeliveryDtos;
using CargoTrack.DTO.DTOs.DeliveryExceptionDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.Business.Services.Cargos
{
    public class CargoService(
        ICargoRepository _cargoRepository,
        ICargoMovementRepository _movementRepository,
        IDeliveryRepository _deliveryRepository,
        IDeliveryExceptionRepository _exceptionRepository,
        IBranchRepository _branchRepository,
        ICargoPricingService _pricingService,
        IAuditLogService _auditLogService) : ICargoService
    {
        private const int MaxFailedAttempts = 3;
        private static readonly Random _random = new();

        // ---------------- CREATE ----------------
        public async Task CreateAsync(CreateCargoDto dto, Guid? userId = null, string? userName = null)
        {
            var originBranch = await _branchRepository.GetByIdAsync(dto.OriginBranchId);
            var destinationBranch = await _branchRepository.GetByIdAsync(dto.DestinationBranchId);

            if (originBranch is null || destinationBranch is null)
                throw new ValidationException("Gönderici veya varış şubesi bulunamadı.");

            bool sameCity = originBranch.CityId == destinationBranch.CityId;

            var desi = _pricingService.CalculateDesi(dto.Width, dto.Height, dto.Length);
            var price = await _pricingService.CalculatePriceAsync(dto.DeliveryType, dto.Weight, desi, sameCity);
            var shipmentDate = DateTime.Now;
            var estimated = _pricingService.CalculateEstimatedArrival(dto.DeliveryType, shipmentDate, sameCity);

            var cargo = new Cargo
            {
                TrackCode = await GenerateUniqueTrackCodeAsync(),
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                OriginBranchId = dto.OriginBranchId,
                DestinationBranchId = dto.DestinationBranchId,
                Weight = dto.Weight,
                Width = dto.Width,
                Height = dto.Height,
                Length = dto.Length,
                Desi = desi,
                Price = price,
                CargoType = dto.CargoType,
                DeliveryType = dto.DeliveryType,
                CargoStatus = CargoStatus.Created,
                ShipmentDate = shipmentDate,
                EstimatedArrivalDate = estimated,
                CurrentBranchId = dto.OriginBranchId
            };

            await _cargoRepository.CreateAsync(cargo);

            // İlk hareket kaydı
            await CreateMovementAsync(cargo.Id, CargoStatus.Created, CargoStatus.Created,
                "Kargo kaydı oluşturuldu.", dto.OriginBranchId, null, null);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.Create,
                nameof(Cargo), cargo.Id, $"Kargo oluşturuldu: {cargo.TrackCode}");
        }

        // ---------------- UPDATE ----------------
        public async Task UpdateAsync(UpdateCargoDto dto, Guid? userId = null, string? userName = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(dto.Id);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            var originBranch = await _branchRepository.GetByIdAsync(dto.OriginBranchId);
            var destinationBranch = await _branchRepository.GetByIdAsync(dto.DestinationBranchId);
            bool sameCity = originBranch is not null && destinationBranch is not null
                            && originBranch.CityId == destinationBranch.CityId;

            cargo.SenderId = dto.SenderId;
            cargo.ReceiverId = dto.ReceiverId;
            cargo.OriginBranchId = dto.OriginBranchId;
            cargo.DestinationBranchId = dto.DestinationBranchId;
            cargo.Weight = dto.Weight;
            cargo.Width = dto.Width;
            cargo.Height = dto.Height;
            cargo.Length = dto.Length;
            cargo.CargoType = dto.CargoType;
            cargo.DeliveryType = dto.DeliveryType;

            cargo.Desi = _pricingService.CalculateDesi(dto.Width, dto.Height, dto.Length);
            cargo.Price = await _pricingService.CalculatePriceAsync(dto.DeliveryType, dto.Weight, cargo.Desi, sameCity);

            await _cargoRepository.UpdateAsync(cargo);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.Update,
                nameof(Cargo), cargo.Id, $"Kargo güncellendi: {cargo.TrackCode}");
        }

        // ---------------- DELETE (soft) ----------------
        public async Task DeleteAsync(Guid id, Guid? userId = null, string? userName = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            await _cargoRepository.DeleteAsync(cargo);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.Delete,
                nameof(Cargo), cargo.Id, $"Kargo silindi: {cargo.TrackCode}");
        }

        // ---------------- GET ----------------
        public async Task<UpdateCargoDto> GetByIdAsync(Guid id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            return new UpdateCargoDto
            {
                Id = cargo.Id,
                SenderId = cargo.SenderId,
                ReceiverId = cargo.ReceiverId,
                OriginBranchId = cargo.OriginBranchId,
                DestinationBranchId = cargo.DestinationBranchId,
                Weight = cargo.Weight,
                Width = cargo.Width,
                Height = cargo.Height,
                Length = cargo.Length,
                CargoType = cargo.CargoType,
                DeliveryType = cargo.DeliveryType
            };
        }

        public async Task<CargoDetailDto> GetDetailAsync(Guid id)
        {
            var cargo = await _cargoRepository.GetWithDetailsAsync(id);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            return MapToDetail(cargo);
        }

        // ---------------- LISTING (role based) ----------------
        public async Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoFilterDto filter)
        {
            var query = BuildBaseQuery();
            return await ApplyFilterAndPaginateAsync(query, filter);
        }

        public async Task<PagedResult<ResultCargoDto>> GetBranchPagedAsync(Guid branchId, CargoFilterDto filter)
        {
            var query = BuildBaseQuery()
                .Where(c => c.OriginBranchId == branchId || c.DestinationBranchId == branchId);
            return await ApplyFilterAndPaginateAsync(query, filter);
        }

        public async Task<PagedResult<ResultCargoDto>> GetUserPagedAsync(Guid userId, CargoFilterDto filter)
        {
            var query = BuildBaseQuery()
                .Where(c => c.SenderId == userId || c.ReceiverId == userId);
            return await ApplyFilterAndPaginateAsync(query, filter);
        }

        // ---------------- PUBLIC TRACKING ----------------
        public async Task<CargoTrackingDto?> GetTrackingAsync(string trackCode)
        {
            if (string.IsNullOrWhiteSpace(trackCode))
                return null;

            var cargo = await _cargoRepository.GetByTrackCodeAsync(trackCode.Trim());
            if (cargo is null)
                return null;

            var movements = cargo.CargoMovements?
                .OrderBy(m => m.MovementDate)
                .Select(MapMovement)
                .ToList() ?? new List<ResultCargoMovementDto>();

            return new CargoTrackingDto
            {
                TrackCode = cargo.TrackCode,
                CargoStatus = cargo.CargoStatus,
                ShipmentDate = cargo.ShipmentDate,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,
                OriginCityName = cargo.OriginBranch?.City?.Name ?? "-",
                DestinationCityName = cargo.DestinationBranch?.City?.Name ?? "-",
                OriginBranchName = cargo.OriginBranch?.Name ?? "-",
                DestinationBranchName = cargo.DestinationBranch?.Name ?? "-",
                Movements = movements
            };
        }

        // ---------------- PUBLIC RECENT ----------------
        public async Task<List<CargoTrackingDto>> GetRecentAsync(int count)
        {
            if (count < 1) count = 6;

            var cargos = await _cargoRepository.Query()
                .Include(c => c.OriginBranch).ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch).ThenInclude(b => b.City)
                .OrderByDescending(c => c.CreatedDate)
                .Take(count)
                .ToListAsync();

            return cargos.Select(c => new CargoTrackingDto
            {
                TrackCode = c.TrackCode,
                CargoStatus = c.CargoStatus,
                ShipmentDate = c.ShipmentDate,
                EstimatedArrivalDate = c.EstimatedArrivalDate,
                OriginCityName = c.OriginBranch?.City?.Name ?? "-",
                DestinationCityName = c.DestinationBranch?.City?.Name ?? "-",
                OriginBranchName = c.OriginBranch?.Name ?? "-",
                DestinationBranchName = c.DestinationBranch?.Name ?? "-",
                Movements = new List<ResultCargoMovementDto>()
            }).ToList();
        }

        // ---------------- STATE MACHINE ----------------
        public async Task ChangeStatusAsync(ChangeCargoStatusDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            // Teslimat ve başarısız teslimat özel metotlarla yapılır.
            if (dto.NewStatus == CargoStatus.Delivered)
                throw new ValidationException("Teslim işlemi için teslimat kodu doğrulaması gereklidir.");
            if (dto.NewStatus == CargoStatus.DeliveryFailed)
                throw new ValidationException("Başarısız teslimat için sorunlu kargo kaydı oluşturulmalıdır.");

            var oldStatus = cargo.CargoStatus;

            if (!CargoStatusRules.CanTransition(oldStatus, dto.NewStatus))
                throw new ValidationException($"Geçersiz durum geçişi: {oldStatus} -> {dto.NewStatus}");

            cargo.CargoStatus = dto.NewStatus;

            // Konum güncelleme
            if (dto.NewStatus == CargoStatus.AtTransferCenter && dto.TransferCenterId.HasValue)
            {
                cargo.CurrentTransferCenterId = dto.TransferCenterId;
                cargo.CurrentBranchId = null;
            }
            else if (dto.BranchId.HasValue)
            {
                cargo.CurrentBranchId = dto.BranchId;
                cargo.CurrentTransferCenterId = null;
            }

            // Dağıtıma çıkınca 6 haneli teslimat kodu üret
            if (dto.NewStatus == CargoStatus.OutForDelivery)
                await EnsureDeliveryCodeAsync(cargo);

            await _cargoRepository.UpdateAsync(cargo);

            await CreateMovementAsync(cargo.Id, oldStatus, dto.NewStatus,
                dto.Description ?? dto.NewStatus.ToString(), dto.BranchId, dto.TransferCenterId, employeeId);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.StatusChange,
                nameof(Cargo), cargo.Id, $"Durum değişikliği: {cargo.TrackCode}",
                oldStatus.ToString(), dto.NewStatus.ToString());
        }

        // ---------------- DELIVERY CONFIRM ----------------
        public async Task ConfirmDeliveryAsync(ConfirmDeliveryDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            if (cargo.CargoStatus != CargoStatus.OutForDelivery)
                throw new ValidationException("Yalnızca dağıtıma çıkmış kargolar teslim edilebilir.");

            if (string.IsNullOrWhiteSpace(cargo.DeliveryCode) || cargo.DeliveryCode != dto.DeliveryCode?.Trim())
                throw new ValidationException("Teslimat kodu hatalı. Teslim işlemi gerçekleştirilemez.");

            var oldStatus = cargo.CargoStatus;
            cargo.CargoStatus = CargoStatus.Delivered;
            cargo.DeliveredToName = dto.ReceivedByName;
            cargo.DeliveredDate = DateTime.Now;
            await _cargoRepository.UpdateAsync(cargo);

            // Delivery kaydını güncelle/oluştur
            var delivery = await _deliveryRepository.GetByCargoIdAsync(cargo.Id);
            if (delivery is null)
            {
                delivery = new Delivery
                {
                    CargoId = cargo.Id,
                    DeliveryCode = cargo.DeliveryCode!,
                    IsConfirmed = true,
                    DeliveredDate = DateTime.Now,
                    ReceivedByName = dto.ReceivedByName,
                    EmployeeId = employeeId
                };
                await _deliveryRepository.CreateAsync(delivery);
            }
            else
            {
                delivery.IsConfirmed = true;
                delivery.DeliveredDate = DateTime.Now;
                delivery.ReceivedByName = dto.ReceivedByName;
                delivery.EmployeeId = employeeId;
                await _deliveryRepository.UpdateAsync(delivery);
            }

            await CreateMovementAsync(cargo.Id, oldStatus, CargoStatus.Delivered,
                $"Kargo teslim edildi. Teslim alan: {dto.ReceivedByName}", cargo.DestinationBranchId, null, employeeId);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.DeliveryConfirmed,
                nameof(Cargo), cargo.Id, $"Teslim edildi: {cargo.TrackCode}",
                oldStatus.ToString(), CargoStatus.Delivered.ToString());
        }

        // ---------------- DELIVERY FAILURE / RETURN ----------------
        public async Task ReportDeliveryFailureAsync(CreateDeliveryExceptionDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(dto.CargoId);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            if (cargo.CargoStatus != CargoStatus.OutForDelivery && cargo.CargoStatus != CargoStatus.DeliveryFailed)
                throw new ValidationException("Yalnızca dağıtımdaki kargolar için başarısız teslimat kaydı oluşturulabilir.");

            var oldStatus = cargo.CargoStatus;
            cargo.FailedAttemptCount += 1;

            // Sorunlu kargo kaydı
            var exception = new DeliveryException
            {
                CargoId = cargo.Id,
                Reason = dto.Reason,
                Description = dto.Description ?? string.Empty,
                ExceptionDate = DateTime.Now,
                AttemptNumber = cargo.FailedAttemptCount,
                EmployeeId = employeeId
            };
            await _exceptionRepository.CreateAsync(exception);

            if (cargo.FailedAttemptCount >= MaxFailedAttempts)
            {
                // İade sürecine al
                cargo.CargoStatus = CargoStatus.InReturnProcess;
                await _cargoRepository.UpdateAsync(cargo);

                await CreateMovementAsync(cargo.Id, oldStatus, CargoStatus.InReturnProcess,
                    $"{MaxFailedAttempts} başarısız deneme sonrası iade sürecine alındı.", cargo.DestinationBranchId, null, employeeId);

                await _auditLogService.LogAsync(userId, userName, AuditActionType.Returned,
                    nameof(Cargo), cargo.Id, $"İade sürecine alındı: {cargo.TrackCode}",
                    oldStatus.ToString(), CargoStatus.InReturnProcess.ToString());
            }
            else
            {
                cargo.CargoStatus = CargoStatus.DeliveryFailed;
                await _cargoRepository.UpdateAsync(cargo);

                await CreateMovementAsync(cargo.Id, oldStatus, CargoStatus.DeliveryFailed,
                    $"Teslim edilemedi ({cargo.FailedAttemptCount}. deneme): {dto.Reason}", cargo.DestinationBranchId, null, employeeId);

                await _auditLogService.LogAsync(userId, userName, AuditActionType.DeliveryFailed,
                    nameof(Cargo), cargo.Id, $"Teslim edilemedi: {cargo.TrackCode}",
                    oldStatus.ToString(), CargoStatus.DeliveryFailed.ToString());
            }
        }

        // ---------------- CUSTOMER DELIVERY PREFERENCE ----------------
        // Müşteri (gönderici/alıcı) dağıtımdaki kargo için teslimat tercihi belirler:
        // Adresteyim / Komşuma teslim et / Başka bir güne randevu.
        public async Task SetDeliveryPreferenceAsync(Guid cargoId, Guid userId, DeliveryPreference preference,
            string? neighborAddress, DateTime? appointmentDate, string? userName = null)
        {
            var cargo = await _cargoRepository.GetWithDetailsAsync(cargoId);
            if (cargo is null)
                throw new ValidationException("Kargo bulunamadı.");

            // Sahiplik kontrolü: yalnızca gönderici veya alıcı işlem yapabilir.
            if (cargo.SenderId != userId && cargo.ReceiverId != userId)
                throw new ValidationException("Bu kargo üzerinde işlem yapma yetkiniz yok.");

            if (cargo.CargoStatus != CargoStatus.OutForDelivery)
                throw new ValidationException("Teslimat tercihi yalnızca dağıtıma çıkmış kargolar için seçilebilir.");

            // Tercih bir kez yapılır; tekrar değiştirilemez.
            if (cargo.DeliveryPreference != DeliveryPreference.None)
                throw new ValidationException("Bu kargo için teslimat tercihiniz zaten alınmıştır.");

            string note;
            switch (preference)
            {
                case DeliveryPreference.AtAddress:
                    cargo.NeighborAddress = null;
                    cargo.AppointmentDate = null;
                    note = "Müşteri adreste olduğunu bildirdi (Adresteyim).";
                    break;

                case DeliveryPreference.DeliverToNeighbor:
                    if (string.IsNullOrWhiteSpace(neighborAddress))
                        throw new ValidationException("Komşu adresi zorunludur.");
                    cargo.NeighborAddress = neighborAddress.Trim();
                    cargo.AppointmentDate = null;
                    note = $"Komşuya teslim talebi. Komşu adresi: {cargo.NeighborAddress}";
                    break;

                case DeliveryPreference.Appointment:
                    if (!appointmentDate.HasValue)
                        throw new ValidationException("Randevu tarihi zorunludur.");
                    if (appointmentDate.Value.Date < DateTime.Now.Date)
                        throw new ValidationException("Randevu tarihi bugünden önce olamaz.");
                    cargo.AppointmentDate = appointmentDate.Value.Date;
                    cargo.NeighborAddress = null;
                    note = $"Yeni teslimat randevusu oluşturuldu: {cargo.AppointmentDate:dd.MM.yyyy}";
                    break;

                default:
                    throw new ValidationException("Geçersiz teslimat tercihi.");
            }

            cargo.DeliveryPreference = preference;
            await _cargoRepository.UpdateAsync(cargo);

            var city = cargo.DestinationBranch?.City?.Name;
            await CreateMovementAsync(cargo.Id, cargo.CargoStatus, cargo.CargoStatus,
                note, cargo.DestinationBranchId, null, null, city);

            await _auditLogService.LogAsync(userId, userName, AuditActionType.Update,
                nameof(Cargo), cargo.Id, $"Teslimat tercihi: {cargo.TrackCode} -> {preference.GetDisplayName()}");
        }

        // ================= Helpers =================
        private IQueryable<Cargo> BuildBaseQuery()
        {
            return _cargoRepository.Query()
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.OriginBranch).ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch).ThenInclude(b => b.City);
        }

        private async Task<PagedResult<ResultCargoDto>> ApplyFilterAndPaginateAsync(IQueryable<Cargo> query, CargoFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(c => c.TrackCode.Contains(filter.Search));

            if (filter.Status.HasValue)
                query = query.Where(c => c.CargoStatus == filter.Status.Value);

            if (filter.DeliveryType.HasValue)
                query = query.Where(c => c.DeliveryType == filter.DeliveryType.Value);

            if (filter.OnlyDelayed)
            {
                var now = DateTime.Now;
                query = query.Where(c => c.EstimatedArrivalDate < now
                                         && c.CargoStatus != CargoStatus.Delivered
                                         && c.CargoStatus != CargoStatus.ReturnedToSender);
            }

            var totalCount = await query.CountAsync();

            int page = filter.Page < 1 ? 1 : filter.Page;
            int pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ResultCargoDto>
            {
                Items = items.Select(MapToResult).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private async Task CreateMovementAsync(Guid cargoId, CargoStatus oldStatus, CargoStatus newStatus,
            string description, Guid? branchId, Guid? transferCenterId, Guid? employeeId, string? city = null)
        {
            var movement = new CargoMovement
            {
                CargoId = cargoId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                MovementDate = DateTime.Now,
                Description = description,
                LocationCity = city,
                BranchId = branchId,
                TransferCenterId = transferCenterId,
                EmployeeId = employeeId
            };
            await _movementRepository.CreateAsync(movement);
        }

        private async Task EnsureDeliveryCodeAsync(Cargo cargo)
        {
            if (!string.IsNullOrWhiteSpace(cargo.DeliveryCode))
                return;

            cargo.DeliveryCode = _random.Next(100000, 1000000).ToString(); // 6 haneli

            var delivery = new Delivery
            {
                CargoId = cargo.Id,
                DeliveryCode = cargo.DeliveryCode,
                IsConfirmed = false
            };
            await _deliveryRepository.CreateAsync(delivery);
        }

        private async Task<string> GenerateUniqueTrackCodeAsync()
        {
            string code;
            do
            {
                code = $"MYC-{DateTime.Now.Year}-{_random.Next(100000, 1000000)}";
            }
            while (await _cargoRepository.AnyAsync(c => c.TrackCode == code));

            return code;
        }

        private static ResultCargoDto MapToResult(Cargo c) => new()
        {
            Id = c.Id,
            TrackCode = c.TrackCode,
            ShipmentDate = c.ShipmentDate,
            EstimatedArrivalDate = c.EstimatedArrivalDate,
            Weight = c.Weight,
            Desi = c.Desi,
            Price = c.Price,
            CargoType = c.CargoType,
            CargoStatus = c.CargoStatus,
            DeliveryType = c.DeliveryType,
            SenderFullName = c.Sender is null ? "-" : $"{c.Sender.FirstName} {c.Sender.LastName}",
            ReceiverFullName = c.Receiver is null ? "-" : $"{c.Receiver.FirstName} {c.Receiver.LastName}",
            OriginBranchName = c.OriginBranch?.Name ?? "-",
            DestinationBranchName = c.DestinationBranch?.Name ?? "-",
            CreatedDate = c.CreatedDate
        };

        private static CargoDetailDto MapToDetail(Cargo c)
        {
            var dto = new CargoDetailDto
            {
                Id = c.Id,
                TrackCode = c.TrackCode,
                ShipmentDate = c.ShipmentDate,
                EstimatedArrivalDate = c.EstimatedArrivalDate,
                Weight = c.Weight,
                Width = c.Width,
                Height = c.Height,
                Length = c.Length,
                Desi = c.Desi,
                Price = c.Price,
                CargoType = c.CargoType,
                CargoStatus = c.CargoStatus,
                DeliveryType = c.DeliveryType,
                DeliveryCode = c.DeliveryCode,
                DeliveredToName = c.DeliveredToName,
                DeliveredDate = c.DeliveredDate,
                FailedAttemptCount = c.FailedAttemptCount,
                DeliveryPreference = c.DeliveryPreference,
                NeighborAddress = c.NeighborAddress,
                AppointmentDate = c.AppointmentDate,
                SenderFullName = c.Sender is null ? "-" : $"{c.Sender.FirstName} {c.Sender.LastName}",
                ReceiverFullName = c.Receiver is null ? "-" : $"{c.Receiver.FirstName} {c.Receiver.LastName}",
                OriginBranchName = c.OriginBranch?.Name ?? "-",
                OriginCityName = c.OriginBranch?.City?.Name ?? "-",
                DestinationBranchName = c.DestinationBranch?.Name ?? "-",
                DestinationCityName = c.DestinationBranch?.City?.Name ?? "-"
            };

            if (c.CargoMovements is not null)
                dto.Movements = c.CargoMovements.OrderBy(m => m.MovementDate).Select(MapMovement).ToList();

            if (c.DeliveryExceptions is not null)
            {
                dto.Exceptions = c.DeliveryExceptions.OrderBy(e => e.ExceptionDate).Select(e => new ResultDeliveryExceptionDto
                {
                    Id = e.Id,
                    CargoId = e.CargoId,
                    CargoTrackCode = c.TrackCode,
                    Reason = e.Reason,
                    Description = e.Description,
                    ExceptionDate = e.ExceptionDate,
                    AttemptNumber = e.AttemptNumber,
                    EmployeeName = e.Employee?.FullName
                }).ToList();
            }

            return dto;
        }

        private static ResultCargoMovementDto MapMovement(CargoMovement m) => new()
        {
            Id = m.Id,
            CargoId = m.CargoId,
            OldStatus = m.OldStatus,
            NewStatus = m.NewStatus,
            MovementDate = m.MovementDate,
            Description = m.Description,
            LocationName = m.Branch?.Name ?? m.TransferCenter?.Name,
            City = m.LocationCity,
            EmployeeName = m.Employee?.FullName
        };
    }
}
