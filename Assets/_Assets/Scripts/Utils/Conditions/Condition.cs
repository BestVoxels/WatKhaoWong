using System.Collections.Generic;
using UnityEngine;

namespace WatKhaoWong.Utils.Conditions
{
    [System.Serializable]
    public class Condition
    {
        #region --Fields-- (Inspector)
        [SerializeField] Level3[] _and4;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        // Take all the Predicate and AND them together ex. "Predicate1 && Predicate2 && Predicate3"
        public bool Check(IEnumerable<IConditionEvaluator> evaluators)
        {
            if (!HasCondition()) return true; // will return True by default when condition is empty. If No Guard Check like this, Bug Occurs by : Start New Game -> pick up items -> Save&Quit -> Click Continue Save, now drag pickup item to slot will throw error.

            foreach (Level3 eachPredAND in _and4)
            {
                if (eachPredAND.Check(evaluators) == false) return false; // if one predicate is false everything is 'false'
            }

            return true; // when none are 'false' it means true
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        bool HasCondition()
        {
            if (_and4 == null) return false;

            return _and4.Length > 0;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        class Level3
        {
            #region --Fields-- (Inspector)
            [SerializeField] Level2[] _or3;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            // Take all the Predicate and OR them together ex. "Predicate1 || Predicate2 || Predicate3"
            public bool Check(IEnumerable<IConditionEvaluator> evaluators)
            {
                foreach (Level2 eachPredOR in _or3)
                {
                    if (eachPredOR.Check(evaluators) == true) return true; // if one predicate is true everything is 'true'
                }

                return false; // when none are 'true' it means false
            }
            #endregion
        }

        [System.Serializable]
        class Level2
        {
            #region --Fields-- (Inspector)
            [SerializeField] Level1[] _and2;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            // Take all the Predicate and AND them together ex. "Predicate1 && Predicate2 && Predicate3"
            public bool Check(IEnumerable<IConditionEvaluator> evaluators)
            {
                foreach (Level1 eachPredAND in _and2)
                {
                    if (eachPredAND.Check(evaluators) == false) return false; // if one predicate is false everything is 'false'
                }

                return true; // when none are 'false' it means true
            }
            #endregion
        }

        [System.Serializable]
        class Level1
        {
            #region --Fields-- (Inspector)
            [SerializeField] Predicate[] _or1;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            // Take all the Predicate and OR them together ex. "Predicate1 || Predicate2 || Predicate3"
            public bool Check(IEnumerable<IConditionEvaluator> evaluators)
            {
                foreach (Predicate eachPredOR in _or1)
                {
                    if (eachPredOR.Check(evaluators) == true) return true; // if one predicate is true everything is 'true'
                }

                return false; // when none are 'true' it means false
            }
            #endregion
        }

        [System.Serializable]
        class Predicate
        {
            #region --Fields-- (Inspector)
            [SerializeField] EConditionType _conditionType;
            [SerializeField] EConditionValue[] _conditionValues;
            [SerializeField] bool _negate = false;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            public bool Check(IEnumerable<IConditionEvaluator> evaluators)
            {
                if (evaluators == null) return true;

                foreach (IConditionEvaluator eachEvaluator in evaluators)
                {
                    bool? result = eachEvaluator.Evaluate(_conditionType, _conditionValues); // Debug 'eachEvaluator.ToString()' to see where each one is from.

                    if (result == null) continue;
                    if (_negate) result = !result;

                    return (bool)result; // return right away either 'true' or 'false'
                }

                return true; // return 'true' when all above evaluators does'nt return anything so the node won't be excluded
            }
            #endregion
        }
        #endregion
    }
}