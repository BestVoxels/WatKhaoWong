using System.Collections.Generic;
using UnityEngine;
using WatKhaoWong.Utils.Conditions;
using WatKhaoWong.UI;

namespace WatKhaoWong.Core
{
    public class ShowHideUIByCondition : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General Settings")]
        [SerializeField] private UIItem[] _showUIByCondition;
        [SerializeField] private UIItem[] _hideUIByCondition;
        //[SerializeField] private Condition _testCondition; // ---DEBUGGER PURPOSE--- search for 'EConditionType.cs | MyUserData.cs | ShowHideUIByCondition.cs'
        #endregion



        #region --Fields-- (In Class)
        private IConditionEvaluator[] _conditionsEvaluator;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _conditionsEvaluator = GameObject.FindWithTag("Player").GetComponentsInChildren<IConditionEvaluator>();
        }

        private void OnEnable()
        {
            UIRefresher.OnUIShowedHidByRoles += ShowUI;
            UIRefresher.OnUIShowedHidByRoles += HideUI;

            UIRefresher.OnAllConditionCheckCalled += ShowUI;
            UIRefresher.OnAllConditionCheckCalled += HideUI;
        }

        private void Start()
        {
            ShowUI();
            HideUI();
        }

        private void OnDisable()
        {
            UIRefresher.OnUIShowedHidByRoles -= ShowUI;
            UIRefresher.OnUIShowedHidByRoles -= HideUI;

            UIRefresher.OnAllConditionCheckCalled -= ShowUI;
            UIRefresher.OnAllConditionCheckCalled -= HideUI;
        }

        //// ---DEBUGGER PURPOSE--- search for 'EConditionType.cs | MyUserData.cs | ShowHideUIByCondition.cs'
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        if (_testCondition.Check(_conditionsEvaluator))
        //            print("Condition is TRUE");
        //        else
        //            print("Condition is FLASE");
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ShowUI()
        {
            foreach (UIItem each in _showUIByCondition)
            {
                if (each.condition.Check(_conditionsEvaluator))
                    each.gameObjectsUI.ForEach(gameObject => gameObject.SetActive(true));
            }
        }

        private void HideUI()
        {
            foreach (UIItem each in _hideUIByCondition)
            {
                if (each.condition.Check(_conditionsEvaluator))
                    each.gameObjectsUI.ForEach(gameObject => gameObject.SetActive(false));
            }
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class UIItem
        {
            public List<GameObject> gameObjectsUI = new();
            public Condition condition;
        }
        #endregion
    }
}