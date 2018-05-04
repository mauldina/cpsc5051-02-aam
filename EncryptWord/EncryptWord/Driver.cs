// AUTHOR: Alicia Mauldin
// FILENAME: Driver.cs
// DATE: 4 May 2018
// REVISION HISTORY: 1.1
// REFERENCES: None

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptWord
{
    public class Driver
    {
        private const int MIN_GUESS = 1; // Minimum range for shift guesses
        private const int MAX_GUESS = 10; // Maximum range for shift guesses

        // Description: Tests all public functionality of the EncryptWord class
        // Preconditions: None
        // Postconditions: None
        public void RunTests()
        {
            Console.WriteLine("Starting tests...");
            Console.WriteLine("TEST 1");
            EncryptWord test1 = new EncryptWord();
            Console.WriteLine("Encrypted word [error]: " 
                + test1.EncodeWord("Tes"));
            Console.WriteLine("Decrypted word [error]: " + test1.DecodeWord());
            test1.ResetEverything();
            Console.WriteLine("Encrypted word: " + test1.EncodeWord("Test 1"));
            Console.WriteLine(test1.GetGuessStatistics());
            GuessAllValues(test1);
            Console.WriteLine(test1.GetGuessStatistics());
            Console.WriteLine("Decrypted word [Test 1]: " + test1.DecodeWord());

            Console.WriteLine("\nTEST 2");
            EncryptWord test2 = new EncryptWord("Hello, World!");
            Console.WriteLine(test2.GetGuessStatistics());
            GuessAllValues(test2);
            Console.WriteLine(test2.GetGuessStatistics());
            Console.WriteLine("Decrypted word [Hello, World!]: " 
                + test2.DecodeWord());

            Console.WriteLine("\nTESTING COMPLETE");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        // Description: Helper function to iterate through guesses for
        //      an EncryptWord object's shift value
        // Preconditions: EncryptWord object must be on (word has been
        //      encrypted in the class)
        // Postconditions: None
        private void GuessAllValues(EncryptWord test)
        {
            for (int i = MIN_GUESS; i < MAX_GUESS; i++)
            {
                Console.WriteLine("Guess [" + i + "]: " 
                    + test.GuessTheShiftValue(i));
            }
        }
    }
}
