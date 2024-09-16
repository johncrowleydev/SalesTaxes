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
            Console.Write("Enter the number of the product you want to add: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
            {
                Console.Write("Enter the quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var item = inventory[index - 1] ?? throw new ApplicationException("Product not found.");
                    var product = item.Product;
                    _cartService.AddCartItem(new CartItemDTO(product, quantity));
                    Console.WriteLine($"{quantity} of {product.Name} added to cart.");
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

        _inventoryService.CreateInventory(new InventoryItemDTO(new ProductDTO(name, price, isImported, category), 1));
        Console.WriteLine("Product added to inventory. Press Enter to continue...");
        Console.ReadLine();
    }

    private void Checkout()
    {
        ResetConsole();
        var receipt = _cartService.GenerateReceipt();
        Console.WriteLine("=== RECEIPT ===");
        foreach (var item in receipt.LineItems)
            Console.WriteLine($"{item.Quantity} x {item.Product.Name}: {item.Total:C2} ({item.Quantity} @ {item.Subtotal:C2} + {item.SalesTaxTotal:C2} sales tax)");
        Console.WriteLine($"Subtotal: {receipt.Subtotal:C2}");
        Console.WriteLine($"Sales Tax: {receipt.SalesTaxTotal:C2}");
        Console.WriteLine($"Total: {receipt.Total:C2}");
        Console.WriteLine();
        Console.WriteLine("Thank you for your purchase. Come again!");
        _cartService.ClearCartItems();
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    private static void DisplayCartList(IList<CartItemDTO> cart, bool showLineNumbers = false)
    {
        for (int i = 0; i < cart.Count; i++)
        {
            var item = cart[i];
            var product = item.Product;
            decimal itemTotalPrice = product.Price * item.Quantity;
            if (showLineNumbers) Console.Write($"{i + 1}. ");
            Console.WriteLine($"{item.Quantity} x {item.Product.Name}: {itemTotalPrice:C2} ({item.Quantity} @ {product.Price:C2} each)");
        }
    }

    private static void DisplayInventoryList(IList<InventoryItemDTO> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            var product = item.Product;
            Console.WriteLine($"{i + 1}. {product.Name} - {product.Price:C2} - {(product.IsImported ? "Imported" : "Domestic")} - {product.Category} - {item.Quantity} In Stock");
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
            Console.Write("Enter the number of the item you want to edit: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= cart.Count)
            {
                Console.Write("Enter the quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var item = cart[index - 1] ?? throw new ApplicationException("Item not found.");
                    var product = item.Product;
                    _cartService.UpdateCartItemQuantity(item.Id, quantity);
                    Console.WriteLine($"Cart has been updated.");
                }
                else
                {
                    Console.WriteLine("Invalid quantity. Please enter a positive number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid item number. Please try again.");
            }

            Console.Write("Would you like to edit another item? (y/n): ");
            var input = Console.ReadLine();
            editMoreItems = input?.ToLower() == "y";
        }

        Console.WriteLine("Finished editing items. Press Enter to continue...");
        Console.ReadLine();
    }

    private void EditProductDetails()
    {
        ResetConsole();
        Console.WriteLine("=== EDIT PRODUCT DETAILS ===");
        var inventory = _inventoryService.ListInventory();
        DisplayInventoryList(inventory);
        Console.Write("Enter the number of the product you want to edit: ");

        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
        {
            var existingItem = inventory[index - 1];
            if (existingItem == null) throw new ApplicationException("");

            // Prompt user for new product details
            Console.WriteLine($"Current Name: {existingItem.Product.Name}");
            Console.Write("Enter new name (or press Enter to keep the current name): ");
            var newName = Console.ReadLine();
            newName = string.IsNullOrEmpty(newName) ? existingItem.Product.Name : newName;

            Console.WriteLine($"Current Price: {existingItem.Product.Price:C}");
            Console.Write("Enter new price (or press Enter to keep the current price): ");
            var priceInput = Console.ReadLine();
            decimal newPrice = string.IsNullOrEmpty(priceInput) ? existingItem.Product.Price : decimal.Parse(priceInput);

            Console.WriteLine($"Is Imported (true/false): {existingItem.Product.IsImported}");
            Console.Write("Enter 'true' or 'false' to update import status (or press Enter to keep the current value): ");
            var importInput = Console.ReadLine();
            bool isImported = string.IsNullOrEmpty(importInput) ? existingItem.Product.IsImported : bool.Parse(importInput);

            Console.WriteLine($"Current Category: {existingItem.Product.Category}");
            Console.WriteLine("Choose new category (0 = General, 1 = Food, 2 = Medical, 3 = Book) or press Enter to keep current:");
            var categoryInput = Console.ReadLine();
            var newCategory = string.IsNullOrEmpty(categoryInput)
                ? existingItem.Product.Category
                : (ProductCategory)Enum.Parse(typeof(ProductCategory), categoryInput);

            Console.WriteLine($"Current Quantity: {existingItem.Quantity}");
            Console.Write("Enter new quantity (or press Enter to keep the current quantity): ");
            var quantityInput = Console.ReadLine();
            int newQuantity = string.IsNullOrEmpty(quantityInput) ? existingItem.Quantity : int.Parse(quantityInput);

            // Create the updated product and inventory item
            var updatedProduct = new ProductDTO(newName, newPrice, isImported, newCategory, existingItem.Product.Id);
            var updatedItem = new InventoryItemDTO(updatedProduct, newQuantity, existingItem.Id);

            // Update the inventory
            try
            {
                _inventoryService.UpdateInventoryItem(updatedItem);
                Console.WriteLine("Product details updated successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("Invalid product number. Please try again.");
        }
    }

    private void InitSession()
    {
        Console.WriteLine("\n=== LOGIN ===\n");

        string? username;
        do
        {
            Console.Write("Enter a username to login: ");
            username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty. Please try again.");
            }
        } while (string.IsNullOrWhiteSpace(username));

        string? state;
        do
        {
            Console.Write("Enter your state: ");
            state = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(state))
            {
                Console.WriteLine("State cannot be empty. Please try again.");
            }
        } while (string.IsNullOrWhiteSpace(state));

        _sessionService.SetCurrentUser(new UserDTO(username, state));
    }

    private void MainMenu()
    {
        ResetConsole();
        Console.WriteLine("=== MAIN MENU ===");
        Console.WriteLine("1. View Product Inventory");
        Console.WriteLine("2. Add Items to Cart");
        Console.WriteLine("3. View/Edit Cart and Checkout");
        Console.WriteLine("4. Manage Product Inventory");
        Console.WriteLine("5. Exit");
        Console.Write("\nChoose an option: ");
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
        Console.WriteLine("3. Edit Product Details");
        Console.WriteLine("4. Return to Main Menu");
        Console.Write("\nChoose an option: ");
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
                Console.WriteLine("Invalid option. Press Enter to continue...");
                Console.ReadLine();
                break;
        }
    }

    private void RemoveCartItem()
    {
        ResetConsole();
        var cart = _cartService.ListCartItems();
        DisplayCartList(cart, true);
        Console.Write("Enter the number of the item you want to remove: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= cart.Count)
        {
            var item = cart[index - 1];
            _cartService.RemoveCartItem(item.Id);
            Console.WriteLine("Cart has been updated.");
        }
        else
        {
            Console.WriteLine("Invalid item number. Please try again.");
        }
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
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

    private void ResetConsole()
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

        var cartItemsCount = _cartService.GetCartItemsCount();
        var inventoryProductCount = _inventoryService.GetInventoryProductCount();
        var inventoryTotalCount = _inventoryService.GetInventoryTotalCount();
        var currentUser = _sessionService.GetCurrentUser();
        Console.WriteLine($"Welcome, {currentUser.Name} in {currentUser.State}!");
        Console.WriteLine($"Store has {inventoryProductCount} products and {inventoryTotalCount} total items in inventory.");
        Console.WriteLine($"You have {cartItemsCount} items in your cart.\n");
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

    public void ViewOrEditCart()
    {
        ResetConsole();
        var cart = _cartService.ListCartItems();

        if (!cart.Any())
        {
            Console.WriteLine("Cart is empty. Press Enter to return...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("=== CART ===");
        DisplayCartList(cart);
        Console.WriteLine();
        Console.WriteLine("1. Checkout");
        Console.WriteLine("2. Edit Item Quantity");
        Console.WriteLine("3. Remove Item");
        Console.WriteLine("4. Return to Main Menu");
        Console.Write("\nChoose an option: ");
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
                Console.WriteLine("Invalid option. Press Enter to continue...");
                Console.ReadLine();
                break;
        }

    }
}
