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

        static int eggs = 0;

        static int playerLevel = 1;

        static void Main(string[] args)
        {

            LoadData();

            bool play = true;

            while (play)
            {

                playerLevel = (pets.Count <= 5) ? pets.Count : 5;

                foreach (Pet pet in pets)
                {

                    pet.UpdateStats();

                }

                int choice = Menu.DisplayMenu("Select a choice", new List<string>() {"Exit", "Collect items", "View food sack", "Raise Pet", "Tend to pets", "See Pets"});

                switch (choice)
                {

                    case 0:

                        play = false;

                        break;

                    case 1:
                        //play game

                        Game game = new Game(playerLevel, inventory);

                        eggs += game.Play() ? 1 : 0;

                        break;

                    case 2:
                        //view food sack

                        Console.Clear();

                        foreach (Food food in inventory.GetFoodSack().Keys)
                        {

                            if (!inventory.HasFood(food)) continue;

                            Console.WriteLine(food.GetDisplayName() + ": " + inventory.GetFoodSack()[food]);

                        }

                        Console.ReadKey();

                        break;

                    case 3:
                        //get new pet

                        Console.Clear();

                        if (eggs == 0)
                        {

                            Console.WriteLine("You have no eggs! Go and forage for items to get started");

                            Console.ReadKey();

                            break;

                        }

                        pets.Add(new Pet(playerLevel));

                        Console.WriteLine("You hatched a new pet!");

                        pets[pets.Count - 1].DisplayPetInfo();

                        Console.ReadKey();

                        break;

                    case 4:
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

                        if (pets[pick].hunger == 0)
                        {

                            Console.WriteLine("Do you want to send of " + pets[pick].name + "? (Y/N)");

                            if (Console.ReadLine() == "Y") pets.RemoveAt(pick);

                        }

                        pets[pick].Feed(inventory);

                        break;

                    case 5:
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

                            try
                            {

                                Food food = (Food)Enum.Parse(typeof(Food), reader.ReadString());

                                int amount = reader.ReadInt32();

                                for (int j = 0; j < amount; j++)
                                {

                                    inventory.AddFood(food);

                                }

                            }
                            catch (EndOfStreamException)
                            {

                                break;

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

            //clean pointless files

            foreach (string directory in Directory.GetFiles(Directory.GetCurrentDirectory() + "/gamedata/pets"))
            {

                if (!IsActivePet(directory.Substring(directory.Length - 36).Remove(directory.Substring(directory.Length - 36).Length - 4)))
                {

                    File.Delete(directory);

                }

            }

        }

        private static bool IsActivePet(string uuid)
        {

            foreach (Pet pet in pets)
            {

                if (pet.uuid == uuid) return true;

            }
            return false;

        }

    }

}
