using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace PlayerDI
{
    class Program
    {
        static void Main(string[] args)
        {
            // 3. ������� ��������� IoC � ������������ �����������
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPlayer, Player>() // ������������ Player ��� ��������
                .AddTransient<IEquipment, Equipment>() // ���������� ����� ���� transient
                .BuildServiceProvider();

            // 4. �������� ��������� Player ����� DI ���������
            var player = serviceProvider.GetRequiredService<IPlayer>();

            // �������������� ������
            player.Health = 100;
            player.Lives = 3;
            player.Nickname = "John";
            player.Skills = new string[] { "Skill1", "Skill2", "Skill3" };

            Console.WriteLine("�������� ������: " + player.Health);
            Console.WriteLine("������� ������: " + player.Nickname);

            // ������� ���������� ����� DI
            var equipment = serviceProvider.GetRequiredService<IEquipment>();
            player.Equipment = equipment;

            // ��������� �������� � ����������
            equipment.AddItem(new Weapon("��������", 50));
            equipment.AddItem(new Parachute());
            equipment.AddItem(new RocketPack(3)); // �������� ����� � 3 ��������

            // ������������ ��������� ���������� �� ������ ����� ���������
            ModifyPlayerAttributes(player);

            Console.ReadKey();
        }

        static void ModifyPlayerAttributes(IPlayer player)
        {
            // 9. ����� �������� �������� ��������� ������ �� ������ ����� ���������
            player.Health -= 20; // ����� ������� ����
            if (player.Equipment != null)
            {
                foreach (var item in player.Equipment.GetItems())
                {
                    if (item is Weapon weapon)
                    {
                        weapon.Ammo -= 10; // ������������ 10 ��������
                    }
                    else if (item is RocketPack rocketPack)
                    {
                        rocketPack.UseCharge(); // ������������ ���� �����
                    }
                }
            }

            Console.WriteLine($"����� {player.Nickname} ������� ����. ������� ��������: {player.Health}");
        }
    }

    // 1. ������� ��������� ��� ������ Player
    public interface IPlayer
    {
        int Health { get; set; }
        int Lives { get; set; }
        string Nickname { get; set; }
        string[] Skills { get; set; }
        IEquipment Equipment { get; set; }
    }

    // 2. ��������� ����� Player, ��������� ���������
    public class Player : IPlayer
    {
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }
        public IEquipment Equipment { get; set; }

        // 11. ��������� ���������� ������ �������� � Equipment
    }

    // ��������� ��� ����������
    public interface IEquipment
    {
        void AddItem(Item item);
        IEnumerable<Item> GetItems();
    }

    // ���������� ����������
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

    // ������� ����� �������� ����������
    public abstract class Item
    {
        public string Name { get; protected set; }

        protected Item(string name)
        {
            Name = name;
        }
    }

    // ������
    public class Weapon : Item
    {
        public int Ammo { get; set; }

        public Weapon(string name, int ammo) : base(name)
        {
            Ammo = ammo;
        }
    }

    // �������
    public class Parachute : Item
    {
        public Parachute() : base("Parachute") { }
    }

    // �������� �����
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