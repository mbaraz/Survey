using System.Data.Entity;
using SurveyInterfaces;
using SurveyModel;
using SurveyModel.Univer;

namespace SurveyDomain
{
    public class DBSurveyRepository : DbContext, IUnitOfWork
    {
        public DBSurveyRepository()
            : base("SurveyDb")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InterviewAnswer>().HasRequired(i => i.SurveyQuestion).WithMany(question => question.InterviewAnswers).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyQuestion>().HasOptional(sq => sq.BoundTag).WithMany().HasForeignKey(sq=>sq.BoundTagId);
            modelBuilder.Entity<SurveyQuestion>().HasOptional(sq => sq.FilterAnswersTag).WithMany().HasForeignKey(sq => sq.FilterAnswersTagId);
            modelBuilder.Entity<SurveyQuestion>().HasOptional(sq => sq.ConditionOnTag).WithMany().HasForeignKey(sq=>sq.ConditionOnTagId);

            modelBuilder.Entity<SubQuestion>().HasOptional(sq => sq.BoundTag).WithMany().HasForeignKey(sq => sq.BoundTagId);
            modelBuilder.Entity<SubQuestion>().HasOptional(sq => sq.FilterAnswersTag).WithMany().HasForeignKey(sq => sq.FilterAnswersTagId);

            modelBuilder.Entity<Interview>().HasOptional(i => i.Interviewer).WithMany().HasForeignKey(
                i => i.InterviewerId);
            modelBuilder.Entity<Interview>().HasRequired(i => i.Respondent).WithMany(respondent => respondent.Interviews).HasForeignKey(
                i => i.RespondentId);
            modelBuilder.Entity<Tag>().HasMany(tag => tag.ValueLabels).WithRequired(label => label.Tag).WillCascadeOnDelete(true);

            modelBuilder.Entity<SurveyInvitation>().HasOptional(invite => invite.Respondent).WithMany(
                res => res.Invitations).HasForeignKey(invitation => invitation.RespondentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyInvitation>().HasOptional(invite => invite.Ticket).WithMany(
                res => res.Invitations).HasForeignKey(invitation => invitation.TicketId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyInvitation>().HasOptional(invite => invite.OriginTicket).WithMany(
                res => res.CreatedInvitations).HasForeignKey(invitation => invitation.OriginTicketId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Interview>().HasMany(interview => interview.QuoteDimensionValues).WithMany(
                qdv => qdv.Interviews);

            modelBuilder.Entity<SurveyProject>().HasMany(project => project.Interviews).WithRequired(interview => interview.SurveyProject).WillCascadeOnDelete(false);

            modelBuilder.Entity<QuoteDimensionValue>().HasRequired(qdv => qdv.TagValueLabel).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>().HasEntitySetName("Univer_Course");
            modelBuilder.Entity<StudentGroup>().HasEntitySetName("Univer_StudentGroup");
        }

        public void Save()
        {
            SaveChanges();
        }
    }
}