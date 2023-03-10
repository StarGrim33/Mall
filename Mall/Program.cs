namespace Mall
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Mall mall = new("Геймер", "Магазин видеоигр, приставок и аксессуаров", "г. Владимир, ул. Юбилейная, д. 52");
            Console.WriteLine($"Приветствуем Вас в магазине {mall.Name}!");
            mall.StartDay();
        }
    }

    class Mall
    {
        private List<Stack> _stacks = new();
        private Queue<Client> _clients = new(8);

        public Mall(string name, string description, string adress)
        {
            Name = name;
            Description = description;
            Adress = adress;

            _stacks = new List<Stack>
            {
                new Stack(new Product("Mount and Blade", 500)),
                new Stack(new Product("Dishonored", 400)),
                new Stack(new Product("Total War: Warhammer", 1000)),
                new Stack(new Product("Ori", 500)),
                new Stack(new Product("The Callisto Protocol", 4000)),
                new Stack(new Product("Assassin`s Creed: Valhalla", 3000)),
                new Stack(new Product("God of War: Ragnarok", 4500)),
                new Stack(new Product("The Last of Us: Part 2", 2300)),
            };

            _clients.Enqueue(new Client("Иван", 500));
            _clients.Enqueue(new Client("Александр", 1000));
            _clients.Enqueue(new Client("Влад", 1500));
            _clients.Enqueue(new Client("Михаил", 2000));
            _clients.Enqueue(new Client("Анатолий", 2500));
            _clients.Enqueue(new Client("Дмитрий", 3000));
            _clients.Enqueue(new Client("Олег", 3500));
            _clients.Enqueue(new Client("Роман", 4000));
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Adress { get; private set; }

        public void StartDay()
        {
            const string CommandServeCustomer = "1";
            const string CommandExit = "2";

            bool isProgramOn = true;

            while (_clients.Count > 0 && isProgramOn)
            {
                Console.WriteLine($"Добрый день.\nВ магазине очередь из {_clients.Count} человек");
                Console.WriteLine($"{CommandServeCustomer}-Обслужить покупателя");
                Console.WriteLine($"{CommandExit}-Выйти");

                string? userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandServeCustomer:
                        ServeClient();
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

        private void ServeClient()
        {
            var client = _clients.Peek();
            bool isClientOn = true;

            while (isClientOn)
            {
                const string CommandBuyProduct = "1";
                const string CommandEndPurchase = "2";

                Console.Clear();
                Console.WriteLine($"Покупатель: {client.Name}, Баланс: {client.Money}");
                client.ShowCart();
                Console.WriteLine("Ассортимент товаров: ");
                ShowProducts();
                Console.WriteLine($"Выберите команду:\n{CommandBuyProduct}-Купить товары\n{CommandEndPurchase}-Закончить");

                string? userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandBuyProduct:
                        Sell();
                        break;

                    case CommandEndPurchase:
                        TryEndPurchase();
                        isClientOn = false;
                        break;
                }
            }
        }

        private void Sell()
        {
            var client = _clients.Peek();

            if (TryReturnProduct(out Stack? stack))
            {
                client.Buy(stack);
                Console.WriteLine("В корзине");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Ошибка");
            }
        }

        private void TryEndPurchase()
        {
            var client = _clients.Peek();

            if (client.Money < 0)
            {
                Console.WriteLine("У вас недостаточно денег, нажмите любую клавишу, чтобы выкинуть товар из корзины");
                Console.ReadKey();
                client.RemoveProductFromCart();
            }
            else
            {
                _clients.Dequeue();
                Console.Clear();
                Console.WriteLine("До свидания!");
                Console.ReadKey();
            }
        }

        private void ShowProducts()
        {
            for (int i = 0; i < _stacks.Count; i++)
            {
                var stack = _stacks[i];
                Console.Write((i) + " ");
                stack.ShowInfo();
                Console.WriteLine();
            }
        }

        private bool TryReturnProduct(out Stack? stack)
        {
            stack = null;

            Console.WriteLine("Выберите товар: ");

            if (int.TryParse(Console.ReadLine(), out int numberProduct) == false)
            {
                Console.WriteLine("Ошибка");
                Console.ReadKey();
                return false;
            }

            if (numberProduct < 0 || numberProduct >= _stacks.Count)
            {
                Console.WriteLine("Такого товара нет");
                Console.ReadKey();
                return false;
            }

            Console.WriteLine("Выберите количество товара: ");

            if (int.TryParse(Console.ReadLine(), out int quantity) == false)
            {
                Console.WriteLine("Нужно ввести число.");
                Console.ReadKey();
                return false;
            }

            if (_stacks[numberProduct].TryGetProduct(quantity) == false)
            {
                Console.WriteLine("Недостаточно количество");
                Console.ReadKey();
                return false;
            }

            stack = new Stack(_stacks[numberProduct].Product, quantity);
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
                Console.WriteLine($"{stack.Product.Name}, количество: {stack.Quantity}");
            }
        }

        public void Buy(Stack stack)
        {
            foreach (Stack currentStack in _cart)
            {
                if (currentStack.Product == stack.Product)
                {
                    currentStack.AddQuantity(stack.Quantity);
                    Money -= stack.Quantity * stack.Product.Cost;
                    return;
                }
            }

            Money -= stack.Quantity * stack.Product.Cost;
            _cart.Add(stack);
        }

        public void RemoveProductFromCart()
        {
            var lastItem = _cart[^1];
            _cart.Remove(lastItem);
            Money += lastItem.Product.Cost;
        }
    }

    class Stack
    {
        public Stack(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Stack(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
        public int Quantity { get; private set; }

        public bool TryGetProduct(int quantity)
        {

            if (quantity < 0)
            {
                return false;
            }

            return true;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{Product.Name}, цена: {Product.Cost}");
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }
    }
}