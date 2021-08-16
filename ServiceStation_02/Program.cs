using System;

namespace ServiceStation_02
{
    internal class Program
    {
        public static void Main(string[] args)
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