using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPets2
{

    public enum Food
    {

        Cheap_Fish = 1,
        Fancy_Fish = 3,

    }

    static class FoodMethods
    {
        
        public static string GetDisplayName(this Food food)
        {

            string name = "";

            foreach (char letter in Enum.GetName(typeof (Food), food))
            {

                if (letter == '_') name += " ";
                else name += letter;

            }

            return name;

        }

    }

}
