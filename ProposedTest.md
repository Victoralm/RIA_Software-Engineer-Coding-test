# Software Engineer Coding test

Please write a small program in C# (or a preferred language of your choice) for each of the exercises

## Denomination routine

An ATM has three cartridges for different denominations:

- 10 EUR cartridge
- 50 EUR cartridge
- 100 EUR cartridge

Now we want to pay out following amounts from the ATM:

- 30 EUR
- 50 EUR
- 60 EUR
- 80 EUR
- 140 EUR
- 230 EUR
- 370 EUR
- 610 EUR
- 980 EUR

Write a program which will calculate for each payout the possible combinations which the ATM can pay out.

For example, for 100 EUR the available payout denominations would be:

- 10 x 10 EUR
- 1 x 50 EUR + 5 x 10 EUR
- 2 x 50 EUR
- 1 x 100 EUR

## REST API

A small REST API to manage customers, which should have two endpoints:

### POST customers

Request:

```json
[
  {
    firstName: 'Aaaa',
    lastName: 'Bbbb',
    age: 20,
    id: 5
  },
  {
    firstName: 'Bbbb',
    lastName: 'Cccc',
    age: 24,
    id: 6
  }
]
```

Multiple customers can be sent in one request.

The server validates every customer of the request:

- checks that every field is supplied
- validates that the age is above 18
- validates that the ID has not been used before

The server then adds each customer as an object to an internal array – the customers will not be   appended to the array but instead it will be inserted at a position so that the customers are sorted by last name and then first name WITHOUT using any available sorting functionality (an example for the inserting is in the Appendix).

The server also persists the array so it will be still available after a restart of the server.

### GET customers

Returns the array of customers with all fields

Write the server and a small simulator which can send several requests for POST customers and GET customers in parallel to the server.

For that program it is not allowed to use any sorting mechanism like array.sort().

The simulated POST customers requests have following requirements:

- Each request should contain at least 2 different customers
- Age should be randomized between 10 and 90
- ID should be increasing sequentially.
- The first names and last names of the Appendix should be used in random combinations

> **Please keep performance in mind. Additionally, if you can host your service, or deploy it on any
serverless framework, it would be a bonus.**

## Appendix

### Data

| First Name | Last Name  |
|------------|------------|
| Leia       | Liberty    |
| Sadie      | Ray        |
| Jose       | Harrison   |
| Sara       | Ronan      |
| Frank      | Drew       |
| Dewey      | Powell     |
| Tomas      | Larsen     |
| Joel       | Chan       |
| Lukas      | Anderson   |
| Carlos     | Lane       |

Example for the inserting mechanism:

Array in server:

```json
[
  { lastName: 'Aaaa', firstName: 'Aaaa', age: 20, id: 3 },
  { lastName: 'Aaaa', firstName: 'Bbbb', age: 56, id: 2 },
  { lastName: 'Cccc', firstName: 'Aaaa', age: 32, id: 5 },
  { lastName: 'Cccc', firstName: 'Bbbb', age: 50, id: 1 },
  { lastName: 'Dddd', firstName: 'Aaaa', age: 70, id: 4 },
]
```

Request POST customers:

```json
[{ lastName: 'Bbbb', firstName: 'Bbbb', age: 26, id: 6 }]
```

Array after insert:

```json
[
  { lastName: 'Aaaa', firstName: 'Aaaa', age: 20, id: 3 },
  { lastName: 'Aaaa', firstName: 'Bbbb', age: 56, id: 2 },
  { lastName: 'Bbbb', firstName: 'Bbbb', age: 26, id: 6 },
  { lastName: 'Cccc', firstName: 'Aaaa', age: 32, id: 5 },
  { lastName: 'Cccc', firstName: 'Bbbb', age: 50, id: 1 },
  { lastName: 'Dddd', firstName: 'Aaaa', age: 70, id: 4 },
]
```

Request POST customers:

```json
[{ lastName: 'Bbbb', firstName: 'Aaaa', age: 28, id: 7 }]
```

Array after insert:

```json
[
  { lastName: 'Aaaa', firstName: 'Aaaa', age: 20, id: 3 },
  { lastName: 'Aaaa', firstName: 'Bbbb', age: 56, id: 2 },
  { lastName: 'Bbbb', firstName: 'Aaaa', age: 28, id: 7 },
  { lastName: 'Bbbb', firstName: 'Bbbb', age: 26, id: 6 },
  { lastName: 'Cccc', firstName: 'Aaaa', age: 32, id: 5 },
  { lastName: 'Cccc', firstName: 'Bbbb', age: 50, id: 1 },
  { lastName: 'Dddd', firstName: 'Aaaa', age: 70, id: 4 },
]
```
