using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace gFitness.Models.ViewModel
{
    public class RegisterView
    {
        [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
        [DisplayName("Kullanıcı Adı")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("E-Posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı boş olamaz.")]
        [StringLength(100, ErrorMessage = "Parola en az 6 karakter olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Parola Tekrar")]
        [Compare("Password", ErrorMessage = "Parolalar aynı değil.")]
        public string ConfirmPassword { get; set; }

        public string RoleId { get; set; }

        [Required(ErrorMessage = "Ad boş olamaz.")]
        [DisplayName("Adınız")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad boş olamaz.")]
        [DisplayName("Soyadınız")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Cinsiyet boş olamaz.")]
        [DisplayName("Cinsiyet")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Telefon numarası boş olamaz.")]
        [DisplayName("Telefon Numarası")]
        public string PhoneNumber { get; set; }
    }

    public class LoginView
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla?")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPassword
    {
        [Required(ErrorMessage = "Lütfen e-posta adresini doğru giriniz.")]
        [DisplayName("E-posta adresi")]

        [EmailAddress()]
        public string Email { get; set; }
    }

    public class UserProfileView
    {
        [Key]
        public int SYSUserID { get; set; }
        public int LOOKUPRoleID { get; set; }
        public string RoleName { get; set; }
        public bool? IsRoleActive { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Login ID")]
        public string LoginName { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
    }

    public class LOOKUPAvailableRole
    {
        [Key]
        public int LOOKUPRoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }

    public class Gender
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class UserRoles
    {
        public int? SelectedRoleID { get; set; }
        public IEnumerable<LOOKUPAvailableRole> UserRoleList { get; set; }
    }

    public class UserGender
    {
        public string SelectedGender { get; set; }
        public IEnumerable<Gender> Gender { get; set; }
    }

    public class UserDataView
    {
        public IEnumerable<UserProfileView> UserProfile { get; set; }
        public UserRoles UserRoles { get; set; }
        public UserGender UserGender { get; set; }
    }
}