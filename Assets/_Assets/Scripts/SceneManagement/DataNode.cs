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


    //[System.Serializable]
    //public class PlayerHistoryEntry : DataNode
    //{
    //    public string Activity;
    //    public AccommodationInfo Accommodation;
    //    public string BuildingName;
    //    public string RoomNumber;
    //    public TransportationInfo Transportation;
    //    public string AdditionalNotes;
    //    public string NotesColor;
    //    public string Status;
    //    public string StatusChangedAt;
    //}

    //[System.Serializable]
    //public class AccommodationInfo : DataNode
    //{
    //    public string IsStaying;
    //    public string StartDate;
    //    public string EndDate;
    //}

    //[System.Serializable]
    //public class TransportationInfo : DataNode
    //{
    //    public string HasCar;
    //    public string CarPlateNumber;
    //}
}