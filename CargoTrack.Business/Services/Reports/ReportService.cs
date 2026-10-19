using CargoTrack.DataAccess.Repositories.Branchs;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Deliveries;
using CargoTrack.DataAccess.Repositories.DeliveryExceptions;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DTO.DTOs.ReportDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.Business.Services.Reports
{
    public class ReportService(
        ICargoRepository _cargoRepository,
        IBranchRepository _branchRepository,
        IEmployeeRepository _employeeRepository,
        IDeliveryRepository _deliveryRepository,
        IDeliveryExceptionRepository _exceptionRepository) : IReportService
    {
        public async Task<PerformanceReportDto> GetPerformanceReportAsync()
        {
            var cargos = await _cargoRepository.Query()
                .Include(c => c.OriginBranch).ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch).ThenInclude(b => b.City)
                .ToListAsync();

            var branches = await _branchRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();
            var deliveries = await _deliveryRepository.GetAllAsync();
            var exceptions = await _exceptionRepository.GetAllAsync();

            var now = DateTime.Now;
            var report = new PerformanceReportDto();

            // Ortalama teslimat süresi (teslim edilmiş kargolar)
            var deliveredCargos = cargos.Where(c => c.CargoStatus == CargoStatus.Delivered && c.DeliveredDate.HasValue).ToList();
            if (deliveredCargos.Count > 0)
            {
                report.AverageDeliveryDurationDays = Math.Round(
                    deliveredCargos.Average(c => (c.DeliveredDate!.Value - c.ShipmentDate).TotalDays), 1);

                var onTime = deliveredCargos.Count(c => c.DeliveredDate!.Value <= c.EstimatedArrivalDate);
                report.OnTimeDeliveryRate = Math.Round(onTime * 100.0 / deliveredCargos.Count, 1);
            }

            // İade oranı
            if (cargos.Count > 0)
            {
                var returned = cargos.Count(c => c.CargoStatus == CargoStatus.ReturnedToSender
                                                 || c.CargoStatus == CargoStatus.InReturnProcess);
                report.ReturnRate = Math.Round(returned * 100.0 / cargos.Count, 1);
            }

            // Şube performansları
            foreach (var branch in branches)
            {
                var sent = cargos.Count(c => c.OriginBranchId == branch.Id);
                var deliveredForBranch = cargos.Count(c => c.DestinationBranchId == branch.Id
                                                           && c.CargoStatus == CargoStatus.Delivered);
                var incoming = cargos.Count(c => c.DestinationBranchId == branch.Id);
                var delayed = cargos.Count(c => c.DestinationBranchId == branch.Id
                                                && c.EstimatedArrivalDate < now
                                                && c.CargoStatus != CargoStatus.Delivered
                                                && c.CargoStatus != CargoStatus.ReturnedToSender);

                report.BranchPerformances.Add(new BranchPerformanceDto
                {
                    BranchId = branch.Id,
                    BranchName = branch.Name,
                    CityName = branch.City?.Name ?? "-",
                    SentCargoCount = sent,
                    DeliveredCargoCount = deliveredForBranch,
                    DelayedCargoCount = delayed,
                    DeliverySuccessRate = incoming == 0 ? 0 : Math.Round(deliveredForBranch * 100.0 / incoming, 1)
                });
            }

            report.TopSenderBranch = report.BranchPerformances.OrderByDescending(b => b.SentCargoCount).FirstOrDefault();
            report.TopDelivererBranch = report.BranchPerformances.OrderByDescending(b => b.DeliveredCargoCount).FirstOrDefault();
            report.MostDelayedBranch = report.BranchPerformances.OrderByDescending(b => b.DelayedCargoCount).FirstOrDefault();

            // Personel performansları
            foreach (var emp in employees)
            {
                var deliveryCount = deliveries.Count(d => d.EmployeeId == emp.Id && d.IsConfirmed);
                var failedCount = exceptions.Count(e => e.EmployeeId == emp.Id);

                report.EmployeePerformances.Add(new EmployeePerformanceDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    BranchName = emp.Branch?.Name ?? "-",
                    DeliveryCount = deliveryCount,
                    FailedDeliveryCount = failedCount,
                    MovementCount = deliveryCount + failedCount
                });
            }

            return report;
        }
    }
}
