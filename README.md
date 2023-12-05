# PropelTech.TechTest.AddressBook
PropelTech.TechTest.AddressBook project contains the projects to cover Step 1 of the Propel tech test - create an address book api.

## Projects

### PropelTech.TechTest.AddressBook.Types
Contains AddressItem type definition. Inherits from a BaseItem to allow easier addition of other types in the future

### PropelTech.TechTest.AddressBook.DataAccess

Contains JSON Flat file repository for AddressItem - AddressJsonFlatFileRepository. Inherits from JsonFlatFileRepository which contains common CRUD methods which could be used for future types.

### PropelTech.TechTest.AddressBook.Tests

Unit tests for AddressJsonFlatFileRepository

## Dependencies

The solution requires .NET 8.0 SDK to be installed

## Instructions

Open solution in Visual Studio, build, runs tests from Test Explorer