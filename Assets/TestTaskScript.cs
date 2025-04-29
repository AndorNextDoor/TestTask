using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace PlayerDI
{
    class Program
    {
        static void Main(string[] args)
        {
            // 3. Создаем контейнер IoC и регистрируем зависимости
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPlayer, Player>() // Регистрируем Player как синглтон
                .AddTransient<IEquipment, Equipment>() // Экипировка может быть transient
                .BuildServiceProvider();

            // 4. Получаем экземпляр Player через DI контейнер
            var player = serviceProvider.GetRequiredService<IPlayer>();

            // Инициализируем игрока
            player.Health = 100;
            player.Lives = 3;
            player.Nickname = "John";
            player.Skills = new string[] { "Skill1", "Skill2", "Skill3" };

            Console.WriteLine("Здоровье игрока: " + player.Health);
            Console.WriteLine("Никнейм игрока: " + player.Nickname);

            // Создаем экипировку через DI
            var equipment = serviceProvider.GetRequiredService<IEquipment>();
            player.Equipment = equipment;

            // Добавляем предметы в экипировку
            equipment.AddItem(new Weapon("Винтовка", 50));
            equipment.AddItem(new Parachute());
            equipment.AddItem(new RocketPack(3)); // Ракетный ранец с 3 зарядами

            // Демонстрация изменения параметров из другой части программы
            ModifyPlayerAttributes(player);

            Console.ReadKey();
        }

        static void ModifyPlayerAttributes(IPlayer player)
        {
            // 9. Легко изменяем значения атрибутов игрока из другой части программы
            player.Health -= 20; // Игрок получил урон
            if (player.Equipment != null)
            {
                foreach (var item in player.Equipment.GetItems())
                {
                    if (item is Weapon weapon)
                    {
                        weapon.Ammo -= 10; // Использовали 10 патронов
                    }
                    else if (item is RocketPack rocketPack)
                    {
                        rocketPack.UseCharge(); // Использовали один заряд
                    }
                }
            }

            Console.WriteLine($"Игрок {player.Nickname} получил урон. Текущее здоровье: {player.Health}");
        }
    }

    // 1. Создаем интерфейс для класса Player
    public interface IPlayer
    {
        int Health { get; set; }
        int Lives { get; set; }
        string Nickname { get; set; }
        string[] Skills { get; set; }
        IEquipment Equipment { get; set; }
    }

    // 2. Реализуем класс Player, используя интерфейс
    public class Player : IPlayer
    {
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }
        public IEquipment Equipment { get; set; }

        // 11. Параметры экипировки теперь хранятся в Equipment
    }

    // Интерфейс для экипировки
    public interface IEquipment
    {
        void AddItem(Item item);
        IEnumerable<Item> GetItems();
    }

    // Реализация экипировки
    public class Equipment : IEquipment
    {
        private readonly List<Item> _items = new List<Item>();

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public IEnumerable<Item> GetItems()
        {
            return _items.AsReadOnly();
        }
    }

    // Базовый класс предмета экипировки
    public abstract class Item
    {
        public string Name { get; protected set; }

        protected Item(string name)
        {
            Name = name;
        }
    }

    // Оружие
    public class Weapon : Item
    {
        public int Ammo { get; set; }

        public Weapon(string name, int ammo) : base(name)
        {
            Ammo = ammo;
        }
    }

    // Парашют
    public class Parachute : Item
    {
        public Parachute() : base("Parachute") { }
    }

    // Ракетный ранец
    public class RocketPack : Item
    {
        public int Charges { get; private set; }

        public RocketPack(int charges) : base("RocketPack")
        {
            Charges = charges;
        }

        public void UseCharge()
        {
            if (Charges > 0)
                Charges--;
        }
    }
}