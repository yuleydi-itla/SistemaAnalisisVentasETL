namespace SistemaVentas.WkService.Proccess;

public interface IExtractor<T>
{
    Task<IEnumerable<T>> ExtractAsync();
}
