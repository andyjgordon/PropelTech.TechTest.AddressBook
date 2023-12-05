using FluentAssertions;
using Microsoft.Extensions.Options;
using PropelTech.TechTest.AddressBook.DataAccess;
using PropelTech.TechTest.AddressBook.Types;
using PropelTech.TechTest.AddressBook.Types.Options;

namespace PropelTech.TechTest.AddressBook.Tests;

public class RepositoryUnitTests : IDisposable
{
    private IOptions<DataOptions> _dataOptions;
    private string _path;

    public RepositoryUnitTests()
    {
        _path = $"..\\..\\..\\Data\\AddressBook_TestData-{Guid.NewGuid()}.csv";
        var options = new DataOptions()
        {
            AddressJsonFlatFilePath = _path
        };

        _dataOptions = Options.Create(options);
    }

    public void Dispose()
    {
        var numTries = 5;

        while (true)
        {
            try
            {
                File.Delete(_path);
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

    [Fact]
    public async Task CanInsertItem()
    {
        var addressItem = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.GetAll();
        items.Should().HaveCount(1);
        items.Should().Contain(addressItem);
    }

    [Fact]
    public async Task CanGetItemById()
    {
        var addressItem = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var item = repository2.GetById(addressItem.Id);
        item.Should().Be(addressItem);
    }

    [Fact]
    public async Task GetItemByIdNotExistsThrows()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        Assert.Throws<ArgumentOutOfRangeException>(() => repository2.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task CanGetAllItems()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.GetAll();
        items.Should().HaveCount(3);
        items.Should().Contain(addressItem1);
        items.Should().Contain(addressItem2);
        items.Should().Contain(addressItem3);
    }

    [Fact]
    public async Task CanUpdateItem()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        addressItem1 = addressItem1 with { FirstName = "Davide", LastName = "Platts", PhoneNumber = "011213478234", Email = "david.platt@eastenders.co.uk" };
        await repository2.Update(addressItem1);
        var items = repository2.GetAll();
        items.Should().HaveCount(1);
        items.First().FirstName.Should().Be("Davide");
        items.First().LastName.Should().Be("Platts");
        items.First().PhoneNumber.Should().Be("011213478234");
        items.First().Email.Should().Be("david.platt@eastenders.co.uk");

    }

    [Fact]
    public async Task UpdateNotExistsItemThrows()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);

        addressItem1 = addressItem1 with { Id = Guid.NewGuid() };

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository2.Update(addressItem1));
    }

    [Fact]
    public async Task DeleteNotExistsItemThrows()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository2.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task CanDeleteItem()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@corrie.co.uk");

        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.GetAll();
        items.Should().HaveCount(3);
        items.Should().Contain(addressItem1);
        items.Should().Contain(addressItem2);
        items.Should().Contain(addressItem3);

        await repository2.Delete(addressItem2.Id);

        var repository3 = new AddressJsonFlatFileRepository(_dataOptions);
        var items2 = repository3.GetAll();
        items2.Should().HaveCount(2);
        items2.Should().Contain(addressItem1);
        items2.Should().NotContain(addressItem2);
        items2.Should().Contain(addressItem3);
    }

    [Fact]
    public async Task CanSearchForItems()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@eastenders.co.uk");

        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.Search("corrie.co.uk");
        items.Should().HaveCount(2);
        items.Should().Contain(addressItem1);
        items.Should().Contain(addressItem2);
        items.Should().NotContain(addressItem3);
    }

    [Fact]
    public async Task CanSearchForSingleItemExactMatch()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@eastenders.co.uk");

        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.Search("Grimshaw");
        items.Should().HaveCount(1);
        items.Should().NotContain(addressItem1);
        items.Should().Contain(addressItem2);
        items.Should().NotContain(addressItem3);
    }

    [Fact]
    public async Task CanSearchCaseInsensitive()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@eastenders.co.uk");

        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await  repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.Search("CORRiE");
        items.Should().HaveCount(2);
        items.Should().Contain(addressItem1);
        items.Should().Contain(addressItem2);
        items.Should().NotContain(addressItem3);
    }

    [Fact]
    public async Task CanSearchNoMatch()
    {
        var addressItem1 = new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk");
        var addressItem2 = new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk");
        var addressItem3 = new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@eastenders.co.uk");

        var repository = new AddressJsonFlatFileRepository(_dataOptions);
        await repository.Insert(addressItem1);
        await repository.Insert(addressItem2);
        await repository.Insert(addressItem3);

        var repository2 = new AddressJsonFlatFileRepository(_dataOptions);
        var items = repository2.Search("neighbours");
        items.Should().HaveCount(0);
    }

    [Fact]
    public void CanLoadExistingData()
    {
        var path = "..\\..\\..\\Data\\AddressBook_TestData-Persist.json";
        var options = new DataOptions()
        {
            AddressJsonFlatFilePath = path
        };
        var dataOptions = Options.Create(options);

        var repository = new AddressJsonFlatFileRepository(dataOptions);
        var items2 = repository.GetAll();
        items2.Should().HaveCount(5);
        items2.Should().Contain(new AddressItem("David", "Platt", "01913478234", "david.platt@corrie.co.uk") with { Id = new Guid("{6B6BC0D1-953B-44F0-8C25-A69D920592D6}") });
        items2.Should().Contain(new AddressItem("Jason", "Grimshaw", "01913478123", "jason.grimshaw@corrie.co.uk") with { Id = new Guid("{1FAE6B5F-72CD-4EF3-A6FC-BE8971B4F5A0}") });
        items2.Should().Contain(new AddressItem("Ken", "Barlow", "019134784929", "ken.barlow@corrie.co.uk") with { Id = new Guid("{6DEF8A47-F659-4E0C-A8FD-FCE84C894142}") });
        items2.Should().Contain(new AddressItem("Rita", "Sullivan", "01913478555", "rita.sullivan@corrie.co.uk") with { Id = new Guid("{8B2753B5-140C-49B6-8481-0744E18DA057}") });
        items2.Should().Contain(new AddressItem("Steve", "McDonald", "01913478555", "steve.mcdonald@corrie.co.uk") with { Id = new Guid("{15E8E3AD-39F9-4FC3-BB5B-E7C130E22B0F}") });
    }
}