using System;
using System.ComponentModel.DataAnnotations;
using SurveyCommon;

namespace SurveyWeb.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Логин / Login")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Электронная почта (вводится по желанию) / E-mail (optional)")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть не короче {2} символов / The {0} must be at least {2} characters long", MinimumLength = 6)]
        [Display(Name = "Пароль / Password")]
        public string Password { get; set; }

        public Guid Invite { get; set; }

        public bool HasActiveInvitation()
        {
            return Invite != Guid.Empty;
        }
    }

    public class StudentRegisterModel
    {
        [Required (ErrorMessage = " ")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Электронная почта")]
        [CustomValidation(typeof(EmailValidator), "IsFormatValidator", ErrorMessage = "должен быть не короче символов")]
        public string Email { get; set; }

        [Required (ErrorMessage = " ")]
        [StringLength(100, ErrorMessage = "{0} должен быть не короче {2} символов", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required (ErrorMessage = "Поле \"{0}\" обязательно")]
        [Display(Name = "Факультет")]
        public string Facility { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно")]
        [Display(Name = "Форма обучения")]
        public string ProgramType { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно")]
        [Display(Name = "Курс обучения")]
        public string YearNum { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно")]
        [Display(Name = "Группа")]
        public int GroupId { get; set; }
    }

    public static class EmailValidator
	{
	    public static ValidationResult IsFormatValidator(string value, ValidationContext context)
	    {
            if (value == null || value.IsValidEmail()) { return ValidationResult.Success; }

            return new ValidationResult("Неправильный адрес электронной почты.");
	    }
	}
}
