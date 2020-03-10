# JsonFlatten   

Extension methods to flatten a JSON.NET `JObject` to an `IDictionary<string, object>` or vice versa.

# Usage   

https://dotnetfiddle.net/jYghhm

This:

```json
{
  "$id": "https://example.com/arrays.schema.json",
  "$schema": "http://json-schema.org/draft-07/schema#",
  "description": "A representation of a person, company, organization, or place",
  "type": "object",
  "properties": {
    "fruits": {
      "type": "array",
      "items": {
        "type": "string"
      }
    },
    "vegetables": {
      "type": "array",
      "items": { "$ref": "#/definitions/veggie" }
    }
  },
  "definitions": {
    "veggie": {
      "type": "object",
      "required": [ "veggieName", "veggieLike" ],
      "properties": {
        "veggieName": {
          "type": "string",
          "description": "The name of the vegetable."
        },
        "veggieLike": {
          "type": "boolean",
          "description": "Do I like this vegetable?"
        }
      }
    }
  }
}
```

Becomes this:

```json
{
    "$id": "https://example.com/arrays.schema.json",
    "$schema": "http://json-schema.org/draft-07/schema#",
    "description": "A representation of a person, company, organization, or place",
    "type": "object",
    "properties.fruits.type": "array",
    "properties.fruits.items.type": "string",
    "properties.vegetables.type": "array",
    "properties.vegetables.items.$ref": "#/definitions/veggie",
    "definitions.veggie.type": "object",
    "definitions.veggie.required[0]": "veggieName",
    "definitions.veggie.required[1]": "veggieLike",
    "definitions.veggie.properties.veggieName.type": "string",
    "definitions.veggie.properties.veggieName.description": "The name of the vegetable.",
    "definitions.veggie.properties.veggieLike.type": "boolean",
    "definitions.veggie.properties.veggieLike.description": "Do I like this vegetable?"
}
```

```csharp
using System;
using System.Collections.Generic;
using JsonFlatten;
using Newtonsoft.Json.Linq;

public class Program
{
	public static void Main()
	{
		var json = @"{ 'array': [ 1, 2, 3 ], 'boolean': true, 'null': null, 'number': 123, 'object': { 'a': 'b', 'c': 'd', 'e': 'f' }, 'date': '2014-01-01T23:28:56.782Z', 'string': 'Hello World', 'emptyString': '', 'emptyObject': {}, 'emptyArray': [] }";
		JObject jObj = JObject.Parse(json);
		
		// Flatten a JObject to a dictionary. var dic = jObj.Flatten(); also works.
		Dictionary<string, object> flattened = new Dictionary<string, object>(jObj.Flatten());
		
		// Retrieve and cast an item from the dictionary
		var date = flattened.Get<DateTime>("date"); // 1/1/2014 11:28:56 PM
        
      		// Unflatten a dictionary to a JObject
		JObject unflattened = flattened.Unflatten();
       		JToken.DeepEquals(jObj, unflattened); // True
		
		// Update an entry
		flattened.Set("date", date.AddDays(5));
        
        	// Try get logic for properties
       		flattened.TryGet<DateTime>("date", out var updatedDate); // updatedDate: 1/6/2014 11:28:56 PM
		
		// Flatten a JObject and exclude any properties that are null or empty e.g. have the value null, "", {}, or []
		var flattenedWithoutEmpty = jObj.Flatten(includeNullAndEmptyValues: false);
	}
}
```
