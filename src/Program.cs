// Author: Griffin Thompson
// License: MIT License
// written in February 2021

// NOTE: I understand that all of my Console.SetCursorPositions have magic numbers,
// they're just used for centering text by doing 
// (ConsoleWidth / 2) - (strlen(what we're going to write) / 2)

using System;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TypingReflexGame
{
    enum Difficulty
    {
        Predictable,
        Random
    }
    class Program
    {
        static System.Timers.Timer _reflexTimer;
        static double _score = 0;
        static double _timeSinceStarting;
        static void Main()
        {
            // Checks our platform and then resizes the temrinal based on that
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {
                ResizeScreen(columns: 40, rows: 100);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                // weierd console sequence that sets the console size on osx
                Console.WriteLine(@"\e[8;100;40t");
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.WindowWidth = 40;
                Console.WindowHeight = 100;
            } else {
                Console.Error.WriteLine("Unknown platform, halting!"); return; 
            }

            LoadingScreen(loadingDuration: 5 /* seconds*/);
            StartScreen();
            var gameDifficulty = DifficultySelect();
            GameLoop((Difficulty)gameDifficulty); // Do I really need an enum for difficulty?
        }
        static void ResizeScreen(uint columns, uint rows)
        {
            if (columns < 20 || columns > 80)
            {
                throw new ArgumentOutOfRangeException(nameof(columns));
            } else if (rows < 100 || rows > 240)
            {
                throw new ArgumentOutOfRangeException(nameof(rows));
            }

            // calling the resize command on linux. not sure if it's present on all distros; probably not
            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/resize";
            proc.StartInfo.Arguments = $"-s {columns} {rows}";
            proc.Start();
            proc.WaitForExit();

            Console.Clear();
        }
        static void LoadingScreen(uint loadingDuration)
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
        static void StartScreen()
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
        static int DifficultySelect()
        {
            bool difficultySelectDone = false;
            Difficulty predictableOrRandom = Difficulty.Predictable;

            while (!difficultySelectDone)
            {
                Console.Clear();

                Console.SetCursorPosition(43, 12);
                Console.Write("(1) Predictable");

                Console.SetCursorPosition(9, 14);
                Console.Write("(2) Random - The amount of time that passes before the letter is shown is random.");
                Console.SetCursorPosition(13, 15);
                Console.Write("Introduces more Difficulty.");
                Console.SetCursorPosition(40, 10);
                Console.Write("Choose Difficulty: ");
                
                var answer = Console.ReadKey();

                if (answer.Key == ConsoleKey.D1 || answer.Key == ConsoleKey.NumPad1)
                {
                    Console.Clear();

                    Console.SetCursorPosition(18, 10);
                    Console.Write("Are you sure you want your Difficulty to be: predictable? (y/n): ");

                    var confirm = Console.ReadKey();

                    if (confirm.Key == ConsoleKey.Y) {
                        predictableOrRandom = Difficulty.Predictable;
                        difficultySelectDone = true;
                    }

                } else if (answer.Key == ConsoleKey.D2 || answer.Key == ConsoleKey.NumPad2)
                {
                    Console.Clear();

                    Console.SetCursorPosition(20, 10);
                    Console.Write("Are you sure you want your Difficulty to be: random? (y/n): ");

                    var confirm = Console.ReadKey();

                    if (confirm.Key == ConsoleKey.Y) {
                        predictableOrRandom = Difficulty.Random;
                        difficultySelectDone = true;
                    }
                }
            }
            Console.Clear();

            Console.SetCursorPosition((predictableOrRandom == Difficulty.Predictable ? 39 : 42), 10);
            Console.Write($"You chose {(predictableOrRandom == Difficulty.Predictable ? "Predictable" : "Random")}");

            Thread.Sleep(2500);
            return (predictableOrRandom == Difficulty.Predictable ? 0 : 1);
        }
        static void GameLoop(Difficulty gameDifficulty)
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

            Console.Clear();
            Console.SetCursorPosition(44, 20);
            Console.Write("Here we go!");
            Thread.Sleep(500);

            countDown(seconds: 3);

            // TODO: Maybe add a way for someone to chose how many times they wanna do this?
            // instead of just being fixed at 5.
            for (int i = 0; i < 5; i++)
            {
                var randomKey = RandomLetter();

                _timeSinceStarting = 0.00;
                _reflexTimer = new System.Timers.Timer();
                _reflexTimer.Interval = 50;
                _reflexTimer.Elapsed += OnTimedEvent;
                _reflexTimer.AutoReset = true;
                _reflexTimer.Start();

                while (true)
                {
                    if (gameDifficulty == Difficulty.Random) {
                        var randWait = new Random((int)DateTime.Now.Ticks);
                        Thread.Sleep(randWait.Next(300, 4000));
                    }

                    Console.Clear();

                    Console.SetCursorPosition(46, 20);
                    Console.Write($"Enter {randomKey}!");

                    var keyEntered = Console.ReadKey();

                    if (keyEntered.Key == randomKey) {
                        _reflexTimer.Stop();
                        break;
                    } else if (_timeSinceStarting > 4) {
                        _reflexTimer.Stop();
                        Console.WriteLine("You didn't hit the key in time!");
                        break;
                    }
                }
                
                const double scoreCeiling = 1000;
                const double pointLossPerSecond = 250;
                var scoreToAdd = (scoreCeiling - (_timeSinceStarting * pointLossPerSecond));
                if (scoreToAdd < 0) scoreToAdd = 0;
                _score += scoreToAdd;


                Console.Clear();

                Console.SetCursorPosition(36, 20);
                Console.Write((scoreToAdd > 0) ? $"Nice job, you got {Math.Round(scoreToAdd)} points!" : "That's too bad, you didn't get any points!");


                // NOTE: Is it bad to access private members like this, even though they belong to the same class?
                if (_timeSinceStarting < 4) {
                    Console.SetCursorPosition(31, 21);
                    Console.Write($"Your reaction speed was {_timeSinceStarting:F2}s. Not bad.");
                } else {
                    Console.SetCursorPosition(18, 21);
                    Console.Write($"Your reaction time was {_timeSinceStarting:F2}s. I know you can to better than that!");
                }
                Thread.Sleep(500);

                Console.SetCursorPosition(30, 22);
                Console.Write("Starting next challenge in 5 seconds...");
                Thread.Sleep(250);

                countDown(seconds: 5);

            }

            Console.WriteLine("Your final score is {0}", _score);
            Console.ReadKey();
        }
        static ConsoleKey RandomLetter()
        {
            // Everything before 65 in the ConsoleKey enum is misc characters
            // so we need to add 65 to our random number so we start at A
            // instead of something weird like NUMPAD_LEFT
            const int consoleKeyOffset = 65;
            var rand = new Random((int)DateTime.Now.Ticks); // just basing our seed based on the current tick. not need and kinda dumb, but it's cool, right?
            int random = rand.Next(0, 25);
            return (ConsoleKey)random + consoleKeyOffset;
        }

        
        public static void OnTimedEvent(object source, ElapsedEventArgs e) // honestly not sure if this is the good way to keep track of timing for this sort of thing. I think
        {                                                                  // I could use async and await to roll my own sort of thing but I have no idea how to do that.
            _timeSinceStarting += 0.05;
        }

        static void countDown(int seconds, bool clearScreen = true)
        {
            if (clearScreen) Console.Clear();

            var secondsToDisplay = seconds;
            for (int i = 1; i <= seconds; i++)
            {
                Console.SetCursorPosition(50, 23);
                Console.Write(secondsToDisplay);
                secondsToDisplay--;
                Thread.Sleep(1000);
            }
        }
    }
}
