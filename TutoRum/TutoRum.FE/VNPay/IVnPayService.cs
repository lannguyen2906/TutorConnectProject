using TutoRum.Services.ViewModels;

namespace TutoRum.FE.VNPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
