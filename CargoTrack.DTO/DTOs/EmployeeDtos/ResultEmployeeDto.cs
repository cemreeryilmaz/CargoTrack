namespace CargoTrack.DTO.DTOs.EmployeeDtos
{
    public class ResultEmployeeDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => string.Join(" ", FirstName, LastName);
        public string? Title { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
