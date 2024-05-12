# ThunderWings Readme

For this project I decided to use an in memory database along with a simple entity framework implementation as my means 
of persisting and accessing data. This was primarily done for ease of use, but also has the added benefit of keeping my
whole solution within the C#/.NET ecosystem.

Example data used can be found in `ThunderWings/Resources/Aircraft.json`

## *Aircraft Controller*

## Endpoints

### Method: POST
*api/v1/aircraft/add*

Adds a list of one or more aircrafts to the in memory database.

### Input Schema
```json
[
  {
    "name": "string",
    "manufacturer": "string",
    "country": "string",
    "role": "string",
    "topSpeed": 0,
    "price": 0
  }
]
```

---
### Method: POST
*api/v1/aircraft/aircraftfiltered?page={REQUESTED PAGE}&pageSize={MAX VALUES PER PAGE}*

Gets a list of aircraft. Page and PageSize are integer values used to denote the page index and maximum values per page respectively

In the body, SortBy denotes the property to sort the results by. Filter options is a dictionary, 
with the key being the property you would like to filter by and the value being what the property must contain
e.g. `"country": "america"`
### Input Schema
```json
{
  "sortBy": "propertyToSortBy",
  "filterOptions": {
    "filterProperty": "filterValue"
  }
}
```
---

---
## *Basket Controller*
## Endpoints

### Method: POST
*api/v1/basket/add*

Submit a list of one or more ids of aircraft to add to the basket. Use the `aircraftfiltered` endpoint from the 
Aircraft Controller find the aircraft you want and use those ids. 

### Input Schema
```json
[
  3,
  30
]
```
### Example Response
```json
{
  "addedAircraft": [
    {
      "id": 3,
      "name": "Sukhoi Su-35",
      "manufacturer": "Sukhoi",
      "country": "Russia",
      "role": "Multirole fighter",
      "topSpeed": 2400,
      "price": 85000000
    }
  ],
  "failedToAdd": "Failed to add Id(s): 30, "
}
```

---
### Method: POST
*api/v1/basket/remove*

Use to remove items from basket

The body is a dictionary where the key is the Id of the aircraft you wish to remove from the basket
and the value is a bool to signify whether you wish to remove all instances of an aircraft from the basket (`true`)
or just one instance (`false`).

### Input Schema
```json
{
  "itemsToRemove": {
    "1": true,      // removes all instances of 1 from basket
    "2": false      // removes only one instance of 2 from basket
  }
}
```
### Example Responses
Success:
```json
{
  "removedAircraftIds": [
    19,
    19
  ],
  "failedToRemove": null
}
```
Failure:
```json
{
  "removedAircraftIds": [],
  "failedToRemove": "Failed to remove Ids: 19, \nBasket did not contain elements with these ids"
}
```

---
### Method: GET
*api/v1/basket/view?page={REQUESTED PAGE}&pageSize={MAX VALUES PER PAGE}*

Returns all elements in the current basket. Results are paginated in the same way as the `aircraftfiltered` endpoint in
the Aircraft Controller.

### Example Response
Request: `api/v1/Basket/view?page=1&pageSize=10`
```json
{
  "basketItems": [
    {
      "id": 2,
      "aircraftId": 1
    },
    {
      "id": 3,
      "aircraftId": 2
    },
    {
      "id": 8,
      "aircraftId": 18
    },
    {
      "id": 7,
      "aircraftId": 3
    },
    {
      "id": 10,
      "aircraftId": 14
    },
    {
      "id": 11,
      "aircraftId": 5
    },
    {
      "id": 12,
      "aircraftId": 5
    },
    {
      "id": 13,
      "aircraftId": 5
    }
  ],
  "currentPage": 1,
  "totalPages": 1
}
```

--- 
### Method: POST
*api/v1/basket/checkout*

Checks out the basket. Returns an invoice with a list of purchased aircraft and a total price

### Example Responses
Success:
```json
{
  "purchasedAircrafts": [
    {
      "id": 1,
      "name": "F-22 Raptor",
      "manufacturer": "Lockheed Martin",
      "country": "United States of America",
      "role": "Stealth air superiority fighter",
      "topSpeed": 1498,
      "price": 150000000
    }
  ],
  "totalPrice": 150000000,
  "message": null
}
```
Failure:
```json
{
  "purchasedAircrafts": null,
  "totalPrice": 0,
  "message": "No items in basket"
}
```