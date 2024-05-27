using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IBaseService<T, TDto, TAddEditDto> where T : BaseEntity where TDto : class where TAddEditDto : class
    {
        TDto? GetByIdDtoObject(int id);
        Task<TDto?> GetByIdDtoObjectAsync(int id);
        List<TDto> GetAllDtoList();
        bool Delete(int id);
        int Add(TAddEditDto item);
        List<int> AddList(List<TAddEditDto> items);
        bool Update(int id, TAddEditDto item);
        TDto MapToDto(T entity);
        T MapAddEditDtoToEntity(TAddEditDto addEditDto, T? entity = null);
    }
}
