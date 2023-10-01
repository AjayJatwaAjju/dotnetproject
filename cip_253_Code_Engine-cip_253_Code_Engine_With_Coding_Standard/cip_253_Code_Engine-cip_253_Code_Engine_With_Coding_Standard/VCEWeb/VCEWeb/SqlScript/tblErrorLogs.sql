-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Create table for log Error details          
-- ============================================  
CREATE TABLE [dbo].[VCE_ErrorLogs](
	[pkErrorLogId] [INT] IDENTITY(1,1) NOT NULL,
	[ErrorDate] [DATETIME] NOT NULL,
	[Source] [NVARCHAR](50) NULL,
	[ErrorMessage] [NVARCHAR](MAX) NULL,
	[Severity] [INT] NULL,
	[UserName] [NVARCHAR](50) NULL,
 CONSTRAINT [PK_EZE_ErrorLogs] PRIMARY KEY CLUSTERED
(
	[pkErrorLogId] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]