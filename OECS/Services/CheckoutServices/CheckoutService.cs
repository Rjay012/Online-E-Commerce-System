using OECS.Repository.CheckoutRepository;

namespace OECS.Services.CheckoutServices
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;
        public CheckoutService(ICheckoutRepository checkoutRepository)
        {
            _checkoutRepository = checkoutRepository;
        }

        public bool CheckAvailableOrderNumber(string orderNumber)
        {
            return _checkoutRepository.CheckAvailableOrderNumber(orderNumber);
        }
    }
}