# ThunderWings Readme

Example data used can be found in `ThunderWings/Resources/Aircraft.json`.

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

Gets a list of aircraft. Page and PageSize are integer values used to denote the page index and maximum values per page respectively.

In the body, SortBy denotes the property to sort the results by. Filter options is a dictionary, 
with the key being the property you would like to filter by and the value being what the property must contain
e.g. `"country": "america"`.
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
NOTE: If you have a mixture of valid Ids and invalid Ids, all valid Ids should be added to the basket, 
while invalid Ids will be noted on the failed to add property.

---
### Method: POST
*api/v1/basket/remove*

Use to remove items from basket.

The body is a dictionary where the key is the Id of the aircraft you wish to remove from the basket
and the value is a bool to signify whether you wish to remove all instances of an aircraft from the basket (`true`)
or just one instance (`false`).

### Input Schema
```json
{
  "itemsToRemove": {
    "19": true,      
    "2": false      
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
  "failedToRemove": "Failed to remove Ids: 2, \nBasket did not contain elements with these ids"
}
```
NOTE: Much like the add to basket endpoint, items that cannot be removed from the basket are noted by the failed to remove 
property.

---
### Method: GET
*api/v1/basket/view?page={REQUESTED PAGE}&pageSize={MAX VALUES PER PAGE}*

Returns all elements in the current basket. Results are paginated in the same way as the `aircraftfiltered` endpoint in
the Aircraft Controller.

### Example Response
Request: `api/v1/Basket/view?page=1&pageSize=5`
```json
{
  "basketItems": [
    {
      "id": 4,
      "aircraftId": 19
    },
    {
      "id": 3,
      "aircraftId": 19
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