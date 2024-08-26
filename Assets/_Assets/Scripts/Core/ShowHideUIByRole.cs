using System.Collections.Generic;
using UnityEngine;
using WatKhaoWong.Identity;
using WatKhaoWong.UI;

namespace WatKhaoWong.Core
{
    public class ShowHideUIByRole : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("General Settings")]
        [SerializeField] private UIItem[] _showUIByRoles;
        [SerializeField] private UIItem[] _hideUIByRoles;
        #endregion



        #region --Fields-- (In Class)
        private IUserData _userData;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _userData = GameObject.FindWithTag("Player").GetComponentInChildren<IUserData>();
        }

        private void OnEnable()
        {
            UIRefresher.OnUIShowedHidByRoles += ShowUI;
            UIRefresher.OnUIShowedHidByRoles += HideUI;
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
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ShowUI()
        {
            foreach (UIItem each in _showUIByRoles)
            {
                if (each.targetRoles.Contains(_userData.GetRole()))
                    each.uI.SetActive(true);
            }
        }

        private void HideUI()
        {
            foreach (UIItem each in _hideUIByRoles)
            {
                if (each.targetRoles.Contains(_userData.GetRole()))
                    each.uI.SetActive(false);
            }
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class UIItem
        {
            public GameObject uI;
            public List<EUserRole> targetRoles = new List<EUserRole>();
        }
        #endregion
    }
}