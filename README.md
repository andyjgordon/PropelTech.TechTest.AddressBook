# PropelTech.TechTest.AddressBook
PropelTech.TechTest.AddressBook project contains the projects to cover Step 1 and Step 2 of the Propel tech test - create an address book api, 

## Notes

I initially planned to use this tech test to try out some .NET features I hadn't used before, hence choosing .NET 8.0, though I dont think in the final project there are any explicit .NET 8 features were used. I used the record type for the first time, and also used minimal api format for the WebAPI, which is something to discuss.

Overall I've tried to demonstrate examples of a few areas for further discussion:
- Unit Testing - only tested the repository, did not test the api (which would be integration testing anyway ;)
- Extendability - made the types and repository extendable to support future features
- Error handling - used  Id not found to demonstrate a potential error and how to convert that to the relevant http status code. But plenty more exceptions could be handled.
- Dependency Injection - used to pass in options to repo and in minimal api
- Resource contention - added some retry logic while saving the JSON file, but this could be expanded and moved onto a seperate thread.

## Projects

### PropelTech.TechTest.AddressBook.Types
Contains AddressItem type definition. Inherits from a BaseItem to allow easier addition of other types in the future

### PropelTech.TechTest.AddressBook.DataAccess

Contains JSON Flat file repository for AddressItem - AddressJsonFlatFileRepository. Inherits from JsonFlatFileRepository which contains common CRUD methods which could be used for future types.

### PropelTech.TechTest.AddressBook.Tests

Unit tests for AddressJsonFlatFileRepository

### PropelTech.TechTest.AddressBook.WebAPI

WebAPI project, uses minimal api format to define the api

## Dependencies

The solution requires .NET 8.0 SDK to be installed

## Instructions

Open solution in Visual Studio, build

For Unit tests - run tests from Test Explorer

For WebAPI - set PropelTech.TechTest.AddressBook.WebAPI as startup project. Start debugger and use swagger UI (or postman or similar) to call endpoints. Comes with the Example JSON data supplied.  