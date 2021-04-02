namespace OECS.Repository.CheckoutRepository
{
    public interface ICheckoutRepository
    {
        bool CheckAvailableOrderNumber(string orderNumber);
    }
}
