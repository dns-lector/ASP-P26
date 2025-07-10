namespace ASP_P26.Models.User
{
    public class UserSignupPageModel
    {
        public UserSignupFormModel? FormModel { get; set; }
        public Dictionary<String,String>? FormErrors { get; set; }
    }
}
