# Sales Taxes Console App

This is a C# console application that calculates sales taxes for purchased products. The app applies different tax rules based on the product type and location (state). It supports managing a list of products in an inventory, and users can add items to a cart and get a receipt that displays the total cost, sales taxes, and itemized prices. Additionally, the app handles tax-free states and import duty calculations.

## Problem Description

The application is designed to handle sales taxes in a shopping scenario:

- **Basic Sales Tax**: 10% applied on all general goods except books, food, and medical products.
- **Import Duty**: 5% additional tax on imported goods.
- **Rounding Rule**: Sales tax is rounded up to the nearest 0.05.
- **Tax-Free States**: States starting with the letter \"M\" (like Montana) do not charge sales tax.

## Features

1. **Sales Tax Calculation**: Automatically calculates applicable taxes for each item.
2. **Itemized Receipt**: Displays all items with their taxed prices, total cost, and total sales taxes.
3. **Inventory Management**: Add, remove, or update products in the inventory.
4. **Cart Management**: Add items to the shopping cart and calculate totals.
5. **Tax-Free State Support**: Allows users to select a state that exempts them from sales tax.
6. **Import Duty Handling**: Automatically applies import duty for imported products.

## Concepts Demonstrated

This application is intended to demonstrate familiarity with basic programming concepts such as variables, data types, and flow of control, object-oriented programming concepts such as abstraction and encapsulation, application architecture design, and ability to deliver a working solution from a brief or specification document.

## Projects

The SalesTaxes application solution contains the following projects:

- **SalesTaxes**: The main console application, which contains the code for the user interface.
- **SalesTaxes.Configuration**: Class library containing an extension method to IServiceCollection to register services with the DI container.
- **SalesTaxes.Contracts.Services**: Class library containing the abstractions for the service layer.
- **SalesTaxes.Services**: Class library containing the service implementations.
- **SalesTaxes.Common**: Class library containing types shared throughout the application such as DTOs and Enums.
- **SalesTaxes.Tests**: xUnit test project containing tests for each of the service classes.

## Getting Started

### Prerequisites

Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/) or another C# IDE
- NuGet packages for xUnit testing and mocking

### Installation

1. Clone the repository:

```bash
git clone https://github.com/johncrowleydev/SalesTaxes.git
```

2. Navigate to the project directory:

```bash
cd SalesTaxes
```

3. Build the solution:

```bash
dotnet build
```

4. Run the console application:

```bash
dotnet run --project SalesTaxApp
```

Alternatively, you can build, run, and debug the solution from Visual Studio.

### Running Unit Tests

To run the unit tests, execute the following command from the root directory of the project:

```bash
dotnet test
```

Alternatively, you can run the tests from Visual Studio.