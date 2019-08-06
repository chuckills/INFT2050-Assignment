/* CLASS NAME: GameDifficulty
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * GameDifficulty class contains the code to configure the difficulty
 * level of the game SpaceOps.
 * 
 * It contains methods and properties to increase and decrease the
 * difficulty level by increasing or decreasing the value of the
 * operands in the game's current equation to be guessed
 * 
 */
 
 namespace Project
{
    class GameDifficulty
    {
        #region Instance Variables
        /// <summary>
        ///     Default ranges for each dificulty. In each difficulty the range for each operand is in pairs.
        ///     The first two pairs are for addition, the second two pairs for subtraction, then multiplication
        ///     and finally division
        /// </summary>
        private static readonly int[] DEFAULTBEGINNER = {1, 11, 1, 11, 1, 11, 1, 11, 1, 6, 1, 6, 1, 11, 1, 6 };
        private static readonly int[] DEFAULTMEDIUM = { 10, 51, 10, 51, 10, 51, 10, 51, 3, 13, 3, 13, 10, 51, 3, 11 };
        private static readonly int[] DEFAULTHARD = { 20, 101, 20, 101, 20, 101, 20, 101, 3, 21, 3, 21, 40, 181, 4, 16 };

        private char cDifficulty;
        
        // These are the working ranges for each type of operation
        private int[] iAddRange;
        private int[] iSubtractRange;
        private int[] iMultiplyRange;
        private int[] iDivideRange;
        #endregion

        #region Constructor
        public GameDifficulty(char _cDifficulty)
        {
            cDifficulty = _cDifficulty;
            switch (cDifficulty)
            {
                case 'b':
                    iAddRange = new [] { 1, 11, 1, 11 };
                    iSubtractRange = new [] { 1, 11, 1, 11 };
                    iMultiplyRange = new [] { 1, 6, 1, 6 };
                    iDivideRange = new [] { 1, 11, 1, 6 };
                    break;
                case 'm':
                    iAddRange = new [] { 10, 51, 10, 51 };
                    iSubtractRange = new [] { 10, 51, 10, 51 };
                    iMultiplyRange = new [] { 3, 13, 3, 13 };
                    iDivideRange = new [] { 10, 51, 3, 11 };
                    break;
                case 'h':
                    iAddRange = new [] { 20, 101, 20, 101 };
                    iSubtractRange = new [] { 20, 101, 20, 101 };
                    iMultiplyRange = new [] { 3, 21, 3, 21 };
                    iDivideRange = new [] { 40, 181, 4, 16 };
                    break;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Range of values an addition question can select from
        /// </summary>
        public int[] AddRange
        {
            get { return iAddRange; }
            set { iAddRange = value; }
        }

        /// <summary>
        ///     Range of values a subtraction question can select from
        /// </summary>
        public int[] SubtractRange
        {
            get { return iSubtractRange; }
            set { iSubtractRange = value; }
        }

        /// <summary>
        ///     Range of values a multiplication question can select from
        /// </summary>
        public int[] MultiplyRange
        {
            get { return iMultiplyRange; }
            set { iMultiplyRange = value; }
        }

        /// <summary>
        ///     Range of values a division question can select from
        /// </summary>
        public int[] DivideRange
        {
            get { return iDivideRange; }
            set { iDivideRange = value; }
        }

        /// <summary>
        ///     A code representing the difficulty level, 'b', 'm', 'h'
        /// </summary>
        public char DifficultyCode
        {
            get { return cDifficulty; }
            set { cDifficulty = value; }
        }
        #endregion

        #region Increase Difficulty Methods
        /// <summary>
        ///     Increases addition range upper values by 5
        /// </summary>
        public void increaseAdd()
        {
            iAddRange[1] += 5;
            iAddRange[3] += 5;
        }

        /// <summary>
        ///     Increases subtraction range upper values by 5
        /// </summary>
        public void increaseSubtract()
        {
            iSubtractRange[1] += 5;
            iSubtractRange[3] += 5;
        }

        /// <summary>
        ///     Increases multiplication range upper values by 1
        /// </summary>
        public void increaseMultiply()
        {
            iMultiplyRange[1]++;
            iMultiplyRange[3]++;
        }

        /// <summary>
        ///     Increases division range upper values by 1
        /// </summary>
        public void increaseDivide()
        {
            iDivideRange[1]++;
            iDivideRange[3]++;
        }
        #endregion

        #region Decrease Difficulty Methods
            #region Decrease Add
        /// <summary>
        ///     Decreases addition range
        /// </summary>
        public void decreaseAdd()
        {
            // Prevents addition range falling below the default range
            switch (cDifficulty)
            {
                case 'b':
                    if (iAddRange[1] > DEFAULTBEGINNER[1])
                    {
                        decreaseAddHelper();
                    }
                    break;
                case 'm':
                    if (iAddRange[1] > DEFAULTMEDIUM[1])
                    {
                        decreaseAddHelper();
                    }
                    break;
                case 'h':
                    if (iAddRange[1] > DEFAULTHARD[1])
                    {
                        decreaseAddHelper();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Decreases addition range upper values by 5
        /// </summary>
        private void decreaseAddHelper()
        {
            iAddRange[1] -= 5;
            iAddRange[3] -= 5;
        }
        #endregion

            #region Decrease Subtract
        /// <summary>
        ///     Decreases the subtraction range
        /// </summary>
        public void decreaseSubtract()
        {
            // Prevents subtraction range falling below the default range
            switch (cDifficulty)
            {
                case 'b':
                    if (iSubtractRange[1] > DEFAULTBEGINNER[5])
                    {
                        decreaseSubtractHelper();
                    }
                    break;
                case 'm':
                    if (iSubtractRange[1] > DEFAULTMEDIUM[5])
                    {
                        decreaseSubtractHelper();
                    }
                    break;
                case 'h':
                    if (iSubtractRange[1] > DEFAULTHARD[5])
                    {
                        decreaseSubtractHelper();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Decrease the subtraction range upper values by 5
        /// </summary>
        private void decreaseSubtractHelper()
        {
            iSubtractRange[1] -= 5;
            iSubtractRange[3] -= 5;
        }
        #endregion

            #region Decrease Multiply
        /// <summary>
        ///     Decreases multiplication range
        /// </summary>
        public void decreaseMultiply()
        {
            // Prevents multiplication range falling below the default range
            switch (cDifficulty)
            {
                case 'b':
                    if (iMultiplyRange[1] > DEFAULTBEGINNER[9])
                    {
                        decreaseMultiplyHelper();
                    }
                    break;
                case 'm':
                    if (iAddRange[1] > DEFAULTMEDIUM[9])
                    {
                        decreaseMultiplyHelper();
                    }
                    break;
                case 'h':
                    if (iAddRange[1] > DEFAULTHARD[9])
                    {
                        decreaseMultiplyHelper();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Decreases the multiplication range upper values by 1
        /// </summary>
        private void decreaseMultiplyHelper()
        {
            iMultiplyRange[1]--;
            iMultiplyRange[3]--;
        }
        #endregion

            #region Decrease Divide
        /// <summary>
        ///     Decrease division range
        /// </summary>
        public void decreaseDivide()
        {
            // Prevents division range falling below the default range
            switch (cDifficulty)
            {
                case 'b':
                    if (iDivideRange[1] > DEFAULTBEGINNER[13])
                    {
                        decreaseDivideHelper();
                    }
                    break;
                case 'm':
                    if (iDivideRange[1] > DEFAULTMEDIUM[13])
                    {
                        decreaseDivideHelper();
                    }
                    break;
                case 'h':
                    if (iAddRange[1] > DEFAULTHARD[13])
                    {
                        decreaseDivideHelper();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Decreases division range upper values by 1
        /// </summary>
        private void decreaseDivideHelper()
        {
            iDivideRange[1]--;
            iDivideRange[3]--;
        }
        #endregion

            #region Reset Difficulty
        /// <summary>
        ///     Resets all ranges to default
        /// </summary>
        public void resetDifficulty()
        {
            switch (cDifficulty)
            {
                case 'b':
                    iAddRange = new int[] { 1, 11, 1, 11 };
                    iSubtractRange = new int[] { 1, 11, 1, 11 };
                    iMultiplyRange = new int[] { 1, 6, 1, 6 };
                    iDivideRange = new int[] { 1, 11, 1, 6 };
                    break;
                case 'm':
                    iAddRange = new int[] { 10, 51, 10, 51 };
                    iSubtractRange = new int[] { 10, 51, 10, 51 };
                    iMultiplyRange = new int[] { 3, 13, 3, 13 };
                    iDivideRange = new int[] { 10, 51, 3, 11 };
                    break;
                case 'h':
                    iAddRange = new int[] { 20, 101, 20, 101 };
                    iSubtractRange = new int[] { 20, 101, 20, 101 };
                    iMultiplyRange = new int[] { 3, 21, 3, 21 };
                    iDivideRange = new int[] { 40, 181, 4, 16 };
                    break;
            }
        }
        #endregion
        #endregion
    }
}
