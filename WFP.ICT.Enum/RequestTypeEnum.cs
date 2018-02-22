namespace WFP.ICT.Enum
{
    public enum RequestTypeEnum
    {
        Login = 101,
        RegisterToken = 102,
        GetAssignments = 103,
        GetAssignment = 104,
        GetUnits = 105,
        GetUnit = 106,

        UploadSign = 201,
        UploadPianoImage = 202,
        SyncTripStart = 203,
        SyncLoad = 204,
        SyncDeliver = 205,
        SyncImage = 206,

        PaymentProcess = 301,
        PaymentSaveCard = 302,
    }
}
