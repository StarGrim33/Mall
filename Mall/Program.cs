using System.Xml.Linq;

namespace Mall
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string CommandServeCustomer = "1";
            const string CommandExit = "2";

            bool isProgramOn = true;

            Mall mall = new("Геймер", "Магазин видеоигр, приставок и аксессуаров", "г. Владимир, ул. Юбилейная, д. 52");
            Queue<Client> clients = new(8);
            Random random = new Random();
            List<string> names = new List<string> { "Иван", "Александр", "Влад", "Алексей", "Михаил", "Анатолий", "Дмитрий", "Олег" };
            List<int> money = new List<int> { 500, 4000 };

            foreach (string name in names)
            {
                clients.Enqueue(new Client(name, random.Next(money[0], money[1])));
            }

            while (clients.Count > 0 || isProgramOn)
            {
                Console.WriteLine($"Добрый день.\nВ магазине очередь из {names.Count} человек");
                Console.WriteLine($"{CommandServeCustomer}-Обслужить покупателей");
                Console.WriteLine($"{CommandExit}-Выйти");

                string? userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandServeCustomer:
                        mall.Sell(clients);
                        break;
                    case CommandExit:
                        isProgramOn = false;
                        break;
                    default:
                        Console.WriteLine("Введите цифру меню");
                        break;
                }
            }
        }

    }

    class Mall
    {
        private List<Stack> _stack = new();

        public Mall(string name, string description, string adress)
        {
            Name = name;
            Description = description;
            Adress = adress;
            _stack = new List<Stack>
            {
                new Stack(new Product("Mount and Blade", 500), 5),
                new Stack(new Product("Dishonored", 400), 12),
                new Stack(new Product("Total War: Warhammer", 1000), 6),
                new Stack(new Product("Ori", 500), 15),
                new Stack(new Product("The Callisto Protocol", 4000), 6),
                new Stack(new Product("Assassin`s Creed: Valhalla", 3000), 8),
                new Stack(new Product("God of War: Ragnarok", 4500), 7),
                new Stack(new Product("The Last of Us: Part 2", 2300), 10),
            };
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Adress { get; private set; }

        public void Sell(Queue<Client> clients) 
        {
            var client = clients.Peek();

            while(client.Money > 0)
            {
                Console.Clear();
                Console.WriteLine($"Покупатель: {client.Name}, Баланс: {client.Money}");
                Console.WriteLine("Ассортимент товаров: ");
                ShowProducts();

                if (TryTakeProduct(out Stack stack))
                {
                    client.Buy(stack);
                    Console.WriteLine("Покупка успешна");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Ошибка");
                }
            }
            
        }

        private void ShowProducts()
        {
            for(int i = 0; i < _stack.Count; i++)
            {
                var stack = _stack[i];
                Console.Write((i+1) + " ");
                stack.ShowInfo();
                Console.WriteLine();
            }
        }

        private bool TryTakeProduct(out Stack stack)
        {
            stack = null;

            Console.WriteLine("Выберите товар: ");

            if (int.TryParse(Console.ReadLine(), out int numberProduct) == false)
            {
                Console.WriteLine("Ошибка");
                return false;
            }

            if (numberProduct < 0 || numberProduct >= _stack.Count)
            {
                Console.WriteLine("Такого товара нет");
                return false;
            }

            Console.WriteLine("Выберите количество товара: ");

            if (int.TryParse(Console.ReadLine(), out int quantity) == false)
            {
                Console.WriteLine("Нужно ввести число.");
                return false;
            }

            if (_stack[numberProduct].TryGetProduct(out stack, quantity) == false)
            {
                Console.WriteLine("Недостаточно количество");
                return false;
            }

            return true;
        }
    }

    class Product
    {
        public Product(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        public string Name { get; private set; }
        public int Cost { get; private set; }

        
    }

    class Client
    {
        private List<Stack> _cart = new List<Stack>();

        public Client(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; private set; }
        public int Money { get; private set; }

        public void ShowCart()
        {
            foreach (Stack stack in _cart)
            {
                stack.ShowInfo();
            }
        }

        public void Buy(Stack stack)
        {
            foreach (Stack currentStack in _cart)
            {
                if (currentStack.Product == stack.Product)
                {
                    currentStack.AddQuantity(stack.Quantity);
                    Money -= stack.Product.Cost;
                    return;
                }
            }

            _cart.Add(stack);
        }
    }

    class Stack
    {
        private List<Product> _stack = new();

        public Stack(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Product Product { get; }
        public int Quantity { get; private set; }

        public bool TryGetProduct(out Stack stack, int quantity)
        {
            stack = null;

            if (quantity < 0)
            {
                return false;
            }

            if (quantity > Quantity)
            {
                return false;
            }

            Quantity -= quantity;
            stack = new Stack(Product, quantity);
            return true;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{Product.Name}, цена: {Product.Cost}, количество: {Quantity}");
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }
    }
}