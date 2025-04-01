namespace jr_api.IServices
{
    public interface IRolService
    {
        Task<IEnumerable<object>> GetRoles();
        Task<IEnumerable<object>> GetPermisos();
        Task<int> GuardarRol(GuardarRolRequest request);


    }

}
