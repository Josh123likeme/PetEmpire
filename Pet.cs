using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UIElements;
    
namespace VirtualPets2
{  

    public class Pet
    {

        public string uuid;

        public string name;
        public int level;

        public int hunger;
        public int maxHunger;

        public long lastPing;
        public long metabolicRate;

        private static string[] syllables = new string[] {"a","be","ce","ef","za","re","ra","op", "ka", "on", "ba" };

        private static Random random = new Random();

        public Pet() { }

        public Pet(int level)
        {

            this.level = level;

            name = GenerateName();
            uuid = GenerateUUID();

            maxHunger = random.Next(level * 10, level * 20);
            hunger = maxHunger;

            lastPing = DateTime.UtcNow.Ticks;
            metabolicRate = (long) random.Next(level * 3600, level * 7200) * 10000000; //with level = 1, 1-2 hours

        }

        private static string GenerateUUID()
        {

            string uuid = "";

            for (int i = 0; i < 32; i++)
            {

                uuid += (char)random.Next(97, 123);

            }

            return uuid;

        }

        public void DisplayPetInfo()
        {

            if (hunger == 0)
            {

                Console.WriteLine(name + " has deceased! :(");

                return;

            }

            string content = "";

            content += "  " + name + " (lvl " + level + ")\n";

            for (int i = 0; i < name.Length + level.ToString().Length + 11; i++)
            {

                content += "~";

            }

            content += "\nHunger: " + hunger + "/" + maxHunger + " (" + (int) ((double) hunger / maxHunger * 100) + "%)";

            long rateInMinutes = metabolicRate / (60 * 10000000);

            long hours = rateInMinutes / 60;
            long minutes = (rateInMinutes % 60);

            content += "\nMetabolic Rate: " + hours + " hours " + minutes + " minutes";

            Console.WriteLine(content);

        }

        public void Feed(Inventory inventory)
        {

            if (hunger == 0) return;

            List<string> choices = new List<string>();

            choices.Add("Back");

            foreach (Food food in inventory.GetFoodSack().Keys)
            {

                if (!inventory.HasFood(food)) continue;

                choices.Add(food.GetDisplayName() + ": " + inventory.GetFoodSack()[food]);

            }

            int choice = Menu.DisplayMenu("Which item do you want to use?", choices);

            if (choice == 0) return;

            inventory.RemoveFood(inventory.GetFoodSack().Keys.ToList()[choice - 1]);

            hunger += (int) inventory.GetFoodSack().Keys.ToList()[choice - 1];

            UpdateStats();

        }

        private static string GenerateName()
        {

            string content = "";

            int numberOfSyllables = random.Next(2, 5);

            for (int i = 0; i < numberOfSyllables; i++)
            {

                content += syllables[random.Next(syllables.Length)];

            }

            return char.ToUpper(content[0]) + content.Substring(1);

        }

        public void UpdateStats()
        {

            Console.WriteLine(lastPing + "\n" + DateTime.UtcNow.Ticks);

            for (long timeLeft = DateTime.UtcNow.Ticks - lastPing; timeLeft  - metabolicRate > 0; timeLeft -= metabolicRate)
            {

                lastPing += metabolicRate;

                hunger -= 1;

            }

            hunger = (hunger < 0) ? 0 : hunger;
            hunger = (hunger > maxHunger) ? maxHunger : hunger;

        }

    }

}
