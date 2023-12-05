using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PropelTech.TechTest.AddressBook.Types;

namespace PropelTech.TechTest.AddressBook.DataAccess;

public abstract class JsonFlatFileRepository<T> : IRepository<T> where T : BaseItem
{
    private readonly string _path;
    protected SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    protected IList<T>? Data { private set; get; }

    public JsonFlatFileRepository(string path )
    {
        _path = path;

        //would move this from constructor and maybe put in an init method, trigged on first read / write?
        if (!File.Exists(_path))
        {
            File.WriteAllText(_path, "{}");
            Data = new List<T>();
        }
        else
        {
            Data = JsonConvert.DeserializeObject<IList<T>>(File.ReadAllText(_path));
        }
    }

    public async Task Delete(Guid id)
    {
        await _semaphore.WaitAsync();

        try
        {
            var item = Data.FirstOrDefault(d => d.Id == id) ?? 
                    throw new ArgumentOutOfRangeException(nameof(id), id, "Item not found");

            Data.Remove(item);
            await Save();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerable<T> GetAll()
    {
        return Data.AsEnumerable();
    }

    public T GetById(Guid id)
    {
        var item = Data.FirstOrDefault(d => d.Id == id);

        return item == null ? throw new ArgumentOutOfRangeException(nameof(id), id, "Item not found") : item;
    }

    public async Task Insert(T obj)
    {
        await _semaphore.WaitAsync();

        try
        {
            obj.Id = Guid.NewGuid();
            Data.Add(obj);
            await Save();                
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected async Task Save()
    {
        var numTries = 5;

        while (true)
        {
            try
            {
                var json = JsonConvert.SerializeObject(Data);
                await File.WriteAllTextAsync(_path, json);
                break;
            }
            catch (IOException e) 
            when (e.Message.Contains("being used by another process"))
            {
                numTries--;
                if (numTries == 0)
                    throw;

                Thread.Sleep(1000);
            }
        }
    }

    public abstract Task Update(T obj);

    public abstract IList<T> Search(string searchString);
}
