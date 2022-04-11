using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPets2
{
    public class Inventory
    {

        Dictionary<Food, int> foodSack = new Dictionary<Food, int>();

        public Inventory()
        {

            foreach (Food food in Enum.GetValues(typeof (Food)))
            {

                foodSack.Add(food, 0);

            }

        }

        public void AddFood(Food food)
        {

            foodSack[food] += 1;

        }

        public void RemoveFood(Food food)
        {

            if (!HasFood(food)) throw new ArgumentException("You dont have any of that food");

            foodSack[food] -= 1;

        }

        public bool HasFood(Food food)
        {

            if (foodSack[food] == 0) return false;

            return true;

        }

        public ref readonly Dictionary<Food, int> GetFoodSack()
        {

            return ref foodSack;

        }

    }

}
