using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using WatKhaoWong.SceneManagement;

namespace WatKhaoWong.Attributes
{
    /// <summary>
    /// --This approach ensures that User retrieve the server's current time securely and accurately without being affected by the User’s device time settings--
    ///
    /// Steps:
    /// -> Writing timestamp to the database first
    /// -> then reading it back once Firebase has replaced it with the actual server time
    ///
    /// Note:
    /// 'Firebase.Database.ServerValue.Timestamp' is returned as an object when written to Firebase Realtime Database,
    /// which means it cannot be directly retrieved as a DateTime value without processing.
    /// </summary>
    public class ServerTime : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Server Time Stuffs")]
        [Tooltip("Delay on Application First Start to make it return Server Time properly. After App Starts for specific sec it has no delay when use after delays ends. Atleast '100 miliseconds', to make it return server time properly.")]
        [Range(1f, 3f)]
        [SerializeField] private float _delayOnStartInSeconds = 1f;
        #endregion



        #region --Fields-- (In Class)
        private float _delayTimer = 0f;

        private SavingWrapper _savingWrapper;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        private void OnEnable()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        }

        private void Start()
        {
            StartCoroutine(StartDelayTimer());
        }

        private void OnDisable()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
        }

        //// ---DEBUGGER PURPOSE---
        //private async void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        Debug.LogWarning(await Now());
        //    }
        //}
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// 'utc' (Greenwich Mean Time zone) + '7 hours' (Diff from Local Thai Time) = [Thailand Time]
        ///
        /// Doing this way because we can make sure every User from Other Places in the world follow Thailand Time.
        /// This is better tha using localTime, becuase it varies in different countries and server time since not all users have same localTime.
        /// </summary>
        /// <returns>Local Thailand Time, same for all users even from different countries.</returns>
        public async Task<DateTime> Now()
        {
            while (IsDelayOnStartActive())
            {
                await Task.Delay(100); // Wait 0.1 sec to check condition again.
            }

            // SAVE CODE -> Writing timestamp to the database first
            _savingWrapper.ForceSave(ECategoryNode.ServerStats, EValueNode.TimeStamp, ServerValue.Timestamp);

            // LOAD CODE -> then reading it back once Firebase has replaced it with the actual server time
            DataSnapshot result = await _savingWrapper.Load(ECategoryNode.ServerStats, EValueNode.TimeStamp);

            if (result == null)
            {
                Debug.LogWarning("Couldn't Load from Server! Default Time will be returned instead.");
                return new DateTime();
            }

            DateTime utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)result.Value).UtcDateTime;

            // Ensure a consistent Gregorian calendar date across users regardless of their device’s local calendar settings
            utcDateTime = ConvertToGregorian(utcDateTime, DateTimeKind.Utc);

            return utcDateTime.AddHours(7);
        }

        /// <summary>
        /// Using This Method might effects the way server behave since NOT all users have same local time.
        /// </summary>
        /// <returns>Local Time, NOT same for all users!</returns>
        public async Task<DateTime> LocalNow()
        {
            // SAVE CODE -> Writing timestamp to the database first
            _savingWrapper.ForceSave(ECategoryNode.ServerStats, EValueNode.TimeStamp, ServerValue.Timestamp);

            // LOAD CODE -> then reading it back once Firebase has replaced it with the actual server time
            DataSnapshot result = await _savingWrapper.Load(ECategoryNode.ServerStats, EValueNode.TimeStamp);

            if (result == null) return new DateTime();

            DateTime localDateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)result.Value).LocalDateTime;

            // Ensure a consistent Gregorian calendar date across users regardless of their device’s local calendar settings
            localDateTime = ConvertToGregorian(localDateTime, DateTimeKind.Local);

            return localDateTime;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsDelayOnStartActive() => _delayTimer < _delayOnStartInSeconds;

        private IEnumerator StartDelayTimer()
        {
            _delayTimer = 0f;

            while (IsDelayOnStartActive())
            {
                _delayTimer += Time.deltaTime;
                yield return null;
            }

            yield break;
        }

        private DateTime ConvertToGregorian(DateTime dateTime, DateTimeKind dateTimeKind)
        {
            GregorianCalendar gregorianCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            return DateTime.SpecifyKind(
                gregorianCalendar.ToDateTime(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day,
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second,
                    dateTime.Millisecond
                ),
                dateTimeKind
            );
        }
        #endregion



        #region --Methods-- (Subscriber)
        /// <summary>
        /// Will be called once after FirebaseAuth instance is created. Around the time of Awake().
        /// </summary>
        private void HandleStateChanged(object obj, EventArgs args)
        {
            StartCoroutine(StartDelayTimer()); // So it get reset and start again when User Log In or Log Out
        }
        #endregion
    }
}