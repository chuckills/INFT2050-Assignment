/* CLASS NAME: Question
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * Question class contains the code to produce a question in the form:
 * 
 *      Operand1 (+-×÷) Operand2 = Answer
 *      
 * for the game SpaceOps.
 *
 * 
 * Each operator has an independant range of values to chose from
 * and is variable as the game progresses
 * 
 */

using System;

namespace Project
{
    class Question
    {
        #region Instance Variables
        private char cOperator;
        private int iOperand1;
        private int iOperand2;
        private int iAnswer;
        #endregion

        #region Constructor
        public Question(char _cOperator, int[] _range)
        {
            cOperator = _cOperator;

            Random myRandom = new Random();

            if (cOperator != '÷')
            {
                iOperand1 = myRandom.Next(_range[0], _range[1]);
                iOperand2 = myRandom.Next(_range[2], _range[3]);
            }

            switch (cOperator)
            {
                case '+':
                    iAnswer = iOperand1 + iOperand2;
                    break;
                case '-':
                    // Swap operands to avoid negative answers
                    if (iOperand1 < iOperand2)
                    {
                        int temp = iOperand1;
                        iOperand1 = iOperand2;
                        iOperand2 = temp;
                    }
                    iAnswer = iOperand1 - iOperand2;
                    break;
                case '×':
                    iAnswer = iOperand1 * iOperand2;
                    break;
                case '÷':
                    // Only chooses operands that produce no remainder
                    do
                    {
                        iOperand1 = myRandom.Next(_range[0], _range[1]);
                        iOperand2 = myRandom.Next(_range[2], _range[3]);

                        // Swap operands to avoid fractional division
                        if (iOperand1 < iOperand2)
                        {
                            int temp = iOperand1;
                            iOperand1 = iOperand2;
                            iOperand2 = temp;
                        }
                    }
                    while (iOperand1 % iOperand2 != 0);
                    iAnswer = iOperand1 / iOperand2;
                    break;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     The first operand in the question
        /// </summary>
        public int Operand1 => iOperand1;

        /// <summary>
        ///     The second operand in the question
        /// </summary>
        public int Operand2 => iOperand2;

        /// <summary>
        ///     The answer to the question
        /// </summary>
        public int Answer => iAnswer;

        /// <summary>
        ///     The question's operator
        /// </summary>
        public char Operator
        {
            get { return cOperator; }
            set { cOperator = value; }
        }
        #endregion
    }
}
