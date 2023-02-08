using System;

namespace SelfTraining.Models
{
    public sealed class SignupConfirmationViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
