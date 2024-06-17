using System.Collections.Generic;
using UnityEngine;
using WatKhaoWong.Identity;

namespace WatKhaoWong.Core
{
    public class ShowHideUIByRole : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General Settings")]
        [SerializeField] private UIItem[] _uIToShowOnStart;
        [SerializeField] private UIItem[] _uIToHideOnStart;
        #endregion



        #region --Fields-- (In Class)
        private AccountRule _account;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _account = GameObject.FindWithTag("Player").GetComponentInChildren<AccountRule>();
        }

        private void Start()
        {
            ShowUIOnStart();
            HideUIOnStart();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ShowUIOnStart()
        {
            foreach (UIItem each in _uIToShowOnStart)
            {
                if (each.targetRoles.Contains(_account.Role))
                    each.uI.SetActive(true);
            }
        }

        private void HideUIOnStart()
        {
            foreach (UIItem each in _uIToHideOnStart)
            {
                if (each.targetRoles.Contains(_account.Role))
                    each.uI.SetActive(false);
            }
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class UIItem
        {
            public GameObject uI;
            public List<EAccountRole> targetRoles = new List<EAccountRole>();
        }
        #endregion
    }
}