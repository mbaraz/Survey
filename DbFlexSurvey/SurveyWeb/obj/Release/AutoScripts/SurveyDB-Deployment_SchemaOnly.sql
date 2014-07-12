SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EdmMetadata](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModelHash] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Respondents](
	[RespondentId] [int] IDENTITY(1,1) NOT NULL,
	[RespondentFirstName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[RespondentFatherName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[RespondentSurName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[RespondentEmail] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[RespondentPhone] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[Token] [uniqueidentifier] NULL,
	[BirthYear] [int] NULL,
	[IsMale] [bit] NULL,
	[MembershipUserName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[RespondentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyProjects](
	[SurveyProjectId] [int] IDENTITY(1,1) NOT NULL,
	[SurveyProjectName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[ProjectUserDescription] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SurveyProjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[SurveyProjectId] [int] NULL,
	[TagName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[IsAgeTag] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyInvitations](
	[SurveyInvitationId] [int] IDENTITY(1,1) NOT NULL,
	[RespondentId] [int] NOT NULL,
	[SurveyProjectId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SurveyInvitationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quotes](
	[QuoteId] [int] IDENTITY(1,1) NOT NULL,
	[SurveyProjectId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Interviews](
	[InterviewId] [int] IDENTITY(1,1) NOT NULL,
	[RespondentId] [int] NOT NULL,
	[InterviewerId] [int] NULL,
	[SurveyProjectId] [int] NOT NULL,
	[Completed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InterviewId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuoteDimensions](
	[QuoteDimensionId] [int] IDENTITY(1,1) NOT NULL,
	[TagId] [int] NOT NULL,
	[QuoteId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteDimensionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyQuestions](
	[SurveyQuestionId] [int] IDENTITY(1,1) NOT NULL,
	[SurveyProjectId] [int] NOT NULL,
	[QuestionName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[QuestionText] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[MultipleAnswerAllowed] [bit] NOT NULL,
	[MaxAnswers] [int] NULL,
	[QuestionOrder] [int] NOT NULL,
	[BoundTagId] [int] NULL,
	[ConditionOnTagId] [int] NULL,
	[ConditionOnTagValue] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SurveyQuestionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagValues](
	[TagValueId] [int] IDENTITY(1,1) NOT NULL,
	[TagId] [int] NOT NULL,
	[InterviewId] [int] NOT NULL,
	[Value] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TagValueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagValueLabels](
	[TagValueLabelId] [int] IDENTITY(1,1) NOT NULL,
	[TagId] [int] NOT NULL,
	[ValueCode] [int] NOT NULL,
	[Label] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[TagValueLabelId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InterviewAnswers](
	[InterviewAnswerId] [int] IDENTITY(1,1) NOT NULL,
	[InterviewId] [int] NOT NULL,
	[SurveyQuestionId] [int] NOT NULL,
	[AnswerSerialized] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[InterviewAnswerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuoteDimensionValues](
	[QuoteDimensionValueId] [int] IDENTITY(1,1) NOT NULL,
	[QuoteDimensionId] [int] NOT NULL,
	[TagValueLabelId] [int] NOT NULL,
	[HardLimit] [int] NOT NULL,
	[SoftLimit] [int] NOT NULL,
	[CurrentValue] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteDimensionValueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnswerVariants](
	[AnswerVariantId] [int] IDENTITY(1,1) NOT NULL,
	[SurveyQuestionId] [int] NOT NULL,
	[AnswerText] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[IsOpenAnswer] [bit] NOT NULL,
	[AnswerCode] [int] NOT NULL,
	[AnswerOrder] [int] NOT NULL,
	[TagValue] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AnswerVariantId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InterviewQuoteDimensionValues](
	[Interview_InterviewId] [int] NOT NULL,
	[QuoteDimensionValue_QuoteDimensionValueId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Interview_InterviewId] ASC,
	[QuoteDimensionValue_QuoteDimensionValueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
ALTER TABLE [dbo].[Tags]  WITH CHECK ADD  CONSTRAINT [Tag_SurveyProject] FOREIGN KEY([SurveyProjectId])
REFERENCES [dbo].[SurveyProjects] ([SurveyProjectId])
GO
ALTER TABLE [dbo].[Tags] CHECK CONSTRAINT [Tag_SurveyProject]
GO
ALTER TABLE [dbo].[SurveyInvitations]  WITH CHECK ADD  CONSTRAINT [SurveyInvitation_Respondent] FOREIGN KEY([RespondentId])
REFERENCES [dbo].[Respondents] ([RespondentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SurveyInvitations] CHECK CONSTRAINT [SurveyInvitation_Respondent]
GO
ALTER TABLE [dbo].[SurveyInvitations]  WITH CHECK ADD  CONSTRAINT [SurveyProject_Invitations] FOREIGN KEY([SurveyProjectId])
REFERENCES [dbo].[SurveyProjects] ([SurveyProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SurveyInvitations] CHECK CONSTRAINT [SurveyProject_Invitations]
GO
ALTER TABLE [dbo].[Quotes]  WITH CHECK ADD  CONSTRAINT [Quote_SurveyProject] FOREIGN KEY([SurveyProjectId])
REFERENCES [dbo].[SurveyProjects] ([SurveyProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Quotes] CHECK CONSTRAINT [Quote_SurveyProject]
GO
ALTER TABLE [dbo].[Interviews]  WITH CHECK ADD  CONSTRAINT [Interview_Interviewer] FOREIGN KEY([InterviewerId])
REFERENCES [dbo].[Respondents] ([RespondentId])
GO
ALTER TABLE [dbo].[Interviews] CHECK CONSTRAINT [Interview_Interviewer]
GO
ALTER TABLE [dbo].[Interviews]  WITH CHECK ADD  CONSTRAINT [Interview_Respondent] FOREIGN KEY([RespondentId])
REFERENCES [dbo].[Respondents] ([RespondentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Interviews] CHECK CONSTRAINT [Interview_Respondent]
GO
ALTER TABLE [dbo].[Interviews]  WITH CHECK ADD  CONSTRAINT [SurveyProject_Interviews] FOREIGN KEY([SurveyProjectId])
REFERENCES [dbo].[SurveyProjects] ([SurveyProjectId])
GO
ALTER TABLE [dbo].[Interviews] CHECK CONSTRAINT [SurveyProject_Interviews]
GO
ALTER TABLE [dbo].[QuoteDimensions]  WITH CHECK ADD  CONSTRAINT [Quote_Dimensions] FOREIGN KEY([QuoteId])
REFERENCES [dbo].[Quotes] ([QuoteId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuoteDimensions] CHECK CONSTRAINT [Quote_Dimensions]
GO
ALTER TABLE [dbo].[QuoteDimensions]  WITH CHECK ADD  CONSTRAINT [QuoteDimension_Tag] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([TagId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuoteDimensions] CHECK CONSTRAINT [QuoteDimension_Tag]
GO
ALTER TABLE [dbo].[SurveyQuestions]  WITH CHECK ADD  CONSTRAINT [SurveyQuestion_BoundTag] FOREIGN KEY([BoundTagId])
REFERENCES [dbo].[Tags] ([TagId])
GO
ALTER TABLE [dbo].[SurveyQuestions] CHECK CONSTRAINT [SurveyQuestion_BoundTag]
GO
ALTER TABLE [dbo].[SurveyQuestions]  WITH CHECK ADD  CONSTRAINT [SurveyQuestion_ConditionOnTag] FOREIGN KEY([ConditionOnTagId])
REFERENCES [dbo].[Tags] ([TagId])
GO
ALTER TABLE [dbo].[SurveyQuestions] CHECK CONSTRAINT [SurveyQuestion_ConditionOnTag]
GO
ALTER TABLE [dbo].[SurveyQuestions]  WITH CHECK ADD  CONSTRAINT [SurveyQuestion_SurveyProject] FOREIGN KEY([SurveyProjectId])
REFERENCES [dbo].[SurveyProjects] ([SurveyProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SurveyQuestions] CHECK CONSTRAINT [SurveyQuestion_SurveyProject]
GO
ALTER TABLE [dbo].[TagValues]  WITH CHECK ADD  CONSTRAINT [TagValue_Interview] FOREIGN KEY([InterviewId])
REFERENCES [dbo].[Interviews] ([InterviewId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TagValues] CHECK CONSTRAINT [TagValue_Interview]
GO
ALTER TABLE [dbo].[TagValues]  WITH CHECK ADD  CONSTRAINT [TagValue_Tag] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([TagId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TagValues] CHECK CONSTRAINT [TagValue_Tag]
GO
ALTER TABLE [dbo].[TagValueLabels]  WITH CHECK ADD  CONSTRAINT [Tag_ValueLabels] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([TagId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TagValueLabels] CHECK CONSTRAINT [Tag_ValueLabels]
GO
ALTER TABLE [dbo].[InterviewAnswers]  WITH CHECK ADD  CONSTRAINT [Interview_Answers] FOREIGN KEY([InterviewId])
REFERENCES [dbo].[Interviews] ([InterviewId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InterviewAnswers] CHECK CONSTRAINT [Interview_Answers]
GO
ALTER TABLE [dbo].[InterviewAnswers]  WITH CHECK ADD  CONSTRAINT [InterviewAnswer_SurveyQuestion] FOREIGN KEY([SurveyQuestionId])
REFERENCES [dbo].[SurveyQuestions] ([SurveyQuestionId])
GO
ALTER TABLE [dbo].[InterviewAnswers] CHECK CONSTRAINT [InterviewAnswer_SurveyQuestion]
GO
ALTER TABLE [dbo].[QuoteDimensionValues]  WITH CHECK ADD  CONSTRAINT [QuoteDimension_Values] FOREIGN KEY([QuoteDimensionId])
REFERENCES [dbo].[QuoteDimensions] ([QuoteDimensionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QuoteDimensionValues] CHECK CONSTRAINT [QuoteDimension_Values]
GO
ALTER TABLE [dbo].[QuoteDimensionValues]  WITH CHECK ADD  CONSTRAINT [QuoteDimensionValue_TagValueLabel] FOREIGN KEY([TagValueLabelId])
REFERENCES [dbo].[TagValueLabels] ([TagValueLabelId])
GO
ALTER TABLE [dbo].[QuoteDimensionValues] CHECK CONSTRAINT [QuoteDimensionValue_TagValueLabel]
GO
ALTER TABLE [dbo].[AnswerVariants]  WITH CHECK ADD  CONSTRAINT [AnswerVariant_SurveyQuestion] FOREIGN KEY([SurveyQuestionId])
REFERENCES [dbo].[SurveyQuestions] ([SurveyQuestionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AnswerVariants] CHECK CONSTRAINT [AnswerVariant_SurveyQuestion]
GO
ALTER TABLE [dbo].[InterviewQuoteDimensionValues]  WITH CHECK ADD  CONSTRAINT [Interview_QuoteDimensionValues_Source] FOREIGN KEY([Interview_InterviewId])
REFERENCES [dbo].[Interviews] ([InterviewId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InterviewQuoteDimensionValues] CHECK CONSTRAINT [Interview_QuoteDimensionValues_Source]
GO
ALTER TABLE [dbo].[InterviewQuoteDimensionValues]  WITH CHECK ADD  CONSTRAINT [Interview_QuoteDimensionValues_Target] FOREIGN KEY([QuoteDimensionValue_QuoteDimensionValueId])
REFERENCES [dbo].[QuoteDimensionValues] ([QuoteDimensionValueId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InterviewQuoteDimensionValues] CHECK CONSTRAINT [Interview_QuoteDimensionValues_Target]
GO
