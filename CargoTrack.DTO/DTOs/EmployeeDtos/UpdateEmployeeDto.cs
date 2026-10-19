namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public Guid? AppUserId { get; set; }
    }
}
