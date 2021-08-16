using System;
using System.Collections.Generic;

namespace ServiceStation_02
{
    internal class Program
    {
        public static void Main(string[] args)
        {
        }
    }

    class ServiceCenter
    {
        
    }

    class Store
    {
        
    }

    class CarCreator
    {
        
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
            foreach (var detail in _details)
            {
                detail.ShowInfo();
            }
        }

        public void DecrementDetailQuantity()
        {
            
        }
    }

    class Detail
    {
        public string Name { get; private set; } 
        public int Condition { get; private set; }
        public int Price { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"{Name} состояние {Condition}, {Price} рублей");
        }
    }
}