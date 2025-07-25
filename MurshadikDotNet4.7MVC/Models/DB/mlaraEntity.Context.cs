﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MurshadikCP.Models.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class mlaraEntities : DbContext
    {
        public mlaraEntities()
            : base("name=mlaraEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agriculture_Calender> Agriculture_Calender { get; set; }
        public virtual DbSet<AlertHazard> AlertHazards { get; set; }
        public virtual DbSet<AlertMessage> AlertMessages { get; set; }
        public virtual DbSet<AlertType> AlertTypes { get; set; }
        public virtual DbSet<app_settings> app_settings { get; set; }
        public virtual DbSet<app_stats> app_stats { get; set; }
        public virtual DbSet<app_type> app_type { get; set; }
        public virtual DbSet<appointment> appointments { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<attention> attentions { get; set; }
        public virtual DbSet<bug_history> bug_history { get; set; }
        public virtual DbSet<calldetail> calldetails { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<category_articles> category_articles { get; set; }
        public virtual DbSet<city> cities { get; set; }
        public virtual DbSet<commentsByUser> commentsByUsers { get; set; }
        public virtual DbSet<consultant_rating> consultant_rating { get; set; }
        public virtual DbSet<currentweathernotificationforCity> currentweathernotificationforCities { get; set; }
        public virtual DbSet<general_notifications> general_notifications { get; set; }
        public virtual DbSet<group_user> group_user { get; set; }
        public virtual DbSet<group> groups { get; set; }
        public virtual DbSet<keyword_group> keyword_group { get; set; }
        public virtual DbSet<keyword> keywords { get; set; }
        public virtual DbSet<LabCollecctionPoint> LabCollecctionPoints { get; set; }
        public virtual DbSet<lab> labs { get; set; }
        public virtual DbSet<market_products> market_products { get; set; }
        public virtual DbSet<market> markets { get; set; }
        public virtual DbSet<medium> media { get; set; }
        public virtual DbSet<Messages_Templates_Notification> Messages_Templates_Notification { get; set; }
        public virtual DbSet<MessagesTemplate> MessagesTemplates { get; set; }
        public virtual DbSet<messagetype> messagetypes { get; set; }
        public virtual DbSet<msg> msgs { get; set; }
        public virtual DbSet<msg_logs> msg_logs { get; set; }
        public virtual DbSet<municipality> municipalities { get; set; }
        public virtual DbSet<notification> notifications { get; set; }
        public virtual DbSet<notification_class> notification_class { get; set; }
        public virtual DbSet<otp> otps { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<past_experiences> past_experiences { get; set; }
        public virtual DbSet<product_categories> product_categories { get; set; }
        public virtual DbSet<product_subscribers> product_subscribers { get; set; }
        public virtual DbSet<product_type> product_type { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<qa_answers> qa_answers { get; set; }
        public virtual DbSet<qa_attachments> qa_attachments { get; set; }
        public virtual DbSet<qa_category> qa_category { get; set; }
        public virtual DbSet<qa_questions> qa_questions { get; set; }
        public virtual DbSet<qa_votes> qa_votes { get; set; }
        public virtual DbSet<region> regions { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<roles_permission> roles_permission { get; set; }
        public virtual DbSet<skill_user> skill_user { get; set; }
        public virtual DbSet<skill> skills { get; set; }
        public virtual DbSet<subscriber> subscribers { get; set; }
        public virtual DbSet<unit> units { get; set; }
        public virtual DbSet<user_access> user_access { get; set; }
        public virtual DbSet<weather_notification> weather_notification { get; set; }
        public virtual DbSet<WeatherData> WeatherDatas { get; set; }
        public virtual DbSet<articles_galleries> articles_galleries { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<UserAccessTypeIdentifier> UserAccessTypeIdentifiers { get; set; }
        public virtual DbSet<NotificationLog> NotificationLogs { get; set; }
        public virtual DbSet<lab_reports> lab_reports { get; set; }
        public virtual DbSet<bug_tracker> bug_tracker { get; set; }
        public virtual DbSet<faq> faqs { get; set; }
        public virtual DbSet<chatmessage> chatmessages { get; set; }
        public virtual DbSet<chat> chats { get; set; }
        public virtual DbSet<article> articles { get; set; }
        public virtual DbSet<group_messages> group_messages { get; set; }
        public virtual DbSet<VideoCategory> VideoCategories { get; set; }
        public virtual DbSet<VideoComment> VideoComments { get; set; }
        public virtual DbSet<VideoLike> VideoLikes { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<VideoTag> VideoTags { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tempuser> tempusers { get; set; }
        public virtual DbSet<GetLatestProductPrice> GetLatestProductPrices { get; set; }
        public virtual DbSet<GuidedGuideLog> GuidedGuideLogs { get; set; }
        public virtual DbSet<UserStatusLog> UserStatusLogs { get; set; }
        public virtual DbSet<FarmVisitMessage> FarmVisitMessages { get; set; }
        public virtual DbSet<FarmVisitOrder> FarmVisitOrders { get; set; }
        public virtual DbSet<FarmVisitReport> FarmVisitReports { get; set; }
        public virtual DbSet<FarmVisitStatus> FarmVisitStatuses { get; set; }
        public virtual DbSet<FarmVisitMedia> FarmVisitMedias { get; set; }
        public virtual DbSet<ConsultantsActiveHoursInDay> ConsultantsActiveHoursInDays { get; set; }
        public virtual DbSet<ConsultantsDailyActiveHour> ConsultantsDailyActiveHours { get; set; }
        public virtual DbSet<ConsultantsStatusLog> ConsultantsStatusLogs { get; set; }
        public virtual DbSet<FarmVisitService> FarmVisitServices { get; set; }
        public virtual DbSet<FarmerWorkerStatu> FarmerWorkerStatus { get; set; }
        public virtual DbSet<JobNotification> JobNotifications { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Nationalty> Nationalties { get; set; }
        public virtual DbSet<WorkerExperience> WorkerExperiences { get; set; }
        public virtual DbSet<WorkerJob> WorkerJobs { get; set; }
        public virtual DbSet<WorkerNotification> WorkerNotifications { get; set; }
        public virtual DbSet<Worker> Workers { get; set; }
        public virtual DbSet<ClinicAppointmentHistory> ClinicAppointmentHistories { get; set; }
        public virtual DbSet<ClinicAppointment> ClinicAppointments { get; set; }
        public virtual DbSet<ClinicDoctor> ClinicDoctors { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<DoctorRating> DoctorRatings { get; set; }
        public virtual DbSet<ConsultationAvailablity> ConsultationAvailablities { get; set; }
        public virtual DbSet<ConsultationAppointment> ConsultationAppointments { get; set; }
        public virtual DbSet<ReefComponent> ReefComponents { get; set; }
        public virtual DbSet<ReefComponentSuggestion> ReefComponentSuggestions { get; set; }
    
        public virtual ObjectResult<GetAllUsers_Result> GetAllUsers()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAllUsers_Result>("GetAllUsers");
        }
    
        public virtual int GenerateAppointments(Nullable<int> lab_id, Nullable<System.DateTime> appointment_Date)
        {
            var lab_idParameter = lab_id.HasValue ?
                new ObjectParameter("lab_id", lab_id) :
                new ObjectParameter("lab_id", typeof(int));
    
            var appointment_DateParameter = appointment_Date.HasValue ?
                new ObjectParameter("appointment_Date", appointment_Date) :
                new ObjectParameter("appointment_Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GenerateAppointments", lab_idParameter, appointment_DateParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetLatestProductPrices")]
        public virtual IQueryable<fun_GetLatestProductPrices_Result> fun_GetLatestProductPrices(Nullable<int> market_id)
        {
            var market_idParameter = market_id.HasValue ?
                new ObjectParameter("market_id", market_id) :
                new ObjectParameter("market_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fun_GetLatestProductPrices_Result>("[mlaraEntities].[fun_GetLatestProductPrices](@market_id)", market_idParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetLatestProductPricesByUser")]
        public virtual IQueryable<fun_GetLatestProductPricesByUser_Result> fun_GetLatestProductPricesByUser(Nullable<int> market_id, Nullable<int> product_id)
        {
            var market_idParameter = market_id.HasValue ?
                new ObjectParameter("market_id", market_id) :
                new ObjectParameter("market_id", typeof(int));
    
            var product_idParameter = product_id.HasValue ?
                new ObjectParameter("product_id", product_id) :
                new ObjectParameter("product_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fun_GetLatestProductPricesByUser_Result>("[mlaraEntities].[fun_GetLatestProductPricesByUser](@market_id, @product_id)", market_idParameter, product_idParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetLatestProductPricesbyYear")]
        public virtual IQueryable<fun_GetLatestProductPricesbyYear_Result> fun_GetLatestProductPricesbyYear(Nullable<int> market_id, Nullable<long> product_id)
        {
            var market_idParameter = market_id.HasValue ?
                new ObjectParameter("market_id", market_id) :
                new ObjectParameter("market_id", typeof(int));
    
            var product_idParameter = product_id.HasValue ?
                new ObjectParameter("product_id", product_id) :
                new ObjectParameter("product_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fun_GetLatestProductPricesbyYear_Result>("[mlaraEntities].[fun_GetLatestProductPricesbyYear](@market_id, @product_id)", market_idParameter, product_idParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetConsultantSkills")]
        public virtual IQueryable<string> fun_GetConsultantSkills(Nullable<long> user_id)
        {
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<string>("[mlaraEntities].[fun_GetConsultantSkills](@user_id)", user_idParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetAllMyLabAppointment")]
        public virtual IQueryable<fun_GetAllMyLabAppointment_Result> fun_GetAllMyLabAppointment(Nullable<long> user_id)
        {
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fun_GetAllMyLabAppointment_Result>("[mlaraEntities].[fun_GetAllMyLabAppointment](@user_id)", user_idParameter);
        }
    
        public virtual int sp_UpdateKeywordCountZero()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_UpdateKeywordCountZero");
        }
    
        public virtual int DeleteUserByID(Nullable<long> user_id)
        {
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteUserByID", user_idParameter);
        }
    
        public virtual ObjectResult<sp_getDataForUsersByDate_Result> sp_getDataForUsersByDate(Nullable<int> app_type, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var app_typeParameter = app_type.HasValue ?
                new ObjectParameter("app_type", app_type) :
                new ObjectParameter("app_type", typeof(int));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getDataForUsersByDate_Result>("sp_getDataForUsersByDate", app_typeParameter, fromDateParameter, toDateParameter);
        }
    
        [DbFunction("mlaraEntities", "fun_GetCategoryTreeIDs")]
        public virtual IQueryable<Nullable<long>> fun_GetCategoryTreeIDs(Nullable<int> catid)
        {
            var catidParameter = catid.HasValue ?
                new ObjectParameter("catid", catid) :
                new ObjectParameter("catid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Nullable<long>>("[mlaraEntities].[fun_GetCategoryTreeIDs](@catid)", catidParameter);
        }
    
        public virtual ObjectResult<GetTop5ConsultantRating_Result> GetTop5ConsultantRating(Nullable<int> region_Id)
        {
            var region_IdParameter = region_Id.HasValue ?
                new ObjectParameter("Region_Id", region_Id) :
                new ObjectParameter("Region_Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTop5ConsultantRating_Result>("GetTop5ConsultantRating", region_IdParameter);
        }
    
        public virtual ObjectResult<GetTop5ConsultantCalling_Result> GetTop5ConsultantCalling(Nullable<int> region_ID)
        {
            var region_IDParameter = region_ID.HasValue ?
                new ObjectParameter("region_ID", region_ID) :
                new ObjectParameter("region_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTop5ConsultantCalling_Result>("GetTop5ConsultantCalling", region_IDParameter);
        }
    
        public virtual ObjectResult<GetChatDetailsByUserID_Result> GetChatDetailsByUserID(Nullable<long> user_from, Nullable<long> user_to)
        {
            var user_fromParameter = user_from.HasValue ?
                new ObjectParameter("user_from", user_from) :
                new ObjectParameter("user_from", typeof(long));
    
            var user_toParameter = user_to.HasValue ?
                new ObjectParameter("user_to", user_to) :
                new ObjectParameter("user_to", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetChatDetailsByUserID_Result>("GetChatDetailsByUserID", user_fromParameter, user_toParameter);
        }
    
        public virtual ObjectResult<GetChatListByID_Result> GetChatListByID(Nullable<long> user_id)
        {
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("user_id", user_id) :
                new ObjectParameter("user_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetChatListByID_Result>("GetChatListByID", user_idParameter);
        }
    
        public virtual ObjectResult<GetMaxProductIDByMarketID_Result> GetMaxProductIDByMarketID(Nullable<long> market_id)
        {
            var market_idParameter = market_id.HasValue ?
                new ObjectParameter("market_id", market_id) :
                new ObjectParameter("market_id", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetMaxProductIDByMarketID_Result>("GetMaxProductIDByMarketID", market_idParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GeneralReport(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GeneralReport", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<GETMARKETREPORT_Result> GETMARKETREPORT(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GETMARKETREPORT_Result>("GETMARKETREPORT", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<GETLabREPORT_Result> GETLabREPORT(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GETLabREPORT_Result>("GETLabREPORT", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<GETConsultantREPORT_Result> GETConsultantREPORT(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GETConsultantREPORT_Result>("GETConsultantREPORT", fromDateParameter, toDateParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int AutoGenerateAppointmentForOneMonth()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AutoGenerateAppointmentForOneMonth");
        }
    
        [DbFunction("mlaraEntities", "fun_getUserOnlineOfflineHours")]
        public virtual IQueryable<fun_getUserOnlineOfflineHours_Result> fun_getUserOnlineOfflineHours(Nullable<System.DateTime> date_From, Nullable<System.DateTime> date_To, Nullable<int> user_id)
        {
            var date_FromParameter = date_From.HasValue ?
                new ObjectParameter("Date_From", date_From) :
                new ObjectParameter("Date_From", typeof(System.DateTime));
    
            var date_ToParameter = date_To.HasValue ?
                new ObjectParameter("Date_To", date_To) :
                new ObjectParameter("Date_To", typeof(System.DateTime));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("User_id", user_id) :
                new ObjectParameter("User_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fun_getUserOnlineOfflineHours_Result>("[mlaraEntities].[fun_getUserOnlineOfflineHours](@Date_From, @Date_To, @User_id)", date_FromParameter, date_ToParameter, user_idParameter);
        }
    
        public virtual int get_totalOnlineHours(Nullable<System.DateTime> date_From, Nullable<System.DateTime> date_To, Nullable<int> user_id)
        {
            var date_FromParameter = date_From.HasValue ?
                new ObjectParameter("Date_From", date_From) :
                new ObjectParameter("Date_From", typeof(System.DateTime));
    
            var date_ToParameter = date_To.HasValue ?
                new ObjectParameter("Date_To", date_To) :
                new ObjectParameter("Date_To", typeof(System.DateTime));
    
            var user_idParameter = user_id.HasValue ?
                new ObjectParameter("User_id", user_id) :
                new ObjectParameter("User_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("get_totalOnlineHours", date_FromParameter, date_ToParameter, user_idParameter);
        }
    
        public virtual int ClinicAppointmentProc(Nullable<int> doctorId, Nullable<System.DateTime> appointmentDate)
        {
            var doctorIdParameter = doctorId.HasValue ?
                new ObjectParameter("DoctorId", doctorId) :
                new ObjectParameter("DoctorId", typeof(int));
    
            var appointmentDateParameter = appointmentDate.HasValue ?
                new ObjectParameter("AppointmentDate", appointmentDate) :
                new ObjectParameter("AppointmentDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ClinicAppointmentProc", doctorIdParameter, appointmentDateParameter);
        }
    }
}
