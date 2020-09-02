set XLS_FILE_NAME=¹Ø¿¨
set SHEET_NAME=LevelConfig

@echo off

call aa_xls2cs %SHEET_NAME% %XLS_FILE_NAME%

pause