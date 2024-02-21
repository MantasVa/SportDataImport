namespace SportDataImport.Interfaces;

internal interface IEuroleagueFeaturesService
{
    Task PrepareFeatureData(string[] seasons);
}
