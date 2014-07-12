using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using SurveyModel;

namespace SurveyDomain
{
    public class DropCreateDBInitializer : CreateDatabaseIfNotExists<DBSurveyRepository>
    {
        protected override void Seed(DBSurveyRepository context)
        {
            base.Seed(context);
            context.Set<Respondent>().Add(new Respondent
                                              {
                                                  IsMale = true,
                                                  MembershipUserName = "tsarev",
                                                  RespondentEmail = "leo@bastilia.ru",
                                              });
        }
    }
}
