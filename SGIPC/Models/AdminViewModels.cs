using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SGIPC.Models
{
    public class AdminDashboardViewModel
    {
        public List<RegisteredUserViewModel> RegisteredUsers { get; set; } = new List<RegisteredUserViewModel>();
        public List<MemberInfoViewModel> ApprovedMembers { get; set; } = new List<MemberInfoViewModel>();
        public List<MemberInfoViewModel> PendingMembers { get; set; } = new List<MemberInfoViewModel>();
        public List<ContactMessageViewModel> ContactMessages { get; set; } = new List<ContactMessageViewModel>();
        public List<AnnouncementViewModel> Announcements { get; set; } = new List<AnnouncementViewModel>();
        public List<ResourceViewModel> Resources { get; set; } = new List<ResourceViewModel>();
        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
        public AnnouncementInputModel NewAnnouncement { get; set; } = new AnnouncementInputModel();
        public ResourceInputModel NewResource { get; set; } = new ResourceInputModel();
        public EventInputModel NewEvent { get; set; } = new EventInputModel();
    }

    public class RegisteredUserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HomePageViewModel
    {
        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
    }

    public class AnnouncementEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
    }

    public class ResourceEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string Description { get; set; }

        public HttpPostedFileBase File { get; set; }
    }

    public class EventViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string STime { get; set; }

        public string ETime { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status
        {
            get
            {
                try
                {
                    DateTime start =
                        Date.Date.Add(TimeSpan.Parse(STime));

                    DateTime end =
                        Date.Date.Add(TimeSpan.Parse(ETime));

                    DateTime now = DateTime.Now;

                    if (now < start)
                        return "Upcoming";

                    if (now >= start && now <= end)
                        return "Ongoing";

                    return "Finished";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }
    }

    public class EventInputModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public string STime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public string ETime { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters.")]
        public string Location { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; }
    }

    public class EventEditModel : EventInputModel
    {
        public int Id { get; set; }
    }

    public class MemberInfoViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RollNumber { get; set; }
        public string Department { get; set; }
        public string Batch { get; set; }
        public string CodeForcesHandle { get; set; }
        public string AtCoderHandle { get; set; }
        public string CodeChefHandle { get; set; }
        public string LeetCodeHandle { get; set; }
        public string VJudgeHandle { get; set; }
        public string Handles { get; set; }
        public string ReasonForJoin { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class ContactMessageViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ResourceViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string MediaType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AnnouncementInputModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
    }

    public class ResourceInputModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please upload a resource file.")]
        public HttpPostedFileBase File { get; set; }
    }
}
