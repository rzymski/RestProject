using DB.Dto.Message;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IBaseService<T, TDto, TAddEditDto> where T : BaseEntity where TDto : BaseDto where TAddEditDto : class
    {
        TDto GetByIdDtoObject(int id);
        List<TDto> GetAllDtoList();
        bool Delete(int id);
        int Add(TAddEditDto item);
        bool Update(int id, TAddEditDto item);
    }
}
