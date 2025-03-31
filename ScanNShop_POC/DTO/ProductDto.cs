namespace ScanNShop_POC.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int? ListId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsChecked { get; set; }
    }
}
