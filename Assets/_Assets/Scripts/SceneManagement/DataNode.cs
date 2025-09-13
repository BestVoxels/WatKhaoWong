namespace WatKhaoWong.SceneManagement
{
    public class DataNode
    {
    }


    [System.Serializable]
    public class GeneralInfo : DataNode
    {
        public string PhoneNumber;
        public string MedicalCondition;
        public EmergencyContact EmergencyContact;
        public SocialAccounts SocialAccounts;
    }
    [System.Serializable]
    public class EmergencyContact : DataNode
    {
        public string PhoneNumber;
        public string Relation;
    }
    [System.Serializable]
    public class SocialAccounts : DataNode
    {
        public string Line;
        public string Facebook;
        public string Instagram;
        public string Tiktok;
    }



    [System.Serializable]
    public class AccountStatus : DataNode
    {
        public string LastCheckinAt;
        public StatusInfo StatusInfo;
        public string BanEndDate;
        public NotesInfo NotesInfo;
    }



    [System.Serializable]
    public class ActiveStay : DataNode
    {
        public string KeyId;
        public StatusInfo StatusInfo;
    }



    [System.Serializable]
    public class StayEntry : DataNode
    {
        public string UserId;
        public string Activity;
        public StayInfo StayInfo;
        public RoomInfo RoomInfo;
        public Transportation Transportation;
        public NotesInfo NotesInfo;
        public StatusInfo StatusInfo;
    }
    [System.Serializable]
    public class StayInfo : DataNode
    {
        public string IsStaying;
        public string StartDate;
        public string EndDate;
    }
    [System.Serializable]
    public class RoomInfo : DataNode
    {
        public string BuildingName;
        public string RoomNumber;
    }
    [System.Serializable]
    public class Transportation : DataNode
    {
        public string HasCar;
        public string CarPlateNumber;
    }
    [System.Serializable]
    public class NotesInfo : DataNode
    {
        public string Text;
        public string Color;
    }
    [System.Serializable]
    public class StatusInfo : DataNode
    {
        public string Status;
        public string StatusUpdatedAt;

        public StatusInfo()
        {
            Status = null;
            StatusUpdatedAt = null;
        }
    }



    [System.Serializable]
    public class PassportInfo : DataNode
    {
        public string PassportNumber;
        public string FullName;
        public string Nationality;
        public string BirthDate;
        public string Gender;
        public string PlaceOfBirth;
        public string IssuingOffice;
        public string IssueDate;
        public string ExpireDate;
    }


    [System.Serializable]
    public class NationalIDInfo : DataNode
    {
        public string NationalID;
        public Address Address;
        public string IssueDate;
        public string ExpireDate;
        public string Issuer;
        public string BirthDate;
        public string Gender;
        public Name Name;
    }
    [System.Serializable]
    public class Address : DataNode
    {
        public string HouseNumber;
        public string District;
        public string Tambon;
        public string Province;
        public string Country;
        public string ShortAddress;
    }
    [System.Serializable]
    public class Name : DataNode
    {
        public string ENPrefix;
        public string ENFirstName;
        public string ENLastName;
        public string THPrefix;
        public string THFirstName;
        public string THLastName;
    }
}