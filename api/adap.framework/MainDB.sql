if not exists (select 1 from sys.objects t1 where t1.name='TestCase')
	CREATE TABLE [dbo].TestCase(
		[TestCaseID] [int] IDENTITY(1,1) NOT NULL,
		[TestCaseGUID] nvarchar(50) NULL,
		[TestCaseName] nvarchar(50) NULL,
		[TestCaseType] nvarchar(50) NULL,
		[Remark] [nvarchar](max) NULL,
	 CONSTRAINT [PK_TestCase] PRIMARY KEY CLUSTERED 
	(
		[TestCaseID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO 