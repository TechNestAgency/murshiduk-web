using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IQARepository : IRepository<qa_questions>
    {
        qa_questions GetQuestionsByID(long id);
    }
}