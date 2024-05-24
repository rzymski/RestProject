using DB.Repositories.Interfaces;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public class BaseService<T, TDto, TAddEditDto> : IBaseService<T, TDto, TAddEditDto> where T : BaseEntity, new() where TDto : class, new() where TAddEditDto : class
    {
        protected readonly IBaseRepository<T> baseRepository;

        public BaseService(IBaseRepository<T> repository)
        {
            this.baseRepository = repository;
        }

        public TDto GetByIdDtoObject(int id)
        {
            var item = baseRepository.GetById(id);
            if (item == null)
                return null;
            var result = MapToDto(item); // Mapowanie encji na DTO
            return result;
        }

        public List<TDto> GetAllDtoList()
        {
            var items = baseRepository.GetAll();
            var results = items.Select(MapToDto).ToList(); // Mapowanie listy encji na listę DTO
            return results;
        }

        public virtual int Add(TAddEditDto item)
        {
            var entity = MapAddEditDtoToEntity(item); // Mapowanie DTO na encję
            baseRepository.Add(entity);
            return entity.Id;
        }

        public virtual List<int> AddList(List<TAddEditDto> items)
        {
            var entities = items.Select(item => MapAddEditDtoToEntity(item)).ToList(); // Mapowanie listy DTO na listę encji
            baseRepository.AddList(entities);
            return entities.Select(e => e.Id).ToList();
        }

        public virtual bool Update(int id, TAddEditDto item)
        {
            var existingItem = baseRepository.GetById(id);
            if (existingItem == null)
                return false;

            MapAddEditDtoToEntity(item, existingItem); // Aktualizacja istniejącej encji na podstawie DTO
            baseRepository.Update(existingItem);
            return true;
        }

        public bool Delete(int id)
        {
            var existingItem = baseRepository.GetById(id);
            if (existingItem == null)
                return false;
            baseRepository.Delete(id);
            return true;
        }

        // Metoda do mapowania encji na DTO
        protected virtual TDto MapToDto(T entity)
        {
            return new TDto();
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
