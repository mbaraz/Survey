using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;
using SurveyModel;

namespace SurveyDomain.Repositories
{
    public class InterviewAnswerRepository : RepositoryBase<InterviewAnswer>, IInterviewAnswerRepository
    {
        public InterviewAnswerRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public InterviewAnswer GetById(int id)
        {
            return DataSet.Single(answer => answer.InterviewAnswerId == id);
        }
    }
}
