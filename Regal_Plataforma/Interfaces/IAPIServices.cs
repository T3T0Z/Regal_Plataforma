public interface IAPIServices
{
    Task<string> GetOrderDetailsAsync(string orderId, string company, string claimNumber, string professionalId);
}