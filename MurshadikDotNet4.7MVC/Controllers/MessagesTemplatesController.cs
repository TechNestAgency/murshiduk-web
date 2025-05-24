using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MurshadikCP.Controllers
{
    [Authorize]
    public class MessagesTemplatesController : BaseController
    {
        // GET: MessagesTemplates
        // Index View for message templates just showing the list of records
        public ActionResult Index(int Id = 0)
        {
            if (currentPageID("MessagesTemplates") > 0)
            {
                if (!CurrentUser.canView(currentPageID("MessagesTemplates")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            ParentMessagesTemplates pmt = new ParentMessagesTemplates();
            pmt.messageList = db.MessagesTemplates.OrderByDescending(x => x.id).ToList();
            if (Id > 0)
            {
                pmt.messagesTemplate = db.MessagesTemplates.Where(x => x.id == Id).FirstOrDefault();
                ViewBag.notification_id = new SelectList(db.Messages_Templates_Notification.ToList(), "id", "name", pmt.messagesTemplate.notification_id);
            }
            else
            {
                pmt.messagesTemplate = new MessagesTemplate();
                ViewBag.notification_id = new SelectList(db.Messages_Templates_Notification.ToList(), "id", "name");
            }

            return View(pmt);
        }

        // Create message templates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentMessagesTemplates msg, string notification_id)
        {
            if (currentPageID("MessagesTemplates") > 0)
            {
                if (!CurrentUser.canInsert(currentPageID("MessagesTemplates")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                if (notification_id != null && notification_id != "")
                {
                    msg.messagesTemplate.notification_id = Convert.ToInt32(notification_id);
                }
                db.MessagesTemplates.Add(msg.messagesTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(msg);
        }

        // edit message templates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParentMessagesTemplates msg, string notification_id, int id)
        {
            if (currentPageID("MessagesTemplates") > 0)
            {
                if (!CurrentUser.canUpdate(currentPageID("MessagesTemplates")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (ModelState.IsValid)
            {
                if (notification_id != null && notification_id != "")
                {
                    msg.messagesTemplate.notification_id = Convert.ToInt32(notification_id);
                }
                msg.messagesTemplate.id = id;
                db.Entry(msg.messagesTemplate).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Successfully Updated!";
            }
            return RedirectToAction("Index");
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (currentPageID("MessagesTemplates") > 0)
            {
                if (!CurrentUser.canDelete(currentPageID("MessagesTemplates")))
                    return RedirectToAction("NotAllow", "Custom");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MessagesTemplate msg = db.MessagesTemplates.Find(id);
            db.MessagesTemplates.Remove(msg);
            db.SaveChanges();
            return Json("Success");
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

    public class ParentMessagesTemplates
    {
        public List<MessagesTemplate> messageList { get; set; }
        public MessagesTemplate messagesTemplate { get; set; }
    }
}