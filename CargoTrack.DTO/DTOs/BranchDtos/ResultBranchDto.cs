using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.CityDtos;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DTO.DTOs.BranchDtos
{
    public class ResultBranchDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid CityId { get; set; }
        public CityDto City { get; set; }
        public IList<CargoDto> OriginCargos { get; set; }
        public IList<CargoDto> DestinationCargos { get; set; }
    }
}
