-- =============================================  
-- Author:  VCE  
-- Create date: 11/05/2023 1:11 PM 
-- Description: This is for getting username based on userId  
-- =============================================  
CREATE FUNCTION [dbo].[fnGetUserNameById]  
(  
 -- Add the parameters for the function here  
 @fkUserId INT  
)  
RETURNS NVARCHAR(MAX) 

AS  

BEGIN  
   -- Declare the return variable here  
   DECLARE @UserName NVARCHAR(MAX)=''  
 
   -- Add the T-SQL statements to compute the return value here  
   SELECT @UserName=UserFirstName+SPACE(1)+UserLastName  
   FROM [dbo].[VCE_Users] WHERE pkUserId=ISNULL(@fkUserId,0)  
     
   -- Return the result of the function  
   RETURN @UserName  
 
END  