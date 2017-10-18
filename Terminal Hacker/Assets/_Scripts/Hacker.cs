using System.Collections;
using UnityEngine;

public class Hacker : MonoBehaviour {
    // Data
    string[] wordsOne = 
        { "blue", "pink", "green", "yellow", "purple", "orange", "grey", "black", "white", "brown", "beige", "tan" };
    string[] wordsTwo = 
        { "duke", "coco", "tiger", "buddy", "bandit", "sunny", "shadow", "muffy", "lassie", "flipper", "rosco" };
    string[] wordsThree = 
        { "wells", "clarke", "gibson", "asimov", "bradbury", "heinlein", "stephenson", "sagan", "anthony" };
    string[] wordsFour = 
        { "maypole", "wellwood", "spadina", "townsend", "nostrand", "divisadero", "thames", "leaside", "marshall" };
    const int unlockFee_3 = 15;
    const int unlockFee_4 = 20;

    public GameObject keyboard;

    // Initial settings
    int tokens = 10;                                    // Game currency
    enum Screen
    { Menu, Help, Guess, Pass, Fail, Egg, Login }       // Game state enum
    Screen currentScreen;                               // Game state placeholder
    enum Access { Locked, Unlocked }                    // Access state enum
    Access levelThree = Access.Locked;                  // Access-state
    Access levelFour = Access.Locked;                   // Access-state
    int currentValue;                                   // Effective difficulty level
    string scrambleWord;                                // Placeholder for scramble-word

	void Start ()
    {
        keyboard.SetActive(false); // disable keyboard (user) input during 'boot-up sequence'
        StartCoroutine(ShowLoad()); // OnUserInput is the primary controller, see below:
    }

    void OnUserInput(string input)
    {
        if (currentScreen == Screen.Login) HandleLoginInput(input);
        else if (input.ToLower() == "load \"*\",8,1") StartCoroutine(ShowEasterEgg());
        else if (currentScreen == Screen.Egg) HandleEggInput(input);
        else if (tokens <= 0) ShowFail();
        else if (input.ToLower() == "menu") ShowMenu();
        else if (input == "?") ShowHelp();
        else if (currentScreen == Screen.Menu) HandleMenuInput(input);
        else if (currentScreen == Screen.Guess) HandleGuessInput(input);
        else if (currentScreen == Screen.Pass) HandlePassInput(input);
        else if (currentScreen == Screen.Help) HandleHelpInput(input);
        else if (currentScreen == Screen.Fail) HandleFailInput(input);
    }

    void HandleEggInput(string input)
    {
        if (input.ToLower() == "help") ShowEggHelp();
        else if (input.ToLower() == "exit") ShowMenu();
        else if (int.Parse(input) > 0 || int.Parse(input) < 0)
        {
            tokens += int.Parse(input);
            Terminal.WriteLine("[TOA: " + tokens + "]");
            Terminal.WriteLine("ENTER COMMAND:");
        }
        else StartCoroutine(ShowEasterEgg());
    }

    void HandleMenuInput(string input)
    {
        if (currentScreen == Screen.Fail) return;
        else if (input == "1") Level(1);
        else if (input == "2") Level(2);
        else if (input == "3")
        {
            if (levelThree == Access.Locked)
            {
                if (tokens == unlockFee_3)
                {
                    ShowMenu();
                    Terminal.WriteLine("Unable to deplete TOA to zero.");
                }
                else if (tokens > unlockFee_3)
                {
                    tokens -= unlockFee_3;
                    levelThree = Access.Unlocked;
                    ShowMenu();
                }
                else
                {
                    ShowMenu();
                    Terminal.WriteLine("Insufficient tokens.");
                }
            }
            else Level(3);
        }
        else if (input == "4")
        {
            if (levelFour == Access.Locked)
            {
                if (tokens == unlockFee_4)
                {
                    ShowMenu();
                    Terminal.WriteLine("Unable to deplete TOA to zero.");
                }
                else if (tokens > unlockFee_4)
                {
                    tokens -= unlockFee_4;
                    levelFour = Access.Unlocked;
                    ShowMenu();
                }
                else
                {
                    ShowMenu();
                    Terminal.WriteLine("Insufficient tokens.");
                }
            }
            else Level(4);
        }
        else
        {
            ShowMenu();
            Terminal.WriteLine("Syntax Error: " + input);
        }
    }

    void HandleGuessInput(string input)
    {
        Terminal.WriteLine("Guess Input: " + input);
        if (input.ToLower() == scrambleWord)
        {
            tokens += (currentValue * 2);
            currentScreen = Screen.Pass;
            ShowReward(currentValue);
            Terminal.WriteLine("Congratulations! You've earned " + (currentValue * 2) + " TOA.");
            Terminal.WriteLine("(reminder, you may enter 'menu' or '?' at any time)");
            Terminal.WriteLine("[TOA: " + tokens +"]");
        }
        else
        {
            tokens -= currentValue;
            //                                                                                              |
            Terminal.WriteLine("Yikes! You've lost " + currentValue + " TOA!");
            Terminal.WriteLine("...be sure to not lose them all!");
            Terminal.WriteLine("(reminder, you may enter 'menu' or '?' at any time)");
            Terminal.WriteLine("[TOA: " + tokens + "]");
        }
    }

    void HandlePassInput(string input)
    {
        Terminal.WriteLine("Pass Input: " + input);
        Terminal.WriteLine("\nPlease enter 'menu' at any time, or '?' for help.\n" +
                           "Otherwise use the menu, then enter a selection '#'.\n\n" +
                           "[TOA: " + tokens + "]");
    }

    void HandleHelpInput(string input)
    {
        ShowHelp();
        Terminal.WriteLine("Syntax Error: " + input);
    }

    void HandleFailInput(string input)
    {
        ShowFail();
        Terminal.WriteLine("Syntax Error: " + input);
    }

    void HandleLoginInput(string input)
    {
        return;
    }

    IEnumerator ShowLoad() // Coroutine to simulate computer booting-up.
    {
        currentScreen = Screen.Login;
        Terminal.ClearScreen();
        //                 |                                                        |
        yield return new WaitForSeconds(.2f);
        Terminal.WriteLine("           **** COMMODORE 64 BASIC v4.20 ****");
        yield return new WaitForSeconds(1.8f); 
        Terminal.WriteLine("");
        Terminal.WriteLine("         64K RAM SYSTEM  33710 BASIC BYTES FREE");
        yield return new WaitForSeconds(1.3f);
        Terminal.WriteLine("");
        Terminal.WriteLine("READY.");
        yield return new WaitForSeconds(.8f);
        Terminal.WriteLine("LOAD \"GTHDB.PRG\",8,1");
        Terminal.WriteLine("");
        yield return new WaitForSeconds(.7f);
        Terminal.WriteLine("SEARCHING FOR GTHDB.PRG");
        yield return new WaitForSeconds(.3f);
        Terminal.WriteLine("LOADING");
        yield return new WaitForSeconds(.6f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(.8f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1.2f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1.4f);
        StartCoroutine(ShowLogin());
    }

    IEnumerator ShowLogin() // Coroutine to simulate sutomatic computer login.
    {
        currentScreen = Screen.Login;
        Terminal.ClearScreen();
        //                 |                                                        |
        Terminal.WriteLine("GTHDB release 13.2.7 (GrubbyPaws Update 3)");
        Terminal.WriteLine("Kernel 2.6.5-21.EL on VIC-64");
        yield return new WaitForSeconds(1.2f);
        Terminal.WriteLine("LOGIN (guest):");
        yield return new WaitForSeconds(1.4f);
        Terminal.WriteLine("PASSWORD:");
        Terminal.WriteLine("");
        yield return new WaitForSeconds(1.6f);
        keyboard.SetActive(true); // reactivate the keyboard!
        ShowMenu(); // Light-show is over, start the game now.
    }

    void ShowMenu() // Primary game interface
    {
        currentScreen = Screen.Menu;
        Terminal.ClearScreen();
        //                 |                                                        |
        Terminal.WriteLine("                    GTHDB: Main Menu");
        Terminal.WriteLine("          [access at anytime by entering 'menu']");
        Terminal.WriteLine("");
        Terminal.WriteLine("Earn tokens of appreciation (TOA) as rewards for your");
        Terminal.WriteLine("success. Please work dilligently however, as failures");
        Terminal.WriteLine("will not be accomodated.");
        Terminal.WriteLine("");
        Terminal.WriteLine("  1) What is your favourite colour?");
        Terminal.WriteLine("  2) What is the name of your first pet?");
        if (levelThree == Access.Locked)
        {
            Terminal.WriteLine("  3) Unlock with 15 TOA.");
        }
        else Terminal.WriteLine("  3) Who is your favourite SciFi author?");
        if (levelFour == Access.Locked)
        {
            Terminal.WriteLine("  4) Unlock with 20 TOA.");
        }
        else Terminal.WriteLine("  4) What is the name of the street you grew up on?");
        Terminal.WriteLine("");
        Terminal.WriteLine("Please enter '?' any time for help, otherwise, please");
        Terminal.WriteLine("select a security question to descramble their answers.");
        Terminal.WriteLine("");
        Terminal.WriteLine("[TOA: " + tokens + "]");
    }

    void ShowReward(int level) // Display ASCII-art rewards for de-scrambles
    {
        switch (level)
        {
            case 1:
                Terminal.WriteLine("                        .----,");
                Terminal.WriteLine("                       /--._(");
                Terminal.WriteLine("                       |____|");
                Terminal.WriteLine("                       [____] .=======.");
                Terminal.WriteLine("                         YY   q.     .p");
                Terminal.WriteLine("                         ||   | `---' |");
                Terminal.WriteLine("                         []   |_______|");
                Terminal.WriteLine("");
                break;
            case 2:
                Terminal.WriteLine("                        |\\__/|");
                Terminal.WriteLine("                        (_^-^)");
                Terminal.WriteLine("                   _     )  (");
                Terminal.WriteLine("                  ((  __/    \\    ( ( (");
                Terminal.WriteLine("                   (   ) ||  ||   ) ) )");
                Terminal.WriteLine("                   '---''--''--'  >+++°>");
                Terminal.WriteLine("");
                break;
            case 3:
                Terminal.WriteLine("                            *");
                Terminal.WriteLine("                           /_\\");
                Terminal.WriteLine("                           | |");
                Terminal.WriteLine("                           |_|");
                Terminal.WriteLine("                           | |");
                Terminal.WriteLine("                           )_(");
                Terminal.WriteLine("                          /| |\\");
                Terminal.WriteLine("                         /_|_|_\\");
                Terminal.WriteLine("");
                break;
            case 4:
                Terminal.WriteLine("                        ,dP\"\"d8b,");
                Terminal.WriteLine("                       d\"   d88\"8b");
                Terminal.WriteLine("                      I8    Y88a88)");
                Terminal.WriteLine("                      `Y, a  )888P");
                Terminal.WriteLine("                        \"b,,a88P\"");
                Terminal.WriteLine("");
                Terminal.WriteLine("                  You are at peak Zen!");
                Terminal.WriteLine("        Now would be a good time for a vacation!");
                //                 |                                                        |
                Terminal.WriteLine("");
                break;
            default:
                Debug.Log("fell to default case in ShowReward().");
                break;
        }
    }

    IEnumerator ShowEasterEgg() // 
    {
        currentScreen = Screen.Egg;
        Terminal.ClearScreen();
        yield return new WaitForSeconds(.3f);
        Terminal.WriteLine("LOAD \"*\",8,1");
        yield return new WaitForSeconds(1.4f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1.4f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1.4f);
        Terminal.WriteLine(".");
        yield return new WaitForSeconds(1.4f);
        Terminal.ClearScreen();
        //                 |                                                        |
        Terminal.WriteLine("        Distributed Social Hacking Tool v3.95f02");
        Terminal.WriteLine("          [enter 'help' or 'exit' at any time]");
        Terminal.WriteLine("");
        Terminal.WriteLine("");
        Terminal.WriteLine("[TOA: " + tokens + "]");
        Terminal.WriteLine("ENTER COMMAND:");
    }

    void ShowEggHelp()
    {
        Terminal.ClearScreen();
        Terminal.WriteLine("        Distributed Social Hacking Tool v3.95f02");
        Terminal.WriteLine("");
        Terminal.WriteLine("Did you forget what you put me for here, boss?");
        Terminal.WriteLine("Okay, okay, I'll give you a hint... Do you have enough");
        Terminal.WriteLine("TOA? If you forgot, you can always get back to the main");
        //                 |                                                        |
        Terminal.WriteLine("menu at any time by entering 'exit', otherwise, gimme a"); 
        Terminal.WriteLine("number, already!");
        Terminal.WriteLine("");
        Terminal.WriteLine("[TOA: " + tokens + "]");
        Terminal.WriteLine("ENTER COMMAND:");
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
        //                 |                                                        |
        Terminal.WriteLine(" * While descrambling security question answers it costs");
        Terminal.WriteLine("   specific TOA to make a guess, but if you are correct,");
        Terminal.WriteLine("   then you will double your TOA! Note: If you manage to");
        Terminal.WriteLine("   deplete your cache of TOA, you will be erased.");
        Terminal.WriteLine("\nPlease enter 'menu' at any time, or '?' for help.\n" +
                           "Otherwise use the menu then make a selection [#].\n\n" +
                           "[TOA: " + tokens + "]");
    }

    void ShowFail()
    {
        Terminal.WriteLine("\nIt seems you have run out of TOA. If I were in\n" +
                   "your shoes, I'd be making myself well, scarce...\n" +
                   "now!\n\n" +
                   "[TOA: " + tokens + "]");
    }

    void ShowScramble(int level)
    {
        if (level == 1) scrambleWord = wordsOne[Random.Range(0, wordsOne.Length)];
        else if (level == 2) scrambleWord = wordsTwo[Random.Range(0, wordsTwo.Length)];
        else if (level == 3) scrambleWord = wordsThree[Random.Range(0, wordsThree.Length)];
        else if (level == 4) scrambleWord = wordsFour[Random.Range(0, wordsFour.Length)];
        string currentWord = scrambleWord.Anagram();
        Terminal.WriteLine("Scramble Level " + level + ": " + currentWord);
    }

    void Level(int level)
        {
        currentValue = level;
        if (tokens < currentValue)
        {
            ShowMenu();
            Terminal.WriteLine("You lack the tokens to make any guesses at that level.");
            return;
        }
        currentScreen = Screen.Guess;
        Terminal.ClearScreen();
        Terminal.WriteLine("GTHDB Level " + currentValue + " | TOA: " + tokens);
        Terminal.WriteLine("This group is worth " + currentValue + " TOA for each guess.");
        Terminal.WriteLine("Unscramble the answer to the security question:");
        Terminal.WriteLine("");
        if (currentValue == 1) Terminal.WriteLine("What is your favourite colour?");
        else if (currentValue == 2) Terminal.WriteLine("What is the name of your first pet?");
        else if (currentValue == 3) Terminal.WriteLine("Who is your favourite SciFi author?");
        else if (currentValue == 4) Terminal.WriteLine("What is the name of the street you grew up on?");
        ShowScramble(level);
    }
}
