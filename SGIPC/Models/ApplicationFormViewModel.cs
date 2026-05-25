using System;
using System.ComponentModel.DataAnnotations;

namespace SGIPC.Models
{
    public class ApplicationFormViewModel
    {
        [Required(ErrorMessage = "✗ Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "✗ Email address is required.")]
        [EmailAddress(ErrorMessage = "✗ Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "✗ Roll number is required.")]
        [StringLength(20, ErrorMessage = "Roll number cannot exceed 20 characters.")]
        [Display(Name = "Roll Number")]
        public string RollNumber { get; set; }

        [Required(ErrorMessage = "✗ Please select a department.")]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "✗ Please select a batch.")]
        [Display(Name = "Batch")]
        public string Batch { get; set; }

        [StringLength(100)]
        [Display(Name = "Codeforces Handle")]
        public string CodeForcesHandle { get; set; }

        [StringLength(100)]
        [Display(Name = "AtCoder Handle")]
        public string AtCoderHandle { get; set; }

        [StringLength(100)]
        [Display(Name = "CodeChef Handle")]
        public string CodeChefHandle { get; set; }

        [StringLength(100)]
        [Display(Name = "LeetCode Handle")]
        public string LeetCodeHandle { get; set; }

        [StringLength(100)]
        [Display(Name = "VJudge Handle")]
        public string VJudgeHandle { get; set; }

        [Required(ErrorMessage = "✗ Please tell us why you want to join SGIPC.")]
        [StringLength(500, ErrorMessage = "Your statement cannot exceed 500 characters.")]
        [Display(Name = "Statement of Interest")]
        public string ReasonForJoin { get; set; }
    }
}

