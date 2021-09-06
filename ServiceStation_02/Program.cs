using System;
using System.Collections.Generic;

namespace ServiceStation_02
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            Creator creator = new Creator();
            List<Car> cars = new List<Car>();
            Store store = new Store();


            for (int i = 0; i < 5; i++)
            {
                cars.Add(creator.CreateNewCar());
            }

            Console.WriteLine($"№№  название\t\tсостояние");
            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].ShowInfo();
                Console.WriteLine($"-----------------------");
            }

            store = creator.CreateNewStore();

            Console.WriteLine($"-------------------");
            Console.WriteLine($"склад");
            Console.WriteLine($"№№  название\t\tколичество\tстоимость");
            Console.WriteLine($"--------------------------------------------------");
            
            store.ShowInfo();

            Console.WriteLine($"нажите любую для наполнения склада");
            Console.ReadKey();
            
            int count = store.GetSlotQuantity();

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"введите номер детали: ");
                int userInput = Convert.ToInt32(Console.ReadLine());
                store.AddQuantity(userInput - 1);

                Console.Clear();
                store.ShowInfo();
            }
            */
            ServiceCenter serviceCenter = new ServiceCenter();
            serviceCenter.Work();
        }
    }

    class ServiceCenter
    {
        private Creator _creator;
        private Queue<Car> _cars;
        private Store _store;

        private int _carNumber;
        int _minCondition;
        int _money;

        public ServiceCenter()
        {
            _creator = new Creator();
            _cars = new Queue<Car>();
            _store = new Store();

            _carNumber = 5;
            _minCondition = 20;
            _money = 0;

            AddCar(_carNumber);
        }

        public void Work()
        {
            PrintTitle();
            PrintTicker();

            _store = _creator.CreateNewStore();

            ShowStore();

            Console.Write($"\nНажмите ENTER для наполнения склада ...");
            Console.ReadKey();

            _store.AddQuantity();

            while (_cars.Count > 0)
            {
                PrintTitle();
                PrintTicker();

                Car car = _cars.Dequeue();

                car.ShowInfo(_minCondition);

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void AddCar(int number)
        {
            for (int i = 0; i < number; i++)
            {
                _cars.Enqueue(_creator.CreateNewCar());
            }
        }

        private string GetWordCarEnding(int amount)
        {
            string ending = "";

            switch (amount)
            {
                case 1:
                    ending = "a";
                    break;
                case 2:
                case 3:
                case 4:
                    ending = "ы";
                    break;
            }

            return ending;
        }

        private void ShowStore()
        {
            Console.WriteLine($"--------------------");
            Console.WriteLine($"\tСКЛАД");
            Console.WriteLine($"--------------------");
            Console.WriteLine($"№№  название\t\tколичество\tстоимость");
            Console.WriteLine($"--------------------------------------------------");

            _store.ShowInfo();
        }

        private void PrintTitle()
        {
            Console.WriteLine("СТО \"Будь проклят то день, когда я сел за баранку этого пылесоса\"");
        }

        private void PrintTicker()
        {
            string ending = GetWordCarEnding(_cars.Count);
            Console.WriteLine($"\nВ очереди не ремонт {_cars.Count} машин{ending}\n");
        }
    }

    class Creator
    {
        private List<Detail> _details;
        private Car _car;
        private Store _store;
        private List<Slot> _slots;
        private static Random _random;
        private int _durable;

        static Creator()
        {
            _random = new Random();
        }

        public Creator()
        {
            _details = new List<Detail>();
            _slots = new List<Slot>();
            _durable = 50;

            LoadDetails();
        }

        public Car CreateNewCar()
        {
            _car = new Car();

            for (int i = 0; i < _details.Count; i++)
            {
                string name = _details[i].Name;
                int condition = _random.Next(_durable);

                _car.AddDetail(name, condition);
            }

            return _car;
        }

        public Store CreateNewStore()
        {
            _store = new Store();

            for (int i = 0; i < _details.Count; i++)
            {
                Detail detail = _details[i];
                int quantity = 0;

                _store.AddSlot(detail, quantity);
            }

            return _store;
        }

        private void LoadDetails()
        {
            int condition = 100;

            _details.Add(new Detail("амортизатор", condition, 685));
            _details.Add(new Detail("плановое ТО", condition, 5000));
            _details.Add(new Detail("ремень ГРМ ", condition, 255));
            _details.Add(new Detail("свеча зажигания", condition, 123));
            _details.Add(new Detail("топливный насос", condition, 770));
            _details.Add(new Detail("тормозной диск", condition, 765));
            _details.Add(new Detail("тормозные накладки", condition, 328));
            _details.Add(new Detail("электро генератор", condition, 2424));
        }
    }

    class Car
    {
        private List<Detail> _details;
        private int usedDetailPrice = 0;

        public Car()
        {
            _details = new List<Detail>();
        }

        public void ShowInfo(int _minCondition)
        {
            int i = 1;

            foreach (var detail in _details)
            {
                Console.Write($"{i:d2}. ");
                detail.ShowInfo(_minCondition);
                i++;
            }
        }

        public void AddDetail(string name, int condition)
        {
            _details.Add(new Detail(name, condition, usedDetailPrice));
        }
    }

    class Store
    {
        private List<Slot> _slots;

        public Store()
        {
            _slots = new List<Slot>();
        }

        public void ShowInfo()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                Console.Write($"{i + 1:d2}. ");
                _slots[i].ShowInfo();
            }
        }

        public void AddSlot(Detail detail, int quantity)
        {
            _slots.Add(new Slot(detail, quantity));
        }

        public void AddQuantity(int index)
        {
            Console.Write($"Вы выбрали деталь ");
            _slots[index].ShowInfo();

            Console.Write($"количество: ");
            int number = Convert.ToInt32(Console.ReadLine());
            _slots[index].AddQuantity(number);
        }

        public int GetSlotQuantity()
        {
            return _slots.Count;
        }
    }

    class Slot
    {
        private Detail _detail;
        private int _quantity;

        public Slot(Detail detail, int quantity)
        {
            _detail = detail;
            _quantity = quantity;
        }

        public void ShowInfo()
        {
            //Console.WriteLine($"{_detail.Name} \t\t{_quantity} \t{_detail.Price}");
            Console.WriteLine("{0,-1} \t{1,5:d2} \t{2,17:n2}", _detail.Name, _quantity, _detail.Price);
        }

        public void AddQuantity(int number)
        {
            _quantity += number;

            if (_quantity < 0)
            {
                _quantity = 0;
            }
        }
    }

    class Detail
    {
        public string Name { get; private set; }
        public int Condition { get; private set; }
        public int Price { get; private set; }

        public Detail(string name, int condition, int price)
        {
            Name = name;
            Condition = condition;
            Price = price;
        }

        public void ShowInfo(int minCondition)
        {
            ConsoleColor color;
            color = Console.ForegroundColor;

            if (Condition <= minCondition)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            //Console.WriteLine($"{Name} \t{Condition:d2} \t\t{Price}");
            Console.WriteLine($"{Name} \t{Condition:d2}");
            Console.ForegroundColor = color;
        }

        public void CreateNewCondition(int value)
        {
            Condition = value;
        }
    }
}