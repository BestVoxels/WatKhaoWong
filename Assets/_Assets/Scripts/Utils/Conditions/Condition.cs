using System.Collections.Generic;
using UnityEngine;

namespace WatKhaoWong.Utils.Conditions
{
    [System.Serializable]
    public class Condition
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Disjunction[] _and;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        // Take all the Predicate and AND them together ex. "Predicate1 && Predicate2 && Predicate3"
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            if (!HasCondition()) return true; // will return True by default when condition is empty. If No Guard Check like this, Bug Occurs by : Start New Game -> pick up items -> Save&Quit -> Click Continue Save, now drag pickup item to slot will throw error.

            foreach (Disjunction eachPredAND in _and)
            {
                if (eachPredAND.Check(evaluators) == false) return false; // if one predicate is false everything is 'false'
            }

            return true; // when none are 'false' it means true
        }

        public bool HasCondition()
        {
            if (_and == null) return false;

            return _and.Length > 0;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class Disjunction
        {
            #region --Fields-- (Inspector)
            [SerializeField] private Predicate[] _or;
            #endregion



            #region --Properties-- (With Backing Fields)
            public Predicate[] Or { get { return _or; } }
            #endregion



            #region --Methods-- (Custom PUBLIC)
            // Take all the Predicate and OR them together ex. "Predicate1 || Predicate2 || Predicate3"
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate eachPredOR in _or)
                {
                    if (eachPredOR.Check(evaluators) == true) return true; // if one predicate is true everything is 'true'
                }

                return false; // when none are 'true' it means false
            }
            #endregion
        }

        [System.Serializable]
        private class Predicate
        {
            #region --Fields-- (Inspector)
            [SerializeField] private EPredicateName _methodName;
            [SerializeField] private string[] _parameters;
            [SerializeField] private bool _negate = false;
            #endregion



            #region --Properties-- (With Backing Fields)
            public EPredicateName MethodName { get { return _methodName; } }
            public string[] Parameters { get { return _parameters; } }
            public bool Negate { get { return _negate; } }
            #endregion



            #region --Methods-- (Custom PUBLIC)
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                if (evaluators == null) return true;

                foreach (IPredicateEvaluator eachEvaluator in evaluators)
                {
                    bool? result = eachEvaluator.Evaluate(_methodName, _parameters); // Debug 'eachEvaluator.ToString()' to see where each one is from.

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