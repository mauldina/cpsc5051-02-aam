// AUTHOR: Alicia Mauldin
// FILENAME: encryptWord.cs
// DATE: 14 April 2018
// REVISION HISTORY: 1.0
// REFERENCES: None

using System;
using System.Linq;

namespace EncryptWord
{
    // Description: Class contains the major functionality for encrypting a
    //      a word using a Caesar cipher shift and storing the encryption for
    //      later decryption. Users are allowed to guess he cipher value
    //      while encryption is enabled and see statistics on guess attempts.
    //
    // Functionality: Intakes a word and returns an encryption of it, 
    //      allows for guessing of the cipher shift value,
    //      tracks guess statistics, decrypts the stored word, and allows
    //      for the word and all statistics to be reset.
    // 
    // Legal states: 
    //     ON - Object is 'on' when a word has been entered for encryption
    //     OFF - Object is 'off' when no word has been entered or when the
    //         reset functionality has been called
    //
    // Dependencies: None
    //
    // Anticipated use: For a simple child's guessing game program
    // 
    // Legal/illegal input: Only accepts strings of at least 4 characters
    //      in length for word encryption and strings should be alpha
    //      characters (other characters including spaces will be accepted but
    //      not encrypted); cipher guesses must be integers
    //
    // Assumptions: 
    //  1) That the primary use case for this program is a simple child's game
    //  2) User will only need to encrypt 4+ character alpha strings
    //  3) User will only attempt to guess the cipher shift using integers
    //      between the range of 1-9
    //  4) Encryption is not necessary for non-ASCII range values outside of 
    //      65-90 and 97-122 (non-alpha characters)
    //  5) Support is not needed for non-English alphabet characters
    //  6) That the guess statistic currently provided are sufficient to the
    //      game designer's needs (guess count, guess average, high guesses,
    //      low guesses)
    //  7) That defining state by encryption status (is there an encrypted
    //      word that has not yet been decrypted) is appropriate given the
    //      requested user functionality
    //  8) That the game designer will use appropriate defenses to avoid
    //      error messages from occuring (i.e., don't call functions which
    //      require the state to be on when it is off)
    //  9) That the game designer will allow for new entries to be process 
    //      if an illegal entry is submitted (e.g., if a 3-character string is
    //      submitted, to allow for new input that meets the 4-character 
    //      minimum)
    //  10) That a cipher shift of between 1-9 characters is sufficient for
    //      encryption
    // 
    // State transition: 
    // 1) OFF -> ON (word is provided for encryption)
    // 2) ON -> ON
    // 3) ON -> OFF (decryption or reset functionality is called)
    // 
    // Interface invariants: 
    // - Only strings >= 4 characters can be encrypted
    // - Only characters between ASCII 65-90 and 97-122 will be encrypted
    // - Only a Caesar shift between 1-9 will be applied to encryption
    // - Only integers can be guess for the Caesar shift
    //
    // Implementation invariants:
    // - For loop cycles through a word to encrypt/decrypt it using the
    //      current cipher value character-by-character
    // - Use of built-in pseudo-random C# Random class for random
    //      number generation of the Caesar shift value
    // 
    // Class invariants:
    // - Only one word at a time can be stored in the object
    //
    public class EncryptWord
    {
        private const int MAX_RAND = 10;    // Max boundary for cipher
        private const int MIN_RAND = 1;     // Min boundary for cipher
        private const int MIN_CHAR_LENGTH = 4; // Min characters for encryption
        private const int MIN_LOWERCASE = 97;  // Min ASCII for lowercase char
        private const int MAX_LOWERCASE = 122; // Max ASCII for lowercase char
        private const int MIN_UPPERCASE = 65;  // Min ASCII for uppercase char
        private const int MAX_UPPERCASE = 90;  // Max ASCII for uppercase char
        private bool _encryptionOn;     // Encryption state
        private string _encryptedWord;  // Stored encrypted word
        private int _caesarShiftValue;  // Caesar shift value
        private int _sumOfGuessValues;  // Sum of all guess values
        private int _guessValueAverage; // Average of all guess values
        private int _totalGuessCount;   // Total guess attempts
        private int _highGuessCount;    // Guess attempts higher than the shift
        private int _lowGuessCount;     // Guess attempts lower than the shift

        // Description: Constructor which randomly generates a cipher value and
        //      initializes all values to zero/null/false. Calls upon the
        //      GenerateCaesarCipher helper function to initialize variables.
        // Preconditions: None
        // Postconditions: No change in state
        public EncryptWord()
        {
            GenerateCaesarShift();
        }

        // Description: Constructor which randomly generates a cipher value and
        //      initializes all values to zero/null/false. Intakes a word and
        //      encrypts it using the cipher. Calls upon the
        //      GenerateCaesarCipher helper function to initialize variables.
        // Preconditions: Word must be at least 4 characters in length and
        //      class state must be off (no stored encrypted word)
        // Postconditions: Class state is enabled upon word encryption
        public EncryptWord(string word)
        {
            GenerateCaesarShift();
            EncodeWord(word);
        }

        // Description: Intakes a word and encrypts it using the ForwardShift
        //      helper function. Word is stored in the program in its encrypted
        //      format for possible later decryption.
        // Preconditions: Word must be at least 4 characters in length and
        //      class state must be off (no stored encrypted word)
        // Postconditions: Class state is enabled upon word encryption
        public string EncodeWord(string word)
        {
            if (_encryptionOn)
            {
                return "Error! Please reset to store a new word.";
            }
            else
            {
                if (word.Length >= MIN_CHAR_LENGTH)
                {
                    _encryptedWord = ForwardShift(word);
                    _encryptionOn = true;
                    return _encryptedWord;
                }
                else
                    return "Error! Value entered must be at least "
                        + MIN_CHAR_LENGTH + " characters long.";
            }
        }

        // Description: Decrypts the stored encrypted word into its original
        //      value using the BackShift helper function.
        // Preconditions: Class state must be on (must have a stored encrypted
        //      word)
        // Postconditions: Class state is reset to off
        public string DecodeWord()
        {
            if (_encryptionOn)
            {
                string result = BackShift();
                ResetEverything();
                return result;
            }
            else
                return "Error! You must encrypt a word first.";
        }

        // Description: Allows a user to guess what the cipher value is and
        //      returns a message advising if the guess was correct, too low,
        //      or too high.
        // Preconditions: Class state must be on (must have a stored encrypted
        //      word)
        // Postconditions: No change to state
        public string GuessTheShiftValue(int guess)
        {
            if (_encryptionOn)
            {
                UpdateGuessStatistics(guess);
                if (guess == _caesarShiftValue)
                    return "Correct! " + guess + " is the shift value.";
                else if (guess < _caesarShiftValue)
                    return guess + " is too low. Guess again.";
                else
                    return guess + " is too high. Guess again.";
            }
            else
                return "Error! You must encrypt a word first.";
        }

        // Description: Displays statistics around user guesses, including the
        //      total number of attempts, the average value guessed, and the
        //      number of guesses that were too high and too low.
        // Preconditions: None
        // Postconditions: No change to state
        public string GetGuessStatistics()
        {
            return "\nGUESS STATISTICS"
                + "\nNumber of guesses: " + _totalGuessCount
                + "\nAverage guess value: " + _guessValueAverage
                + "\nNumber of high guesses: " + _highGuessCount
                + "\nNumber of low guesses: " + _lowGuessCount + "\n";
        }

        // Description: Resets the object back to its original state, including
        //      resetting all variables to zero/null/false.
        // Preconditions: None
        // Postconditions: Class state is reset to off
        public void ResetEverything()
        {
            GenerateCaesarShift();
        }

        // Description: Helper function to reset all variables and generate
        //      a new cipher value; resets class state to off.
        private void GenerateCaesarShift()
        {
            Random rand = new Random();
            _caesarShiftValue = rand.Next(MIN_RAND, MAX_RAND);
            _totalGuessCount = 0;
            _sumOfGuessValues = 0;
            _guessValueAverage = 0;
            _highGuessCount = 0;
            _lowGuessCount = 0;
            _encryptedWord = null;
            _encryptionOn = false;
        }

        // Description: Helper function to update all the guess variables
        //      whenever a new guess attempt is made.
        private void UpdateGuessStatistics(int guess)
        {
            _totalGuessCount++;
            _sumOfGuessValues += guess;
            _guessValueAverage = _sumOfGuessValues / _totalGuessCount;
            if (guess > _caesarShiftValue)
                _highGuessCount++;
            else if (guess < _caesarShiftValue)
                _lowGuessCount++;
        }

        // Description: Helper function to take a char value (in ASCII format)
        //      and shift it based on the cipher.
        private int ShiftCharValue(int charAscii, int shift)
        {
            int shiftedCharValue = charAscii + shift;

            // If the ASCII is for a lowercase letter
            if (charAscii >= MIN_LOWERCASE && charAscii <= MAX_LOWERCASE)
            {
                if (shiftedCharValue > MAX_LOWERCASE)
                    return (shiftedCharValue + MIN_LOWERCASE - MIN_RAND)
                        % MAX_LOWERCASE;
                else if (shiftedCharValue < MIN_LOWERCASE)
                    return shiftedCharValue - MIN_LOWERCASE + MAX_LOWERCASE
                        + MIN_RAND;
                else
                    return shiftedCharValue;
            }

            // If the ASCII is for uppercase letter
            else if (charAscii >= MIN_UPPERCASE && charAscii <= MAX_UPPERCASE)
            {
                if (shiftedCharValue > MAX_UPPERCASE)
                    return (shiftedCharValue + MIN_UPPERCASE - MIN_RAND)
                        % MAX_UPPERCASE;
                else if (shiftedCharValue < MIN_UPPERCASE)
                    return shiftedCharValue - MIN_UPPERCASE + MAX_UPPERCASE
                        + MIN_RAND;
                else
                    return shiftedCharValue;
            }

            // If the ASCII is for any other value
            else
                return charAscii;
        }

        // Description: Helper function which takes a word and encrypts it by
        //      shifting it forward by the cipher value amount.
        private string ForwardShift(string word)
        {
            string encoded = "";
            for (int i = 0; i < word.Length; i++)
            {
                char newChar = (char)ShiftCharValue(word.ElementAt(i),
                    _caesarShiftValue);
                encoded += newChar;
            }
            return encoded;
        }

        // Description: Helper function which takes a word and decrypts it by
        //      shifting it backward by the cipher value amount.
        private string BackShift()
        {
            string decoded = "";
            for (int i = 0; i < _encryptedWord.Length; i++)
            {
                char newChar = (char)ShiftCharValue(_encryptedWord.ElementAt(i),
                    -_caesarShiftValue);
                decoded += newChar;
            }
            return decoded;
        }
    }
}
