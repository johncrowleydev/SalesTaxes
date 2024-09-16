using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes;

internal class App
{
    private readonly ICartService _cartService;
    private readonly IInventoryService _inventoryService;
    private readonly ISessionService _sessionService;

    public App(ICartService cartService, IInventoryService inventoryService, ISessionService sessionService)
    {
        _cartService = cartService;
        _inventoryService = inventoryService;
        _sessionService = sessionService;
    }

    public void Run()
    {
        InitSession();
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
            Print("Enter the number of the product you want to add: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
            {
                Print("Enter the quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var item = inventory[index - 1] ?? throw new ApplicationException("Product not found.");
                    if (quantity > item.Quantity) PrintLine($"Invalid Quantity. {item.Quantity} available in stock.");
                    else
                    {
                        var product = item.Product;
                        _cartService.AddCartItem(new CartItemDTO(product, quantity));
                        PrintLine($"{quantity} of {product.Name} added to cart.");
                    }
                }
                else
                {
                    PrintLine("Invalid quantity. Please enter a positive number.", ConsoleColor.Red);
                }
            }
            else
            {
                PrintLine("Invalid product number. Please try again.", ConsoleColor.Red);
            }

            Print("Would you like to add another item? (y/n): ");
            var input = Console.ReadLine();
            addMoreItems = input?.ToLower() == "y";
        }

        PrintLine("Finished adding items. Press Enter to continue...");
        Console.ReadLine();
    }

    private void AddProductToInventory()
    {
        ResetConsole();
        Print("Enter product name: ");
        var name = Console.ReadLine() ?? "Un-named product";

        Print("Enter product price: ");
        decimal price;
        if (!decimal.TryParse(Console.ReadLine(), out price))
        {
            PrintLine("Invalid price. Press Enter to return...", ConsoleColor.Red);
            Console.ReadLine();
            return;
        }

        Print("Is the product imported? (yes/no): ");
        bool isImported = Console.ReadLine()?.ToLower() == "yes";

        PrintLine("Choose category:");
        PrintLine("1. General");
        PrintLine("2. Book");
        PrintLine("3. Food");
        PrintLine("4. Medical");
        var categoryChoice = Console.ReadLine();

        ProductCategory category = categoryChoice switch
        {
            "1" => ProductCategory.General,
            "2" => ProductCategory.Book,
            "3" => ProductCategory.Food,
            "4" => ProductCategory.Medical,
            _ => ProductCategory.General,
        };

        _inventoryService.CreateInventory(new InventoryItemDTO(new ProductDTO(name, price, isImported, category), 1));
        PrintLine("Product added to inventory. Press Enter to continue...");
        Console.ReadLine();
    }

    private void Checkout()
    {
        ResetConsole();
        var receipt = _cartService.GenerateReceipt();
        PrintLine("=== RECEIPT ===");
        foreach (var item in receipt.LineItems)
            PrintLine($"{item.Quantity} x {item.Product.Name}: {item.Total:C2} ({item.Quantity} @ {item.Subtotal:C2} + {item.SalesTaxTotal:C2} sales tax)");
        PrintLine($"Subtotal: {receipt.Subtotal:C2}");
        PrintLine($"Sales Tax: {receipt.SalesTaxTotal:C2}");
        PrintLine($"Total: {receipt.Total:C2}");
        PrintLine();
        PrintLine("Thank you for your purchase. Come again!");
        _cartService.ClearCartItems();
        PrintLine("Press Enter to return...");
        Console.ReadLine();
    }

    private static void DisplayCartList(IList<CartItemDTO> cart, bool showLineNumbers = false)
    {
        for (int i = 0; i < cart.Count; i++)
        {
            var item = cart[i];
            var product = item.Product;
            decimal itemTotalPrice = product.Price * item.Quantity;
            if (showLineNumbers) Print($"{i + 1}. ");
            PrintLine($"{item.Quantity} x {item.Product.Name}: {itemTotalPrice:C2} ({item.Quantity} @ {product.Price:C2} each)");
        }
    }

    private static void DisplayInventoryList(IList<InventoryItemDTO> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            var product = item.Product;
            PrintLine($"{i + 1}. {product.Name} - {product.Price:C2} - {(product.IsImported ? "Imported" : "Domestic")} - {product.Category} - {item.Quantity} In Stock");
        }
    }

    private void EditCartItemQuantity()
    {
        bool editMoreItems = true;

        while (editMoreItems)
        {
            ResetConsole();
            var cart = _cartService.ListCartItems();
            DisplayCartList(cart, true);
            Print("Enter the number of the item you want to edit: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= cart.Count)
            {
                Print("Enter the quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var item = cart[index - 1] ?? throw new ApplicationException("Item not found.");
                    var product = item.Product;
                    _cartService.UpdateCartItemQuantity(item.Id, quantity);
                    PrintLine($"Cart has been updated.");
                }
                else
                {
                    PrintLine("Invalid quantity. Please enter a positive number.", ConsoleColor.Red);
                }
            }
            else
            {
                PrintLine("Invalid item number. Please try again.", ConsoleColor.Red);
            }

            Print("Would you like to edit another item? (y/n): ");
            var input = Console.ReadLine();
            editMoreItems = input?.ToLower() == "y";
        }

        PrintLine("Finished editing items. Press Enter to continue...");
        Console.ReadLine();
    }

    private void EditProductDetails()
    {
        ResetConsole();
        PrintLine("=== EDIT PRODUCT DETAILS ===");
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        Print("Enter the number of the product you want to edit: ");

        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
        {
            var existingItem = inventory[index - 1];
            if (existingItem == null) throw new ApplicationException("");

            // Prompt user for new product details
            PrintLine($"Current Name: {existingItem.Product.Name}");
            Print("Enter new name (or press Enter to keep the current name): ");
            var newName = Console.ReadLine();
            newName = string.IsNullOrEmpty(newName) ? existingItem.Product.Name : newName;

            PrintLine($"Current Price: {existingItem.Product.Price:C}");
            Print("Enter new price (or press Enter to keep the current price): ");
            var priceInput = Console.ReadLine();
            decimal newPrice = string.IsNullOrEmpty(priceInput) ? existingItem.Product.Price : decimal.Parse(priceInput);

            PrintLine($"Is Imported (true/false): {existingItem.Product.IsImported}");
            Print("Enter 'true' or 'false' to update import status (or press Enter to keep the current value): ");
            var importInput = Console.ReadLine();
            bool isImported = string.IsNullOrEmpty(importInput) ? existingItem.Product.IsImported : bool.Parse(importInput);

            PrintLine($"Current Category: {existingItem.Product.Category}");
            PrintLine("Choose new category (0 = General, 1 = Food, 2 = Medical, 3 = Book) or press Enter to keep current:");
            var categoryInput = Console.ReadLine();
            var newCategory = string.IsNullOrEmpty(categoryInput)
                ? existingItem.Product.Category
                : (ProductCategory)Enum.Parse(typeof(ProductCategory), categoryInput);

            PrintLine($"Current Quantity: {existingItem.Quantity}");
            Print("Enter new quantity (or press Enter to keep the current quantity): ");
            var quantityInput = Console.ReadLine();
            int newQuantity = string.IsNullOrEmpty(quantityInput) ? existingItem.Quantity : int.Parse(quantityInput);

            // Create the updated product and inventory item
            var updatedProduct = new ProductDTO(newName, newPrice, isImported, newCategory, existingItem.Product.Id);
            var updatedItem = new InventoryItemDTO(updatedProduct, newQuantity, existingItem.Id);

            // Update the inventory
            try
            {
                _inventoryService.UpdateInventoryItem(updatedItem);
                PrintLine("Product details updated successfully.");
            }
            catch (ArgumentException ex)
            {
                PrintLine($"Error: {ex.Message}", ConsoleColor.Red);
            }

            PrintLine("Press Enter to continue...");
            Console.ReadLine();
        }
        else
        {
            PrintLine("Invalid product number. Please try again.", ConsoleColor.Red);
        }
    }

    private void InitSession()
    {
        PrintLine("\n=== LOGIN ===\n");

        string? username;
        do
        {
            Print("Enter a username to login: ");
            username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                PrintLine("Username cannot be empty. Please try again.", ConsoleColor.Red);
            }
        } while (string.IsNullOrWhiteSpace(username));

        string? state;
        do
        {
            Print("Enter your state: ");
            state = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(state))
            {
                PrintLine("State cannot be empty. Please try again.", ConsoleColor.Red);
            }
        } while (string.IsNullOrWhiteSpace(state));

        _sessionService.SetCurrentUser(new UserDTO(username, state));
    }

    private void MainMenu()
    {
        ResetConsole();
        PrintLine("=== MAIN MENU ===");
        PrintLine("1. View Product Inventory");
        PrintLine("2. Add Items to Cart");
        PrintLine("3. View/Edit Cart and Checkout");
        PrintLine("4. Manage Product Inventory");
        PrintLine("5. Exit");
        Print("\nChoose an option: ");
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
                ViewOrEditCart();
                break;
            case "4":
                ManageInventory();
                break;
            case "5":
                Environment.Exit(0);
                return;
            default:
                PrintLine("Invalid option. Press Enter to continue...", ConsoleColor.Red);
                Console.ReadLine();
                break;
        }
    }

    private void ManageInventory()
    {
        ResetConsole();
        PrintLine("=== MANAGE INVENTORY ===");
        PrintLine("1. Add Product");
        PrintLine("2. Remove Product");
        PrintLine("3. Edit Product Details");
        PrintLine("4. Return to Main Menu");
        Print("\nChoose an option: ");
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
                EditProductDetails();
                break;
            case "4":
                return;
            default:
                PrintLine("Invalid option. Press Enter to continue...", ConsoleColor.Red);
                Console.ReadLine();
                break;
        }
    }

    private static void Print(string message = "", ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    private static void PrintLine(string message = "", ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void RemoveCartItem()
    {
        ResetConsole();
        var cart = _cartService.ListCartItems();
        DisplayCartList(cart, true);
        Print("Enter the number of the item you want to remove: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= cart.Count)
        {
            var item = cart[index - 1];
            _cartService.RemoveCartItem(item.Id);
            PrintLine("Cart has been updated.");
        }
        else
        {
            PrintLine("Invalid item number. Please try again.", ConsoleColor.Red);
        }
        PrintLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private void RemoveProduct()
    {
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        Print("Enter the number of the product you want to remove: ");

        int index;
        if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= inventory.Count)
        {
            inventory.RemoveAt(index - 1);
            PrintLine("Product removed from inventory. Press Enter to continue...");
        }
        else
        {
            PrintLine("Invalid input. Press Enter to continue...", ConsoleColor.Red);
        }
        Console.ReadLine();
    }

    private void ResetConsole()
    {
        Console.Clear();
        PrintLine(@"
   _____       _             _______                  
  / ____|     | |           |__   __|                 
 | (___   __ _| | ___  ___     | | __ ___  _____  ___ 
  \___ \ / _` | |/ _ \/ __|    | |/ _` \ \/ / _ \/ __|
  ____) | (_| | |  __/\__ \    | | (_| |>  <  __/\__ \
 |_____/ \__,_|_|\___||___/    |_|\__,_/_/\_\___||___/
                                                      
 by John Crowley
", ConsoleColor.Yellow);

        var cartItemsCount = _cartService.GetCartItemsCount();
        var inventoryProductCount = _inventoryService.GetInventoryProductCount();
        var inventoryTotalCount = _inventoryService.GetInventoryTotalCount();
        var currentUser = _sessionService.GetCurrentUser();
        PrintLine($"Welcome, {currentUser.Name} in {currentUser.State}!", ConsoleColor.Blue);
        PrintLine($"Store has {inventoryProductCount} products and {inventoryTotalCount} total items in inventory.", ConsoleColor.Green);
        PrintLine($"You have {cartItemsCount} items in your cart.\n", ConsoleColor.Green);
    }

    public void ViewInventory()
    {
        ResetConsole();
        PrintLine("=== Inventory ===");
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        PrintLine("Press Enter to return...");
        Console.ReadLine();
    }

    public void ViewOrEditCart()
    {
        ResetConsole();
        var cart = _cartService.ListCartItems();

        if (!cart.Any())
        {
            PrintLine("Cart is empty. Press Enter to return...");
            Console.ReadLine();
            return;
        }

        PrintLine("=== CART ===");
        DisplayCartList(cart);
        PrintLine();
        PrintLine("1. Checkout");
        PrintLine("2. Edit Item Quantity");
        PrintLine("3. Remove Item");
        PrintLine("4. Return to Main Menu");
        Print("\nChoose an option: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Checkout();
                break;
            case "2":
                EditCartItemQuantity();
                break;
            case "3":
                RemoveCartItem();
                break;
            case "4":
                return;
            default:
                PrintLine("Invalid option. Press Enter to continue...", ConsoleColor.Red);
                Console.ReadLine();
                break;
        }

    }
}
