using System.Collections.Generic;
using DB.Dto.Message;
using DB.Repositories.Interfaces;
using DB.Entities;
using System.Linq;

namespace DB.Services.Interfaces
{
    public class BaseService<T, TDto, TAddEditDto> : IBaseService<T, TDto, TAddEditDto> where T : BaseEntity where TDto : BaseDto, new() where TAddEditDto : class
    {
        protected readonly IBaseRepository<T> repository;

        public BaseService(IBaseRepository<T> repository)
        {
            this.repository = repository;
        }

        public TDto GetByIdDtoObject(int id)
        {
            var item = repository.GetById(id);
            if (item == null)
                return null;
            var result = MapToDto(item); // Mapowanie encji na DTO
            return result;
        }

        public List<TDto> GetAllDtoList()
        {
            var items = repository.GetAll();
            var results = items.Select(MapToDto).ToList(); // Mapowanie listy encji na listę DTO
            return results;
        }

        public int Add(TAddEditDto item)
        {
            var entity = MapAddEditDtoToEntity(item); // Mapowanie DTO na encję
            repository.Add(entity);
            return entity.Id;
        }

        public bool Update(int id, TAddEditDto item)
        {
            var existingItem = repository.GetById(id);
            if (existingItem == null)
                return false;

            MapAddEditDtoToEntity(item, existingItem); // Aktualizacja istniejącej encji na podstawie DTO
            repository.Update(existingItem);
            return true;
        }

        public bool Delete(int id)
        {
            var existingItem = repository.GetById(id);
            if (existingItem == null)
                return false;
            repository.Delete(id);
            return true;
        }

        // Metoda do mapowania encji na DTO
        protected virtual TDto MapToDto(T entity)
        {
            return new TDto
            {
                Id = entity.Id
            };
        }

        // Metoda do mapowania DTO na encję
        protected virtual T MapAddEditDtoToEntity(TAddEditDto addEditDto, T entity = null)
        {
            if (entity == null)
                entity = Activator.CreateInstance<T>(); // Utwórz nową instancję encji, jeśli nie została dostarczona
            return entity;
        }
    }
}
