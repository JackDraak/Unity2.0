using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour {
    int tokens = 10;                        // Game currency
    int level = 0;                          // Game state -- depreciated?
    int currentValue = 0;                   // Effective difficulty level
    enum Screen { Menu, Help, Guess, Pass}  // Game state 
    Screen currentScreen;

	// Use this for initialization
	void Start ()
    {
        ShowMenu();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnUserInput(string input)
    {
        if (input == "menu") ShowMenu();
        else if (currentScreen == Screen.Menu) HandleMenuInput(input);
    }

    private void HandleMenuInput(string input)
    {
        if (input == "1") LevelOne();
        else if (input == "2") LevelTwo();
        else
        {
 /*           if (level == -1)
            {
                ShowHelp();
                Terminal.WriteLine("Syntax Error: " + input);
            } */
            if (level == 0)
            {
                ShowMenu();
                Terminal.WriteLine("Syntax Error: " + input);
            }
            else if (level == 1)
            {
                LevelOne();
                Terminal.WriteLine("Syntax Error: " + input);
            }
            else if (level == 2)
            {
                LevelTwo();
                Terminal.WriteLine("Syntax Error: " + input);
            }
        }
    }

/*    void ShowHelp()
    {
        level = -1;
        currentScreen = Screen.Help;
        Terminal.ClearScreen();
        Terminal.WriteLine("New user assistance:");
        Terminal.WriteLine("");
        Terminal.WriteLine("This terminal can be controlled by entering --");
        Terminal.WriteLine("");
        Terminal.WriteLine("   ? - will display the user help.");
        Terminal.WriteLine("   menu - will display the Main Menu.");
        Terminal.WriteLine("   [#] - select menu options for further options.");
        Terminal.WriteLine("");
    }
*/

    void LevelOne()
    {
        level = 1;
        currentValue = level;
        currentScreen = Screen.Guess;
        Terminal.ClearScreen();
        Terminal.WriteLine("GTHDB Level One:\n" +
                           "What is your favourite colour?");
        Terminal.WriteLine("");
        Terminal.WriteLine("(This group is worth " + currentValue + " TOA for each answer)");
        Terminal.WriteLine("");
    }

    void LevelTwo()
    {
        level = 2;
        currentValue = level;
        currentScreen = Screen.Guess;
        Terminal.ClearScreen();
        Terminal.WriteLine("GTHDB Level Two:\n" +
                           "What is the name of your first pet?");
        Terminal.WriteLine("");
        Terminal.WriteLine("(This group is worth " + currentValue + " TOA for each answer)");
        Terminal.WriteLine("");
    }

    void ShowMenu()
    {
        level = 0;
        currentScreen = Screen.Menu;
        Terminal.ClearScreen();
        //  ........10........20........30........40........50........60........70........80
        //  Terminal.WriteLine("0123456789012345678901234567890123456789---3456789012345678901234567890123456789");
        // Terminal.WriteLine("Welcome, " + user + ", to the GTHDB.\n");
        Terminal.WriteLine("Thank you for your assistance with our global\n" +
                           "distributed effort which we call 'Terminal-Hacker'\n" +
                           "You will be rewarded tokens of appreciation (TOA)\n" +
                           "for your success. Please work dilligently however,\n" +
                           "as failures will not be accomodated.\n");
        Terminal.WriteLine("Please descramble one of the following:\n\n" +
                           "  1) What is your favourite colour?\n" +
                           "  2) What is the name of your first pet?\n"); // +
                   //        "  3) Who is your favourite SciFi author?\n" +
                   //        "  4) Name of the street you grew up on?");
        Terminal.WriteLine("\n" +
                           "[TOA: " + tokens + "]"); // TODO replace with actual TOA count
    }
}
