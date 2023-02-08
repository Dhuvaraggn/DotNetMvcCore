namespace SelfTraining.Models
{
    public class AuthResponseModel : BaseResponseModel
    {
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public TokenModel Tokens { get; set; }
    }
}
