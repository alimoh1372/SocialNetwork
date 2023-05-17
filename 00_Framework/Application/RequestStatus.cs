namespace _00_Framework.Application
{
    public enum RequestStatus
    {
        WithoutRequest = 0,
        RequestPending = 1,
        RequestAccepted=2,
        RevertRequestPending=3,
        RevertRequestAccepted=4,
        ErrorWithRelationNumbers=5,
        UnknownError=6
    }
}