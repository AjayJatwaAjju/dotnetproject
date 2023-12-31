-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Create table for log Error details
-- ============================================

CREATE TABLE [dbo].[ErrorLogs](
	[pkErrorLogId] [INT] IDENTITY(1,1) NOT NULL,
	[ErrorDate] [DATETIME] NOT NULL,
	[Source] [NVARCHAR](50) NULL,
	[ErrorMessage] [NVARCHAR](MAX) NULL,
	[Severity] [INT] NULL,
	[UserName] [NVARCHAR](50) NULL,
 CONSTRAINT [PK_EZE_ErrorLogs] PRIMARY KEY CLUSTERED
(
	[pkErrorLogId] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Log sp Error details
-- ============================================
CREATE PROCEDURE [dbo].[spErrorLog]

AS

-- Declare the parameters for the stored procedure here
DECLARE @ErrorMessage NVARCHAR(4000);    
DECLARE @ErrorSeverity INT;    
DECLARE @ErrorState INT;    
   
SELECT    
    @ErrorMessage = 'Procedure: ' + ERROR_PROCEDURE() + ' Line: ' + CAST(ERROR_LINE() AS VARCHAR(8)) + SPACE(1) + ERROR_MESSAGE(),    
    @ErrorSeverity = ERROR_SEVERITY(),      
    @ErrorState = ERROR_STATE();    
 
--Set Error Parameter
IF @ErrorState = 0

BEGIN    
SET @ErrorState = 1    
END    
   
RAISERROR (@ErrorMessage, -- Message text.    
           @ErrorSeverity, -- Severity.    
           @ErrorState); -- State.
   
--Inserting the Error details
INSERT INTO ErrorLogs (ErrorDate,Source,ErrorMessage,Severity,UserName)    
VALUES(GETUTCDATE(),'DB',CAST(ERROR_NUMBER() AS VARCHAR(8)) + SPACE(1) + CAST(ERROR_STATE() AS VARCHAR(8)) + SPACE(1) + ERROR_MESSAGE(), ERROR_SEVERITY(),SYSTEM_USER)    
   
RETURN  