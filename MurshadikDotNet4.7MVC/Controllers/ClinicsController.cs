
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;
using Microsoft.AspNet.Identity;
using MurshadikCP.Models.DB;

namespace MurshadikCP.Controllers
{
	public class ClinicsController : BaseController
	{
		private mlaraEntities db = new mlaraEntities();

		// GET: Clinics
		public ActionResult Index()
		{
			return View(db.Clinics.ToList());
		}

		// GET: Clinics/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		// GET: Clinics/Create
		public ActionResult Create()
		{
			return View();
		}

		public ActionResult DoctorClinic(int Id)
		{
			var Clinic = (from c in db.Clinics
					  where c.Id == Id  
					
					  select new DoctorClinicsDto
					  {
						  Id = c.Id,
						  ClinicName = c.Name,
						  ClinicImg = c.Img,
						  Doctors = c.ClinicDoctors.Join(db.users,dc=>dc.Doctor.UserId,us=>us.id ,(dc,us)=> new Doctor
						  {
							  Id = dc.Doctor.Id,
							  Name = us.name,
							  WorkHouer = dc.Doctor.WorkingHours,
							  WorkDays = dc.Doctor.WorkingDays

						  }).ToList()
						 
					  }).FirstOrDefault();
			if (Clinic is null)
			{
				Clinic = new DoctorClinicsDto();
				Clinic.Doctors = new List<Doctor>();			
			}

		

			return View(Clinic);
		}

		// POST: Clinics/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Name,Description")] Clinic clinic, HttpPostedFileBase Img)

		{
			if (ModelState.IsValid)
			{
				Guid imgGuid = Guid.NewGuid();
				if (Img != null && Img.ContentLength > 0)
				{
					var img = imgGuid.ToString() + Path.GetExtension(Img.FileName);
					var path = Path.Combine(Server.MapPath("~/Media/Images/Clinics/"), img);
					Img.SaveAs(path);
					clinic.Img = img;
					clinic.CreatedAt = DateTime.Now;
					clinic.CreateBy = CurrentUser.Id;
				}
				db.Clinics.Add(clinic);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(clinic);
		}

		// GET: Clinics/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		// POST: Clinics/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,Description,Img,CreatedAt,UpdatedAt,CreateBy,UpdateBy")] Clinic clinic)
		{
			if (ModelState.IsValid)
			{
				db.Entry(clinic).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(clinic);
		}

		// GET: Clinics/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		// POST: Clinics/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Clinic clinic = db.Clinics.Find(id);
			db.Clinics.Remove(clinic);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
	

	public class DoctorClinicsDto
	{
		public long Id { get; set; }
		public string ClinicName { get; set; }
		public string ClinicImg { get; set; }
		public List<Doctor> Doctors { get; set; }
		
	}
	public class Doctor
	{

		public long Id { get; set; }
		public string Name { get; set; }
		public int WorkHouer { get; set; }
		public string WorkDays { get; set; }
	}
}
