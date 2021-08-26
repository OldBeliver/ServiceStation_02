using System;
using System.Collections.Generic;

namespace ServiceStation_02
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Creator creator = new Creator();
            List<Car> cars = new List<Car>();

            for (int i = 0; i < 5; i++)
            {
                //Car car = creator.CreateNewCar();

                cars.Add(creator.CreateNewCar());
            }

            Console.WriteLine($"№№  название\t\tсостояние");
            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].ShowInfo();
                Console.WriteLine($"-----------------------");
            }
        }
    }

    class ServiceCenter
    {
        
    }

    class Creator
    {
        private List<Detail> _details;
        private Car _car;
        private Store _store;
        private static Random _random;
        private int _durable;

        static Creator()
        {
            _random = new Random();
        }

        public Creator()
        {
            _details = new List<Detail>();
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
        
        public Car()
        {
            _details = new List<Detail>();
        }

        public void ShowInfo()
        {
            int i = 1;

            foreach (var detail in _details)
            {
                Console.Write($"{i:d2}. ");
                detail.ShowInfo();
                i++;
            }
        }

        public void AddDetail(string name, int condition)
        {
            
            _details.Add(new Detail(name, condition, 0));
        }
    }

    class Store
    {
        private List<Detail> _store;

        public Store()
        {
            _store = new List<Detail>();
        }

        public void ShowInfo()
        {
            for (int i = 0; i < _store.Count; i++)
            {
                Console.Write($"{i+1:d2}. ");
                _store[i].ShowInfo();
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

        public void ShowInfo()
        {
            //Console.WriteLine($"{Name} состояние {Condition}, {Price} рублей");
            Console.WriteLine($"{Name} \t{Condition}");
        }

        public void CreateNewCondition(int value)
        {
            Condition = value;
        }
    }
}