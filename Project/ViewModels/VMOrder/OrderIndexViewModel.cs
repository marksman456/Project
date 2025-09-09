namespace Project.ViewModels.VMOrder
{
    public class OrderIndexViewModel
    {
        public int OrderID { get; set; }
        public string? OrderNumber { get; set; }
        public string? MemberName { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? Status { get; set; }
    }
}