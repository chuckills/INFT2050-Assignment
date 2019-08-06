/* CLASS NAME: Score
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * Score class contains code to collect a player's score in the game SpaceOps
 * 
 * Each of the operators are tallied individually and accuracy, total correct,
 * total questions and score value are calculated. A point is given for each
 * correct answer and half a point is taken away for incorrect answers.
 * 
 */

namespace Project
{
    public class Score
    {
        #region Instance Variables
        private int iAddQuestions;
        private int iAddCorrect;
        private int iSubtractQuestions;
        private int iSubtractCorrect;
        private int iMultiplyQuestions;
        private int iMultiplyCorrect;
        private int iDivideQuestions;
        private int iDivideCorrect;
        #endregion

        #region Constructors
        public Score()
        {
            iAddQuestions = 0;
            iAddCorrect = 0;
            iSubtractQuestions = 0;
            iSubtractCorrect = 0;
            iMultiplyQuestions = 0;
            iMultiplyCorrect = 0;
            iDivideQuestions = 0;
            iDivideCorrect = 0;
        }

        public Score(int _add, int _addC, int _subt, int _subtC, int _mult, int _multC, int _div, int _divC)
        {
            iAddQuestions = _add;
            iAddCorrect = _addC;
            iSubtractQuestions = _subt;
            iSubtractCorrect = _subtC;
            iMultiplyQuestions = _mult;
            iMultiplyCorrect = _multC;
            iDivideQuestions = _div;
            iDivideCorrect = _divC;
        }
        #endregion
          
        #region Properties
        /// <summary>
        ///     The number of addition questions asked
        /// </summary>
        public int AddQuestions
        {
            get { return iAddQuestions; }
            set { iAddQuestions = value; }
        }

        /// <summary>
        ///     The number of addition questions correct
        /// </summary>
        public int AddCorrect
        {
            get { return iAddCorrect; }
            set { iAddCorrect = value; }
        }

        /// <summary>
        ///     The number of subtraction questions asked
        /// </summary>
        public int SubtractQuestions
        {
            get { return iSubtractQuestions; }
            set { iSubtractQuestions = value; }
        }

        /// <summary>
        ///     The number of subtraction questions correct
        /// </summary>
        public int SubtractCorrect
        {
            get { return iSubtractCorrect; }
            set { iSubtractCorrect = value; }
        }

        /// <summary>
        ///     The number of multiplication questions asked
        /// </summary>
        public int MultiplyQuestions
        {
            get { return iMultiplyQuestions; }
            set { iMultiplyQuestions = value; }
        }

        /// <summary>
        ///     The number of multiplication questions correct
        /// </summary>
        public int MultiplyCorrect
        {
            get { return iMultiplyCorrect; }
            set { iMultiplyCorrect = value; }
        }

        /// <summary>
        ///     The number of divide questions asked
        /// </summary>
        public int DivideQuestions
        {
            get { return iDivideQuestions; }
            set { iDivideQuestions = value; }
        }

        /// <summary>
        ///     The number of divide questions correct
        /// </summary>
        public int DivideCorrect
        {
            get { return iDivideCorrect; }
            set { iDivideCorrect = value; }
        }

        /// <summary>
        ///     Total questions asked
        /// </summary>
        public int TotalQuestions => sumTotalQuestions();
    
        /// <summary>
        ///     Total questions correct
        /// </summary>
        public int TotalCorrect => sumTotalCorrect();

        /// <summary>
        ///     Overall accuracy
        /// </summary>
        public double TotalAccuracy => calcAccuracy(sumTotalCorrect(), sumTotalQuestions());
        
        /// <summary>
        ///     Addition accuracy
        /// </summary>
        public double AddAccuracy => calcAccuracy(iAddCorrect, iAddQuestions);

        /// <summary>
        ///     Subtraction accuracy
        /// </summary>
        public double SubtractAccuracy => calcAccuracy(iSubtractCorrect, iSubtractQuestions);
        
        /// <summary>
        ///     Multiplication accuracy
        /// </summary>
        public double MultiplyAccuracy => calcAccuracy(iMultiplyCorrect, iMultiplyQuestions);

        /// <summary>
        ///     Division accuracy
        /// </summary>
        public double DivideAccuracy => calcAccuracy(iDivideCorrect, iDivideQuestions);
        #endregion

        #region Helper Methods
        /// <summary>
        ///     Adds the totals of each operator questions asked variable
        /// </summary>
        /// <returns></returns>
        private int sumTotalQuestions() => iAddQuestions + iSubtractQuestions + iMultiplyQuestions + iDivideQuestions;

        /// <summary>
        ///     Adds the totals of each operator questions correct variable
        /// </summary>
        /// <returns></returns>
        private int sumTotalCorrect() => iAddCorrect + iSubtractCorrect + iMultiplyCorrect + iDivideCorrect;

        /// <summary>
        ///     Calculates the accuracy using the provided arguments
        /// </summary>
        /// <param name="_correct"></param>
        /// <param name="_total"></param>
        /// <returns></returns>
        private double calcAccuracy(int _correct, int _total)
        {
            if (_total == 0)
                return 0.0;
            return (double)_correct / _total;
        }

        /// <summary>
        ///     Calculates a quantifyable score
        /// </summary>
        /// <returns></returns>
        public double calcScore() => TotalCorrect - (TotalQuestions - TotalCorrect) / 2.0;

        /// <summary>
        ///     Creates a string representation of a Score object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => 
            string.Format("Total Questions: {0}\nTotal Correct: {1}\nOverall Accuracy: {2:P1}\nAddition Accuracy: " +
                                 "{3:P1}\nSubtraction Accuracy: {4:P1}\nMultiplication Accuracy: {5:P1}\nDivision Accuracy: {6:P1}\n\n",
                TotalQuestions, TotalCorrect, TotalAccuracy, AddAccuracy, SubtractAccuracy, MultiplyAccuracy, DivideAccuracy);
        #endregion
    }
}
