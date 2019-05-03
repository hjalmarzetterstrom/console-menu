using System;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
    /// <summary>
    /// Prompt kick-ass menus or lists in a console application.
    /// </summary>
    public static class Menu
    {
        /// <summary>
        /// Prompts a menu in the console window. Select option with number keys. <i><b>NOTE: </b>Limited to 9 options!</i>
        /// </summary>
        /// <param name="title">Title at the top of the menu.</param>
        /// <param name="options">All the options in the menu.</param>
        /// <returns>Integer depending on optin picked by user. ZERO-based</returns>
        public static int ShowMenu(string title, params string[] options)
        {
            Console.CursorVisible = false;

            bool choiceMade = false;
            char tempChoice = '\0';
            int choice = 0;

            UpdateMenu(title, options);

            while (!choiceMade)
            {
                PromptMenu(title, options, ref choiceMade, ref tempChoice, ref choice);
            }

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            //Make the return value 0-based.
            choice--;

            return choice;
        }

        static void UpdateMenu(string title, string[] options)
        {
            Console.Clear(); //Clear the window to make sure options are selected correctly.

            Console.WriteLine(title + "\n-------------------");

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i + 1, options[i]);
            }

            Console.WriteLine("-------------------");
        }

        static void PromptMenu(string title, string[] options, ref bool choiceMade, ref char tempChoice, ref int choice)
        {
            if ((tempChoice = Console.ReadKey(true).KeyChar) != '\r')
            {
                if (int.TryParse(tempChoice.ToString(), out choice) && choice.Between(0, options.Length))
                {
                    UpdateMenu(title, options, choice);
                }
            }
            else
            {
                choiceMade = choice != 0;
            }
        }

        static void UpdateMenu(string title, string[] options, int choice)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(title + "\n-------------------");

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i + 1, options[i]);
            }

            Console.WriteLine("-------------------");

            Console.ForegroundColor = ConsoleColor.Green;

            if (choice != 0)
            {
                Console.SetCursorPosition(0, 1 + choice);
                Console.Write("{0} - {1}", choice, options[choice - 1]);
            }
        }

        /// <summary>
        /// Prompts a list of options in the console window. Select option by typing and searching and/or the arrow keys.
        /// </summary>
        /// <param name="title">Title at the top of the menu.</param>
        /// <param name="options">All the options in the menu.</param>
        /// <returns>Integer depending on optin picked by user. ZERO-based</returns>
        public static int ShowList(string title, params string[] options)
        {
            UpdateList(title, options);

            int choice = PromptList(title, options);

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            return choice;
        }

        static int PromptList(string title, string[] options)
        {
            bool pressedEnter = false;
            bool choiceMade = false;
            char currentInput = '\0';
            StringBuilder searchTerm = new StringBuilder();
            string match = "";
            int choice = 0;
            int position = 3;
            bool matchFound = false;
            ConsoleKeyInfo input;

            while (!pressedEnter || !choiceMade)
            {
                //reset position if entire searchTerm has been backspaced.
                if (searchTerm.ToString() == "")
                    position = 3;

                Console.SetCursorPosition(title.Length + searchTerm.Length, 0);
                input = Console.ReadKey(true);

                if (input.KeyChar != '\0')
                {
                    currentInput = input.KeyChar;
                    pressedEnter = currentInput == '\r';
                    choiceMade = searchTerm.Length > 0;

                    if (!pressedEnter)
                    {
                        FindInputMatch(title, options, currentInput, searchTerm, ref match, ref position, ref matchFound, out choice);
                    }
                }
                else
                {
                    if (input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow)
                        ArrowKeyInput(title, options, ref match, ref position, ref input, ref searchTerm, out choice);

                    matchFound = false;
                }
            }
            return choice;
        }

        static void FindInputMatch(string title, string[] options, char currentInput, StringBuilder searchTerm, ref string match, ref int position, ref bool matchFound, out int choice)
        {
            bool clickedBackspace = CheckBackspace(title, options, currentInput, searchTerm, ref match, position, ref matchFound);

            //Determine whether the input has a valid match...
            if (!matchFound && !clickedBackspace)
            {
                FindFirstMatch(options, currentInput, searchTerm, ref match, ref position, ref matchFound);
            }
            else if (currentInput != '\b')
            {
                CheckIfStillMatch(title, options, currentInput, searchTerm, ref match, ref position);
            }

            //..update list accordingly.
            if (matchFound && searchTerm.Length < match.Length + 1)
            {
                UpdateList(title, match, searchTerm.Length, position);
            }

            choice = Array.IndexOf(options, match);
        }

        static void ArrowKeyInput(string title, string[] options, ref string match, ref int position, ref ConsoleKeyInfo input, ref StringBuilder searchTerm, out int choice)
        {
            choice = 0;

            if (match == "")
            {
                match = options.First();
                UpdateList(title, match, match.Length, position);
                searchTerm = new StringBuilder(match);
            }
            else
            {

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (match != options.First())
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, position);
                            Console.Write(match);

                            choice = position - 4;
                            match = options[choice];
                            UpdateList(title, match, match.Length, --position);
                            searchTerm = new StringBuilder(match);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (match != options.Last())
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, position);
                            Console.Write(match);

                            choice = position - 2;
                            match = options[choice];
                            UpdateList(title, match, match.Length, ++position);
                            searchTerm = new StringBuilder(match);
                        }
                        break;
                }
            }
        }

        static void CheckIfStillMatch(string title, string[] options, char currentInput, StringBuilder searchTerm, ref string match, ref int position)
        {
            searchTerm.Append(currentInput);

            if (match.TakeUntill(searchTerm.Length).ToLower() != searchTerm.ToString().ToLower())
            {
                bool newMatchFound = FindNewMatch(title, options, searchTerm, ref match, ref position);

                if (!newMatchFound)
                {
                    Console.SetCursorPosition(title.Length, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(match.TakeUntill(searchTerm.Length - 1) + searchTerm.ToString().Last());
                    System.Threading.Thread.Sleep(50);

                    searchTerm.Remove(searchTerm.Length - 1, 1);
                }
            }
        }

        static bool FindNewMatch(string title, string[] options, StringBuilder searchTerm, ref string match, ref int position)
        {
            UpdateList(title, options);

            int newPosition = 3;
            bool newMatchFound = false;

            foreach (var item in options)
            {
                if (item.TakeUntill(searchTerm.Length).ToLower() == searchTerm.ToString())
                {
                    match = item;
                    position = newPosition;
                    newMatchFound = true;

                    break;
                }
                else
                {
                    newPosition++;
                }
            }
            return newMatchFound;
        }

        static void FindFirstMatch(string[] options, char currentInput, StringBuilder searchTerm, ref string match, ref int position, ref bool matchFound)
        {
            position = 3;

            foreach (string str in options)
            {
                if (char.ToLower(str[0]) == char.ToLower(currentInput))
                {
                    match = str;
                    matchFound = true;
                    searchTerm.Append(currentInput);

                    break;
                }
                else
                {
                    position++;
                }
            }
        }

        static bool CheckBackspace(string title, string[] options, char currentInput, StringBuilder searchTerm, ref string match, int position, ref bool matchFound)
        {
            if (currentInput == '\b' && searchTerm.Length > 0)
            {
                searchTerm.Remove(searchTerm.Length - 1, 1);

                if (searchTerm.Length == 0)
                {
                    matchFound = false;
                    UpdateList(title, options);
                    match = "";
                }
                else
                {
                    UpdateList(title, match, searchTerm.Length, position);
                    matchFound = true;
                }
            }

            return currentInput == '\b';
        }

        static void UpdateList(string title, string match, int breakPoint, int position)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            //Clear the first row and write out the title only.
            Console.SetCursorPosition(0, 0);
            Console.Write(title.PadRight(Console.BufferWidth, ' '));

            //Write out the search term next to the title.
            Console.SetCursorPosition(title.Length, 0);
            Console.Write(match.Substring(0, breakPoint));

            //Write out the rest of the matching word.
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(match.Substring(breakPoint));

            Console.ForegroundColor = ConsoleColor.Green;

            //Write out the search term in the list.
            Console.SetCursorPosition(0, position);
            Console.Write(match.Substring(0, breakPoint));

            //write out the rest of the word.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(match.Substring(breakPoint));
        }

        static void UpdateList(string title, string[] options)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(title);
            Console.WriteLine("\n-------------------");

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(options[i]);
            }

            Console.WriteLine("-------------------");

            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
