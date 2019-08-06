/* CLASS NAME: ScoreComparer
 * AUTHOR: Greg Choice
 * STUDENT NUMBER: c9311718
 * DATE: 19/05/2017
 * INFT2050 Assignment
 * 
 * Implementation of the IComparer interface to compare
 * two Score objects for sorting
 * 
 */

using System.Collections.Generic;

namespace Project
{
    class ScoreComparer : IComparer<Score>
    {
        public int Compare(Score _thisScore, Score _otherScore)
        {
            if (_thisScore.calcScore() < _otherScore.calcScore())
            {
                return 1;
            }
            if (_thisScore.calcScore() > _otherScore.calcScore())
            {
                return -1;
            }
            return 0;
        }
    }
}
