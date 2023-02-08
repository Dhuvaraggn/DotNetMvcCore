namespace SelfTraining.Models
{

    public enum CognitoStatusCodes
    {
        USER_UNCONFIRMED,
        API_ERROR,
        USER_NOTFOUND
    }
    public class BaseResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CognitoStatusCodes Status { get; set; }
    }
}
