using Microsoft.Extensions.Options;
using PropelTech.TechTest.AddressBook.Types;
using PropelTech.TechTest.AddressBook.Types.Options;

namespace PropelTech.TechTest.AddressBook.DataAccess;

public class AddressJsonFlatFileRepository : JsonFlatFileRepository<AddressItem>
{
    public AddressJsonFlatFileRepository(IOptions<DataOptions> options) 
        : base(options.Value.AddressJsonFlatFilePath)
    {
    }

    public override async Task Update(AddressItem addressItem)
    {
        await _semaphore.WaitAsync();

        try
        {
            var item = Data.FirstOrDefault(d => d.Id == addressItem.Id);
            if (item != null)
            {
                var index = Data.IndexOf(item);
                var updateItem = item with { FirstName = addressItem.FirstName, 
                                            LastName = addressItem.LastName, 
                                            PhoneNumber = addressItem.PhoneNumber, 
                                            Email = addressItem.Email };

                Data[index] = updateItem;
                await Save();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(addressItem.Id), addressItem.Id, "Item not found");
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public override IList<AddressItem> Search(string searchString)
    {
        return Data.Where( d =>  d.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || d.LastName.ToLower().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || d.Email.ToLower().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || d.PhoneNumber.ToLower().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
