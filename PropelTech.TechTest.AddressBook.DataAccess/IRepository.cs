using PropelTech.TechTest.AddressBook.Types;

namespace PropelTech.TechTest.AddressBook.DataAccess;

public interface IRepository<T> where T : BaseItem
{
    IEnumerable<T> GetAll();
    T GetById(Guid id);
    Task Insert(T obj);
    Task Delete(Guid id);
    Task Update(T obj);
    IList<T> Search(string searchString);
}
