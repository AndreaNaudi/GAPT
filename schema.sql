CREATE TABLE [Proposal] (
	Id int NOT NULL,
	CreatedBy varchar(225),
	CreatedOn datetime,
	GeneralId int,
	ProgrammeRationaleId int,
	ExternalReviewId int,
	IncomeExpenditureId int,
	ApprovalId int,
	InPrincipalId int,
	Submitted bit NOT NULL DEFAULT '0',
	InPrincipalApproved bit,
	RequiresModification bit NOT NULL,
	IsInEdit bit,
	UserEditing nvarchar(128),
  CONSTRAINT [PK_PROPOSAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [General] (
	Id int NOT NULL,
	Title varchar(500),
	AreasStudy varchar(500),
	LevelId int,
	FacultyId int,
	DeliveryId int,
	FirstDateIntake varchar(225),
	ExpectedStudents int,
	MaxStudents int,
	CappingReason varchar(4000),
	DurationSemesters int,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_GENERAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Level] (
	Id int NOT NULL,
	Name varchar(225) NOT NULL,
  CONSTRAINT [PK_REF_LEVEL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Faculty] (
	Id int NOT NULL,
	Name varchar(225) NOT NULL,
	Dean nvarchar(128) NOT NULL,
  CONSTRAINT [PK_REF_FACULTY] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Delivery] (
	Id int NOT NULL,
	Name varchar(225) NOT NULL,
  CONSTRAINT [PK_REF_DELIVERY] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Proposer_General] (
	Id int NOT NULL,
	GeneralId int NOT NULL,
	UserId nvarchar(128) NOT NULL,
  CONSTRAINT [PK_PROPOSER_GENERAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Department] (
	Id int NOT NULL,
	Name varchar(225) NOT NULL,
	FacultyId int NOT NULL,
	HoD nvarchar(128) NOT NULL,
  CONSTRAINT [PK_REF_DEPARTMENT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Department_General] (
	Id int NOT NULL,
	GeneralId int NOT NULL,
	DepartmentId int NOT NULL,
	Type int NOT NULL,
  CONSTRAINT [PK_DEPARTMENT_GENERAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [ProgrammeRationale] (
	Id int NOT NULL,
	RationaleId int,
	DemandId int,
	PsId int,
	TentativePsId int,
  CONSTRAINT [PK_PROGRAMMERATIONALE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Rationale] (
	Id int NOT NULL,
	Justification text,
	Fit text,
	Differences text,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_RATIONALE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Demand] (
	Id int NOT NULL,
	Description text,
	Period text,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_DEMAND] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [ProgrammeOfStudy] (
	Id int NOT NULL,
	KnowledgeUnderstanding text,
	IntellectualDevelopment text,
	KeyTransferableSkills text,
	OtherSkills text,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_PROGRAMMEOFSTUDY] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [TentativePs] (
	Id int NOT NULL,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_TENTATIVEPS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Year] (
	Id int NOT NULL,
	TotalEcts int,
	TentativePsId int,
	YearNo int,
  CONSTRAINT [PK_YEAR] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Unit] (
	Id int NOT NULL,
	Code varchar(20) NOT NULL,
	Title varchar(225) NOT NULL,
	DepartmentId int NOT NULL,
  CONSTRAINT [PK_REF_UNIT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Year_Unit] (
	Id int NOT NULL,
	YearId int NOT NULL,
	UnitId int NOT NULL,
	Period int,
	Coe int,
	Ects int,
	Lecturer nvarchar(128),
	Compensating int,
	CompensatingReason text,
  CONSTRAINT [PK_YEAR_UNIT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [ExternalReview] (
	Id int NOT NULL,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_EXTERNALREVIEW] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Reviewer] (
	Id int NOT NULL,
	Name varchar(225),
	Affiliation varchar(225),
	Address text,
	Email varchar(225),
	Telephone varchar(225),
  CONSTRAINT [PK_REVIEWER] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [ExternalReview_Reviewer] (
	Id int NOT NULL,
	ExternalReviewId int NOT NULL,
	ReviewerId int NOT NULL,
  CONSTRAINT [PK_EXTERNALREVIEW_REVIEWER] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [EndorsementCollab] (
	Id int NOT NULL,
	DepartmentId int,
	Selection bit,
	HeldDate varchar(50) NOT NULL,
	SignedBy varchar(225),
	SignedDate datetime,
  CONSTRAINT [PK_ENDORSEMENTCOLLAB] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Approval_Endorsement] (
	Id int NOT NULL,
	ApprovalId int NOT NULL,
	EndorsementId int NOT NULL,
  CONSTRAINT [PK_APPROVAL_ENDORSEMENT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [StatementServ] (
	Id int NOT NULL,
	DepartmentId int,
	Selection bit,
	Reservations text NOT NULL,
	SignedBy varchar(225),
	SignedDate datetime,
  CONSTRAINT [PK_STATEMENTSERV] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Approval_Statement] (
	Id int NOT NULL,
	ApprovalId int NOT NULL,
	StatementId int NOT NULL,
  CONSTRAINT [PK_APPROVAL_STATEMENT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [IncomeExpenditure] (
	Id int NOT NULL,
  CONSTRAINT [PK_INCOMEEXPENDITURE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [StatementIE] (
	Id int NOT NULL,
	Upload varchar(500) NOT NULL,
  CONSTRAINT [PK_STATEMENTIE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [IncomeExpenditure_StatementIE] (
	Id int NOT NULL,
	IncomeExpenditureId int NOT NULL,
	StatementId int NOT NULL,
  CONSTRAINT [PK_INCOMEEXPENDITURE_STATEMENTIE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Approval] (
	Id int NOT NULL,
	ThreadId int NOT NULL,
  CONSTRAINT [PK_APPROVAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Approval_Recommendation] (
	Id int NOT NULL,
	ApprovalId int NOT NULL,
	RecommendationId int NOT NULL,
  CONSTRAINT [PK_APPROVAL_RECOMMENDATION] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [RecommendationFics] (
	Id int NOT NULL,
	FacultyId int,
	Selection bit,
	HeldDateA varchar(50) NOT NULL,
	HeldDateB varchar(50) NOT NULL,
	SignedBy varchar(225),
	SignedDate datetime,
  CONSTRAINT [PK_RECOMMENDATIONFICS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [InPrincipal] (
	Id int NOT NULL,
  CONSTRAINT [PK_INPRINCIPAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [PvcApproval] (
	Id int NOT NULL,
	Selection bit,
	SignedBy varchar(225),
	SignedDate datetime,
	Upload varchar(500) NOT NULL,
	CouncilRef bit,
  CONSTRAINT [PK_PVCAPPROVAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [InPrincipal_Pvc] (
	Id int NOT NULL,
	InPrincipalId int NOT NULL,
	PvcApprovalId int NOT NULL,
  CONSTRAINT [PK_INPRINCIPAL_PVC] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [SenateDecision] (
	Id int NOT NULL,
	Selection bit NOT NULL,
	SignedBy varchar(225),
	SignedDate datetime,
  CONSTRAINT [PK_SENATEDECISION] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [InPrincipal_Senate] (
	Id int NOT NULL,
	InPrincipalId int NOT NULL,
	SenateDecisionId int NOT NULL,
  CONSTRAINT [PK_INPRINCIPAL_SENATE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [CouncilDecision] (
	Id int NOT NULL,
	Selection bit NOT NULL,
	SignedBy varchar(225),
	SignedDate datetime,
  CONSTRAINT [PK_COUNCILDECISION] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [InPrincipal_Council] (
	Id int NOT NULL,
	InPrincipalId int NOT NULL,
	CouncilDecisionId int NOT NULL,
  CONSTRAINT [PK_INPRINCIPAL_COUNCIL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Whitelist] (
	Id int NOT NULL,
	Email varchar(225) NOT NULL UNIQUE,
	PrimaryRoleId nvarchar(128) NOT NULL,
  CONSTRAINT [PK_WHITELIST] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Shared_General] (
	Id int NOT NULL,
	GeneralId int NOT NULL,
	UserId nvarchar(128) NOT NULL,
  CONSTRAINT [PK_SHARED_GENERAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Ref_Type] (
	Id int NOT NULL,
	Name varchar(225) NOT NULL,
  CONSTRAINT [PK_REF_TYPE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Type_General] (
	Id int NOT NULL,
	GeneralId int NOT NULL,
	TypeId int NOT NULL,
  CONSTRAINT [PK_TYPE_GENERAL] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Comment] (
	Id int NOT NULL,
	Description text NOT NULL,
	UserId nvarchar(128) NOT NULL,
	Date datetime NOT NULL,
	ReplyTo int,
	Type varchar(25),
  CONSTRAINT [PK_COMMENT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Thread] (
	Id int NOT NULL,
  CONSTRAINT [PK_THREAD] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Thread_Comment] (
	Id int NOT NULL,
	ThreadId int NOT NULL,
	CommentId int NOT NULL,
  CONSTRAINT [PK_THREAD_COMMENT] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk0] FOREIGN KEY ([GeneralId]) REFERENCES [General]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk0]
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk1] FOREIGN KEY ([ProgrammeRationaleId]) REFERENCES [ProgrammeRationale]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk1]
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk2] FOREIGN KEY ([ExternalReviewId]) REFERENCES [ExternalReview]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk2]
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk3] FOREIGN KEY ([IncomeExpenditureId]) REFERENCES [IncomeExpenditure]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk3]
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk4] FOREIGN KEY ([ApprovalId]) REFERENCES [Approval]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk4]
GO
ALTER TABLE [Proposal] WITH CHECK ADD CONSTRAINT [Proposal_fk5] FOREIGN KEY ([InPrincipalId]) REFERENCES [InPrincipal]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposal] CHECK CONSTRAINT [Proposal_fk5]
GO

ALTER TABLE [General] WITH CHECK ADD CONSTRAINT [General_fk0] FOREIGN KEY ([LevelId]) REFERENCES [Ref_Level]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [General] CHECK CONSTRAINT [General_fk0]
GO
ALTER TABLE [General] WITH CHECK ADD CONSTRAINT [General_fk1] FOREIGN KEY ([FacultyId]) REFERENCES [Ref_Faculty]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [General] CHECK CONSTRAINT [General_fk1]
GO
ALTER TABLE [General] WITH CHECK ADD CONSTRAINT [General_fk2] FOREIGN KEY ([DeliveryId]) REFERENCES [Ref_Delivery]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [General] CHECK CONSTRAINT [General_fk2]
GO
ALTER TABLE [General] WITH CHECK ADD CONSTRAINT [General_fk3] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [General] CHECK CONSTRAINT [General_fk3]
GO




ALTER TABLE [Proposer_General] WITH CHECK ADD CONSTRAINT [Proposer_General_fk0] FOREIGN KEY ([GeneralId]) REFERENCES [General]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Proposer_General] CHECK CONSTRAINT [Proposer_General_fk0]
GO

ALTER TABLE [Ref_Department] WITH CHECK ADD CONSTRAINT [Ref_Department_fk0] FOREIGN KEY ([FacultyId]) REFERENCES [Ref_Faculty]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Ref_Department] CHECK CONSTRAINT [Ref_Department_fk0]
GO

ALTER TABLE [Department_General] WITH CHECK ADD CONSTRAINT [Department_General_fk0] FOREIGN KEY ([GeneralId]) REFERENCES [General]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Department_General] CHECK CONSTRAINT [Department_General_fk0]
GO
ALTER TABLE [Department_General] WITH CHECK ADD CONSTRAINT [Department_General_fk1] FOREIGN KEY ([DepartmentId]) REFERENCES [Ref_Department]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [Department_General] CHECK CONSTRAINT [Department_General_fk1]
GO

ALTER TABLE [ProgrammeRationale] WITH CHECK ADD CONSTRAINT [ProgrammeRationale_fk0] FOREIGN KEY ([RationaleId]) REFERENCES [Rationale]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ProgrammeRationale] CHECK CONSTRAINT [ProgrammeRationale_fk0]
GO
ALTER TABLE [ProgrammeRationale] WITH CHECK ADD CONSTRAINT [ProgrammeRationale_fk1] FOREIGN KEY ([DemandId]) REFERENCES [Demand]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ProgrammeRationale] CHECK CONSTRAINT [ProgrammeRationale_fk1]
GO
ALTER TABLE [ProgrammeRationale] WITH CHECK ADD CONSTRAINT [ProgrammeRationale_fk2] FOREIGN KEY ([PsId]) REFERENCES [ProgrammeOfStudy]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ProgrammeRationale] CHECK CONSTRAINT [ProgrammeRationale_fk2]
GO
ALTER TABLE [ProgrammeRationale] WITH CHECK ADD CONSTRAINT [ProgrammeRationale_fk3] FOREIGN KEY ([TentativePsId]) REFERENCES [TentativePs]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ProgrammeRationale] CHECK CONSTRAINT [ProgrammeRationale_fk3]
GO

ALTER TABLE [Rationale] WITH CHECK ADD CONSTRAINT [Rationale_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [Rationale] CHECK CONSTRAINT [Rationale_fk0]
GO

ALTER TABLE [Demand] WITH CHECK ADD CONSTRAINT [Demand_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [Demand] CHECK CONSTRAINT [Demand_fk0]
GO

ALTER TABLE [ProgrammeOfStudy] WITH CHECK ADD CONSTRAINT [ProgrammeOfStudy_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [ProgrammeOfStudy] CHECK CONSTRAINT [ProgrammeOfStudy_fk0]
GO

ALTER TABLE [TentativePs] WITH CHECK ADD CONSTRAINT [TentativePs_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [TentativePs] CHECK CONSTRAINT [TentativePs_fk0]
GO

ALTER TABLE [Year] WITH CHECK ADD CONSTRAINT [Year_fk0] FOREIGN KEY ([TentativePsId]) REFERENCES [TentativePs]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Year] CHECK CONSTRAINT [Year_fk0]
GO

ALTER TABLE [Ref_Unit] WITH CHECK ADD CONSTRAINT [Ref_Unit_fk0] FOREIGN KEY ([DepartmentId]) REFERENCES [Ref_Department]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Ref_Unit] CHECK CONSTRAINT [Ref_Unit_fk0]
GO

ALTER TABLE [Year_Unit] WITH CHECK ADD CONSTRAINT [Year_Unit_fk0] FOREIGN KEY ([YearId]) REFERENCES [Year]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Year_Unit] CHECK CONSTRAINT [Year_Unit_fk0]
GO
ALTER TABLE [Year_Unit] WITH CHECK ADD CONSTRAINT [Year_Unit_fk1] FOREIGN KEY ([UnitId]) REFERENCES [Ref_Unit]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Year_Unit] CHECK CONSTRAINT [Year_Unit_fk1]
GO

ALTER TABLE [ExternalReview] WITH CHECK ADD CONSTRAINT [ExternalReview_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [ExternalReview] CHECK CONSTRAINT [ExternalReview_fk0]
GO


ALTER TABLE [ExternalReview_Reviewer] WITH CHECK ADD CONSTRAINT [ExternalReview_Reviewer_fk0] FOREIGN KEY ([ExternalReviewId]) REFERENCES [ExternalReview]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ExternalReview_Reviewer] CHECK CONSTRAINT [ExternalReview_Reviewer_fk0]
GO
ALTER TABLE [ExternalReview_Reviewer] WITH CHECK ADD CONSTRAINT [ExternalReview_Reviewer_fk1] FOREIGN KEY ([ReviewerId]) REFERENCES [Reviewer]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ExternalReview_Reviewer] CHECK CONSTRAINT [ExternalReview_Reviewer_fk1]
GO

ALTER TABLE [EndorsementCollab] WITH CHECK ADD CONSTRAINT [EndorsementCollab_fk0] FOREIGN KEY ([DepartmentId]) REFERENCES [Ref_Department]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [EndorsementCollab] CHECK CONSTRAINT [EndorsementCollab_fk0]
GO

ALTER TABLE [Approval_Endorsement] WITH CHECK ADD CONSTRAINT [Approval_Endorsement_fk0] FOREIGN KEY ([ApprovalId]) REFERENCES [Approval]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Endorsement] CHECK CONSTRAINT [Approval_Endorsement_fk0]
GO
ALTER TABLE [Approval_Endorsement] WITH CHECK ADD CONSTRAINT [Approval_Endorsement_fk1] FOREIGN KEY ([EndorsementId]) REFERENCES [EndorsementCollab]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Endorsement] CHECK CONSTRAINT [Approval_Endorsement_fk1]
GO

ALTER TABLE [StatementServ] WITH CHECK ADD CONSTRAINT [StatementServ_fk0] FOREIGN KEY ([DepartmentId]) REFERENCES [Ref_Department]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [StatementServ] CHECK CONSTRAINT [StatementServ_fk0]
GO

ALTER TABLE [Approval_Statement] WITH CHECK ADD CONSTRAINT [Approval_Statement_fk0] FOREIGN KEY ([ApprovalId]) REFERENCES [Approval]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Statement] CHECK CONSTRAINT [Approval_Statement_fk0]
GO
ALTER TABLE [Approval_Statement] WITH CHECK ADD CONSTRAINT [Approval_Statement_fk1] FOREIGN KEY ([StatementId]) REFERENCES [StatementServ]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Statement] CHECK CONSTRAINT [Approval_Statement_fk1]
GO



ALTER TABLE [IncomeExpenditure_StatementIE] WITH CHECK ADD CONSTRAINT [IncomeExpenditure_StatementIE_fk0] FOREIGN KEY ([IncomeExpenditureId]) REFERENCES [IncomeExpenditure]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [IncomeExpenditure_StatementIE] CHECK CONSTRAINT [IncomeExpenditure_StatementIE_fk0]
GO
ALTER TABLE [IncomeExpenditure_StatementIE] WITH CHECK ADD CONSTRAINT [IncomeExpenditure_StatementIE_fk1] FOREIGN KEY ([StatementId]) REFERENCES [StatementIE]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [IncomeExpenditure_StatementIE] CHECK CONSTRAINT [IncomeExpenditure_StatementIE_fk1]
GO

ALTER TABLE [Approval] WITH CHECK ADD CONSTRAINT [Approval_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [Approval] CHECK CONSTRAINT [Approval_fk0]
GO

ALTER TABLE [Approval_Recommendation] WITH CHECK ADD CONSTRAINT [Approval_Recommendation_fk0] FOREIGN KEY ([ApprovalId]) REFERENCES [Approval]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Recommendation] CHECK CONSTRAINT [Approval_Recommendation_fk0]
GO
ALTER TABLE [Approval_Recommendation] WITH CHECK ADD CONSTRAINT [Approval_Recommendation_fk1] FOREIGN KEY ([RecommendationId]) REFERENCES [RecommendationFics]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Approval_Recommendation] CHECK CONSTRAINT [Approval_Recommendation_fk1]
GO

ALTER TABLE [RecommendationFics] WITH CHECK ADD CONSTRAINT [RecommendationFics_fk0] FOREIGN KEY ([FacultyId]) REFERENCES [Ref_Faculty]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [RecommendationFics] CHECK CONSTRAINT [RecommendationFics_fk0]
GO



ALTER TABLE [InPrincipal_Pvc] WITH CHECK ADD CONSTRAINT [InPrincipal_Pvc_fk0] FOREIGN KEY ([InPrincipalId]) REFERENCES [InPrincipal]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Pvc] CHECK CONSTRAINT [InPrincipal_Pvc_fk0]
GO
ALTER TABLE [InPrincipal_Pvc] WITH CHECK ADD CONSTRAINT [InPrincipal_Pvc_fk1] FOREIGN KEY ([PvcApprovalId]) REFERENCES [PvcApproval]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Pvc] CHECK CONSTRAINT [InPrincipal_Pvc_fk1]
GO


ALTER TABLE [InPrincipal_Senate] WITH CHECK ADD CONSTRAINT [InPrincipal_Senate_fk0] FOREIGN KEY ([InPrincipalId]) REFERENCES [InPrincipal]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Senate] CHECK CONSTRAINT [InPrincipal_Senate_fk0]
GO
ALTER TABLE [InPrincipal_Senate] WITH CHECK ADD CONSTRAINT [InPrincipal_Senate_fk1] FOREIGN KEY ([SenateDecisionId]) REFERENCES [SenateDecision]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Senate] CHECK CONSTRAINT [InPrincipal_Senate_fk1]
GO


ALTER TABLE [InPrincipal_Council] WITH CHECK ADD CONSTRAINT [InPrincipal_Council_fk0] FOREIGN KEY ([InPrincipalId]) REFERENCES [InPrincipal]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Council] CHECK CONSTRAINT [InPrincipal_Council_fk0]
GO
ALTER TABLE [InPrincipal_Council] WITH CHECK ADD CONSTRAINT [InPrincipal_Council_fk1] FOREIGN KEY ([CouncilDecisionId]) REFERENCES [CouncilDecision]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [InPrincipal_Council] CHECK CONSTRAINT [InPrincipal_Council_fk1]
GO


ALTER TABLE [Shared_General] WITH CHECK ADD CONSTRAINT [Shared_General_fk0] FOREIGN KEY ([GeneralId]) REFERENCES [General]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Shared_General] CHECK CONSTRAINT [Shared_General_fk0]
GO


ALTER TABLE [Type_General] WITH CHECK ADD CONSTRAINT [Type_General_fk0] FOREIGN KEY ([GeneralId]) REFERENCES [General]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Type_General] CHECK CONSTRAINT [Type_General_fk0]
GO
ALTER TABLE [Type_General] WITH CHECK ADD CONSTRAINT [Type_General_fk1] FOREIGN KEY ([TypeId]) REFERENCES [Ref_Type]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Type_General] CHECK CONSTRAINT [Type_General_fk1]
GO

ALTER TABLE [Comment] WITH CHECK ADD CONSTRAINT [Comment_fk0] FOREIGN KEY ([ReplyTo]) REFERENCES [Comment]([Id])
ON UPDATE NO ACTION
GO
ALTER TABLE [Comment] CHECK CONSTRAINT [Comment_fk0]
GO


ALTER TABLE [Thread_Comment] WITH CHECK ADD CONSTRAINT [Thread_Comment_fk0] FOREIGN KEY ([ThreadId]) REFERENCES [Thread]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Thread_Comment] CHECK CONSTRAINT [Thread_Comment_fk0]
GO
ALTER TABLE [Thread_Comment] WITH CHECK ADD CONSTRAINT [Thread_Comment_fk1] FOREIGN KEY ([CommentId]) REFERENCES [Comment]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Thread_Comment] CHECK CONSTRAINT [Thread_Comment_fk1]
GO
