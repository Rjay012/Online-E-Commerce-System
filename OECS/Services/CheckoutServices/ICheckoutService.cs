namespace OECS.Services.CheckoutServices
{
    public interface ICheckoutService
    {
        bool CheckAvailableOrderNumber(string orderNumber);
    }
}
