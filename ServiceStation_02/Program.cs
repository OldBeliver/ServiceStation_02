using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStation_02
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ServiceCenter serviceCenter = new ServiceCenter();
            serviceCenter.Work();
        }
    }

    class ServiceCenter
    {
        private Creator _creator;
        private Queue<Car> _cars;
        private Store _store;
        private List<PerformedWork> _performedWorks;
        private Ending _ending;
       
        private int _carNumber;
        private int _minCondition;
        private int _money;
        private int _penalty;

        public ServiceCenter()
        {
            _carNumber = 5;
            _minCondition = 20;
            _money = 0;
            _penalty = 200;

            _creator = new Creator();
            _cars = new Queue<Car>();
            _performedWorks = new List<PerformedWork>();
            _ending = new Ending();
        }

        public void Work()
        {
            bool isOpen = true;

            AddCar(_carNumber);

            ShowTitle();
            ShowTicker();

            _store = _creator.CreateNewStore();

            ShowStore();

            Console.Write($"\nНажмите ENTER для наполнения склада ...");
            Console.ReadKey();

            while (isOpen)
            {
                Console.Clear();
                ShowStore();

                Console.Write($"\nexit - выход или\nВведите номер детали: ");
                string userInput = Console.ReadLine();

                if (userInput == "exit")
                {
                    isOpen = false;
                }

                if (userInput == "test")
                {
                    _store.AddQuantity();
                }

                bool result = int.TryParse(userInput, out int number);
                int upperLimit = _store.GetSlotQuantity();

                if (result && number > 0 && number <= upperLimit)
                {
                    int index = number - 1;

                    _store.AddQuantity(index);
                }
            }

            while (_cars.Count > 0)
            {
                Console.Clear();
                ShowTitle();
                ShowTicker();
                ShowStore();
                
                Car car = _cars.Dequeue();
                car.ShowInfo(_minCondition);
                RepaireCar(car);
            }
            
            Console.Write($"рабочий день окончен\nприбыль за сегодня {_money:n2} рубл{_ending.GetRubleEnding(_money)}\nнажмите любую для завершения ...");
            Console.ReadKey();
        }

        private void AddCar(int number)
        {
            for (int i = 0; i < number; i++)
            {
                _cars.Enqueue(_creator.CreateNewCar());
            }
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

        private void ShowTitle()
        {
            Console.WriteLine("СТО \"Будь проклят то день, когда я сел за баранку этого пылесоса\"");
            Console.WriteLine($"баланс {_money:n2} рубл{_ending.GetRubleEnding(_money)}");
        }

        private void ShowTicker()
        {   
            Console.WriteLine($"\nВ очереди не ремонт {_cars.Count} машин{_ending.GetCarEnding(_money)}\n");
        }

        private void RepaireCar(Car car)
        {
            bool doRepaire = true;
            int detailIndex = 0;
            int currentPay;
            _performedWorks.Clear();

            while (doRepaire)
            {
                Console.Clear();
                ShowTitle();
                ShowTicker();
                ShowStore();
                Console.WriteLine();
                car.ShowInfo(_minCondition);

                Console.Write($"\n1-8 - номер детали со склада для замены\nnext - закончить ремонт\nВведите команду: ");
                string userInput = Console.ReadLine();

                if (userInput == "next")
                {
                    _money -= CalculatePenalty(car);
                    Console.WriteLine($"Начисляем денежку за ремонт");
                    _money += CalculateCosts();
                    doRepaire = false;
                }
                else
                {
                    bool result = int.TryParse(userInput, out int number);

                    if (result)
                    {
                        int upperLimit = _store.GetCount();

                        if (number > 0 && number <= upperLimit)
                        {
                            detailIndex = number - 1;
                        }
                        else
                        {
                            result = false;
                        }
                    }

                    if (result && (!_store.AvailableQuantity(detailIndex) || !car.AvaliableCondition(detailIndex, _minCondition)))
                    {
                        Console.WriteLine($"штраф за попытку заменить годную деталь");
                        currentPay = _store.GetPrice(detailIndex) * (-1);
                        DateTime timeNow = DateTime.Now;
                        _performedWorks.Add(new PerformedWork((currentPay), _store.GetName(detailIndex), timeNow));
                    }

                    if (result && _store.AvailableQuantity(detailIndex) && car.AvaliableCondition(detailIndex, _minCondition))
                    {
                        _store.DecreaseQuantity(detailIndex);
                        car.DoConditionDetailAsNew(detailIndex);
                        currentPay = _store.GetPrice(detailIndex) * 3 / 2;
                        DateTime timeNow = DateTime.Now;
                        _performedWorks.Add(new PerformedWork((currentPay), _store.GetName(detailIndex), timeNow));
                    }
                    
                    Console.WriteLine($"выполненные работы");
                    for (int i = 0; i < _performedWorks.Count; i++)
                    {
                        _performedWorks[i].ShowInfo();
                    }
                }

                Console.Write($"любую для продолжения ... ");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private int CalculatePenalty(Car car)
        {
            int penalty = car.failRepair(_minCondition) * _penalty;
            Console.WriteLine($"Штраф за отсутствие детали на складе - {penalty} рубл{_ending.GetRubleEnding(_money)}");
            Console.ReadKey();
            return penalty;
        }

        private int CalculateCosts()
        {
            return _performedWorks.Sum(work => work.Price);
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
        private int _storeMaxCapacity;

        static Creator()
        {
            _random = new Random();
        }

        public Creator()
        {
            _details = new List<Detail>();
            _slots = new List<Slot>();
            _durable = 50;
            _storeMaxCapacity = 40;

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
            _store = new Store(_storeMaxCapacity);

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
            Console.WriteLine($"Чек-лист поломок машины");
            Console.WriteLine($"---------------------------------");
            Console.WriteLine($" №  деталь\t\tсостояние");
            Console.WriteLine($"---------------------------------");
            var filteredByCondition = _details.OrderBy(detail => detail.Condition).ToList();
            
            for (int i = 0; i < filteredByCondition.Count; i++)
            {
                Console.Write($"{i+1:d2}. ");
                filteredByCondition[i].ShowInfo(_minCondition);
            }
        }

        public void AddDetail(string name, int condition)
        {
            _details.Add(new Detail(name, condition, usedDetailPrice));
        }
        public int failRepair(int minCondition)
        {
            var failRepair = _details.Where(detail => detail.Condition <= minCondition);
            return failRepair.Count();
        }

        public bool AvaliableCondition(int index, int _minCondition)
        {
            return _details[index].Condition <= _minCondition;
        }

        public void DoConditionDetailAsNew(int index)
        {
            _details[index].SetConditionAsNew();
        }
    }

    class Store
    {
        private List<Slot> _slots;
        private int _currentCapacity;

        public int MaxCapacity { get; private set; }

        public Store(int maxCapacity)
        {
            _slots = new List<Slot>();
            MaxCapacity = maxCapacity;
        }

        public void ShowInfo()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                Console.Write($"{i + 1:d2}. ");
                _slots[i].ShowInfo();
            }

            _currentCapacity = CalculateCurrentCapacity();

            ConsoleColor color;
            color = Console.ForegroundColor;

            if (_currentCapacity >= MaxCapacity)
            {
                Console.ForegroundColor = ConsoleColor.Green;

            }
            Console.WriteLine($"\nВместимость склада {_currentCapacity}/{MaxCapacity}");
            Console.ForegroundColor = color;
        }

        public void AddSlot(Detail detail, int quantity)
        {
            _slots.Add(new Slot(detail, quantity));
        }

        public void AddQuantity(int index)
        {
            Console.Write($"Вы выбрали деталь ");
            _slots[index].ShowInfo();

            Console.Write($"добавить количество: ");
            int.TryParse(Console.ReadLine(), out int number);

            _currentCapacity = CalculateCurrentCapacity();
            if (_currentCapacity + number <= MaxCapacity)
            {
                _slots[index].AddQuantity(number);
            }
            else
            {
                Console.WriteLine($"Превышен лимит склада ...");
                Console.ReadKey();
            }
        }

        public void AddQuantity()
        {
            int cheatDetail = 10;

            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].AddQuantity(cheatDetail);
            }
        }

        public int GetSlotQuantity()
        {
            return _slots.Count;
        }

        private int CalculateCurrentCapacity()
        {
            int amount = _slots.Sum(detail => detail.Quantity);
            return amount;
        }

        public int GetCount()
        {
            return _slots.Count;
        }

        public bool AvailableQuantity(int index)
        {
            return _slots[index].Quantity > 0;
        }

        public int GetPrice(int index)
        {
            return _slots[index].GetPrice();
        }

        public string GetName(int index)
        {
            return _slots[index].GetName();
        }

        public void DecreaseQuantity(int index)
        {
            if (_slots[index].Quantity > 0)
            {
                _slots[index].DeleteDetail();
            }
        }

        public int GetNewDetailCondition(int index)
        {
            return _slots[index].GetCondition();
        }

        public void ReplaceDetail(int index)
        {
            _slots[index].SetConditionAsNew();
        }
    }

    class Slot
    {
        private Detail _detail;

        public int Quantity { get; private set; }

        public Slot(Detail detail, int quantity)
        {
            _detail = detail;

            Quantity = quantity;
        }

        public void ShowInfo()
        {
            Console.WriteLine("{0,-1} \t{1,5:d2} \t{2,17:n2}", _detail.Name, Quantity, _detail.Price);
        }

        public void AddQuantity(int number)
        {
            Quantity += number;

            if (Quantity < 0)
            {
                Quantity = 0;
            }
        }
        public int GetPrice()
        {
            return _detail.Price;
        }

        public string GetName()
        {
            return _detail.Name;
        }

        public int GetCondition()
        {
            return _detail.Condition;
        }

        public void DeleteDetail()
        {
            Quantity--;
        }

        public void SetConditionAsNew()
        {  
            _detail.SetConditionAsNew();
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

            Console.WriteLine($"{Name} \t{Condition:d2}");
            Console.ForegroundColor = color;
        }

        public void SetConditionAsNew()
        {   
            Condition = 100;
        }
    }

    class PerformedWork
    {
        private Ending _ending;

        public int Price { get; private set; }
        public string Detail { get; private set; }
        public DateTime Time { get; private set; }

        public PerformedWork(int price, string detail, DateTime time)
        {
            Price = price;
            Detail = detail;
            Time = time;
            _ending = new Ending();
        }

        public void ShowInfo()
        {
            Console.WriteLine($"замена {Detail}, {Price} рубл{_ending.GetRubleEnding(Price)}, время {Time}");
        }
    }

    class Ending
    {
       public string GetCarEnding(int number)
        {
            int amount = GetLastNumeral(number);
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

        public string GetRubleEnding(int number)
        {
            int amount = GetLastNumeral(number);
            string ending = "ей";

            switch (amount)
            {
                case 1:
                    ending = "ь";
                    break;
                case 2:
                case 3:
                case 4:
                    ending = "я";
                    break;
            }

            return ending;
        }

        private int GetLastNumeral(int number)
        {
            return number % 10;
        }
    }
}