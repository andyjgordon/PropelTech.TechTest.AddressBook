using Newtonsoft.Json;

namespace PropelTech.TechTest.AddressBook.Types;

public sealed record AddressItem ([property: JsonProperty("first_name")] string FirstName, [property: JsonProperty("last_name")] string LastName, 
                                    [property: JsonProperty("phone")] string PhoneNumber, [property: JsonProperty("email")] string Email) 
    : BaseItem ();
