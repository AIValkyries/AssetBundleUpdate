set XLS_FILE_NAME=����
set SHEET_NAME=TaskInfo

@echo off

call aa_xls2cs %SHEET_NAME% %XLS_FILE_NAME%

pause