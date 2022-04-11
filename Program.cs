using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UIElements;

namespace VirtualPets2
{
    class Program
    {

        static Inventory inventory = new Inventory();

        static List<Pet> pets = new List<Pet>();

        static void Main(string[] args)
        {

            LoadData();

            //<debug>

            //inventory.AddFood(Food.CHEAP_FISH);

            //<\debug>

            bool play = true;

            while (play)
            {

                foreach (Pet pet in pets)
                {

                    pet.UpdateStats();

                }

                int choice = Menu.DisplayMenu("Select a choice", new List<string>() {"Exit", "Collect items", "Raise Pet", "Tend to pets", "See Pets"});

                switch (choice)
                {

                    case 0:

                        play = false;

                        break;

                    case 1:
                        //play game
                        break;

                    case 2:
                        //get new pet
                        break;

                    case 3:
                        //tend to pets

                        Console.Clear();

                        if (pets.Count == 0)
                        {

                            Console.WriteLine("You have no pets! Go and forage for items to get started");

                            Console.ReadKey();

                            break;

                        }

                        List<string> names = new List<string>();

                        foreach (Pet pet in pets)
                        {

                            names.Add(pet.name);

                        }

                        

                        int pick = Menu.DisplayMenu("Which pet do you want to tend to?", names);

                        pets[pick].Feed(inventory);

                        break;

                    case 4:
                        //display pets

                        Console.Clear();

                        if (pets.Count == 0)
                        {

                            Console.WriteLine("You have no pets! Go and forage for items to get started");

                            Console.ReadKey();

                            break;

                        }

                        foreach (Pet pet in pets)
                        {

                            Console.Write("\n");

                            pet.DisplayPetInfo();

                        }

                        Console.ReadKey();

                        break;

                }

                SaveData();

            } 

        }

        public static void LoadData()
        {

            try
            {

                using (var stream = File.Open(Directory.GetCurrentDirectory() + "/gamedata/inventory.bin", FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {

                        for (int i = 0; i < inventory.GetFoodSack().Keys.Count; i++)
                        {

                            Food food = (Food)Enum.Parse(typeof(Food), reader.ReadString());

                            int amount = reader.ReadInt32();

                            for (int j = 0; j < amount; j++)
                            {

                                inventory.AddFood(food);

                            }

                        }

                    }

                }

                foreach (string directory in Directory.GetFiles(Directory.GetCurrentDirectory() + "/gamedata/pets"))
                {

                    using (var stream = File.Open(directory, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                        {

                            Pet pet = new Pet();

                            string uuid = directory.Remove(0, (Directory.GetCurrentDirectory() + "/gamedata/pets").Length);
                            uuid = uuid.Remove(uuid.Length - 4);
                            pet.uuid = uuid;
                            pet.name = reader.ReadString();
                            pet.level = reader.ReadInt32();
                            pet.hunger = reader.ReadInt32();
                            pet.maxHunger = reader.ReadInt32();
                            pet.lastPing = reader.ReadInt64();
                            pet.metabolicRate = reader.ReadInt64();

                            pets.Add(pet);

                        }

                    }

                }

            }
            catch (FileNotFoundException)
            {

                using (var stream = File.Open("gamedata/inventory.bin", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {

                        foreach (Food food in inventory.GetFoodSack().Keys)
                        {

                            writer.Write(food.GetType().Name);
                            writer.Write(inventory.GetFoodSack()[food]);

                        }

                    }

                }

            }
            catch (DirectoryNotFoundException)
            {

                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/gamedata/pets");

            }

        }

        public static void SaveData()
        {

            using (var stream = File.Open("gamedata/inventory.bin", FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {

                    foreach (Food food in inventory.GetFoodSack().Keys)
                    {

                        writer.Write(Enum.GetName(typeof (Food), food));
                        writer.Write(inventory.GetFoodSack()[food]);

                    }

                }

            }

            foreach (Pet pet in pets)
            {

                using (var stream = File.Open("gamedata/pets/" + pet.uuid + ".bin", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {

                        writer.Write(pet.name);
                        writer.Write(pet.level);
                        writer.Write(pet.hunger);
                        writer.Write(pet.maxHunger);
                        writer.Write(pet.lastPing);
                        writer.Write(pet.metabolicRate);

                    }

                }

            }

        }

    }

}
