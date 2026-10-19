using CargoTrack.Business.Services.Cargos;
using CargoTrack.DataAccess.Repositories.Branchs;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.CargoMovementDtos;
using CargoTrack.DTO.DTOs.DashboardDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.Business.Services.Dashboards
{
    public class DashboardService(
        ICargoRepository _cargoRepository,
        ICargoMovementRepository _movementRepository,
        IBranchRepository _branchRepository,
        IEmployeeRepository _employeeRepository,
        ICargoService _cargoService) : IDashboardService
    {
        // ---------------- ADMIN ----------------
        public async Task<AdminDashboardDto> GetAdminDashboardAsync(int userCount)
        {
            var cargos = await _cargoRepository.Query()
                .Include(c => c.OriginBranch)
                .ToListAsync();

            var now = DateTime.Now;
            int total = cargos.Count;
            int delivered = cargos.Count(c => c.CargoStatus == CargoStatus.Delivered);
            int active = cargos.Count(c => c.CargoStatus != CargoStatus.Delivered
                                           && c.CargoStatus != CargoStatus.ReturnedToSender);
            int delayed = cargos.Count(c => IsDelayed(c, now));

            var dto = new AdminDashboardDto
            {
                TotalCargoCount = total,
                ActiveCargoCount = active,
                DeliveredCargoCount = delivered,
                DelayedCargoCount = delayed,
                DeliveryRate = total == 0 ? 0 : Math.Round(delivered * 100.0 / total, 1),
                ActiveBranchCount = await _branchRepository.CountAsync(),
                ActiveEmployeeCount = await _employeeRepository.CountAsync(e => e.IsActive),
                UserCount = userCount
            };

            // Son 7 günlük trafik
            for (int i = 6; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var count = cargos.Count(c => c.CreatedDate.Date == day);
                dto.Last7DaysTraffic.Add(new ChartPointDto { Label = day.ToString("dd.MM"), Value = count });
            }

            // Durum dağılımı
            foreach (CargoStatus status in Enum.GetValues<CargoStatus>())
            {
                var count = cargos.Count(c => c.CargoStatus == status);
                if (count > 0)
                    dto.StatusDistribution.Add(new ChartPointDto { Label = status.GetDisplayName(), Value = count });
            }

            // En yoğun 5 şube (gönderilen kargoya göre)
            dto.Top5BusiestBranches = cargos
                .GroupBy(c => c.OriginBranch?.Name ?? "-")
                .Select(g => new ChartPointDto { Label = g.Key, Value = g.Count() })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToList();

            // Son eklenen kargolar
            var recentFilter = new CargoFilterDto { Page = 1, PageSize = 5 };
            var recent = await _cargoService.GetPagedAsync(recentFilter);
            dto.RecentCargos = recent.Items;

            return dto;
        }

        // ---------------- MANAGER ----------------
        public async Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);

            var cargos = await _cargoRepository.Query()
                .Where(c => c.OriginBranchId == branchId || c.DestinationBranchId == branchId)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.OriginBranch)
                .Include(c => c.DestinationBranch)
                .ToListAsync();

            var now = DateTime.Now;
            var today = now.Date;

            var dto = new ManagerDashboardDto
            {
                BranchName = branch?.Name ?? "-",
                TodayIncomingCount = cargos.Count(c => c.DestinationBranchId == branchId && c.CreatedDate.Date == today),
                TodayOutgoingCount = cargos.Count(c => c.OriginBranchId == branchId && c.CreatedDate.Date == today),
                InDistributionCount = cargos.Count(c => c.CargoStatus == CargoStatus.OutForDelivery),
                DeliveredCount = cargos.Count(c => c.CargoStatus == CargoStatus.Delivered),
                DelayedCount = cargos.Count(c => IsDelayed(c, now)),
                ProblematicCount = cargos.Count(c => c.CargoStatus == CargoStatus.DeliveryFailed
                                                     || c.CargoStatus == CargoStatus.InReturnProcess)
            };

            // Dağıtım listesi
            dto.DistributionList = cargos
                .Where(c => c.CargoStatus == CargoStatus.OutForDelivery)
                .Select(MapToResult)
                .ToList();

            // Son hareketler (şubeye ait)
            var movements = await _movementRepository.Query()
                .Where(m => m.BranchId == branchId)
                .Include(m => m.Branch)
                .Include(m => m.Employee)
                .OrderByDescending(m => m.MovementDate)
                .Take(10)
                .ToListAsync();

            dto.RecentMovements = movements.Select(m => new ResultCargoMovementDto
            {
                Id = m.Id,
                CargoId = m.CargoId,
                OldStatus = m.OldStatus,
                NewStatus = m.NewStatus,
                MovementDate = m.MovementDate,
                Description = m.Description,
                LocationName = m.Branch?.Name,
                EmployeeName = m.Employee?.FullName
            }).ToList();

            return dto;
        }

        // ---------------- USER ----------------
        public async Task<UserDashboardDto> GetUserDashboardAsync(Guid userId)
        {
            var cargos = await _cargoRepository.Query()
                .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.OriginBranch)
                .Include(c => c.DestinationBranch)
                .ToListAsync();

            var dto = new UserDashboardDto
            {
                ActiveCargoCount = cargos.Count(c => c.CargoStatus != CargoStatus.Delivered
                                                     && c.CargoStatus != CargoStatus.ReturnedToSender),
                PastDeliveryCount = cargos.Count(c => c.CargoStatus == CargoStatus.Delivered),
                SentCargoCount = cargos.Count(c => c.SenderId == userId),
                ReceivedCargoCount = cargos.Count(c => c.ReceiverId == userId)
            };

            dto.ActiveCargos = cargos
                .Where(c => c.CargoStatus != CargoStatus.Delivered && c.CargoStatus != CargoStatus.ReturnedToSender)
                .OrderByDescending(c => c.CreatedDate)
                .Take(10)
                .Select(MapToResult)
                .ToList();

            var lastCargo = cargos.OrderByDescending(c => c.CreatedDate).FirstOrDefault();
            if (lastCargo is not null)
                dto.LastCargo = await _cargoService.GetDetailAsync(lastCargo.Id);

            return dto;
        }

        // ---------------- Helpers ----------------
        private static bool IsDelayed(Cargo c, DateTime now)
            => c.EstimatedArrivalDate < now
               && c.CargoStatus != CargoStatus.Delivered
               && c.CargoStatus != CargoStatus.ReturnedToSender;

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
    }
}
