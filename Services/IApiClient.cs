using static OpenFoodFactService.Services.ApiClient;

namespace OpenFoodFactService.Services
{
    public interface IApiClient
    {
         OpenFoodFactInfo ConnectToApi(string id);
    }
}