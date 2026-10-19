namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public Guid BranchId { get; set; }
        public Guid? AppUserId { get; set; }
    }
}
