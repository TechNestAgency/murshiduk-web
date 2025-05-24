
using Microsoft.Owin.Security.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
	public  class WorkerDto
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public string Name { get; set; }
		public string Job { get; set; }
		public bool Isbusy { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; }
		public string Img { get; set; }
		public int ExpectedSalary { get;  set; }
		public string Nationalty { get; internal set; }
		public string NationaltyAr { get; internal set; }
	}
	public class WorkerDetail
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public bool Isbusy { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; }
		public List<JobDto> Jobs { get; set; }
		public List<ExperienceDto> Experiences { get; set; }
		public int ExpectedSalary { get;  set; }
		public string Nationalty { get; internal set; }
		public string NationaltyAr { get; internal set; }
		public string Img { get; set; }
	}
	public class JobDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	 
	}
	public class ExperienceDto
	{
		public long Id { get; set; }
		[Required]
	    public long WorkerId { get; set; }
		[Required]

		public string Description { get; set; }
		[Required]

		public DateTime FromDate { get; set; }
		[Required]

		public DateTime ToDate { get; set; }

		
	}

	public class WorkerData
	{
		public long Id { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		public long IDNumber { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public DateTime DateOfBirth { get; set; }
		[Required]
		public int ExpectedSalary { get; set; }
		[Required]
		public int NationaltyId { get; set; }
		
		[Required]
		public bool Isbusy { get; set; }
		public long UserId { get; set; }


	}

}