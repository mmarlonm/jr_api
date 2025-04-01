namespace jr_api.IServices
{
    public interface IPermisoService
    {
        Task<IEnumerable<object>> GetPermisos();
    }
}
