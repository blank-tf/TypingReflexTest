// Author: Griffin Thompson
// License: MIT License
// written in February 2021

using System;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TypingReflexGame
{
    enum letters
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        NULL
    }
    enum difficulty
    {
        PREDICTABLE,
        RANDOM
    }
    class Program
    {
        public static System.Timers.Timer reflexTimer;
        public static double score = 0;
        public static double timeSinceStarting = 0.00;
        static void Main(string[] args)
        {
            // Checks our platform and then resizes the temrinal based on that
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                resizeScreen(40, 100);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // weierd console sequence that sets the console size on osx
                Console.WriteLine(@"\e[8;100;40t");
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowWidth = 40;
                Console.WindowHeight = 100;
            } else //unknown platform
            {
                Console.Error.WriteLine("Unknown platform, halting!");
                return;
            }
            resizeScreen(40, 100);
            loadingScreen(5);
            startScreen();
            var gameDifficulty = difficultySelect();
            gameLoop((difficulty)gameDifficulty);
        }
        static void resizeScreen(uint COLUMNS, uint ROWS)
        {
            if (COLUMNS < 20 || COLUMNS > 80)
            {
                throw new ArgumentOutOfRangeException("Columns too high or low!");
            } else if (ROWS < 100 || ROWS > 240)
            {
                throw new ArgumentOutOfRangeException("Rows too high or low!");
            }

            // calling the resize command on linux. not sure if it's present on all distros; probably not
            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/resize";
            proc.StartInfo.Arguments = String.Format("-s {0} {1}", COLUMNS, ROWS);
            proc.Start();
            proc.WaitForExit();

            Console.Clear();
        }
        static void loadingScreen(uint loadingDuration)
        {
            // 1 full second divided by the sum of all sleeps.
            // This is so when you plug in a loading Duration,
            // you can just ask for seconds instead of something
            // arbitrary likes number of cycles.
            //
            // e.x 1 (second) / 0.5 (sum of all thread.sleep) = 2.5
            double loadingTimeCompensated = loadingDuration * 2.5;
            for (double i = 0; i < loadingTimeCompensated; i++)
            {
                Console.SetCursorPosition(50, 0);
                Console.Write("|");
                Thread.Sleep(100);

                Console.SetCursorPosition(50, 0);
                Console.Write("/");
                Thread.Sleep(100);

                Console.SetCursorPosition(50, 0);
                Console.Write("-");
                Thread.Sleep(100);

                Console.SetCursorPosition(50, 0);
                Console.Write("\\");
                Thread.Sleep(100);
            }
            Console.SetCursorPosition(0, 0);
        }
        /// <summary>
        /// Prints our start screen. I didn't need to put this in
        /// a function, but it looks tidier than using #region imo
        /// </summary>
        static void startScreen()
        {
            Console.WriteLine(
@"/\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\
//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//
//\\ \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/ //\\
\\//                                                        \\//
 \/                                                          \/
 /\                                                          /\
//\\                   Typing Reflex Test                   //\\
\\//                Author: Griffin Thompson                \\//
//\\             Licensed under the MIT License             //\\
\\//                                                        \\//
 \/                                                          \/
 /\                                                          /\
//\\                                                        //\\
\\//                                                        \\//
//\\                 Press any key to start                 //\\
\\//                                                        \\//
 \/                                                          \/
 /\                                                          /\
//\\                                                        //\\
\\// /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\  /\ \\//
//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//
 \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \/  \//");
            Console.SetCursorPosition(43, 15);
            Console.ReadKey();
            Console.Clear();
        } 
        static int difficultySelect()
        {
            bool difficultySelectDone = false;
            difficulty predictableOrRandom = difficulty.PREDICTABLE;
            while (!difficultySelectDone)
            {
                Console.Clear();

                Console.SetCursorPosition(43, 12);
                Console.Write("(1) Predictable");

                Console.SetCursorPosition(9, 14);
                Console.Write("(2) Random - The amount of time that passes before the letter is shown is random.");
                Console.SetCursorPosition(13, 15);
                Console.Write("Introduces more difficulty.");
                Console.SetCursorPosition(40, 10);
                Console.Write("Choose difficulty: ");
                
                var answer = Console.ReadKey();

                if (answer.Key == ConsoleKey.D1 || answer.Key == ConsoleKey.NumPad1)
                {
                    Console.Clear();

                    Console.SetCursorPosition(18, 10);
                    Console.Write("Are you sure you want your difficulty to be: predictable? (y/n): ");

                    var confirm = Console.ReadKey();

                    if (confirm.Key == ConsoleKey.Y)
                    {
                        predictableOrRandom = difficulty.PREDICTABLE;
                        difficultySelectDone = true;
                    }

                } else if (answer.Key == ConsoleKey.D2 || answer.Key == ConsoleKey.NumPad2)
                {
                    Console.Clear();

                    Console.SetCursorPosition(20, 10);
                    Console.Write("Are you sure you want your difficulty to be: random? (y/n): ");

                    var confirm = Console.ReadKey();

                    if (confirm.Key == ConsoleKey.Y)
                    {
                        predictableOrRandom = difficulty.RANDOM;
                        difficultySelectDone = true;
                    }

                } else
                {
                    Console.WriteLine("Invalid option chosen.");
                }
            }
            Console.Clear();

            Console.SetCursorPosition((predictableOrRandom == difficulty.PREDICTABLE ? 39 : 42), 10);
            Console.Write($"You chose {(predictableOrRandom == difficulty.PREDICTABLE ? "PREDICTABLE" : "RANDOM")}");

            Thread.Sleep(2500);
            return (predictableOrRandom == difficulty.PREDICTABLE ? 0 : 1);
        }
        static void gameLoop(difficulty gameDifficulty)
        {
            Console.Clear();

            Console.SetCursorPosition(16, 20);
            Console.Write("The game is very simple, a single letter will pop up on the screen,");

            Console.SetCursorPosition(24, 21);
            Console.Write("your goal is to click the key as soon as you see it.");

            Console.SetCursorPosition(18, 22);
            Console.Write("The faster you hit it, the more score you get. You get 5 tries.");

            Console.SetCursorPosition(33, 24);
            Console.WriteLine("Press any key when you're ready...");

            Console.ReadKey();

            for (int i = 0; i < 5; i++)
            {
                randomLetter(gameDifficulty);
            }
            Console.WriteLine("Your final score is {0}", score);
            Console.ReadKey();

        }
        static void randomLetter(difficulty gameDifficulty)
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            int random = rand.Next(0, 25);

            var expectedLetter = (letters)random;

            timeSinceStarting = 0.00;
            reflexTimer = new System.Timers.Timer();
            reflexTimer.Interval = 50;
            reflexTimer.Elapsed += OnTimedEvent;
            reflexTimer.AutoReset = true;
            reflexTimer.Start();

            while (true)
            {
                if (gameDifficulty == difficulty.RANDOM)
                {
                    var randWait = new Random((int)DateTime.Now.Ticks);
                    Thread.Sleep(randWait.Next(300, 4000));
                }
                Console.Clear();

                Console.SetCursorPosition(46, 20);
                Console.Write($"Enter {(ConsoleKey)random + 65}!");

                Console.SetCursorPosition(0, 39);
                 Console.Write($"{timeSinceStarting:F2}s");
                
                var keyEntered = Console.ReadKey();

                if (keyEntered.Key == (ConsoleKey)random + 65)
                {
                    reflexTimer.Stop();
                    break;
                } else if (timeSinceStarting > 4)
                {
                    reflexTimer.Stop();
                    Console.WriteLine("you didn't hit the key in time!");
                    break;
                }
            }
            var scoreToAdd = (1000 - (timeSinceStarting * 250));
            scoreToAdd = (scoreToAdd < 0 ? 0 : scoreToAdd);
            score += scoreToAdd;


            Console.Clear();

            Console.SetCursorPosition(36, 20);
            Console.Write((scoreToAdd > 0) ? $"Nice job, you got {Math.Round(scoreToAdd)} points!" : "That's too bad, you didn't get any points!");

            if (timeSinceStarting < 4)
            {
                Console.SetCursorPosition(31, 21);
                Console.Write($"Your reaction speed was {timeSinceStarting:F2}s. Not bad.");
            }
            else
            {
                Console.SetCursorPosition(18, 21);
                Console.Write($"Your reaction time was {timeSinceStarting:F2}s. I know you can to better than that!");
            }
            Thread.Sleep(500);

            Console.SetCursorPosition(30, 22);
            Console.Write("Starting next challenge in 5 seconds...");
            Thread.Sleep(250);

            Console.SetCursorPosition(50, 23);
            Console.Write("5");
            Thread.Sleep(1000);

            Console.SetCursorPosition(50, 23);
            Console.Write("4");
            Thread.Sleep(1000);

            Console.SetCursorPosition(50, 23);
            Console.Write("3");
            Thread.Sleep(1000);

            Console.SetCursorPosition(50, 23);
            Console.Write("2");
            Thread.Sleep(1000);

            Console.SetCursorPosition(50, 23);
            Console.Write("1");
            Thread.Sleep(1000);
        }

        
        public static void OnTimedEvent(object source, ElapsedEventArgs e) // honestly not sure if this is the good way to keep track of timing for this sort of thing. I think
        {                                                                  // I could use async and await to roll my own sort of thing but I have no idea how to do that.
            timeSinceStarting += 0.05;
        }
    }
}
