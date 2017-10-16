using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour {
    // Data
    string[] wordsOne = { "blue", "pink", "green", "yellow", "purple", "orange" };
    string[] wordsTwo = { "duke", "coco", "tiger", "buddy", "bandit", "sunny" };
    string[] wordsThree = { "wells", "clarke", "gibson", "asimov", "bradbury", "heinlein" };
    string[] wordsFour = { "maypole", "wellwood", "spadina", "townsend", "nostrand", "divisadero" };

    // Initial settings
    int wordOnePos = 0;
    int wordTwoPos = 0;
    int wordThreePos = 0;
    int wordFourPos = 0;
    int tokens = 10;                        // Game currency
    int currentValue = 0;                   // Effective difficulty level & TOA factor [depreciated?]
    enum Screen { Menu, Help, Guess, Pass}  // Game state 
    Screen currentScreen;                   // Game state placeholder
    string scrambleWord;                    // Placeholder for current scramble-word

	// Use this for initialization
	void Start ()
    {
        ShowMenu(); // OnUserInput is the primary controller, initialize scene with ShowMenu()
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnUserInput(string input)
    {
        if (input == "menu") ShowMenu();
        else if (input == "?") ShowHelp();
        else if (currentScreen == Screen.Menu) HandleMenuInput(input);
        else if (currentScreen == Screen.Guess) HandleGuessInput(input);
        else if (currentScreen == Screen.Pass) HandlePassInput(input);
        else if (currentScreen == Screen.Help) HandleHelpInput(input);
    }

    private void HandleMenuInput(string input)
    {
        if (input == "1") Level(1);
        else if (input == "2") Level(2);
        else if (input == "3") Level(3);
        else if (input == "4") Level(4);
        else
        {
            ShowMenu();
            Terminal.WriteLine("Syntax Error: " + input);
        }

    }

    private void HandleGuessInput(string input)
    {
        Terminal.WriteLine("Guess Input: " + input);
    }

    private void HandlePassInput(string input)
    {
        Terminal.WriteLine("Pass Input: " + input);
    }

    private void HandleHelpInput(string input)
    {
        ShowHelp();
        Terminal.WriteLine("Syntax Error: " + input);
    }

    void ShowHelp()
    {
        currentScreen = Screen.Help;
        Terminal.ClearScreen();
        Terminal.WriteLine("New user assistance:");
        Terminal.WriteLine("This terminal can be controlled by entering --");
        Terminal.WriteLine("");
        Terminal.WriteLine("   ? - will display the user help.");
        Terminal.WriteLine("   menu - will display the Main Menu.");
        Terminal.WriteLine("   [#] - select menu options for further options.");
        Terminal.WriteLine("");
        Terminal.WriteLine(" * While descrambling security question answers it");
        Terminal.WriteLine("   costs specific TOA to make a guess, but if you");
        Terminal.WriteLine("   are correct you will double your TOA. If you");
        Terminal.WriteLine("   deplete your cache of TOA, you will be erased.");
        Terminal.WriteLine("\nPlease enter 'menu' at any time, or '?' for help.\n" +
                           "otherwise use the menu then make a selection [#].\n\n" +
                           "[TOA: " + tokens + "]");
    }

    void Level(int level)
        {
        currentValue = level;
        currentScreen = Screen.Guess;
        Terminal.ClearScreen();
        Terminal.WriteLine("GTHDB Level " + currentValue + " | TOA: " + tokens);
        if (currentValue == 1) Terminal.WriteLine("What is your favourite colour?");
        else if (currentValue == 2) Terminal.WriteLine("What is the name of your first pet?");
        else if (currentValue == 3) Terminal.WriteLine("Who is your favourite SciFi author?");
        else if (currentValue == 4) Terminal.WriteLine("What is the name of the street you grew up on?");
        Terminal.WriteLine("");
        Terminal.WriteLine("This group is worth " + currentValue + " TOA for each answer.");
        Terminal.WriteLine("Unscramble the answer to the security question:");
        Terminal.WriteLine("");
        ShowScramble(level);
    }

    void ShowScramble(int level)
    {
        Terminal.WriteLine("Scramble Level " + level + ": __TBD__");
    }

    void ShowMenu()
    {
        currentScreen = Screen.Menu;
        Terminal.ClearScreen();
        Terminal.WriteLine("Thank you for your assistance with our global\n" +
                           "distributed effort which we call 'Terminal-Hacker'\n" +
                           "You will be rewarded tokens of appreciation (TOA)\n" +
                           "for your success. Please work dilligently however,\n" +
                           "as failures will not be accomodated.\n");
        Terminal.WriteLine("Please descramble one of the following:\n" +
                           "  1) What is your favourite colour?\n" +
                           "  2) What is the name of your first pet?\n" +
                           "  3) Who is your favourite SciFi author?\n" +
                           "  4) Name of the street you grew up on?");
        Terminal.WriteLine("\nPlease enter 'menu' at any time, or '?' for help.\n" +
                           "otherwise, please make a menu selection [#].\n\n" +
                           "[TOA: " + tokens + "]");
    }
}
