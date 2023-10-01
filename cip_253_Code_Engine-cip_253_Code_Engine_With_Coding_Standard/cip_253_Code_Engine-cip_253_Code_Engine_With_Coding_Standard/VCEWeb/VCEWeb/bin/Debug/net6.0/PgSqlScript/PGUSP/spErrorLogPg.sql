-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Create table for log Error details
-- ============================================
CREATE TABLE ErrorLogs(
pkErrorLogId INT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) NOT NULL,
ErrorDate TIMESTAMP(3) NOT NULL,
Source VARCHAR(50) NULL,
ErrorMessage TEXT NULL,
Severity TEXT NULL,
UserName VARCHAR(50) NULL,
 CONSTRAINT PK_ErrorLogs PRIMARY KEY
(
pkErrorLogId
));

-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: CREATE FUNCTION NAMED spErrorLog
-- ============================================
CREATE OR REPLACE FUNCTION spErrorLog(
errorsource text,errormessage text,errorcode text)
    RETURNS VOID
    LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
INSERT INTO ErrorLogs (errordate, source, errormessage, severity, username)
VALUES (CURRENT_TIMESTAMP, errorsource, errormessage, errorcode, CURRENT_USER);
EXCEPTION
WHEN OTHERS
THEN
-- Handle the exception here if needed
-- You can insert error into another error log table if required
-- INSERT INTO another_error_log (errordate, errormessage) VALUES (CURRENT_TIMESTAMP, ERROR_MESSAGE());
RETURN;
END;
$BODY$;