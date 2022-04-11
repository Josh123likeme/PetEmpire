using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPets2
{
    public class Game
    { 

        private char[,] board;

        private static Random random = new Random();

        private Inventory inventory;
        private Inventory newFood = new Inventory();

        private int x,y;
        private int energy;

        private bool foundEgg = false;

        public Game(int level, Inventory inventory)
        {

            this.inventory = inventory;

            board = new char[random.Next(level * 7) + 5, random.Next(level * 10) + 5];

            x = random.Next(board.GetLength(1));
            y = random.Next(board.GetLength(0));

            energy = (board.GetLength(1) * 2) - 4;

            int numberOfFood = random.Next(level * 2, level * 5);

            for (int i = 0; i < numberOfFood; i++)
            {

                board[random.Next(board.GetLength(0)), random.Next(board.GetLength(1))] = 'x';

            }

            board[random.Next(board.GetLength(0)), random.Next(board.GetLength(1))] = 'e';

        }

        public bool Play()
        {

            Render();

            while (energy > 0)
            {

                if (board[y,x] == 'x')
                {

                    Food foodToAdd = (Food)Enum.GetValues(typeof(Food)).GetValue(random.Next(Enum.GetValues(typeof(Food)).Length));

                    inventory.AddFood(foodToAdd);
                    newFood.AddFood(foodToAdd);

                    board[y, x] = '\0';

                }
                else if (board[y, x] == 'e')
                {

                    foundEgg = true;

                    board[y, x] = '\0';

                }

                ConsoleKey key = Console.ReadKey().Key;

                switch (key)
                {

                    case ConsoleKey.W:
                        y = (y - 1 >= 0) ? y - 1 : y;
                        break;
                    case ConsoleKey.A:
                        x = (x - 1 >= 0) ? x - 1 : x;
                        break;
                    case ConsoleKey.S:
                        y = (y + 1 < board.GetLength(0)) ? y + 1 : y;
                        break;
                    case ConsoleKey.D:
                        x = (x + 1 < board.GetLength(1)) ? x + 1 : x;
                        break;

                }

                energy--;

                Render();

            }

            Console.Clear();

            Console.WriteLine("You got:");

            foreach (Food food in newFood.GetFoodSack().Keys)
            {

                if (!inventory.HasFood(food)) continue;

                Console.WriteLine(food.GetDisplayName() + ": " + newFood.GetFoodSack()[food]);

            }

            if (foundEgg) Console.WriteLine("You found an egg!");

            Console.ReadKey();

            return foundEgg;

        }

        private void Render()
        {  

            StringBuilder content = new StringBuilder();

            for (int i = 0; i < board.GetLength(1) + 2; i++)
            {

                content.Append("██");

            }

            content.Append("\n");

            for (int row = 0; row < board.GetLength(0); row++)
            {

                content.Append("██");

                for (int column = 0; column < board.GetLength(1); column++)
                {

                    if (row == y && column == x)
                    {

                        content.Append("ME");

                        continue;

                    }

                    switch (board[row, column])
                    {

                        case 'x':
                            content.Append("x ");
                            break;
                        case 'e':
                            content.Append("e ");
                            break;

                        default:
                            content.Append("  ");
                            break;

                    }
                    


                }

                content.Append("██\n");

            }

            for (int i = 0; i < board.GetLength(1) + 2; i++)
            {

                content.Append("██");

            }

            content.Append("\nEnergy: ");

            for (int i = 0; i < energy; i++)
            {

                content.Append("I");

            }

            Console.Clear();

            Console.WriteLine(content);

        }

    }

}
