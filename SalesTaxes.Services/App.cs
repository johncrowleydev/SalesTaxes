using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class App : IApp
{
    private readonly ICartService _cartService;
    private readonly IInventoryService _inventoryService;
    private readonly ISalesTaxService _salesTaxService;

    public App(ICartService cartService, IInventoryService inventoryService, ISalesTaxService salesTaxService)
    {
        _cartService = cartService;
        _inventoryService = inventoryService;
        _salesTaxService = salesTaxService;
    }

    public void Run()
    {
        while (true)
        {
            MainMenu();
        }
    }

    private void AddItemsToCart()
    {
        bool addMoreItems = true;

        while (addMoreItems)
        {
            ResetConsole();
            var inventory = _inventoryService.ListInventory();
            DisplayInventoryList(inventory);
            Console.Write("Enter the number of the product you want to add: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
            {
                Console.Write("Enter the quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    _cartService.AddCartItem(new CartItemDTO(inventory[index - 1], quantity));
                    Console.WriteLine($"{quantity} of {inventory[index - 1].Name} added to cart.");
                }
                else
                {
                    Console.WriteLine("Invalid quantity. Please enter a positive number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid product number. Please try again.");
            }

            Console.Write("Would you like to add another item? (y/n): ");
            var input = Console.ReadLine();
            addMoreItems = input?.ToLower() == "y";
        }

        Console.WriteLine("Finished adding items. Press Enter to continue...");
        Console.ReadLine();
    }

    private void AddProductToInventory()
    {
        ResetConsole();
        Console.Write("Enter product name: ");
        var name = Console.ReadLine() ?? "Un-named product";

        Console.Write("Enter product price: ");
        decimal price;
        if (!decimal.TryParse(Console.ReadLine(), out price))
        {
            Console.WriteLine("Invalid price. Press Enter to return...");
            Console.ReadLine();
            return;
        }

        Console.Write("Is the product imported? (yes/no): ");
        bool isImported = Console.ReadLine()?.ToLower() == "yes";

        Console.WriteLine("Choose category:");
        Console.WriteLine("1. General");
        Console.WriteLine("2. Book");
        Console.WriteLine("3. Food");
        Console.WriteLine("4. Medical");
        var categoryChoice = Console.ReadLine();

        ProductCategory category = categoryChoice switch
        {
            "1" => ProductCategory.General,
            "2" => ProductCategory.Book,
            "3" => ProductCategory.Food,
            "4" => ProductCategory.Medical,
            _ => ProductCategory.General,
        };

        _inventoryService.CreateInventory(new ProductDTO(name, price, isImported, category));
        Console.WriteLine("Product added to inventory. Press Enter to continue...");
        Console.ReadLine();
    }

    private void Checkout()
    {
        ResetConsole();
        var cart = _cartService.ListCartItems();

        if (!cart.Any())
        {
            Console.WriteLine("Cart is empty. Press Enter to return...");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter your state: ");
        var state = Console.ReadLine();
        var isTaxFreeState = state?.StartsWith("M", StringComparison.OrdinalIgnoreCase) ?? false;

        decimal totalSalesTaxes = 0;
        decimal totalCost = 0;

        Console.WriteLine("=== Receipt ===");
        foreach (var item in cart)
        {
            decimal salesTaxPerItem = _salesTaxService.CalculateSalesTax(item.Product, isTaxFreeState);
            decimal itemTotalPricePerUnit = item.Product.Price + salesTaxPerItem;

            // Multiply by the quantity of the item
            decimal itemTotalPrice = itemTotalPricePerUnit * item.Quantity;
            decimal itemTotalSalesTax = salesTaxPerItem * item.Quantity;

            totalSalesTaxes += itemTotalSalesTax;
            totalCost += itemTotalPrice;

            Console.WriteLine($"{item.Quantity} x {item.Product.Name}: {itemTotalPrice:C2} ({item.Quantity} @ {itemTotalPricePerUnit:C2} each)");
        }

        Console.WriteLine($"Sales Taxes: {totalSalesTaxes:C2}");
        Console.WriteLine($"Total: {totalCost:C2}");

        _cartService.ClearCartItems();
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    private static void DisplayInventoryList(IList<ProductDTO> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            var product = inventory[i];
            Console.WriteLine($"{i + 1}. {product.Name} - {product.Price:C2} - {(product.IsImported ? "Imported" : "Domestic")} - {product.Category}");
        }
    }

    private void MainMenu()
    {
        ResetConsole();
        Console.WriteLine("=== MAIN MENU ===");
        Console.WriteLine("1. View Product Inventory");
        Console.WriteLine("2. Add Items to Cart");
        Console.WriteLine("3. View Cart and Checkout");
        Console.WriteLine("4. Manage Product Inventory");
        Console.WriteLine("5. Exit");
        Console.Write("Choose an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ViewInventory();
                break;
            case "2":
                AddItemsToCart();
                break;
            case "3":
                Checkout();
                break;
            case "4":
                ManageInventory();
                break;
            case "5":
                Environment.Exit(0);
                return;
            default:
                Console.WriteLine("Invalid option. Press Enter to continue...");
                Console.ReadLine();
                break;
        }
    }

    private void ManageInventory()
    {
        ResetConsole();
        Console.WriteLine("=== MANAGE INVENTORY ===");
        Console.WriteLine("1. Add Product");
        Console.WriteLine("2. Remove Product");
        Console.WriteLine("3. Return to Main Menu");
        Console.Write("Choose an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                AddProductToInventory();
                break;
            case "2":
                RemoveProduct();
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid option. Press Enter to continue...");
                Console.ReadLine();
                break;
        }
    }

    private void RemoveProduct()
    {
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        Console.Write("Enter the number of the product you want to remove: ");
        
        int index;
        if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= inventory.Count)
        {
            inventory.RemoveAt(index - 1);
            Console.WriteLine("Product removed from inventory. Press Enter to continue...");
        }
        else
        {
            Console.WriteLine("Invalid input. Press Enter to continue...");
        }
        Console.ReadLine();
    }

    private static void ResetConsole()
    {
        Console.Clear();
        Console.WriteLine(@"
   _____       _             _______                  
  / ____|     | |           |__   __|                 
 | (___   __ _| | ___  ___     | | __ ___  _____  ___ 
  \___ \ / _` | |/ _ \/ __|    | |/ _` \ \/ / _ \/ __|
  ____) | (_| | |  __/\__ \    | | (_| |>  <  __/\__ \
 |_____/ \__,_|_|\___||___/    |_|\__,_/_/\_\___||___/
                                                      
 by John Crowley
");
    }

    public void ViewInventory()
    {
        ResetConsole();
        Console.WriteLine("=== Inventory ===");
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }
}
